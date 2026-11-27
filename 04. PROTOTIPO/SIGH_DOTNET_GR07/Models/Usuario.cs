// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>Cuenta de acceso al SIGH (tabla XEUSU_USUAR). La contraseña se guarda como hash SHA-256.</summary>
[Table("XEUSU_USUAR")]
public class Usuario
{
    [Key]
    [Column("XEUSU_CODIGO", TypeName = "varchar(20)")]
    public string CodigoUsuario { get; set; } = "";

    [Column("PEEMP_CODIGO", TypeName = "char(10)")]
    public string CodigoEmpleado { get; set; } = "";

    [Column("XEEST_CODIGO", TypeName = "char(1)")]
    public string CodigoEstado { get; set; } = "";

    [Column("XEUSU_PASSWO", TypeName = "varchar(255)")]
    public string Password { get; set; } = "";

    [Column("XEUSU_INTFAL")]
    public short IntentosFallidos { get; set; }

    [Column("XEUSU_FECCRE", TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    [Column("XEUSU_FECMOD", TypeName = "datetime")]
    public DateTime? FechaModificacion { get; set; }

    [Column("XEUSU_ULTACC", TypeName = "datetime")]
    public DateTime? UltimoAcceso { get; set; }

    [Column("XEUSU_PIEFIR", TypeName = "varchar(100)")]
    public string? PieFirma { get; set; }

    [ForeignKey(nameof(CodigoEmpleado))]
    public Empleado Empleado { get; set; } = null!;

    [ForeignKey(nameof(CodigoEstado))]
    public Estado Estado { get; set; } = null!;

    /// <summary>Historial de perfiles (XEUXP_USUPE); el vigente es el que no tiene fecha de retiro.</summary>
    public List<UsuPer> Perfiles { get; set; } = new();
}
