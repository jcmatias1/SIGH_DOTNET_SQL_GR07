// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SIGH_DOTNET_GR07.Services;

/// <summary>Funciones de seguridad: hash SHA-256, política de contraseñas, tokens y dirección IP.</summary>
public static class SeguridadUtil
{
    /// <summary>Hash SHA-256 en 64 caracteres hexadecimales en minúsculas.</summary>
    public static string Sha256(string texto)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(texto));
        return Convert.ToHexStringLower(hash);
    }

    /// <summary>Compara la contraseña escrita con el hash guardado en tiempo constante.</summary>
    public static bool Coincide(string? passwordPlano, string? hashGuardado)
    {
        if (passwordPlano is null || hashGuardado is null)
        {
            return false;
        }
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(Sha256(passwordPlano)),
            Encoding.UTF8.GetBytes(hashGuardado.Trim().ToLowerInvariant()));
    }

    /// <summary>
    /// Política de contraseñas del hospital: de 8 a 64 caracteres, con al menos una letra y un número.
    /// Devuelve null si la cumple o el mensaje del error.
    /// </summary>
    public static string? ValidarPolitica(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
        {
            return "La contraseña debe tener al menos 8 caracteres.";
        }
        if (password.Length > 64)
        {
            return "La contraseña no puede superar 64 caracteres.";
        }
        if (!Regex.IsMatch(password, "[A-Za-zÁÉÍÓÚáéíóúÑñ]"))
        {
            return "La contraseña debe incluir al menos una letra.";
        }
        if (!Regex.IsMatch(password, "[0-9]"))
        {
            return "La contraseña debe incluir al menos un número.";
        }
        return null;
    }

    /// <summary>Token aleatorio de 256 bits (64 caracteres hexadecimales) para los enlaces de recuperación.</summary>
    public static string GenerarToken() => Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(32));

    /// <summary>IP del cliente recortada a 45 caracteres (tamaño de XESES_DIRIP y XEAUD_DIRIP).</summary>
    public static string? IpCliente(HttpContext contexto)
    {
        IPAddress? direccion = contexto.Connection.RemoteIpAddress;
        if (direccion is null)
        {
            return null;
        }
        if (direccion.IsIPv4MappedToIPv6)
        {
            direccion = direccion.MapToIPv4();
        }
        string ip = IPAddress.IPv6Loopback.Equals(direccion) ? "127.0.0.1" : direccion.ToString();
        return ip.Length > 45 ? ip[..45] : ip;
    }

    /// <summary>"JUAN CARLOS" -> "Juan Carlos".</summary>
    public static string Capitalizar(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return "";
        }
        var partes = texto.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => char.ToUpperInvariant(p[0]) + p[1..]);
        return string.Join(' ', partes);
    }

    /// <summary>Texto sin espacios a los extremos (null se vuelve cadena vacía).</summary>
    public static string Texto(string? valor) => valor?.Trim() ?? "";
}
