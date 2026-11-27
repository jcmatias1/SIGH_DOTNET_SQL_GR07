// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>Estado de una cuenta de usuario (tabla XEEST_ESTAD): A activo, I inactivo, B bloqueado.</summary>
[Table("XEEST_ESTAD")]
public class Estado
{
    [Key]
    [Column("XEEST_CODIGO", TypeName = "char(1)")]
    public string CodigoEstado { get; set; } = "";

    [Column("XEEST_DESCRI", TypeName = "varchar(30)")]
    public string DescripcionEstado { get; set; } = "";
}
