// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>Evento de auditoría (tabla XEAUD_AUDIT). El usuario es nulo cuando la cuenta escrita no existe.</summary>
[Table("XEAUD_AUDIT")]
public class Auditoria
{
    [Key]
    [Column("XEAUD_CODIGO")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long CodigoEvento { get; set; }

    [Column("XEUSU_CODIGO", TypeName = "varchar(20)")]
    public string? CodigoUsuario { get; set; }

    [Column("XEAUD_FECHOR", TypeName = "datetime")]
    public DateTime FechaHora { get; set; }

    [Column("XEAUD_MODULO", TypeName = "varchar(40)")]
    public string Modulo { get; set; } = "";

    [Column("XEAUD_OPERAC", TypeName = "varchar(40)")]
    public string Operacion { get; set; } = "";

    [Column("XEAUD_RESULT", TypeName = "varchar(15)")]
    public string Resultado { get; set; } = "";

    [Column("XEAUD_DETALL", TypeName = "varchar(500)")]
    public string? Detalle { get; set; }

    [Column("XEAUD_DIRIP", TypeName = "varchar(45)")]
    public string? DireccionIp { get; set; }

    [ForeignKey(nameof(CodigoUsuario))]
    public Usuario? Usuario { get; set; }
}
