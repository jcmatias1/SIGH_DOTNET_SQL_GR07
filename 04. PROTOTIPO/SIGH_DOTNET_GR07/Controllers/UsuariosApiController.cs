// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.AspNetCore.Mvc;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Services;

namespace SIGH_DOTNET_GR07.Controllers;

/// <summary>API REST del CRUD de usuarios (opción S001). Responde JSON a la vista Usuarios.</summary>
[ApiController]
[Route("seguridad/usuarios/api")]
public class UsuariosApiController(UsuarioServicio servicio) : ControllerBase
{
    private string Actor => UsuarioSesion.Desde(User)!.CodigoUsuario;
    private string? Ip => SeguridadUtil.IpCliente(HttpContext);

    [HttpGet]
    public async Task<List<UsuarioDto>> Listar() => await servicio.ListarAsync();

    [HttpGet("empleados")]
    public async Task<List<EmpleadoDto>> EmpleadosSinCuenta() => await servicio.EmpleadosSinCuentaAsync();

    [HttpGet("perfiles")]
    public async Task<List<PerfilSimpleDto>> Perfiles() => await servicio.PerfilesAsync();

    [HttpGet("{codigo}")]
    public async Task<UsuarioDetalleDto> Detalle(string codigo) => await servicio.DetalleAsync(codigo);

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] UsuarioFormDto datos) =>
        StatusCode(StatusCodes.Status201Created, await servicio.CrearAsync(datos, Actor, Ip));

    [HttpPut("{codigo}")]
    public async Task<UsuarioDto> Actualizar(string codigo, [FromBody] UsuarioFormDto datos) =>
        await servicio.ActualizarAsync(codigo, datos, Actor, Ip);

    /// <summary>accion: desactivar | activar | desbloquear (el borrado de usuarios es lógico).</summary>
    [HttpPatch("{codigo}/{accion}")]
    public async Task<UsuarioDto> CambiarEstado(string codigo, string accion) =>
        await servicio.CambiarEstadoAsync(codigo, accion, Actor, Ip);
}
