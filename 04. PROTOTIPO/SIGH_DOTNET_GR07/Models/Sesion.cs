// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>Sesión de trabajo (tabla XESES_SESIO). Estado: A abierta, C cerrada por el usuario, E expirada.</summary>
[Table("XESES_SESIO")]
public class Sesion
{
    [Key]
    [Column("XESES_CODIGO")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long CodigoSesion { get; set; }

    [Column("XEUSU_CODIGO", TypeName = "varchar(20)")]
    public string CodigoUsuario { get; set; } = "";

    [Column("XESES_FECINI", TypeName = "datetime")]
    public DateTime FechaInicio { get; set; }

    [Column("XESES_FECFIN", TypeName = "datetime")]
    public DateTime? FechaFin { get; set; }

    [Column("XESES_DIRIP", TypeName = "varchar(45)")]
    public string? DireccionIp { get; set; }

    [Column("XESES_ESTADO", TypeName = "char(1)")]
    public string EstadoSesion { get; set; } = "A";

    [ForeignKey(nameof(CodigoUsuario))]
    public Usuario Usuario { get; set; } = null!;
}
