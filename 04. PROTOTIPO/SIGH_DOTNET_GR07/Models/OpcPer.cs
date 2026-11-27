// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>Permiso: opción asignada a un perfil (tabla XEOXP_OPCPE). Revocar solo llena la fecha de retiro.</summary>
[Table("XEOXP_OPCPE")]
public class OpcPer
{
    [Column("XEPER_CODIGO", TypeName = "char(3)")]
    public string CodigoPerfil { get; set; } = "";

    [Column("XEOPC_CODIGO", TypeName = "char(4)")]
    public string CodigoOpcion { get; set; } = "";

    [Column("XEOXP_FECASI", TypeName = "datetime")]
    public DateTime FechaAsignacion { get; set; }

    [Column("XEOXP_FECRET", TypeName = "datetime")]
    public DateTime? FechaRetiro { get; set; }

    public Perfil Perfil { get; set; } = null!;

    public Opcion Opcion { get; set; } = null!;
}
