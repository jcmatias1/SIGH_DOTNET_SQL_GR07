// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.EntityFrameworkCore;
using SIGH_DOTNET_GR07.Data;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Models;

namespace SIGH_DOTNET_GR07.Services;

/// <summary>
/// Registro y consulta de la auditoría (RF-AUD-001, RF-AUD-004). Registrar solo agrega el evento
/// al contexto: se guarda en el mismo SaveChanges de la operación que lo origina, así ambos se
/// confirman juntos.
/// </summary>
public class AuditoriaServicio(SighDbContext db)
{
    public const string Modulo = "SEGURIDAD";

    public Task RegistrarAsync(string? codigoUsuario, string operacion, string resultado, string? detalle, string? ip) =>
        RegistrarAsync(Modulo, codigoUsuario, operacion, resultado, detalle, ip);

    /// <summary>Registra el evento en el módulo indicado (por ejemplo PERSONAL).</summary>
    public async Task RegistrarAsync(string modulo, string? codigoUsuario, string operacion, string resultado, string? detalle, string? ip)
    {
        bool existe = codigoUsuario is not null
            && (db.Usuarios.Local.Any(u => u.CodigoUsuario == codigoUsuario)
                || await db.Usuarios.AnyAsync(u => u.CodigoUsuario == codigoUsuario));
        db.Auditorias.Add(new Auditoria
        {
            CodigoUsuario = existe ? codigoUsuario : null,
            FechaHora = DateTime.Now,
            Modulo = Recortar(modulo, 40)!,
            Operacion = Recortar(operacion, 40)!,
            Resultado = Recortar(resultado, 15)!,
            Detalle = Recortar(detalle, 500),
            DireccionIp = Recortar(ip, 45)
        });
    }

    /// <summary>Registra el evento y lo guarda de inmediato (eventos sin otra operación asociada).</summary>
    public async Task RegistrarYGuardarAsync(string? codigoUsuario, string operacion, string resultado, string? detalle, string? ip)
    {
        await RegistrarAsync(codigoUsuario, operacion, resultado, detalle, ip);
        await db.SaveChangesAsync();
    }

    /// <summary>Últimos 300 eventos, del más reciente al más antiguo (RF-AUD-002).</summary>
    public async Task<List<AuditoriaDto>> ListarRecientesAsync()
    {
        return await db.Auditorias.AsNoTracking()
            .OrderByDescending(a => a.FechaHora).ThenByDescending(a => a.CodigoEvento)
            .Take(300)
            .Select(a => new AuditoriaDto(a.CodigoEvento, a.FechaHora, a.CodigoUsuario, a.Modulo, a.Operacion,
                a.Resultado, a.Detalle, a.DireccionIp))
            .ToListAsync();
    }

    private static string? Recortar(string? texto, int maximo) =>
        texto is null ? null : texto.Length > maximo ? texto[..maximo] : texto;
}
