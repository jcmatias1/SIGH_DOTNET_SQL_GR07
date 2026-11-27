// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Personal
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>Género del empleado (tabla PEGEN_GENER).</summary>
[Table("PEGEN_GENER")]
public class Genero
{
    [Key]
    [Column("PEGEN_CODIGO", TypeName = "char(1)")]
    public string CodigoGenero { get; set; } = "";

    [Column("PEGEN_DESCRI", TypeName = "varchar(20)")]
    public string Descripcion { get; set; } = "";
}
