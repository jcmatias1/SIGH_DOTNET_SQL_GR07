// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SIGH_DOTNET_GR07.Filters;
using SIGH_DOTNET_GR07.Services;

namespace SIGH_DOTNET_GR07.Controllers;

/// <summary>Caso de uso ECUD1 Iniciar sesión: muestra el formulario, valida las credenciales y abre la sesión.</summary>
[AccesoPublico]
public class AccesoController(
    AutenticacionServicio autenticacion,
    RegistroSesiones registro,
    IOptions<SighOpciones> opciones) : Controller
{
    [HttpGet("/")]
    public IActionResult Raiz() => SesionActiva() ? Redirect("~/inicio") : Redirect("~/login");

    /// <summary>Paso 1: el sistema muestra el formulario con los campos usuario y contraseña.</summary>
    [HttpGet("/login")]
    public async Task<IActionResult> Login()
    {
        if (SesionActiva())
        {
            return Redirect("~/inicio");
        }
        if (User.Identity?.IsAuthenticated == true)
        {
            // Cookie de una sesión que ya venció: se elimina.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        IQueryCollection consulta = Request.Query;
        if (consulta.ContainsKey("expirada"))
        {
            ViewData["Aviso"] = "Su sesión se cerró por inactividad. Inicie sesión nuevamente.";
        }
        else if (consulta.ContainsKey("salir"))
        {
            ViewData["Info"] = "Cerró sesión correctamente.";
        }
        else if (consulta.ContainsKey("restablecida"))
        {
            ViewData["Info"] = "Su contraseña fue actualizada. Ya puede iniciar sesión.";
        }
        return View();
    }

    /// <summary>Paso 2: valida credenciales y estado; si todo es correcto abre la sesión y registra el acceso.</summary>
    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromForm] string? codigoUsuario, [FromForm] string? password)
    {
        string? ip = SeguridadUtil.IpCliente(HttpContext);
        ResultadoLogin resultado = await autenticacion.AutenticarAsync(codigoUsuario, password, ip);
        if (!resultado.EsExitoso || resultado.Usuario is null)
        {
            ViewData["Error"] = resultado.Mensaje;
            ViewData["TipoError"] = resultado.Tipo.ToString();
            ViewData["CodigoUsuario"] = codigoUsuario;
            return View();
        }

        UsuarioSesion usuario = resultado.Usuario;
        // SignInAsync emite una cookie nueva en cada inicio de sesión (evita la fijación de sesión).
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            usuario.APrincipal(CookieAuthenticationDefaults.AuthenticationScheme),
            new AuthenticationProperties { IsPersistent = false });
        registro.Registrar(usuario.CodigoSesion, usuario.CodigoUsuario, usuario.DireccionIp);
        return Redirect("~/bienvenida");
    }

    private bool SesionActiva()
    {
        UsuarioSesion? usuario = UsuarioSesion.Desde(User);
        return usuario is not null
            && registro.EstaActiva(usuario.CodigoSesion, TimeSpan.FromMinutes(opciones.Value.MinutosSesion));
    }
}
