// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SIGH_DOTNET_GR07.Services;

namespace SIGH_DOTNET_GR07.Controllers;

/// <summary>Paso 3 de ECUD1 (menú por perfil) y flujo CERRAR SESIÓN. Requiere sesión (lo exige SeguridadFiltro).</summary>
public class InicioController(
    AutenticacionServicio autenticacion,
    MenuServicio menu,
    RegistroSesiones registro) : Controller
{
    [HttpGet("/bienvenida")]
    public IActionResult Bienvenida() => View(UsuarioSesion.Desde(User));

    [HttpGet("/inicio")]
    public async Task<IActionResult> Index()
    {
        UsuarioSesion usuario = UsuarioSesion.Desde(User)!;
        ViewData["Titulo"] = "Inicio";
        ViewData["Activo"] = "INICIO";
        ViewBag.Menu = await menu.MenuAsync(usuario.CodigoUsuario);
        ViewBag.Sesiones = await autenticacion.UltimasSesionesAsync(usuario.CodigoUsuario);
        return View(usuario);
    }

    /// <summary>CERRAR SESIÓN: cierra el registro en XESES_SESIO, lo audita y vuelve al login.</summary>
    [HttpPost("/logout")]
    public async Task<IActionResult> Logout()
    {
        UsuarioSesion? usuario = UsuarioSesion.Desde(User);
        if (usuario is not null)
        {
            await autenticacion.CerrarSesionAsync(usuario.CodigoSesion, usuario.CodigoUsuario, SeguridadUtil.IpCliente(HttpContext),
                "C", "Sesión " + usuario.CodigoSesion + " cerrada por el usuario");
            registro.Quitar(usuario.CodigoSesion);
        }
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("~/login?salir");
    }

    [HttpGet("/logout")]
    public IActionResult LogoutGet() => Redirect("~/inicio");
}
