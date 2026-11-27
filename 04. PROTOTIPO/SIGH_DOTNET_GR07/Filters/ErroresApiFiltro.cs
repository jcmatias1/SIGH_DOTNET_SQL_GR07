// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Services;

namespace SIGH_DOTNET_GR07.Filters;

/// <summary>Convierte los errores de las API en respuestas JSON { "mensaje": "..." } que muestra SweetAlert2.</summary>
public class ErroresApiFiltro(ILogger<ErroresApiFiltro> log) : IExceptionFilter
{
    public void OnException(ExceptionContext contexto)
    {
        switch (contexto.Exception)
        {
            case ReglaNegocioException regla:
                contexto.Result = new BadRequestObjectResult(new MensajeDto(regla.Message));
                contexto.ExceptionHandled = true;
                break;
            case DbUpdateException bd:
                log.LogWarning("[SIGH] Error de base de datos: {Error}", bd.InnerException?.Message ?? bd.Message);
                contexto.Result = new ConflictObjectResult(new MensajeDto("La operación viola una restricción de la base de datos."));
                contexto.ExceptionHandled = true;
                break;
        }
    }
}
