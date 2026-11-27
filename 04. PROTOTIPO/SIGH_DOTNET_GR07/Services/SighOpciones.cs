// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
namespace SIGH_DOTNET_GR07.Services;

/// <summary>Parámetros de seguridad leídos de la sección "Sigh" de appsettings.json.</summary>
public class SighOpciones
{
    /// <summary>Intentos fallidos permitidos antes de bloquear la cuenta.</summary>
    public int MaxIntentos { get; set; } = 3;

    /// <summary>Minutos que dura el bloqueo automático.</summary>
    public int MinutosBloqueo { get; set; } = 15;

    /// <summary>Minutos de inactividad tras los que se cierra la sesión.</summary>
    public int MinutosSesion { get; set; } = 15;

    /// <summary>Minutos de validez del enlace de recuperación de contraseña.</summary>
    public int MinutosVigenciaEnlace { get; set; } = 30;

    /// <summary>Modo taller: muestra el enlace en pantalla cuando no hay correo configurado.</summary>
    public bool MostrarEnlaceRecuperacion { get; set; }
}

/// <summary>Servidor SMTP para enviar el enlace de recuperación (sección "Correo").</summary>
public class CorreoOpciones
{
    public string Host { get; set; } = "";
    public int Puerto { get; set; } = 587;
    public string Usuario { get; set; } = "";
    public string Clave { get; set; } = "";
}
