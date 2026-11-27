// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>
/// Asignación de un perfil a un usuario (tabla XEUXP_USUPE). La clave compuesta
/// (usuario, perfil) se define en SighDbContext. Retirar un perfil solo llena la fecha de retiro.
/// </summary>
[Table("XEUXP_USUPE")]
public class UsuPer
{
    [Column("XEUSU_CODIGO", TypeName = "varchar(20)")]
    public string CodigoUsuario { get; set; } = "";

    [Column("XEPER_CODIGO", TypeName = "char(3)")]
    public string CodigoPerfil { get; set; } = "";

    [Column("XEUXP_FECASI", TypeName = "datetime")]
    public DateTime FechaAsignacion { get; set; }

    [Column("XEUXP_FECRET", TypeName = "datetime")]
    public DateTime? FechaRetiro { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public Perfil Perfil { get; set; } = null!;
}
