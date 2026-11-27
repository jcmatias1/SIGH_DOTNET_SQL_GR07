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
/// CRUD de departamentos (opción P001 «Administrar departamentos»). Los cargos (P002) se crean
/// dentro de un departamento, por eso no se elimina un departamento que tenga cargos o empleados.
/// </summary>
public class DepartamentoServicio(SighDbContext db, AuditoriaServicio auditoria)
{
    public const string Modulo = "PERSONAL";

    private static readonly Regex FormatoCodigo = new("^[A-Z]{3}$");
    private static readonly Regex FormatoDescripcion = new("^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ /.\\-]+$");
    private static readonly Regex Espacios = new("\\s+");

    public async Task<List<DepartamentoFilaDto>> ListarAsync()
    {
        return await db.Departamentos.AsNoTracking()
            .OrderBy(d => d.Descripcion)
            .Select(d => new DepartamentoFilaDto(d.CodigoDepartamento, d.Descripcion,
                db.Cargos.Count(c => c.CodigoDepartamento == d.CodigoDepartamento),
                db.Empleados.Count(e => e.CodigoDepartamento == d.CodigoDepartamento)))
            .ToListAsync();
    }

    /// <summary>CREATE: registra un departamento nuevo.</summary>
    public async Task<DepartamentoFilaDto> CrearAsync(DepartamentoFormDto datos, string actor, string? ip)
    {
        string codigo = SeguridadUtil.Texto(datos.CodigoDepartamento).ToUpperInvariant();
        if (!FormatoCodigo.IsMatch(codigo))
        {
            throw new ReglaNegocioException("El código del departamento debe tener exactamente 3 letras. Ejemplo: LAB.");
        }
        if (await db.Departamentos.AnyAsync(d => d.CodigoDepartamento == codigo))
        {
            throw new ReglaNegocioException("Ya existe un departamento con el código " + codigo + ".");
        }
        string descripcion = ValidarDescripcion(datos.Descripcion);
        if (await db.Departamentos.AnyAsync(d => d.Descripcion == descripcion))
        {
            throw new ReglaNegocioException("Ya existe un departamento llamado «" + descripcion + "».");
        }
        db.Departamentos.Add(new Departamento { CodigoDepartamento = codigo, Descripcion = descripcion });
        await auditoria.RegistrarAsync(Modulo, actor, "CREAR DEPARTAMENTO", "EXITOSO",
            "Se creó el departamento " + codigo + " - " + descripcion, ip);
        await db.SaveChangesAsync();
        return new DepartamentoFilaDto(codigo, descripcion, 0, 0);
    }

    /// <summary>UPDATE: solo cambia la descripción; el código es la clave y no cambia.</summary>
    public async Task ActualizarAsync(string codigoDepartamento, DepartamentoFormDto datos, string actor, string? ip)
    {
        Departamento departamento = await BuscarAsync(codigoDepartamento);
        string descripcion = ValidarDescripcion(datos.Descripcion);
        if (await db.Departamentos.AnyAsync(d => d.Descripcion == descripcion
                                                 && d.CodigoDepartamento != departamento.CodigoDepartamento))
        {
            throw new ReglaNegocioException("Ya existe otro departamento llamado «" + descripcion + "».");
        }
        string anterior = departamento.Descripcion;
        departamento.Descripcion = descripcion;
        await auditoria.RegistrarAsync(Modulo, actor, "MODIFICAR DEPARTAMENTO", "EXITOSO",
            "Departamento " + departamento.CodigoDepartamento + ": «" + anterior + "» -> «" + descripcion + "»", ip);
        await db.SaveChangesAsync();
    }

    /// <summary>DELETE: solo si el departamento no tiene cargos ni empleados.</summary>
    public async Task EliminarAsync(string codigoDepartamento, string actor, string? ip)
    {
        Departamento departamento = await BuscarAsync(codigoDepartamento);
        string codigo = departamento.CodigoDepartamento;
        int cargos = await db.Cargos.CountAsync(c => c.CodigoDepartamento == codigo);
        if (cargos > 0)
        {
            throw new ReglaNegocioException("No se puede eliminar: el departamento tiene " + cargos
                + " cargo(s). Elimine primero sus cargos.");
        }
        int empleados = await db.Empleados.CountAsync(e => e.CodigoDepartamento == codigo);
        if (empleados > 0)
        {
            throw new ReglaNegocioException("No se puede eliminar: el departamento tiene " + empleados
                + " empleado(s) asignado(s).");
        }
        db.Departamentos.Remove(departamento);
        await auditoria.RegistrarAsync(Modulo, actor, "ELIMINAR DEPARTAMENTO", "EXITOSO",
            "Se eliminó el departamento " + codigo + " - " + departamento.Descripcion, ip);
        await db.SaveChangesAsync();
    }

    private async Task<Departamento> BuscarAsync(string codigoDepartamento)
    {
        string codigo = SeguridadUtil.Texto(codigoDepartamento).ToUpperInvariant();
        return await db.Departamentos.FirstOrDefaultAsync(d => d.CodigoDepartamento == codigo)
            ?? throw new ReglaNegocioException("El departamento no existe.");
    }

    private static string ValidarDescripcion(string? descripcion)
    {
        string valor = Espacios.Replace(SeguridadUtil.Texto(descripcion), " ");
        if (valor.Length < 3)
        {
            throw new ReglaNegocioException("La descripción del departamento es obligatoria (mínimo 3 caracteres).");
        }
        if (valor.Length > 60)
        {
            throw new ReglaNegocioException("La descripción del departamento no puede superar 60 caracteres.");
        }
        if (!FormatoDescripcion.IsMatch(valor))
        {
            throw new ReglaNegocioException("La descripción del departamento solo admite letras, espacios y los signos / . -");
        }
        return valor;
    }
}
