// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Personal
using Microsoft.AspNetCore.Mvc;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Services;

namespace SIGH_DOTNET_GR07.Controllers;

/// <summary>API REST de Mantenimiento de empleados (opción P003). DELETE hace borrado lógico.</summary>
[ApiController]
[Route("personal/empleados/api")]
public class EmpleadosApiController(EmpleadoServicio servicio, CargoServicio cargos) : ControllerBase
{
    private string Actor => UsuarioSesion.Desde(User)!.CodigoUsuario;
    private string? Ip => SeguridadUtil.IpCliente(HttpContext);

    [HttpGet]
    public async Task<List<EmpleadoFichaDto>> Listar() => await servicio.ListarAsync();

    /// <summary>Combos del formulario y el código que tendrá el próximo empleado.</summary>
    [HttpGet("catalogos")]
    public async Task<object> Catalogos() => new
    {
        generos = await servicio.GenerosAsync(),
        estadosCiviles = await servicio.EstadosCivilesAsync(),
        cargos = await cargos.ListarAsync(),
        siguienteCodigo = await servicio.SiguienteCodigoAsync()
    };

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] EmpleadoFormDto datos) =>
        StatusCode(StatusCodes.Status201Created, await servicio.CrearAsync(datos, Actor, Ip));

    [HttpPut("{codigo}")]
    public async Task<MensajeDto> Actualizar(string codigo, [FromBody] EmpleadoFormDto datos)
    {
        await servicio.ActualizarAsync(codigo, datos, Actor, Ip);
        return new MensajeDto("Empleado actualizado");
    }

    [HttpDelete("{codigo}")]
    public async Task<MensajeDto> Borrar(string codigo)
    {
        await servicio.BorrarAsync(codigo, Actor, Ip);
        return new MensajeDto("Empleado desactivado");
    }
}
