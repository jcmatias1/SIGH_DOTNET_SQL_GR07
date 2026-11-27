// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.EntityFrameworkCore;
using SIGH_DOTNET_GR07.Data;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Models;

namespace SIGH_DOTNET_GR07.Services;

/// <summary>
/// Arma el menú del usuario a partir de sus perfiles vigentes y decide si una ruta está permitida
/// (RF-SEG-003). Los permisos se leen en cada solicitud: una revocación rige desde la siguiente petición.
/// </summary>
public class MenuServicio(SighDbContext db)
{
    private static readonly Dictionary<string, string> IconosSistema = new()
    {
        ["PER"] = "bi-person-badge",
        ["SEG"] = "bi-shield-lock"
    };

    private static readonly Dictionary<string, string> IconosOpcion = new()
    {
        ["P001"] = "bi-building",
        ["P002"] = "bi-briefcase",
        ["P003"] = "bi-person-vcard",
        ["S001"] = "bi-people",
        ["S002"] = "bi-diagram-3",
        ["S003"] = "bi-journal-text"
    };

    /// <summary>Opciones permitidas: perfiles vigentes del usuario -> opciones vigentes de esos perfiles.</summary>
    public async Task<List<Opcion>> OpcionesPermitidasAsync(string codigoUsuario)
    {
        var codigos = from up in db.UsuPers
                      join op in db.OpcPers on up.CodigoPerfil equals op.CodigoPerfil
                      where up.CodigoUsuario == codigoUsuario && up.FechaRetiro == null && op.FechaRetiro == null
                      select op.CodigoOpcion;

        return await db.Opciones.AsNoTracking()
            .Include(o => o.Sistema)
            .Where(o => codigos.Contains(o.CodigoOpcion))
            .OrderBy(o => o.CodigoSistema).ThenBy(o => o.CodigoOpcion)
            .ToListAsync();
    }

    /// <summary>Menú agrupado por sistema, en el orden en que se muestra.</summary>
    public async Task<List<MenuSistema>> MenuAsync(string codigoUsuario)
    {
        var grupos = new List<MenuSistema>();
        foreach (Opcion opcion in await OpcionesPermitidasAsync(codigoUsuario))
        {
            MenuSistema? grupo = grupos.FirstOrDefault(g => g.Codigo == opcion.CodigoSistema);
            if (grupo is null)
            {
                grupo = new MenuSistema(opcion.CodigoSistema, opcion.Sistema.DescripcionSistema,
                    IconosSistema.GetValueOrDefault(opcion.CodigoSistema, "bi-grid"), new List<OpcionMenu>());
                grupos.Add(grupo);
            }
            grupo.Opciones.Add(new OpcionMenu(opcion.CodigoOpcion, opcion.DescripcionOpcion, opcion.UrlOpcion ?? "/inicio",
                IconosOpcion.GetValueOrDefault(opcion.CodigoOpcion, "bi-box-arrow-in-right")));
        }
        return grupos;
    }

    /// <summary>
    /// Opción que protege una ruta: la de URL más larga que sea prefijo de la ruta pedida.
    /// Ej.: /seguridad/usuarios/api/admin está protegida por S001 (/seguridad/usuarios).
    /// La comparación ignora mayúsculas porque el enrutamiento de ASP.NET Core también las ignora.
    /// </summary>
    public async Task<Opcion?> OpcionRequeridaAsync(string ruta)
    {
        var opciones = await db.Opciones.AsNoTracking().Include(o => o.Sistema)
            .Where(o => o.UrlOpcion != null && o.UrlOpcion != "")
            .ToListAsync();
        return opciones
            .Where(o => ruta.Equals(o.UrlOpcion, StringComparison.OrdinalIgnoreCase)
                        || ruta.StartsWith(o.UrlOpcion + "/", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(o => o.UrlOpcion!.Length)
            .FirstOrDefault();
    }

    public async Task<bool> TieneOpcionAsync(string codigoUsuario, string codigoOpcion) =>
        await db.UsuPers.AnyAsync(up => up.CodigoUsuario == codigoUsuario && up.FechaRetiro == null
            && up.Perfil.Opciones.Any(op => op.CodigoOpcion == codigoOpcion && op.FechaRetiro == null));
}
