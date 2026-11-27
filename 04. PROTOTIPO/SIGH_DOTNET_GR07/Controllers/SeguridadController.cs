// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.AspNetCore.Mvc;

namespace SIGH_DOTNET_GR07.Controllers;

/// <summary>
/// Pantallas del subsistema de Seguridad. Cada ruta está protegida por su opción (S001, S002, S003);
/// los datos se cargan desde las API con fetch.
/// </summary>
public class SeguridadController : Controller
{
    [HttpGet("/seguridad/usuarios")]
    public IActionResult Usuarios() => Pantalla("Administrar usuarios", "S001");

    [HttpGet("/seguridad/perfiles")]
    public IActionResult Perfiles() => Pantalla("Administrar perfiles", "S002");

    [HttpGet("/seguridad/auditoria")]
    public IActionResult Auditoria() => Pantalla("Consultar auditoría", "S003");

    private ViewResult Pantalla(string titulo, string opcion)
    {
        ViewData["Titulo"] = titulo;
        ViewData["Activo"] = opcion;
        return View();
    }
}
