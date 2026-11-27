// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Personal
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>Departamento del hospital (tabla PEDEP_DEPAR). Agrupa los cargos del personal.</summary>
[Table("PEDEP_DEPAR")]
public class Departamento
{
    [Key]
    [Column("PEDEP_CODIGO", TypeName = "char(3)")]
    public string CodigoDepartamento { get; set; } = "";

    [Column("PEDEP_DESCRI", TypeName = "varchar(60)")]
    public string Descripcion { get; set; } = "";

    public List<Cargo> Cargos { get; set; } = new();
}
