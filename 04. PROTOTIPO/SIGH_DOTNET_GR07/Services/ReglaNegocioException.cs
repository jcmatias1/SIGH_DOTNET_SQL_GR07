// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
namespace SIGH_DOTNET_GR07.Services;

/// <summary>Regla de negocio incumplida. Las API la convierten en HTTP 400 con { "mensaje": "..." }.</summary>
public class ReglaNegocioException(string mensaje) : Exception(mensaje);
