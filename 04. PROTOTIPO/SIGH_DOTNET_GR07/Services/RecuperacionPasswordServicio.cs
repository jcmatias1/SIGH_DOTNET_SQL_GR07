// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SIGH_DOTNET_GR07.Data;
using SIGH_DOTNET_GR07.Models;

namespace SIGH_DOTNET_GR07.Services;

/// <summary>Resultado de la solicitud: si se permitió y el enlace (solo en modo taller).</summary>
public record SolicitudRecuperacion(bool Permitida, string? EnlaceDemo);

/// <summary>
/// Flujo RECUPERAR CONTRASEÑA de ECUD1 (RF-SEG-005): genera un enlace temporal de un solo uso,
/// lo envía al correo institucional del empleado y permite fijar una contraseña nueva que cumpla
/// la política. La contraseña actual nunca se muestra ni se envía.
/// </summary>
public class RecuperacionPasswordServicio(
    SighDbContext db,
    AuditoriaServicio auditoria,
    IOptions<SighOpciones> opciones,
    IOptions<CorreoOpciones> correo,
    ILogger<RecuperacionPasswordServicio> log)
{
    private const int MaxSolicitudes = 5;
    private const int HorasVentana = 5;

    private readonly SighOpciones _config = opciones.Value;
    private readonly CorreoOpciones _correo = correo.Value;

    /// <summary>
    /// El usuario escribe su usuario o su correo. La respuesta en pantalla es la misma exista o no
    /// la cuenta, para no revelar qué cuentas existen.
    /// </summary>
    public async Task<SolicitudRecuperacion> SolicitarAsync(string? identificador, string urlBase, string? ip)
    {
        string valor = SeguridadUtil.Texto(identificador);
        if (valor.Length == 0)
        {
            return new SolicitudRecuperacion(true, null);
        }

        IQueryable<Usuario> consulta = db.Usuarios.Include(u => u.Empleado);
        Usuario? usuario = valor.Contains('@')
            ? await consulta.FirstOrDefaultAsync(u => u.Empleado.Email == valor)
            : await consulta.FirstOrDefaultAsync(u => u.CodigoUsuario == valor);

        if (usuario is null)
        {
            await auditoria.RegistrarYGuardarAsync(null, "RECUPERAR CONTRASEÑA", "FALLIDO", "Identificador no registrado: " + valor, ip);
            return new SolicitudRecuperacion(true, null);
        }
        if (usuario.CodigoEstado == AutenticacionServicio.EstadoInactivo)
        {
            await auditoria.RegistrarYGuardarAsync(usuario.CodigoUsuario, "RECUPERAR CONTRASEÑA", "DENEGADO", "Cuenta desactivada", ip);
            return new SolicitudRecuperacion(true, null);
        }

        DateTime ahora = DateTime.Now;
        DateTime desde = ahora.AddHours(-HorasVentana);
        int recientes = await db.TokensRecuperacion.CountAsync(t => t.CodigoUsuario == usuario.CodigoUsuario && t.FechaCreacion > desde);
        if (recientes >= MaxSolicitudes)
        {
            await auditoria.RegistrarYGuardarAsync(usuario.CodigoUsuario, "RECUPERAR CONTRASEÑA", "DENEGADO",
                "Se superó el límite de " + MaxSolicitudes + " enlaces en " + HorasVentana + " horas", ip);
            return new SolicitudRecuperacion(false, null);
        }

        var token = new TokenRecuperacion
        {
            CodigoUsuario = usuario.CodigoUsuario,
            Token = SeguridadUtil.GenerarToken(),
            FechaCreacion = ahora,
            FechaExpiracion = ahora.AddMinutes(_config.MinutosVigenciaEnlace),
            Usado = false
        };
        db.TokensRecuperacion.Add(token);

        string enlace = urlBase + "/restablecer-password?token=" + token.Token;
        string destino = usuario.Empleado.Email;
        bool enviado = await EnviarCorreoAsync(destino, usuario.CodigoUsuario, enlace);
        await auditoria.RegistrarAsync(usuario.CodigoUsuario, "RECUPERAR CONTRASEÑA", "SOLICITADO",
            (enviado ? "Enlace enviado a " : "Enlace generado (correo no configurado) para ") + destino, ip);
        await db.SaveChangesAsync();
        return new SolicitudRecuperacion(true, _config.MostrarEnlaceRecuperacion && !enviado ? enlace : null);
    }

    public async Task<bool> TokenValidoAsync(string? token) => await BuscarTokenValidoAsync(token) is not null;

    /// <summary>Devuelve null si se cambió la contraseña o el mensaje del error.</summary>
    public async Task<string?> RestablecerAsync(string? token, string? password, string? confirmacion, string? ip)
    {
        TokenRecuperacion? recuperacion = await BuscarTokenValidoAsync(token);
        if (recuperacion is null)
        {
            return "El enlace no es válido o ya venció. Solicite uno nuevo.";
        }
        if (password is null || password != confirmacion)
        {
            return "Las contraseñas no coinciden.";
        }
        string? error = SeguridadUtil.ValidarPolitica(password);
        if (error is not null)
        {
            return error;
        }
        Usuario usuario = recuperacion.Usuario;
        if (SeguridadUtil.Coincide(password, usuario.Password))
        {
            return "La nueva contraseña debe ser diferente a la anterior.";
        }
        usuario.Password = SeguridadUtil.Sha256(password);
        usuario.FechaModificacion = DateTime.Now;
        usuario.IntentosFallidos = 0;
        if (usuario.CodigoEstado == AutenticacionServicio.EstadoBloqueado)
        {
            usuario.Estado = await db.Estados.FindAsync(AutenticacionServicio.EstadoActivo)
                ?? throw new InvalidOperationException("No existe el estado A");
        }
        recuperacion.Usado = true;
        await auditoria.RegistrarAsync(usuario.CodigoUsuario, "RESTABLECER CONTRASEÑA", "EXITOSO",
            "Contraseña cambiada con enlace de recuperación", ip);
        await db.SaveChangesAsync();
        return null;
    }

    private async Task<TokenRecuperacion?> BuscarTokenValidoAsync(string? token)
    {
        string valor = SeguridadUtil.Texto(token);
        if (valor.Length == 0)
        {
            return null;
        }
        DateTime ahora = DateTime.Now;
        return await db.TokensRecuperacion
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.Token == valor && !t.Usado && t.FechaExpiracion > ahora
                                      && t.Usuario.CodigoEstado != AutenticacionServicio.EstadoInactivo);
    }

    private async Task<bool> EnviarCorreoAsync(string destino, string codigoUsuario, string enlace)
    {
        if (string.IsNullOrWhiteSpace(_correo.Usuario) || string.IsNullOrWhiteSpace(_correo.Host))
        {
            log.LogInformation("[SIGH] Enlace de recuperación para {Usuario} ({Correo}): {Enlace}", codigoUsuario, destino, enlace);
            return false;
        }
        try
        {
            using var cliente = new SmtpClient(_correo.Host, _correo.Puerto)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_correo.Usuario, _correo.Clave)
            };
            using var mensaje = new MailMessage(_correo.Usuario, destino)
            {
                Subject = "SIGH - Restablecer contraseña",
                Body = CorreoHtml(codigoUsuario, enlace),
                IsBodyHtml = true
            };
            await cliente.SendMailAsync(mensaje);
            return true;
        }
        catch (Exception ex) when (ex is SmtpException or InvalidOperationException or FormatException)
        {
            log.LogWarning("[SIGH] No se pudo enviar el correo ({Error}). Enlace para {Usuario}: {Enlace}", ex.Message, codigoUsuario, enlace);
            return false;
        }
    }

    private string CorreoHtml(string codigoUsuario, string enlace) => $"""
        <div style="font-family:Segoe UI,Arial,sans-serif;background:#eef3f9;padding:28px">
          <div style="max-width:560px;margin:auto;background:#fff;border-radius:10px;overflow:hidden;border:1px solid #d6e0ec">
            <div style="background:#0b3d75;color:#fff;padding:20px 26px">
              <div style="font-size:20px;font-weight:700">SIGH · Hospital General Monster Salud</div>
              <div style="font-size:12px;opacity:.8">Sistema Integral de Gestión Hospitalaria</div>
            </div>
            <div style="padding:26px;color:#1f2d3d;line-height:1.6">
              <h2 style="margin-top:0;color:#0b3d75">Restablecer contraseña</h2>
              <p>Se solicitó restablecer la contraseña del usuario <b>{WebUtility.HtmlEncode(codigoUsuario)}</b>.</p>
              <p>El enlace vence en <b>{_config.MinutosVigenciaEnlace} minutos</b> y solo puede usarse una vez.</p>
              <p style="text-align:center;margin:26px 0">
                <a href="{enlace}" style="background:#0b5cad;color:#fff;padding:12px 22px;border-radius:6px;text-decoration:none;font-weight:600">Restablecer contraseña</a>
              </p>
              <p style="font-size:12px;color:#5b6b7c">Si usted no lo solicitó, ignore este correo. El SIGH nunca le pedirá su contraseña.</p>
            </div>
          </div>
        </div>
        """;
}
