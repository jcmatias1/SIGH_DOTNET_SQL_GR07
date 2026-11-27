// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.Extensions.Options;

namespace SIGH_DOTNET_GR07.Services;

/// <summary>
/// Tarea en segundo plano (RF-SEG-004): cada 30 segundos marca como expiradas (E) en XESES_SESIO
/// las sesiones con más de 15 minutos sin actividad. Al detener el servidor cierra las que queden.
/// </summary>
public class CierreSesionesServicio(
    RegistroSesiones registro,
    IServiceScopeFactory fabrica,
    IOptions<SighOpciones> opciones,
    ILogger<CierreSesionesServicio> log) : BackgroundService
{
    private readonly TimeSpan _inactividad = TimeSpan.FromMinutes(opciones.Value.MinutosSesion);

    protected override async Task ExecuteAsync(CancellationToken detener)
    {
        await ExpirarHuerfanasAsync();
        using var temporizador = new PeriodicTimer(TimeSpan.FromSeconds(30));
        try
        {
            while (await temporizador.WaitForNextTickAsync(detener))
            {
                foreach (RegistroSesiones.Entrada sesion in registro.TomarVencidas(_inactividad))
                {
                    await CerrarAsync(sesion, "Sesión " + sesion.CodigoSesion + " cerrada por "
                        + _inactividad.TotalMinutes + " minutos de inactividad");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // El servidor se está deteniendo.
        }
    }

    public override async Task StopAsync(CancellationToken cancelar)
    {
        foreach (RegistroSesiones.Entrada sesion in registro.TomarTodas())
        {
            await CerrarAsync(sesion, "Sesión " + sesion.CodigoSesion + " finalizada al detener el servidor");
        }
        await base.StopAsync(cancelar);
    }

    private async Task ExpirarHuerfanasAsync()
    {
        try
        {
            using IServiceScope alcance = fabrica.CreateScope();
            var autenticacion = alcance.ServiceProvider.GetRequiredService<AutenticacionServicio>();
            int cerradas = await autenticacion.ExpirarSesionesHuerfanasAsync();
            if (cerradas > 0)
            {
                log.LogInformation("[SIGH] {Cantidad} sesión(es) abierta(s) de una ejecución anterior se marcaron como expiradas", cerradas);
            }
        }
        catch (Exception ex)
        {
            log.LogWarning("[SIGH] No se pudo revisar XESES_SESIO al iniciar: {Error}", ex.Message);
        }
    }

    private async Task CerrarAsync(RegistroSesiones.Entrada sesion, string detalle)
    {
        try
        {
            using IServiceScope alcance = fabrica.CreateScope();
            var autenticacion = alcance.ServiceProvider.GetRequiredService<AutenticacionServicio>();
            await autenticacion.CerrarSesionAsync(sesion.CodigoSesion, sesion.CodigoUsuario, sesion.DireccionIp, "E", detalle);
        }
        catch (Exception ex)
        {
            log.LogWarning("[SIGH] No se pudo registrar el fin de la sesión {Sesion}: {Error}", sesion.CodigoSesion, ex.Message);
        }
    }
}
