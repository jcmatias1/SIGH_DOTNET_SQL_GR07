// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Personal
using Microsoft.AspNetCore.Mvc;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Services;

namespace SIGH_DOTNET_GR07.Controllers;

/// <summary>API REST del CRUD de departamentos (opción P001).</summary>
[ApiController]
[Route("personal/departamentos/api")]
public class DepartamentosApiController(DepartamentoServicio servicio) : ControllerBase
{
    private string Actor => UsuarioSesion.Desde(User)!.CodigoUsuario;
    private string? Ip => SeguridadUtil.IpCliente(HttpContext);

    [HttpGet]
    public async Task<List<DepartamentoFilaDto>> Listar() => await servicio.ListarAsync();

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] DepartamentoFormDto datos) =>
        StatusCode(StatusCodes.Status201Created, await servicio.CrearAsync(datos, Actor, Ip));

    [HttpPut("{departamento}")]
    public async Task<MensajeDto> Actualizar(string departamento, [FromBody] DepartamentoFormDto datos)
    {
        await servicio.ActualizarAsync(departamento, datos, Actor, Ip);
        return new MensajeDto("Departamento actualizado");
    }

    [HttpDelete("{departamento}")]
    public async Task<MensajeDto> Eliminar(string departamento)
    {
        await servicio.EliminarAsync(departamento, Actor, Ip);
        return new MensajeDto("Departamento eliminado");
    }
}
