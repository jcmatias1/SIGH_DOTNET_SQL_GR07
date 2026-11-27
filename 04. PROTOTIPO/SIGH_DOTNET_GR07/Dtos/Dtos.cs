// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
namespace SIGH_DOTNET_GR07.Dtos;

/// <summary>Fila del listado de usuarios y respuesta de crear / modificar / cambiar estado.</summary>
public record UsuarioDto(
    string CodigoUsuario,
    string CodigoEmpleado,
    string NombreEmpleado,
    string Cedula,
    string Email,
    string CodigoEstado,
    string DescripcionEstado,
    string? CodigoPerfil,
    string DescripcionPerfil,
    string? PieFirma,
    short IntentosFallidos,
    DateTime FechaCreacion,
    DateTime? UltimoAcceso);

/// <summary>Datos que envía el formulario de usuario (todos opcionales: las reglas se validan en el servicio).</summary>
public record UsuarioFormDto(
    string? CodigoUsuario,
    string? CodigoEmpleado,
    string? CodigoPerfil,
    string? Password,
    string? PieFirma);

/// <summary>Asignación de perfil del historial (vigente si FechaRetiro es nula).</summary>
public record PerfilHistorialDto(string CodigoPerfil, string Descripcion, DateTime FechaAsignacion, DateTime? FechaRetiro);

/// <summary>Sesión registrada en XESES_SESIO.</summary>
public record SesionResumenDto(long Codigo, DateTime Inicio, DateTime? Fin, string? Ip, string Estado);

/// <summary>Detalle completo de una cuenta: datos del empleado, perfiles y últimas sesiones.</summary>
public record UsuarioDetalleDto(
    UsuarioDto Cuenta,
    string Cargo,
    string Departamento,
    string? Telefono,
    string? RegistroProfesional,
    DateTime? FechaModificacion,
    List<PerfilHistorialDto> Perfiles,
    List<SesionResumenDto> Sesiones);

/// <summary>Empleado activo que todavía no tiene cuenta (para el formulario Nuevo usuario).</summary>
public record EmpleadoDto(string CodigoEmpleado, string NombreCompleto, string Cedula, string Email, string Cargo);

/// <summary>Perfil para las listas desplegables.</summary>
public record PerfilSimpleDto(string Codigo, string Descripcion);

/// <summary>Opción de un sistema y si está asignada (vigente) al perfil.</summary>
public record OpcionPermisoDto(string CodigoOpcion, string Descripcion, string CodigoSistema, string Sistema, string? Url, bool Asignada);

/// <summary>Perfil con sus contadores de usuarios y la matriz de opciones.</summary>
public record PerfilDto(
    string CodigoPerfil,
    string Descripcion,
    string? Observaciones,
    int UsuariosActivos,
    int AsignacionesHistoricas,
    List<OpcionPermisoDto> Opciones);

/// <summary>Datos que envía el formulario de perfil.</summary>
public record PerfilFormDto(string? CodigoPerfil, string? Descripcion, string? Observaciones);

/// <summary>Evento de la auditoría para la consulta S003.</summary>
public record AuditoriaDto(long Codigo, DateTime FechaHora, string? Usuario, string Modulo, string Operacion,
    string Resultado, string? Detalle, string? DireccionIp);

/// <summary>Opción del menú lateral.</summary>
public record OpcionMenu(string Codigo, string Descripcion, string Url, string Icono);

/// <summary>Grupo del menú: un sistema con sus opciones permitidas.</summary>
public record MenuSistema(string Codigo, string Descripcion, string Icono, List<OpcionMenu> Opciones);

/// <summary>Mensaje simple que devuelven las API (éxito o error).</summary>
public record MensajeDto(string Mensaje);

// ---------- Subsistema de Personal ----------

/// <summary>Departamento para las listas desplegables.</summary>
public record DepartamentoDto(string Codigo, string Descripcion);

/// <summary>Fila del listado de departamentos con sus contadores de cargos y empleados.</summary>
public record DepartamentoFilaDto(string Codigo, string Descripcion, int Cargos, int Empleados);

/// <summary>Datos que envía el formulario de departamento.</summary>
public record DepartamentoFormDto(string? CodigoDepartamento, string? Descripcion);

/// <summary>Fila del listado de cargos.</summary>
public record CargoDto(string CodigoCargo, string Descripcion, string CodigoDepartamento, string Departamento, int Empleados);

/// <summary>Datos que envía el formulario de cargo.</summary>
public record CargoFormDto(string? CodigoCargo, string? Descripcion, string? CodigoDepartamento);

/// <summary>Elemento de un combo (género, estado civil).</summary>
public record CatalogoDto(string Codigo, string Descripcion);

/// <summary>Empleado completo para la tabla y el formulario de Mantenimiento de empleados (P003).</summary>
public record EmpleadoFichaDto(string CodigoEmpleado, string Cedula, string Apellidos, string Nombres,
    string FechaNacimiento, string CodigoGenero, string Genero, string? CodigoEstadoCivil, string EstadoCivil,
    string? RegistroProfesional, string Estado, string CodigoDepartamento, string Departamento,
    string CodigoCargo, string Cargo, string? Direccion, string? Telefono, string Email, bool TieneUsuario);

/// <summary>Datos que envía el formulario de Mantenimiento de empleados (las reglas se validan en el servicio).</summary>
public record EmpleadoFormDto(string? Cedula, string? Apellidos, string? Nombres, string? FechaNacimiento,
    string? CodigoGenero, string? CodigoEstadoCivil, string? RegistroProfesional, string? Estado,
    string? CodigoDepartamento, string? CodigoCargo, string? Direccion, string? Telefono, string? Email);
