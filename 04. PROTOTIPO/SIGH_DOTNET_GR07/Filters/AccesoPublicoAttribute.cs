// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
namespace SIGH_DOTNET_GR07.Filters;

/// <summary>Marca los controladores que se abren sin sesión: login y recuperación de contraseña.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AccesoPublicoAttribute : Attribute;
