// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.AspNetCore.Mvc;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Services;

namespace SIGH_DOTNET_GR07.Controllers;

/// <summary>API de solo lectura de la auditoría (opción S003).</summary>
[ApiController]
[Route("seguridad/auditoria/api")]
public class AuditoriaApiController(AuditoriaServicio servicio) : ControllerBase
{
    [HttpGet]
    public async Task<List<AuditoriaDto>> Listar() => await servicio.ListarRecientesAsync();
}
