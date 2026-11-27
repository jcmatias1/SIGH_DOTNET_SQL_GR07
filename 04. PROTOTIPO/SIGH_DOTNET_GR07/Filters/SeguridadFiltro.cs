// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Models;
using SIGH_DOTNET_GR07.Services;

namespace SIGH_DOTNET_GR07.Filters;

/// <summary>
/// Filtro global que protege todas las páginas y API (equivale al interceptor del taller Java):
/// 1) exige una sesión activa y la cierra tras 15 minutos de inactividad (RF-SEG-004);
/// 2) busca la opción cuya URL protege la ruta y comprueba que algún perfil vigente del usuario
///    la tenga (RF-SEG-003); si no, responde 403 y lo deja en la auditoría (RF-AUD-004).
/// </summary>
public class SeguridadFiltro(
    MenuServicio menu,
    AuditoriaServicio auditoria,
    RegistroSesiones registro,
    IOptions<SighOpciones> opciones) : IAsyncActionFilter
{
    public const string NombreCookie = ".SIGH.Sesion";

    public async Task OnActionExecutionAsync(ActionExecutingContext contexto, ActionExecutionDelegate siguiente)
    {
        HttpContext http = contexto.HttpContext;

        // Las páginas protegidas no se guardan en caché: tras cerrar sesión, «Atrás» no las muestra.
        http.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate";
        http.Response.Headers.Pragma = "no-cache";

        if (contexto.ActionDescriptor.EndpointMetadata.OfType<AccesoPublicoAttribute>().Any())
        {
            await siguiente();
            return;
        }

        string ruta = http.Request.Path.Value ?? "/";
        bool esApi = ruta.Contains("/api", StringComparison.OrdinalIgnoreCase);
        TimeSpan inactividad = TimeSpan.FromMinutes(opciones.Value.MinutosSesion);

        UsuarioSesion? usuario = UsuarioSesion.Desde(http.User);
        bool expirada = false;
        if (usuario is not null && !registro.Tocar(usuario.CodigoSesion, inactividad))
        {
            // La cookie existe pero la sesión ya venció o el servidor se reinició.
            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            usuario = null;
            expirada = true;
        }
        else if (usuario is null)
        {
            expirada = http.Request.Cookies.ContainsKey(NombreCookie);
        }

        if (usuario is null)
        {
            contexto.Result = esApi
                ? new ObjectResult(new MensajeDto("Su sesión terminó. Inicie sesión nuevamente.")) { StatusCode = StatusCodes.Status401Unauthorized }
                : new RedirectResult(expirada ? "~/login?expirada" : "~/login");
            return;
        }

        Opcion? opcion = await menu.OpcionRequeridaAsync(ruta);
        if (opcion is not null && !await menu.TieneOpcionAsync(usuario.CodigoUsuario, opcion.CodigoOpcion))
        {
            await auditoria.RegistrarYGuardarAsync(usuario.CodigoUsuario, "ACCESO DENEGADO", "DENEGADO",
                http.Request.Method + " " + ruta + " requiere la opción " + opcion.CodigoOpcion, SeguridadUtil.IpCliente(http));
            if (esApi || contexto.Controller is not Controller controlador)
            {
                contexto.Result = new ObjectResult(new MensajeDto("Su perfil no tiene permiso para esta operación."))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
            else
            {
                controlador.ViewData["Titulo"] = "Acceso denegado";
                controlador.ViewData["OpcionDenegada"] = opcion.DescripcionOpcion;
                ViewResult vista = controlador.View("AccesoDenegado");
                vista.StatusCode = StatusCodes.Status403Forbidden;
                contexto.Result = vista;
            }
            return;
        }

        await siguiente();
    }
}
