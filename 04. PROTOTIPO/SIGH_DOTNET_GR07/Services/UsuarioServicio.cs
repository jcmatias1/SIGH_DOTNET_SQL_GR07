// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SIGH_DOTNET_GR07.Data;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Models;

namespace SIGH_DOTNET_GR07.Services;

/// <summary>
/// CRUD de usuarios del caso de uso ECUD2 Administrar usuarios y roles (RF-SEG-002, RF-SEG-003):
/// Create (crear cuenta), Read (listar y ver detalle), Update (modificar y asignar perfil) y
/// Delete lógico (desactivar). Ninguna cuenta se borra de la base: así se conserva su historial
/// de sesiones y de auditoría.
/// </summary>
public class UsuarioServicio(SighDbContext db, AuditoriaServicio auditoria)
{
    private static readonly Dictionary<string, string> Cargos = new()
    {
        ["ADS"] = "Administrador de sistemas",
        ["MED"] = "Médico",
        ["ENF"] = "Enfermero/a",
        ["FAR"] = "Farmacéutico/a"
    };

    private static readonly Dictionary<string, string> Departamentos = new()
    {
        ["ADM"] = "Administración",
        ["CEX"] = "Consulta externa",
        ["HOS"] = "Hospitalización",
        ["FAR"] = "Farmacia"
    };

    private static readonly Regex FormatoUsuario = new("^[a-z][a-z0-9._]{3,19}$");

    /// <summary>Consulta base con todo lo que necesita el DTO.</summary>
    private IQueryable<Usuario> ConDatos() => db.Usuarios
        .Include(u => u.Empleado)
        .Include(u => u.Estado)
        .Include(u => u.Perfiles).ThenInclude(up => up.Perfil);

    public async Task<List<UsuarioDto>> ListarAsync()
    {
        var usuarios = await ConDatos().AsNoTracking().OrderBy(u => u.CodigoUsuario).ToListAsync();
        return usuarios.Select(ADto).ToList();
    }

    /// <summary>READ individual: cuenta, datos del empleado, historial de perfiles y últimas sesiones.</summary>
    public async Task<UsuarioDetalleDto> DetalleAsync(string codigoUsuario)
    {
        Usuario u = await BuscarUsuarioAsync(codigoUsuario, seguimiento: false);
        Empleado e = u.Empleado;
        var perfiles = u.Perfiles
            .OrderByDescending(up => up.FechaAsignacion)
            .Select(up => new PerfilHistorialDto(up.CodigoPerfil, up.Perfil.DescripcionPerfil, up.FechaAsignacion, up.FechaRetiro))
            .ToList();
        var sesiones = await db.Sesiones.AsNoTracking()
            .Where(s => s.CodigoUsuario == u.CodigoUsuario)
            .OrderByDescending(s => s.FechaInicio).ThenByDescending(s => s.CodigoSesion)
            .Take(5)
            .Select(s => new SesionResumenDto(s.CodigoSesion, s.FechaInicio, s.FechaFin, s.DireccionIp, s.EstadoSesion))
            .ToListAsync();
        return new UsuarioDetalleDto(ADto(u),
            Cargos.GetValueOrDefault(e.CodigoCargo, e.CodigoCargo),
            Departamentos.GetValueOrDefault(e.CodigoDepartamento, e.CodigoDepartamento),
            e.Telefono, e.RegistroProfesional, u.FechaModificacion, perfiles, sesiones);
    }

    /// <summary>Empleados activos que aún no tienen cuenta (para el formulario Nuevo usuario).</summary>
    public async Task<List<EmpleadoDto>> EmpleadosSinCuentaAsync()
    {
        var empleados = await db.Empleados.AsNoTracking()
            .Where(e => e.EstadoEmpleado == "A" && !db.Usuarios.Any(u => u.CodigoEmpleado == e.CodigoEmpleado))
            .OrderBy(e => e.Apellidos)
            .ToListAsync();
        return empleados
            .Select(e => new EmpleadoDto(e.CodigoEmpleado, e.Nombres + " " + e.Apellidos, e.Cedula, e.Email,
                Cargos.GetValueOrDefault(e.CodigoCargo, e.CodigoCargo)))
            .ToList();
    }

    public async Task<List<PerfilSimpleDto>> PerfilesAsync() =>
        await db.Perfiles.AsNoTracking()
            .OrderBy(p => p.CodigoPerfil)
            .Select(p => new PerfilSimpleDto(p.CodigoPerfil, p.DescripcionPerfil))
            .ToListAsync();

