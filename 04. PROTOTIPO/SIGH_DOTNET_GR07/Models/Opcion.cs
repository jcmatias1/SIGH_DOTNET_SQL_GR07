// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>Opción del menú (tabla XEOPC_OPCIO). Su URL protege esa ruta y todas las que empiezan por ella.</summary>
[Table("XEOPC_OPCIO")]
public class Opcion
{
    [Key]
    [Column("XEOPC_CODIGO", TypeName = "char(4)")]
    public string CodigoOpcion { get; set; } = "";

    [Column("XESIS_CODIGO", TypeName = "char(3)")]
    public string CodigoSistema { get; set; } = "";

    [Column("XEOPC_DESCRI", TypeName = "varchar(80)")]
    public string DescripcionOpcion { get; set; } = "";

    [Column("XEOPC_URL", TypeName = "varchar(150)")]
    public string? UrlOpcion { get; set; }

    [ForeignKey(nameof(CodigoSistema))]
    public Sistema Sistema { get; set; } = null!;
}
