// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>Perfil o rol del SIGH (tabla XEPER_PERFI): ADM, MED, ENF, FAR...</summary>
[Table("XEPER_PERFI")]
public class Perfil
{
    [Key]
    [Column("XEPER_CODIGO", TypeName = "char(3)")]
    public string CodigoPerfil { get; set; } = "";

    [Column("XEPER_DESCRI", TypeName = "varchar(60)")]
    public string DescripcionPerfil { get; set; } = "";

    [Column("XEPER_OBSERV", TypeName = "varchar(200)")]
    public string? Observaciones { get; set; }

    public List<UsuPer> Usuarios { get; set; } = new();

    public List<OpcPer> Opciones { get; set; } = new();
}
