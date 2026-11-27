// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.Collections.Concurrent;

namespace SIGH_DOTNET_GR07.Services;

/// <summary>
/// Registro en memoria de las sesiones abiertas y de su última actividad (servicio Singleton).
/// Permite cerrar la sesión exactamente a los 15 minutos de inactividad (RF-SEG-004) y marcar
/// en XESES_SESIO las sesiones que expiraron.
/// </summary>
public class RegistroSesiones
{
    /// <summary>Sesión abierta y momento de su última solicitud.</summary>
    public record Entrada(long CodigoSesion, string CodigoUsuario, string? DireccionIp, DateTime UltimaActividad);

    private readonly ConcurrentDictionary<long, Entrada> _sesiones = new();

    public void Registrar(long codigoSesion, string codigoUsuario, string? ip)
    {
        _sesiones[codigoSesion] = new Entrada(codigoSesion, codigoUsuario, ip, DateTime.Now);
    }

    /// <summary>
    /// Renueva la actividad de una sesión. Devuelve false si la sesión no está registrada
    /// (se cerró o se reinició el servidor) o si ya superó el tiempo de inactividad.
    /// </summary>
    public bool Tocar(long codigoSesion, TimeSpan inactividadMaxima)
    {
        if (!_sesiones.TryGetValue(codigoSesion, out Entrada? entrada))
        {
            return false;
        }
        if (DateTime.Now - entrada.UltimaActividad >= inactividadMaxima)
        {
            return false;
        }
        _sesiones[codigoSesion] = entrada with { UltimaActividad = DateTime.Now };
        return true;
    }

    public bool EstaActiva(long codigoSesion, TimeSpan inactividadMaxima) =>
        _sesiones.TryGetValue(codigoSesion, out Entrada? entrada)
        && DateTime.Now - entrada.UltimaActividad < inactividadMaxima;

    public void Quitar(long codigoSesion) => _sesiones.TryRemove(codigoSesion, out _);

    /// <summary>Saca del registro y devuelve las sesiones que superaron el tiempo de inactividad.</summary>
    public List<Entrada> TomarVencidas(TimeSpan inactividadMaxima)
    {
        var vencidas = new List<Entrada>();
        foreach (Entrada entrada in _sesiones.Values)
        {
            if (DateTime.Now - entrada.UltimaActividad >= inactividadMaxima && _sesiones.TryRemove(entrada.CodigoSesion, out _))
            {
                vencidas.Add(entrada);
            }
        }
        return vencidas;
    }

    /// <summary>Saca del registro todas las sesiones (al detener el servidor).</summary>
    public List<Entrada> TomarTodas()
    {
        var todas = _sesiones.Values.ToList();
        _sesiones.Clear();
        return todas;
    }
}
