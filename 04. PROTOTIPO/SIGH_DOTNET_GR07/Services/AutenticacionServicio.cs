// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SIGH_DOTNET_GR07.Data;
using SIGH_DOTNET_GR07.Models;

namespace SIGH_DOTNET_GR07.Services;

public enum TipoResultado { Exitoso, Credenciales, Bloqueado, Inactivo, SinPerfil }

/// <summary>Resultado de un intento de inicio de sesión.</summary>
public record ResultadoLogin(TipoResultado Tipo, string Mensaje, UsuarioSesion? Usuario)
{
    public bool EsExitoso => Tipo == TipoResultado.Exitoso;
}

/// <summary>
/// Lógica del caso de uso ECUD1 Iniciar sesión (RF-SEG-001, RF-SEG-004, RF-AUD-004): valida las
/// credenciales y el estado de la cuenta, bloquea tras varios intentos fallidos, abre y cierra las
/// sesiones en XESES_SESIO y deja cada intento en la auditoría.
/// </summary>
public class AutenticacionServicio(SighDbContext db, AuditoriaServicio auditoria, IOptions<SighOpciones> opciones)
{
    public const string EstadoActivo = "A";
    public const string EstadoInactivo = "I";
    public const string EstadoBloqueado = "B";

    /// <summary>Mensaje genérico: no revela si falló el usuario o la contraseña.</summary>
    public const string MensajeCredenciales = "Usuario o contraseña incorrectos.";

    private readonly SighOpciones _config = opciones.Value;

    public async Task<ResultadoLogin> AutenticarAsync(string? codigoUsuario, string? password, string? ip)
    {
        string codigo = SeguridadUtil.Texto(codigoUsuario);
        if (codigo.Length == 0 || string.IsNullOrEmpty(password))
        {
            return new ResultadoLogin(TipoResultado.Credenciales, "Ingrese su usuario y contraseña.", null);
        }

        Usuario? usuario = await db.Usuarios
            .Include(u => u.Empleado)
            .Include(u => u.Estado)
            .FirstOrDefaultAsync(u => u.CodigoUsuario == codigo);

        if (usuario is null)
        {
            await auditoria.RegistrarYGuardarAsync(null, "INICIAR SESION", "FALLIDO", "Usuario inexistente: " + codigo, ip);
            return new ResultadoLogin(TipoResultado.Credenciales, MensajeCredenciales, null);
        }

        codigo = usuario.CodigoUsuario;

        if (usuario.CodigoEstado == EstadoInactivo)
        {
            await auditoria.RegistrarYGuardarAsync(codigo, "INICIAR SESION", "DENEGADO", "Intento de acceso con cuenta desactivada", ip);
            return new ResultadoLogin(TipoResultado.Inactivo,
                "La cuenta no está habilitada. Comuníquese con el administrador del sistema.", null);
        }

        if (usuario.CodigoEstado == EstadoBloqueado)
        {
            int minutosRestantes = MinutosRestantesBloqueo(usuario);
            if (minutosRestantes > 0)
            {
                await auditoria.RegistrarYGuardarAsync(codigo, "INICIAR SESION", "BLOQUEADO", "Intento de acceso con cuenta bloqueada", ip);
                return new ResultadoLogin(TipoResultado.Bloqueado, "La cuenta está bloqueada temporalmente por intentos fallidos. "
                    + "Intente nuevamente en " + minutosRestantes + " minuto(s) o recupere su contraseña.", null);
            }
            // Ya pasó el tiempo de bloqueo: la cuenta se habilita de nuevo.
            usuario.Estado = await EstadoAsync(EstadoActivo);
            usuario.IntentosFallidos = 0;
            await auditoria.RegistrarAsync(codigo, "DESBLOQUEO AUTOMATICO", "EXITOSO",
                "Terminó el tiempo de bloqueo de " + _config.MinutosBloqueo + " minutos", ip);
        }

        if (!SeguridadUtil.Coincide(password, usuario.Password))
        {
            return await RegistrarIntentoFallidoAsync(usuario, ip);
        }

        List<UsuPer> perfiles = await db.UsuPers
            .Include(up => up.Perfil)
            .Where(up => up.CodigoUsuario == codigo && up.FechaRetiro == null)
            .ToListAsync();
        if (perfiles.Count == 0)
        {
            await auditoria.RegistrarYGuardarAsync(codigo, "INICIAR SESION", "DENEGADO", "El usuario no tiene un perfil vigente", ip);
            return new ResultadoLogin(TipoResultado.SinPerfil,
                "Su cuenta no tiene un perfil asignado. Comuníquese con el administrador del sistema.", null);
        }

        DateTime ahora = DateTime.Now;
        DateTime? accesoAnterior = usuario.UltimoAcceso;
        usuario.IntentosFallidos = 0;
        usuario.UltimoAcceso = ahora;

        var sesion = new Sesion
        {
            CodigoUsuario = codigo,
            FechaInicio = ahora,
            DireccionIp = ip,
            EstadoSesion = "A"
        };
        db.Sesiones.Add(sesion);

        // La sesión necesita su número (IDENTITY) antes de escribir la auditoría: dos guardados en una transacción.
        await using (var transaccion = await db.Database.BeginTransactionAsync())
        {
            await db.SaveChangesAsync();
            await auditoria.RegistrarAsync(codigo, "INICIAR SESION", "EXITOSO",
                "Inicio de sesión correcto (sesión " + sesion.CodigoSesion + ")", ip);
            await db.SaveChangesAsync();
            await transaccion.CommitAsync();
        }

        string nombres = SeguridadUtil.Capitalizar(usuario.Empleado.Nombres);
        string apellidos = SeguridadUtil.Capitalizar(usuario.Empleado.Apellidos);
        var datos = new UsuarioSesion
        {
            CodigoUsuario = codigo,
            NombreCompleto = (nombres + " " + apellidos).Trim(),
            Iniciales = Inicial(nombres) + Inicial(apellidos),
            PieFirma = usuario.PieFirma,
            Perfiles = perfiles.Select(p => p.Perfil.DescripcionPerfil).ToList(),
            CodigoSesion = sesion.CodigoSesion,
            InicioSesion = ahora,
            UltimoAccesoAnterior = accesoAnterior,
            DireccionIp = ip
        };
        return new ResultadoLogin(TipoResultado.Exitoso, "Bienvenido " + datos.NombreCompleto, datos);
    }

