// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Personal
using Microsoft.AspNetCore.Mvc;

namespace SIGH_DOTNET_GR07.Controllers;

/// <summary>
/// Pantallas del subsistema de Personal. Cada ruta está protegida por su opción (P001, P002, P003);
/// los datos se cargan desde las API con fetch.
/// </summary>
public class PersonalController : Controller
{
    [HttpGet("/personal/departamentos")]
    public IActionResult Departamentos() => Pantalla("Administrar departamentos", "P001");

    [HttpGet("/personal/cargos")]
    public IActionResult Cargos() => Pantalla("Administrar cargos", "P002");

    [HttpGet("/personal/empleados")]
    public IActionResult Empleados() => Pantalla("Mantenimiento de empleados", "P003");

    private ViewResult Pantalla(string titulo, string opcion)
    {
        ViewData["Titulo"] = titulo;
        ViewData["Activo"] = opcion;
        return View();
    }
}
