// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.Globalization;
using System.Security.Claims;

namespace SIGH_DOTNET_GR07.Services;

/// <summary>
/// Datos del usuario que inició sesión. Viajan como claims dentro de la cookie de autenticación
/// (cifrada y firmada por ASP.NET Core), equivalentes al atributo de sesión del taller Java.
/// </summary>
public class UsuarioSesion
{
    private const string Formato = "dd/MM/yyyy HH:mm";

    public string CodigoUsuario { get; init; } = "";
    public string NombreCompleto { get; init; } = "";
    public string Iniciales { get; init; } = "";
    public string? PieFirma { get; init; }
    public List<string> Perfiles { get; init; } = new();
    public long CodigoSesion { get; init; }
    public DateTime InicioSesion { get; init; }
    public DateTime? UltimoAccesoAnterior { get; init; }
    public string? DireccionIp { get; init; }

    public string PerfilesTexto => string.Join(", ", Perfiles);
    public string InicioSesionTexto => InicioSesion.ToString(Formato, CultureInfo.InvariantCulture);
    public string UltimoAccesoTexto => UltimoAccesoAnterior?.ToString(Formato, CultureInfo.InvariantCulture) ?? "Primer acceso";

    /// <summary>Convierte los datos en la identidad que se guarda en la cookie.</summary>
    public ClaimsPrincipal APrincipal(string esquema)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, CodigoUsuario),
            new("sigh:nombre", NombreCompleto),
            new("sigh:iniciales", Iniciales),
            new("sigh:pieFirma", PieFirma ?? ""),
            new("sigh:perfiles", string.Join("|", Perfiles)),
            new("sigh:sesion", CodigoSesion.ToString(CultureInfo.InvariantCulture)),
            new("sigh:inicio", InicioSesion.ToString("o", CultureInfo.InvariantCulture)),
            new("sigh:accesoAnterior", UltimoAccesoAnterior?.ToString("o", CultureInfo.InvariantCulture) ?? ""),
            new("sigh:ip", DireccionIp ?? "")
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, esquema));
    }

    /// <summary>Reconstruye los datos desde la cookie; null si no hay un usuario autenticado.</summary>
    public static UsuarioSesion? Desde(ClaimsPrincipal? principal)
    {
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }
        string Valor(string tipo) => principal.FindFirst(tipo)?.Value ?? "";
        if (!long.TryParse(Valor("sigh:sesion"), NumberStyles.Integer, CultureInfo.InvariantCulture, out long sesion))
        {
            return null;
        }
        DateTime.TryParse(Valor("sigh:inicio"), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime inicio);
        DateTime? anterior = DateTime.TryParse(Valor("sigh:accesoAnterior"), CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind, out DateTime a) ? a : null;
        string pie = Valor("sigh:pieFirma");
        string ip = Valor("sigh:ip");
        return new UsuarioSesion
        {
            CodigoUsuario = Valor(ClaimTypes.Name),
            NombreCompleto = Valor("sigh:nombre"),
            Iniciales = Valor("sigh:iniciales"),
            PieFirma = pie.Length == 0 ? null : pie,
            Perfiles = Valor("sigh:perfiles").Split('|', StringSplitOptions.RemoveEmptyEntries).ToList(),
            CodigoSesion = sesion,
            InicioSesion = inicio,
            UltimoAccesoAnterior = anterior,
            DireccionIp = ip.Length == 0 ? null : ip
        };
    }
}
