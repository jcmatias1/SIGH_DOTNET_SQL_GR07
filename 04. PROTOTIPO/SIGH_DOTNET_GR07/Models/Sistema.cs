// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>Sistema o módulo del hospital (tabla XESIS_SISTE) que agrupa opciones del menú.</summary>
[Table("XESIS_SISTE")]
public class Sistema
{
    [Key]
    [Column("XESIS_CODIGO", TypeName = "char(3)")]
    public string CodigoSistema { get; set; } = "";

    [Column("XESIS_DESCRI", TypeName = "varchar(60)")]
    public string DescripcionSistema { get; set; } = "";
}