    /// <summary>CREAR CUENTA DE USUARIO (flujo básico de ECUD2).</summary>
    public async Task<UsuarioDto> CrearAsync(UsuarioFormDto datos, string actor, string? ip)
    {
        string codigo = SeguridadUtil.Texto(datos.CodigoUsuario).ToLowerInvariant();
        if (!FormatoUsuario.IsMatch(codigo))
        {
            throw new ReglaNegocioException("El usuario debe tener de 4 a 20 caracteres: letras minúsculas, "
                + "números, punto o guion bajo, y empezar con una letra.");
        }
        if (await db.Usuarios.AnyAsync(u => u.CodigoUsuario == codigo))
        {
            throw new ReglaNegocioException("Ya existe una cuenta con el usuario «" + codigo + "».");
        }
        string codigoEmpleado = SeguridadUtil.Texto(datos.CodigoEmpleado);
        Empleado empleado = await db.Empleados.FirstOrDefaultAsync(e => e.CodigoEmpleado == codigoEmpleado)
            ?? throw new ReglaNegocioException("Seleccione un empleado válido.");
        if (await db.Usuarios.AnyAsync(u => u.CodigoEmpleado == empleado.CodigoEmpleado))
        {
            throw new ReglaNegocioException("El empleado seleccionado ya tiene una cuenta de usuario.");
        }
        Perfil perfil = await BuscarPerfilAsync(datos.CodigoPerfil);
        string? errorPassword = SeguridadUtil.ValidarPolitica(datos.Password);
        if (errorPassword is not null)
        {
            throw new ReglaNegocioException(errorPassword);
        }

        DateTime ahora = DateTime.Now;
        var usuario = new Usuario
        {
            CodigoUsuario = codigo,
            Empleado = empleado,
            CodigoEmpleado = empleado.CodigoEmpleado,
            Estado = await EstadoAsync(AutenticacionServicio.EstadoActivo),
            Password = SeguridadUtil.Sha256(datos.Password!),
            IntentosFallidos = 0,
            FechaCreacion = ahora,
            PieFirma = LimpiarPieFirma(datos.PieFirma)
        };
        db.Usuarios.Add(usuario);
        AsignarPerfil(usuario, perfil, ahora);
        await auditoria.RegistrarAsync(actor, "CREAR USUARIO", "EXITOSO",
            "Se creó el usuario " + codigo + " con perfil " + perfil.CodigoPerfil, ip);
        await db.SaveChangesAsync();
        return ADto(usuario);
    }

    /// <summary>Modificar el pie de firma, el perfil (ASIGNAR ROLES Y PERMISOS) y, si se escribe, la contraseña.</summary>
    public async Task<UsuarioDto> ActualizarAsync(string codigoUsuario, UsuarioFormDto datos, string actor, string? ip)
    {
        Usuario usuario = await BuscarUsuarioAsync(codigoUsuario, seguimiento: true);
        Perfil perfil = await BuscarPerfilAsync(datos.CodigoPerfil);
        DateTime ahora = DateTime.Now;
        var cambios = new StringBuilder();

        string perfilAnterior = PerfilVigente(usuario)?.CodigoPerfil ?? "-";
        if (perfil.CodigoPerfil != perfilAnterior)
        {
            if (actor == usuario.CodigoUsuario)
            {
                throw new ReglaNegocioException("No puede cambiar su propio perfil.");
            }
            AsignarPerfil(usuario, perfil, ahora);
            cambios.Append("perfil ").Append(perfilAnterior).Append(" -> ").Append(perfil.CodigoPerfil).Append("; ");
        }

        string? pieFirma = LimpiarPieFirma(datos.PieFirma);
        if (pieFirma != usuario.PieFirma)
        {
            usuario.PieFirma = pieFirma;
            cambios.Append("pie de firma; ");
        }

        if (!string.IsNullOrWhiteSpace(datos.Password))
        {
            string? errorPassword = SeguridadUtil.ValidarPolitica(datos.Password);
            if (errorPassword is not null)
            {
                throw new ReglaNegocioException(errorPassword);
            }
            usuario.Password = SeguridadUtil.Sha256(datos.Password);
            cambios.Append("contraseña restablecida por el administrador; ");
        }

        if (cambios.Length == 0)
        {
            throw new ReglaNegocioException("No hay cambios para guardar.");
        }

        usuario.FechaModificacion = ahora;
        await auditoria.RegistrarAsync(actor, "MODIFICAR USUARIO", "EXITOSO",
            "Usuario " + usuario.CodigoUsuario + ": " + cambios.ToString().Trim(), ip);
        await db.SaveChangesAsync();
        return ADto(usuario);
    }

