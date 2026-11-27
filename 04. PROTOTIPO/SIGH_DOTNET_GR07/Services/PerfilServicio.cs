// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SIGH_DOTNET_GR07.Data;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Models;

namespace SIGH_DOTNET_GR07.Services;

/// <summary>
/// CRUD de perfiles (roles) y gestión de permisos (ECUD2, RF-SEG-003, RF-CAT-001): crear, consultar,
/// modificar y eliminar perfiles, y definir qué opciones de cada sistema tiene cada uno. Asignar o
/// revocar una opción actualiza XEOXP_OPCPE; la revocación solo pone la fecha de retiro.
/// </summary>
public class PerfilServicio(SighDbContext db, AuditoriaServicio auditoria)
{
    /// <summary>Opciones que el Administrador no puede perder: el sistema quedaría sin quien lo administre.</summary>
    private static readonly HashSet<string> OpcionesCriticasAdm = new() { "S001", "S002" };

    private static readonly Regex FormatoCodigo = new("^[A-Z]{3}$");
    private static readonly Regex FormatoNombre = new("^[A-ZÁÉÍÓÚÑÜ ]+$");
    private static readonly Regex Espacios = new("\\s+");

    public async Task<List<PerfilDto>> ListarAsync()
    {
        var opciones = await db.Opciones.AsNoTracking()
            .Include(o => o.Sistema)
            .OrderBy(o => o.CodigoSistema).ThenBy(o => o.CodigoOpcion)
            .ToListAsync();

        var perfiles = await db.Perfiles.AsNoTracking()
            .Include(p => p.Opciones)
            .Include(p => p.Usuarios)
            .OrderBy(p => p.CodigoPerfil)
            .ToListAsync();

        return perfiles.Select(perfil =>
        {
            var vigentes = perfil.Opciones.Where(op => op.FechaRetiro == null).Select(op => op.CodigoOpcion).ToHashSet();
            var permisos = opciones
                .Select(o => new OpcionPermisoDto(o.CodigoOpcion, o.DescripcionOpcion, o.CodigoSistema,
                    o.Sistema.DescripcionSistema, o.UrlOpcion, vigentes.Contains(o.CodigoOpcion)))
                .ToList();
            return new PerfilDto(perfil.CodigoPerfil, perfil.DescripcionPerfil, perfil.Observaciones,
                perfil.Usuarios.Count(up => up.FechaRetiro == null), perfil.Usuarios.Count, permisos);
        }).ToList();
    }

    /// <summary>CREATE: registra un perfil sin opciones; luego se le asignan con los interruptores.</summary>
    public async Task<PerfilDto> CrearAsync(PerfilFormDto datos, string actor, string? ip)
    {
        string codigo = SeguridadUtil.Texto(datos.CodigoPerfil).ToUpperInvariant();
        if (!FormatoCodigo.IsMatch(codigo))
        {
            throw new ReglaNegocioException("El código del perfil debe tener exactamente 3 letras. Ejemplo: LAB.");
        }
        if (await db.Perfiles.AnyAsync(p => p.CodigoPerfil == codigo))
        {
            throw new ReglaNegocioException("Ya existe un perfil con el código " + codigo + ".");
        }
        string descripcion = ValidarDescripcion(datos.Descripcion);
        // La intercalación Modern_Spanish_CI_AS compara sin distinguir mayúsculas
        if (await db.Perfiles.AnyAsync(p => p.DescripcionPerfil == descripcion))
        {
            throw new ReglaNegocioException("Ya existe un perfil llamado «" + descripcion + "».");
        }
        var perfil = new Perfil
        {
            CodigoPerfil = codigo,
            DescripcionPerfil = descripcion,
            Observaciones = ValidarObservaciones(datos.Observaciones)
        };
        db.Perfiles.Add(perfil);
        await auditoria.RegistrarAsync(actor, "CREAR PERFIL", "EXITOSO", "Se creó el perfil " + codigo + " - " + descripcion, ip);
        await db.SaveChangesAsync();
        return new PerfilDto(codigo, descripcion, perfil.Observaciones, 0, 0, new List<OpcionPermisoDto>());
    }

