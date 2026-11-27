// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.AspNetCore.Mvc;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Services;

namespace SIGH_DOTNET_GR07.Controllers;

/// <summary>API REST del CRUD de perfiles y de la asignación de opciones (opción S002).</summary>
[ApiController]
[Route("seguridad/perfiles/api")]
public class PerfilesApiController(PerfilServicio servicio) : ControllerBase
{
    private string Actor => UsuarioSesion.Desde(User)!.CodigoUsuario;
    private string? Ip => SeguridadUtil.IpCliente(HttpContext);

    [HttpGet]
    public async Task<List<PerfilDto>> Listar() => await servicio.ListarAsync();

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] PerfilFormDto datos) =>
        StatusCode(StatusCodes.Status201Created, await servicio.CrearAsync(datos, Actor, Ip));

    [HttpPut("{perfil}")]
    public async Task<MensajeDto> Actualizar(string perfil, [FromBody] PerfilFormDto datos)
    {
        await servicio.ActualizarAsync(perfil, datos, Actor, Ip);
        return new MensajeDto("Perfil actualizado");
    }

    [HttpDelete("{perfil}")]
    public async Task<MensajeDto> Eliminar(string perfil)
    {
        await servicio.EliminarAsync(perfil, Actor, Ip);
        return new MensajeDto("Perfil eliminado");
    }

    [HttpPost("{perfil}/opciones/{opcion}")]
    public async Task<MensajeDto> Asignar(string perfil, string opcion)
    {
        await servicio.AsignarAsync(perfil, opcion, Actor, Ip);
        return new MensajeDto("Opción asignada");
    }

    [HttpDelete("{perfil}/opciones/{opcion}")]
    public async Task<MensajeDto> Revocar(string perfil, string opcion)
    {
        await servicio.RevocarAsync(perfil, opcion, Actor, Ip);
        return new MensajeDto("Opción revocada");
    }
}
