// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Personal
using Microsoft.AspNetCore.Mvc;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Services;

namespace SIGH_DOTNET_GR07.Controllers;

/// <summary>API REST del CRUD de cargos (opción P002). Un cargo se identifica por departamento + código.</summary>
[ApiController]
[Route("personal/cargos/api")]
public class CargosApiController(CargoServicio servicio) : ControllerBase
{
    private string Actor => UsuarioSesion.Desde(User)!.CodigoUsuario;
    private string? Ip => SeguridadUtil.IpCliente(HttpContext);

    [HttpGet]
    public async Task<List<CargoDto>> Listar() => await servicio.ListarAsync();

    [HttpGet("departamentos")]
    public async Task<List<DepartamentoDto>> Departamentos() => await servicio.DepartamentosAsync();

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CargoFormDto datos) =>
        StatusCode(StatusCodes.Status201Created, await servicio.CrearAsync(datos, Actor, Ip));

    [HttpPut("{departamento}/{cargo}")]
    public async Task<MensajeDto> Actualizar(string departamento, string cargo, [FromBody] CargoFormDto datos)
    {
        await servicio.ActualizarAsync(departamento, cargo, datos, Actor, Ip);
        return new MensajeDto("Cargo actualizado");
    }

    [HttpDelete("{departamento}/{cargo}")]
    public async Task<MensajeDto> Eliminar(string departamento, string cargo)
    {
        await servicio.EliminarAsync(departamento, cargo, Actor, Ip);
        return new MensajeDto("Cargo eliminado");
    }
}
