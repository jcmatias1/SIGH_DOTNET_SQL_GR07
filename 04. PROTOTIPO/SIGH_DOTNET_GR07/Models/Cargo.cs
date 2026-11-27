// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Personal
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>
/// Cargo del personal (tabla PECAR_CARGO). Su clave es departamento + código (FK_DEP_CAR):
/// el mismo código puede repetirse en departamentos distintos.
/// </summary>
[Table("PECAR_CARGO")]
public class Cargo
{
    [Column("PEDEP_CODIGO", TypeName = "char(3)")]
    public string CodigoDepartamento { get; set; } = "";

    [Column("PECAR_CODIGO", TypeName = "char(3)")]
    public string CodigoCargo { get; set; } = "";

    [Column("PECAR_DESCRI", TypeName = "varchar(60)")]
    public string Descripcion { get; set; } = "";

    [ForeignKey(nameof(CodigoDepartamento))]
    public Departamento Departamento { get; set; } = null!;
}