    /// <summary>
    /// Cierra el registro de la sesión en XESES_SESIO.
    /// estadoFinal: C = cerrada por el usuario, E = expirada por inactividad.
    /// </summary>
    public async Task CerrarSesionAsync(long codigoSesion, string codigoUsuario, string? ip, string estadoFinal, string detalle)
    {
        Sesion? sesion = await db.Sesiones.FindAsync(codigoSesion);
        if (sesion is null || sesion.EstadoSesion != "A")
        {
            return;
        }
        sesion.FechaFin = DateTime.Now;
        sesion.EstadoSesion = estadoFinal;
        string operacion = estadoFinal == "E" ? "SESION EXPIRADA" : "CERRAR SESION";
        await auditoria.RegistrarAsync(codigoUsuario, operacion, "EXITOSO", detalle, ip);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Al arrancar, las sesiones que quedaron abiertas en la base no tienen actividad registrada
    /// (el servidor se detuvo): se marcan como expiradas.
    /// </summary>
    public async Task<int> ExpirarSesionesHuerfanasAsync()
    {
        var abiertas = await db.Sesiones.Where(s => s.EstadoSesion == "A").ToListAsync();
        foreach (Sesion sesion in abiertas)
        {
            sesion.FechaFin = DateTime.Now;
            sesion.EstadoSesion = "E";
            await auditoria.RegistrarAsync(sesion.CodigoUsuario, "SESION EXPIRADA", "EXITOSO",
                "Sesión " + sesion.CodigoSesion + " finalizada al reiniciar el servidor", sesion.DireccionIp);
        }
        await db.SaveChangesAsync();
        return abiertas.Count;
    }

    public async Task<List<Sesion>> UltimasSesionesAsync(string codigoUsuario) =>
        await db.Sesiones.AsNoTracking()
            .Where(s => s.CodigoUsuario == codigoUsuario)
            .OrderByDescending(s => s.FechaInicio).ThenByDescending(s => s.CodigoSesion)
            .Take(5)
            .ToListAsync();

    private async Task<ResultadoLogin> RegistrarIntentoFallidoAsync(Usuario usuario, string? ip)
    {
        short intentos = (short)(usuario.IntentosFallidos + 1);
        usuario.IntentosFallidos = intentos;

        if (intentos >= _config.MaxIntentos)
        {
            usuario.Estado = await EstadoAsync(EstadoBloqueado);
            usuario.FechaModificacion = DateTime.Now;
            await auditoria.RegistrarYGuardarAsync(usuario.CodigoUsuario, "INICIAR SESION", "BLOQUEADO",
                "Cuenta bloqueada tras " + intentos + " intentos fallidos", ip);
            return new ResultadoLogin(TipoResultado.Bloqueado, "Se superó el número de intentos permitidos. La cuenta quedó bloqueada "
                + _config.MinutosBloqueo + " minutos. Puede recuperar su contraseña.", null);
        }

        await auditoria.RegistrarYGuardarAsync(usuario.CodigoUsuario, "INICIAR SESION", "FALLIDO",
            "Contraseña incorrecta (intento " + intentos + " de " + _config.MaxIntentos + ")", ip);
        return new ResultadoLogin(TipoResultado.Credenciales, MensajeCredenciales, null);
    }

    private int MinutosRestantesBloqueo(Usuario usuario)
    {
        DateTime desde = usuario.FechaModificacion ?? usuario.FechaCreacion;
        double segundos = (desde.AddMinutes(_config.MinutosBloqueo) - DateTime.Now).TotalSeconds;
        return segundos <= 0 ? 0 : (int)Math.Ceiling(segundos / 60);
    }

    private async Task<Estado> EstadoAsync(string codigo) =>
        await db.Estados.FindAsync(codigo) ?? throw new InvalidOperationException("No existe el estado " + codigo);

    private static string Inicial(string texto) => texto.Length == 0 ? "" : texto[..1].ToUpperInvariant();
}
