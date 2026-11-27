// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.AspNetCore.Mvc;
using SIGH_DOTNET_GR07.Filters;
using SIGH_DOTNET_GR07.Services;

namespace SIGH_DOTNET_GR07.Controllers;

/// <summary>Flujo alterno RECUPERAR CONTRASEÑA de ECUD1 (RF-SEG-005).</summary>
[AccesoPublico]
public class RecuperacionController(RecuperacionPasswordServicio recuperacion) : Controller
{
    [HttpGet("/recuperar-password")]
    public IActionResult Solicitar() => View();

    [HttpPost("/recuperar-password")]
    public async Task<IActionResult> Solicitar([FromForm] string? identificador)
    {
        string urlBase = Request.Scheme + "://" + Request.Host + Request.PathBase;
        SolicitudRecuperacion solicitud = await recuperacion.SolicitarAsync(identificador, urlBase, SeguridadUtil.IpCliente(HttpContext));
        if (!solicitud.Permitida)
        {
            ViewData["Error"] = "Se enviaron demasiados enlaces de recuperación. Intente nuevamente más tarde.";
        }
        else
        {
            ViewData["Mensaje"] = "Si los datos corresponden a una cuenta habilitada, enviamos un enlace temporal a su "
                + "correo institucional. Revise su bandeja de entrada.";
            ViewData["EnlaceDemo"] = solicitud.EnlaceDemo;
        }
        ViewData["Identificador"] = identificador;
        return View();
    }

    [HttpGet("/restablecer-password")]
    public async Task<IActionResult> Restablecer([FromQuery] string? token)
    {
        ViewData["Token"] = token;
        ViewData["TokenValido"] = await recuperacion.TokenValidoAsync(token);
        return View();
    }

    [HttpPost("/restablecer-password")]
    public async Task<IActionResult> Restablecer([FromForm] string? token, [FromForm] string? password, [FromForm] string? confirmacion)
    {
        string? error = await recuperacion.RestablecerAsync(token, password, confirmacion, SeguridadUtil.IpCliente(HttpContext));
        if (error is null)
        {
            return Redirect("~/login?restablecida");
        }
        ViewData["Token"] = token;
        ViewData["TokenValido"] = await recuperacion.TokenValidoAsync(token);
        ViewData["Error"] = error;
        return View();
    }
}