    /// <summary>DESACTIVAR CUENTA (borrado lógico), volver a activarla o desbloquearla.</summary>
    public async Task<UsuarioDto> CambiarEstadoAsync(string codigoUsuario, string accion, string actor, string? ip)
    {
        Usuario usuario = await BuscarUsuarioAsync(codigoUsuario, seguimiento: true);
        string estadoActual = usuario.CodigoEstado;
        string nuevoEstado;
        string operacion;

        switch (accion.ToLowerInvariant())
        {
            case "desactivar":
                if (actor == usuario.CodigoUsuario)
                {
                    throw new ReglaNegocioException("No puede desactivar su propia cuenta.");
                }
                if (estadoActual == AutenticacionServicio.EstadoInactivo)
                {
                    throw new ReglaNegocioException("La cuenta ya está desactivada.");
                }
                nuevoEstado = AutenticacionServicio.EstadoInactivo;
                operacion = "DESACTIVAR USUARIO";
                break;
            case "activar":
                if (estadoActual != AutenticacionServicio.EstadoInactivo)
                {
                    throw new ReglaNegocioException("Solo se pueden activar cuentas desactivadas.");
                }
                nuevoEstado = AutenticacionServicio.EstadoActivo;
                operacion = "ACTIVAR USUARIO";
                break;
            case "desbloquear":
                if (estadoActual != AutenticacionServicio.EstadoBloqueado)
                {
                    throw new ReglaNegocioException("La cuenta no está bloqueada.");
                }
                nuevoEstado = AutenticacionServicio.EstadoActivo;
                operacion = "DESBLOQUEAR USUARIO";
                break;
            default:
                throw new ReglaNegocioException("Acción no válida.");
        }

        usuario.Estado = await EstadoAsync(nuevoEstado);
        usuario.CodigoEstado = nuevoEstado;
        usuario.IntentosFallidos = 0;
        usuario.FechaModificacion = DateTime.Now;
        await auditoria.RegistrarAsync(actor, operacion, "EXITOSO",
            "Usuario " + usuario.CodigoUsuario + ": estado " + estadoActual + " -> " + nuevoEstado, ip);
        await db.SaveChangesAsync();
        return ADto(usuario);
    }

    /// <summary>Retira el perfil vigente (fecha de retiro) y asigna el nuevo, conservando el historial.</summary>
    private static void AsignarPerfil(Usuario usuario, Perfil perfil, DateTime ahora)
    {
        foreach (UsuPer vigente in usuario.Perfiles.Where(up => up.FechaRetiro == null))
        {
            vigente.FechaRetiro = ahora;
        }
        UsuPer? asignacion = usuario.Perfiles.FirstOrDefault(up => up.CodigoPerfil == perfil.CodigoPerfil);
        if (asignacion is null)
        {
            asignacion = new UsuPer
            {
                CodigoUsuario = usuario.CodigoUsuario,
                CodigoPerfil = perfil.CodigoPerfil,
                Usuario = usuario,
                Perfil = perfil
            };
            usuario.Perfiles.Add(asignacion);
        }
        asignacion.FechaAsignacion = ahora;
        asignacion.FechaRetiro = null;
    }

    private static UsuPer? PerfilVigente(Usuario usuario) =>
        usuario.Perfiles.Where(up => up.FechaRetiro == null).MaxBy(up => up.FechaAsignacion);

    private async Task<Usuario> BuscarUsuarioAsync(string codigoUsuario, bool seguimiento)
    {
        string codigo = SeguridadUtil.Texto(codigoUsuario);
        IQueryable<Usuario> consulta = seguimiento ? ConDatos() : ConDatos().AsNoTracking();
        return await consulta.FirstOrDefaultAsync(u => u.CodigoUsuario == codigo)
            ?? throw new ReglaNegocioException("La cuenta no existe.");
    }

    private async Task<Perfil> BuscarPerfilAsync(string? codigoPerfil)
    {
        string codigo = SeguridadUtil.Texto(codigoPerfil).ToUpperInvariant();
        return await db.Perfiles.FirstOrDefaultAsync(p => p.CodigoPerfil == codigo)
            ?? throw new ReglaNegocioException("Seleccione un perfil válido.");
    }

    private async Task<Estado> EstadoAsync(string codigo) =>
        await db.Estados.FindAsync(codigo) ?? throw new InvalidOperationException("No existe el estado " + codigo);

    private static UsuarioDto ADto(Usuario u)
    {
        Empleado e = u.Empleado;
        UsuPer? perfil = PerfilVigente(u);
        return new UsuarioDto(u.CodigoUsuario, e.CodigoEmpleado, e.Nombres + " " + e.Apellidos, e.Cedula, e.Email,
            u.Estado.CodigoEstado, u.Estado.DescripcionEstado,
            perfil?.Perfil.CodigoPerfil, perfil?.Perfil.DescripcionPerfil ?? "Sin perfil",
            u.PieFirma, u.IntentosFallidos, u.FechaCreacion, u.UltimoAcceso);
    }

    private static string? LimpiarPieFirma(string? pieFirma)
    {
        string valor = SeguridadUtil.Texto(pieFirma);
        if (valor.Length > 100)
        {
            throw new ReglaNegocioException("El pie de firma no puede superar 100 caracteres.");
        }
        return valor.Length == 0 ? null : valor;
    }
}
