// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Personal
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SIGH_DOTNET_GR07.Data;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Models;

namespace SIGH_DOTNET_GR07.Services;

/// <summary>
/// CRUD de cargos del personal (opción P002 «Administrar cargos»). Cada cargo pertenece a un
/// departamento; su clave es departamento + código. Todas las operaciones quedan auditadas en
/// el módulo PERSONAL.
/// </summary>
public class CargoServicio(SighDbContext db, AuditoriaServicio auditoria)
{
    public const string Modulo = "PERSONAL";

    private static readonly Regex FormatoCodigo = new("^[A-Z]{3}$");
    private static readonly Regex FormatoDescripcion = new("^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ /.\\-]+$");
    private static readonly Regex Espacios = new("\\s+");

    public async Task<List<CargoDto>> ListarAsync()
    {
        return await db.Cargos.AsNoTracking()
            .OrderBy(c => c.Departamento.Descripcion).ThenBy(c => c.Descripcion)
            .Select(c => new CargoDto(c.CodigoCargo, c.Descripcion, c.CodigoDepartamento, c.Departamento.Descripcion,
                db.Empleados.Count(e => e.CodigoDepartamento == c.CodigoDepartamento && e.CodigoCargo == c.CodigoCargo)))
            .ToListAsync();
    }

    public async Task<List<DepartamentoDto>> DepartamentosAsync()
    {
        return await db.Departamentos.AsNoTracking()
            .OrderBy(d => d.Descripcion)
            .Select(d => new DepartamentoDto(d.CodigoDepartamento, d.Descripcion))
            .ToListAsync();
    }

    /// <summary>CREATE: registra un cargo nuevo dentro de un departamento.</summary>
    public async Task<CargoDto> CrearAsync(CargoFormDto datos, string actor, string? ip)
    {
        string codigo = SeguridadUtil.Texto(datos.CodigoCargo).ToUpperInvariant();
        if (!FormatoCodigo.IsMatch(codigo))
        {
            throw new ReglaNegocioException("El código del cargo debe tener exactamente 3 letras. Ejemplo: MED.");
        }
        string codigoDepartamento = SeguridadUtil.Texto(datos.CodigoDepartamento).ToUpperInvariant();
        if (codigoDepartamento == "")
        {
            throw new ReglaNegocioException("Seleccione el departamento del cargo.");
        }
        Departamento departamento = await db.Departamentos.FirstOrDefaultAsync(d => d.CodigoDepartamento == codigoDepartamento)
            ?? throw new ReglaNegocioException("El departamento seleccionado no existe.");
        if (await db.Cargos.AnyAsync(c => c.CodigoDepartamento == codigoDepartamento && c.CodigoCargo == codigo))
        {
            throw new ReglaNegocioException("Ya existe el cargo " + codigo + " en el departamento "
                + departamento.Descripcion + ".");
        }
        string descripcion = ValidarDescripcion(datos.Descripcion);
        if (await db.Cargos.AnyAsync(c => c.CodigoDepartamento == codigoDepartamento && c.Descripcion == descripcion))
        {
            throw new ReglaNegocioException("Ya existe un cargo llamado «" + descripcion + "» en el departamento "
                + departamento.Descripcion + ".");
        }
        db.Cargos.Add(new Cargo { CodigoDepartamento = codigoDepartamento, CodigoCargo = codigo, Descripcion = descripcion });
        await auditoria.RegistrarAsync(Modulo, actor, "CREAR CARGO", "EXITOSO",
            "Se creó el cargo " + codigoDepartamento + "/" + codigo + " - " + descripcion
            + " (departamento " + departamento.Descripcion + ")", ip);
        await db.SaveChangesAsync();
        return new CargoDto(codigo, descripcion, codigoDepartamento, departamento.Descripcion, 0);
    }

    /// <summary>UPDATE: solo cambia la descripción; departamento y código forman la clave y no cambian.</summary>
    public async Task ActualizarAsync(string codigoDepartamento, string codigoCargo, CargoFormDto datos, string actor, string? ip)
    {
        Cargo cargo = await BuscarAsync(codigoDepartamento, codigoCargo);
        string descripcion = ValidarDescripcion(datos.Descripcion);
        if (await db.Cargos.AnyAsync(c => c.CodigoDepartamento == cargo.CodigoDepartamento
                                          && c.Descripcion == descripcion && c.CodigoCargo != cargo.CodigoCargo))
        {
            throw new ReglaNegocioException("Ya existe otro cargo llamado «" + descripcion + "» en el departamento "
                + cargo.Departamento.Descripcion + ".");
        }
        string anterior = cargo.Descripcion;
        cargo.Descripcion = descripcion;
        await auditoria.RegistrarAsync(Modulo, actor, "MODIFICAR CARGO", "EXITOSO",
            "Cargo " + cargo.CodigoDepartamento + "/" + cargo.CodigoCargo + ": «" + anterior + "» -> «" + descripcion + "»", ip);
        await db.SaveChangesAsync();
    }

    /// <summary>DELETE: elimina el cargo solo si ningún empleado lo tiene asignado.</summary>
    public async Task EliminarAsync(string codigoDepartamento, string codigoCargo, string actor, string? ip)
    {
        Cargo cargo = await BuscarAsync(codigoDepartamento, codigoCargo);
        int empleados = await db.Empleados.CountAsync(e => e.CodigoDepartamento == cargo.CodigoDepartamento
                                                           && e.CodigoCargo == cargo.CodigoCargo);
        if (empleados > 0)
        {
            throw new ReglaNegocioException("No se puede eliminar: el cargo está asignado a " + empleados + " empleado(s).");
        }
        db.Cargos.Remove(cargo);
        await auditoria.RegistrarAsync(Modulo, actor, "ELIMINAR CARGO", "EXITOSO",
            "Se eliminó el cargo " + cargo.CodigoDepartamento + "/" + cargo.CodigoCargo + " - " + cargo.Descripcion
            + " (departamento " + cargo.Departamento.Descripcion + ")", ip);
        await db.SaveChangesAsync();
    }

    private async Task<Cargo> BuscarAsync(string codigoDepartamento, string codigoCargo)
    {
        string dep = SeguridadUtil.Texto(codigoDepartamento).ToUpperInvariant();
        string car = SeguridadUtil.Texto(codigoCargo).ToUpperInvariant();
        return await db.Cargos.Include(c => c.Departamento)
                   .FirstOrDefaultAsync(c => c.CodigoDepartamento == dep && c.CodigoCargo == car)
               ?? throw new ReglaNegocioException("El cargo no existe.");
    }

    private static string ValidarDescripcion(string? descripcion)
    {
        string valor = Espacios.Replace(SeguridadUtil.Texto(descripcion), " ");
        if (valor.Length < 3)
        {
            throw new ReglaNegocioException("La descripción del cargo es obligatoria (mínimo 3 caracteres).");
        }
        if (valor.Length > 60)
        {
            throw new ReglaNegocioException("La descripción del cargo no puede superar 60 caracteres.");
        }
        if (!FormatoDescripcion.IsMatch(valor))
        {
            throw new ReglaNegocioException("La descripción del cargo solo admite letras, espacios y los signos / . -");
        }
        return valor;
    }
}
