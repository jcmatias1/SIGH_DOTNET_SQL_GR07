// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Personal
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>Estado civil del empleado (tabla PEESC_ESTCI).</summary>
[Table("PEESC_ESTCI")]
public class EstadoCivil
{
    [Key]
    [Column("PEESC_CODIGO", TypeName = "char(1)")]
    public string CodigoEstadoCivil { get; set; } = "";

    [Column("PEESC_DESCRI", TypeName = "varchar(20)")]
    public string Descripcion { get; set; } = "";
}
