// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>Enlace temporal de un solo uso para restablecer la contraseña (tabla XEREC_RECUP).</summary>
[Table("XEREC_RECUP")]
public class TokenRecuperacion
{
    [Key]
    [Column("XEREC_CODIGO")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long CodigoToken { get; set; }

    [Column("XEUSU_CODIGO", TypeName = "varchar(20)")]
    public string CodigoUsuario { get; set; } = "";

    [Column("XEREC_TOKEN", TypeName = "varchar(120)")]
    public string Token { get; set; } = "";

    [Column("XEREC_FECCRE", TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    [Column("XEREC_FECEXP", TypeName = "datetime")]
    public DateTime FechaExpiracion { get; set; }

    [Column("XEREC_USADO")]
    public bool Usado { get; set; }

    [ForeignKey(nameof(CodigoUsuario))]
    public Usuario Usuario { get; set; } = null!;
}
