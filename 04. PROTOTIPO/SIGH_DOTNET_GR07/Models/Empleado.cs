// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_DOTNET_GR07.Models;

/// <summary>Empleado del hospital (tabla PEEMP_EMPLE del subsistema de Personal). Cada cuenta pertenece a un empleado.</summary>
[Table("PEEMP_EMPLE")]
public class Empleado
{
    [Key]
    [Column("PEEMP_CODIGO", TypeName = "char(10)")]
    public string CodigoEmpleado { get; set; } = "";

    [Column("PEGEN_CODIGO", TypeName = "char(1)")]
    public string CodigoGenero { get; set; } = "";

    [Column("PEESC_CODIGO", TypeName = "char(1)")]
    public string? CodigoEstadoCivil { get; set; }

    [Column("PEDEP_CODIGO", TypeName = "char(3)")]
    public string CodigoDepartamento { get; set; } = "";

    [Column("PECAR_CODIGO", TypeName = "char(3)")]
    public string CodigoCargo { get; set; } = "";

    [Column("PEEMP_CEDULA", TypeName = "varchar(13)")]
    public string Cedula { get; set; } = "";

    [Column("PEEMP_APELLI", TypeName = "varchar(60)")]
    public string Apellidos { get; set; } = "";

    [Column("PEEMP_NOMBRE", TypeName = "varchar(60)")]
    public string Nombres { get; set; } = "";

    [Column("PEEMP_FECNAC", TypeName = "date")]
    public DateTime FechaNacimiento { get; set; }

    [Column("PEEMP_EMAIL", TypeName = "varchar(100)")]
    public string Email { get; set; } = "";

    [Column("PEEMP_TELEFO", TypeName = "varchar(15)")]
    public string? Telefono { get; set; }

    [Column("PEEMP_DIRECC", TypeName = "varchar(150)")]
    public string? Direccion { get; set; }

    [Column("PEEMP_REGPRO", TypeName = "varchar(30)")]
    public string? RegistroProfesional { get; set; }

    [Column("PEEMP_ESTADO", TypeName = "char(1)")]
    public string EstadoEmpleado { get; set; } = "";
}