    /// <summary>UPDATE: cambia el nombre y las observaciones; el código es la clave y no cambia.</summary>
    public async Task ActualizarAsync(string codigoPerfil, PerfilFormDto datos, string actor, string? ip)
    {
        Perfil perfil = await BuscarAsync(codigoPerfil);
        string descripcion = ValidarDescripcion(datos.Descripcion);
        if (await db.Perfiles.AnyAsync(p => p.DescripcionPerfil == descripcion && p.CodigoPerfil != perfil.CodigoPerfil))
        {
            throw new ReglaNegocioException("Ya existe otro perfil llamado «" + descripcion + "».");
        }
        string anterior = perfil.DescripcionPerfil;
        perfil.DescripcionPerfil = descripcion;
        perfil.Observaciones = ValidarObservaciones(datos.Observaciones);
        await auditoria.RegistrarAsync(actor, "MODIFICAR PERFIL", "EXITOSO",
            "Perfil " + perfil.CodigoPerfil + ": «" + anterior + "» -> «" + descripcion + "»", ip);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// DELETE: borra físicamente el perfil y su configuración de opciones, solo si nunca se asignó
    /// a un usuario (así no se pierde el historial de XEUXP_USUPE).
    /// </summary>
    public async Task EliminarAsync(string codigoPerfil, string actor, string? ip)
    {
        Perfil perfil = await BuscarAsync(codigoPerfil);
        if (perfil.CodigoPerfil == "ADM")
        {
            throw new ReglaNegocioException("El perfil Administrador no se puede eliminar.");
        }
        int vigentes = await db.UsuPers.CountAsync(up => up.CodigoPerfil == perfil.CodigoPerfil && up.FechaRetiro == null);
        if (vigentes > 0)
        {
            throw new ReglaNegocioException("No se puede eliminar: el perfil está asignado a " + vigentes
                + " usuario(s). Cambie primero el perfil de esos usuarios.");
        }
        int historicas = await db.UsuPers.CountAsync(up => up.CodigoPerfil == perfil.CodigoPerfil);
        if (historicas > 0)
        {
            throw new ReglaNegocioException("No se puede eliminar: el perfil tiene " + historicas
                + " asignación(es) en el historial de usuarios. Para conservar ese historial, retire sus opciones.");
        }
        var permisos = await db.OpcPers.Where(op => op.CodigoPerfil == perfil.CodigoPerfil).ToListAsync();
        db.OpcPers.RemoveRange(permisos);
        db.Perfiles.Remove(perfil);
        await auditoria.RegistrarAsync(actor, "ELIMINAR PERFIL", "EXITOSO",
            "Se eliminó el perfil " + perfil.CodigoPerfil + " - " + perfil.DescripcionPerfil, ip);
        await db.SaveChangesAsync();
    }

    public async Task AsignarAsync(string codigoPerfil, string codigoOpcion, string actor, string? ip)
    {
        Perfil perfil = await BuscarAsync(codigoPerfil);
        string codigoOpc = SeguridadUtil.Texto(codigoOpcion).ToUpperInvariant();
        Opcion opcion = await db.Opciones.FirstOrDefaultAsync(o => o.CodigoOpcion == codigoOpc)
            ?? throw new ReglaNegocioException("La opción no existe.");

        OpcPer? asignacion = await db.OpcPers.FirstOrDefaultAsync(op =>
            op.CodigoPerfil == perfil.CodigoPerfil && op.CodigoOpcion == opcion.CodigoOpcion);
        if (asignacion is not null && asignacion.FechaRetiro == null)
        {
            throw new ReglaNegocioException("La opción ya está asignada a este perfil.");
        }
        if (asignacion is null)
        {
            asignacion = new OpcPer { CodigoPerfil = perfil.CodigoPerfil, CodigoOpcion = opcion.CodigoOpcion };
            db.OpcPers.Add(asignacion);
        }
        asignacion.FechaAsignacion = DateTime.Now;
        asignacion.FechaRetiro = null;
        await auditoria.RegistrarAsync(actor, "ASIGNAR PERMISO", "EXITOSO",
            "Perfil " + perfil.CodigoPerfil + " recibe la opción " + opcion.CodigoOpcion + " (" + opcion.DescripcionOpcion + ")", ip);
        await db.SaveChangesAsync();
    }

    public async Task RevocarAsync(string codigoPerfil, string codigoOpcion, string actor, string? ip)
    {
        string perfil = SeguridadUtil.Texto(codigoPerfil).ToUpperInvariant();
        string opcion = SeguridadUtil.Texto(codigoOpcion).ToUpperInvariant();
        if (perfil == "ADM" && OpcionesCriticasAdm.Contains(opcion))
        {
            throw new ReglaNegocioException("No se puede quitar esta opción al Administrador: el sistema quedaría sin quien administre usuarios y permisos.");
        }
        OpcPer asignacion = await db.OpcPers.FirstOrDefaultAsync(op =>
                op.CodigoPerfil == perfil && op.CodigoOpcion == opcion && op.FechaRetiro == null)
            ?? throw new ReglaNegocioException("La opción no está asignada a este perfil.");
        asignacion.FechaRetiro = DateTime.Now;
        await auditoria.RegistrarAsync(actor, "REVOCAR PERMISO", "EXITOSO",
            "Perfil " + perfil + " pierde la opción " + opcion, ip);
        await db.SaveChangesAsync();
    }

    private async Task<Perfil> BuscarAsync(string codigoPerfil)
    {
        string codigo = SeguridadUtil.Texto(codigoPerfil).ToUpperInvariant();
        return await db.Perfiles.FirstOrDefaultAsync(p => p.CodigoPerfil == codigo)
            ?? throw new ReglaNegocioException("El perfil no existe.");
    }

    private static string ValidarDescripcion(string? descripcion)
    {
        string valor = SeguridadUtil.Texto(descripcion).ToUpperInvariant();
        if (valor.Length < 3)
        {
            throw new ReglaNegocioException("El nombre del perfil es obligatorio (mínimo 3 caracteres).");
        }
        if (valor.Length > 60)
        {
            throw new ReglaNegocioException("El nombre del perfil no puede superar 60 caracteres.");
        }
        if (!FormatoNombre.IsMatch(valor))
        {
            throw new ReglaNegocioException("El nombre del perfil solo admite letras y espacios.");
        }
        return Espacios.Replace(valor, " ");
    }

    private static string? ValidarObservaciones(string? observaciones)
    {
        string valor = SeguridadUtil.Texto(observaciones);
        if (valor.Length > 200)
        {
            throw new ReglaNegocioException("Las observaciones no pueden superar 200 caracteres.");
        }
        return valor.Length == 0 ? null : valor;
    }
}
