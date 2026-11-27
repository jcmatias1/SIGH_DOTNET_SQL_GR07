// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Personal
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SIGH_DOTNET_GR07.Data;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Models;

namespace SIGH_DOTNET_GR07.Services;

/// <summary>
/// Mantenimiento de empleados (opción P003). CRUD sobre PEEMP_EMPLE con borrado lógico: «Borrar»
/// no elimina la fila, cambia PEEMP_ESTADO a I (inactivo). Un empleado con cuenta de usuario activa
/// no se puede desactivar hasta que se desactive su cuenta en Seguridad.
/// </summary>
public class EmpleadoServicio(SighDbContext db, AuditoriaServicio auditoria)
{
    public const string Modulo = "PERSONAL";
    public const string Activo = "A";
    public const string Inactivo = "I";

    private static readonly Regex FormatoNombre = new("^[A-ZÁÉÍÓÚÜÑ ]+$");
    private static readonly Regex FormatoEmail = new("^[A-Za-z0-9._%+\\-]+@[A-Za-z0-9.\\-]+\\.[A-Za-z]{2,}$");
    private static readonly Regex FormatoRegistro = new("^[A-Z0-9\\-/.]+$");
    private static readonly Regex FormatoTelefono = new("^\\d{7,10}$");
    private static readonly Regex Espacios = new("\\s+");

    /// <summary>LISTAR: todos los empleados (activos e inactivos) con las descripciones de sus códigos.</summary>
    public async Task<List<EmpleadoFichaDto>> ListarAsync()
    {
        var generos = await db.Generos.AsNoTracking().ToDictionaryAsync(g => g.CodigoGenero, g => g.Descripcion);
        var estados = await db.EstadosCiviles.AsNoTracking().ToDictionaryAsync(e => e.CodigoEstadoCivil, e => e.Descripcion);
        var cargos = await db.Cargos.AsNoTracking().Include(c => c.Departamento).ToListAsync();
        var conCuenta = (await db.Usuarios.AsNoTracking().Select(u => u.CodigoEmpleado).ToListAsync()).ToHashSet();
        var empleados = await db.Empleados.AsNoTracking()
            .OrderBy(e => e.Apellidos).ThenBy(e => e.Nombres)
            .ToListAsync();

        return empleados.Select(e =>
        {
            Cargo? cargo = cargos.FirstOrDefault(c => c.CodigoDepartamento == e.CodigoDepartamento && c.CodigoCargo == e.CodigoCargo);
            return new EmpleadoFichaDto(e.CodigoEmpleado, e.Cedula, e.Apellidos, e.Nombres,
                e.FechaNacimiento.ToString("yyyy-MM-dd"), e.CodigoGenero, generos.GetValueOrDefault(e.CodigoGenero, e.CodigoGenero),
                e.CodigoEstadoCivil, e.CodigoEstadoCivil is null ? "" : estados.GetValueOrDefault(e.CodigoEstadoCivil, e.CodigoEstadoCivil),
                e.RegistroProfesional, e.EstadoEmpleado, e.CodigoDepartamento,
                cargo?.Departamento.Descripcion ?? e.CodigoDepartamento, e.CodigoCargo, cargo?.Descripcion ?? e.CodigoCargo,
                e.Direccion, e.Telefono, e.Email, conCuenta.Contains(e.CodigoEmpleado));
        }).ToList();
    }

    public async Task<List<CatalogoDto>> GenerosAsync() =>
        await db.Generos.AsNoTracking().OrderBy(g => g.Descripcion)
            .Select(g => new CatalogoDto(g.CodigoGenero, g.Descripcion)).ToListAsync();

    public async Task<List<CatalogoDto>> EstadosCivilesAsync() =>
        await db.EstadosCiviles.AsNoTracking().OrderBy(e => e.Descripcion)
            .Select(e => new CatalogoDto(e.CodigoEstadoCivil, e.Descripcion)).ToListAsync();

    /// <summary>Código que tendrá el próximo empleado: EMP + 7 dígitos (EMP0000007).</summary>
    public async Task<string> SiguienteCodigoAsync()
    {
        string? ultimo = await db.Empleados.OrderByDescending(e => e.CodigoEmpleado).Select(e => e.CodigoEmpleado).FirstOrDefaultAsync();
        string digitos = new((ultimo ?? "").Where(char.IsDigit).ToArray());
        int numero = digitos.Length > 0 ? int.Parse(digitos, CultureInfo.InvariantCulture) : 0;
        return "EMP" + (numero + 1).ToString("D7", CultureInfo.InvariantCulture);
    }

    /// <summary>GUARDAR: registra un empleado nuevo (activo).</summary>
    public async Task<EmpleadoFichaDto> CrearAsync(EmpleadoFormDto datos, string actor, string? ip)
    {
        var e = new Empleado { CodigoEmpleado = await SiguienteCodigoAsync(), EstadoEmpleado = Activo };
        await AplicarAsync(e, datos, nuevo: true);
        db.Empleados.Add(e);
        await auditoria.RegistrarAsync(Modulo, actor, "CREAR EMPLEADO", "EXITOSO",
            "Se registró el empleado " + e.CodigoEmpleado + " - " + e.Apellidos + " " + e.Nombres + " (cédula " + e.Cedula + ")", ip);
        await db.SaveChangesAsync();
        return (await ListarAsync()).First(f => f.CodigoEmpleado == e.CodigoEmpleado);
    }

    /// <summary>ACTUALIZAR: modifica todos los datos menos el código; permite reactivar un empleado inactivo.</summary>
    public async Task ActualizarAsync(string codigoEmpleado, EmpleadoFormDto datos, string actor, string? ip)
    {
        Empleado e = await BuscarAsync(codigoEmpleado);
        string estadoAnterior = e.EstadoEmpleado;
        await AplicarAsync(e, datos, nuevo: false);
        string estado = SeguridadUtil.Texto(datos.Estado).ToUpperInvariant();
        if (estado != Activo && estado != Inactivo)
        {
            throw new ReglaNegocioException("Seleccione el estado del empleado (Activo o Inactivo).");
        }
        if (estado == Inactivo && estadoAnterior == Activo)
        {
            await ValidarSinCuentaActivaAsync(e);
        }
        e.EstadoEmpleado = estado;
        string cambioEstado = estado == estadoAnterior ? ""
            : " Estado: " + NombreEstado(estadoAnterior) + " -> " + NombreEstado(estado) + ".";
        await auditoria.RegistrarAsync(Modulo, actor, "MODIFICAR EMPLEADO", "EXITOSO",
            "Se actualizaron los datos del empleado " + e.CodigoEmpleado + " - " + e.Apellidos + " " + e.Nombres + "." + cambioEstado, ip);
        await db.SaveChangesAsync();
    }

    /// <summary>BORRAR: borrado lógico, el empleado pasa a Inactivo y se conserva su historial.</summary>
    public async Task BorrarAsync(string codigoEmpleado, string actor, string? ip)
    {
        Empleado e = await BuscarAsync(codigoEmpleado);
        if (e.EstadoEmpleado == Inactivo)
        {
            throw new ReglaNegocioException("El empleado " + e.CodigoEmpleado + " ya está inactivo.");
        }
        await ValidarSinCuentaActivaAsync(e);
        e.EstadoEmpleado = Inactivo;
        await auditoria.RegistrarAsync(Modulo, actor, "BORRAR EMPLEADO", "EXITOSO",
            "Borrado lógico del empleado " + e.CodigoEmpleado + " - " + e.Apellidos + " " + e.Nombres + " (estado: Inactivo)", ip);
        await db.SaveChangesAsync();
    }

    private async Task ValidarSinCuentaActivaAsync(Empleado e)
    {
        if (await db.Usuarios.AnyAsync(u => u.CodigoEmpleado == e.CodigoEmpleado && u.CodigoEstado != Inactivo))
        {
            throw new ReglaNegocioException("El empleado " + e.CodigoEmpleado
                + " tiene una cuenta de usuario activa. Desactive primero su cuenta en Seguridad > Administrar usuarios.");
        }
    }

    /// <summary>Valida y copia los datos del formulario al empleado.</summary>
    private async Task AplicarAsync(Empleado e, EmpleadoFormDto d, bool nuevo)
    {
        string cedula = SeguridadUtil.Texto(d.Cedula);
        // Los registros anteriores pueden tener cédulas de prueba: solo se valida el dígito verificador si cambia
        bool cedulaNueva = nuevo || cedula != e.Cedula;
        if (cedulaNueva && !CedulaValida(cedula))
        {
            throw new ReglaNegocioException("La cédula no es válida: debe tener 10 dígitos y un dígito verificador correcto.");
        }
        if (await db.Empleados.AnyAsync(x => x.Cedula == cedula && x.CodigoEmpleado != e.CodigoEmpleado))
        {
            throw new ReglaNegocioException("Ya existe un empleado con la cédula " + cedula + ".");
        }
        string apellidos = Nombre(d.Apellidos, "Los apellidos");
        string nombres = Nombre(d.Nombres, "Los nombres");

        if (!DateTime.TryParseExact(SeguridadUtil.Texto(d.FechaNacimiento), "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime fecha))
        {
            throw new ReglaNegocioException("Ingrese la fecha de nacimiento.");
        }
        DateTime hoy = DateTime.Today;
        int edad = hoy.Year - fecha.Year - (fecha.Date > hoy.AddYears(-(hoy.Year - fecha.Year)) ? 1 : 0);
        if (edad < 18 || edad > 100)
        {
            throw new ReglaNegocioException("La fecha de nacimiento no es válida: el empleado debe tener entre 18 y 100 años.");
        }

        string genero = SeguridadUtil.Texto(d.CodigoGenero).ToUpperInvariant();
        if (genero == "" || !await db.Generos.AnyAsync(g => g.CodigoGenero == genero))
        {
            throw new ReglaNegocioException("Seleccione el género.");
        }
        string estadoCivil = SeguridadUtil.Texto(d.CodigoEstadoCivil).ToUpperInvariant();
        if (estadoCivil == "" || !await db.EstadosCiviles.AnyAsync(x => x.CodigoEstadoCivil == estadoCivil))
        {
            throw new ReglaNegocioException("Seleccione el estado civil.");
        }

        string registro = SeguridadUtil.Texto(d.RegistroProfesional).ToUpperInvariant();
        if (registro.Length > 30 || (registro != "" && !FormatoRegistro.IsMatch(registro)))
        {
            throw new ReglaNegocioException("El registro profesional admite hasta 30 letras, números y los signos - / .");
        }

        string dep = SeguridadUtil.Texto(d.CodigoDepartamento).ToUpperInvariant();
        string car = SeguridadUtil.Texto(d.CodigoCargo).ToUpperInvariant();
        if (dep == "" || car == "" || !await db.Cargos.AnyAsync(c => c.CodigoDepartamento == dep && c.CodigoCargo == car))
        {
            throw new ReglaNegocioException("Seleccione el cargo del empleado.");
        }

        string direccion = Espacios.Replace(SeguridadUtil.Texto(d.Direccion), " ");
        if (direccion.Length > 150)
        {
            throw new ReglaNegocioException("La dirección no puede superar 150 caracteres.");
        }
        string telefono = SeguridadUtil.Texto(d.Telefono);
        if (telefono != "" && !FormatoTelefono.IsMatch(telefono))
        {
            throw new ReglaNegocioException("El teléfono debe tener entre 7 y 10 dígitos.");
        }
        string email = SeguridadUtil.Texto(d.Email).ToLowerInvariant();
        if (email.Length > 100 || !FormatoEmail.IsMatch(email))
        {
            throw new ReglaNegocioException("Ingrese un email válido. Ejemplo: nombre@monstersalud.ec");
        }
        if (await db.Empleados.AnyAsync(x => x.Email == email && x.CodigoEmpleado != e.CodigoEmpleado))
        {
            throw new ReglaNegocioException("El email " + email + " ya pertenece a otro empleado.");
        }

        e.Cedula = cedula;
        e.Apellidos = apellidos;
        e.Nombres = nombres;
        e.FechaNacimiento = fecha;
        e.CodigoGenero = genero;
        e.CodigoEstadoCivil = estadoCivil;
        e.RegistroProfesional = registro == "" ? null : registro;
        e.CodigoDepartamento = dep;
        e.CodigoCargo = car;
        e.Direccion = direccion == "" ? null : direccion;
        e.Telefono = telefono == "" ? null : telefono;
        e.Email = email;
    }

    /// <summary>Cédula ecuatoriana: 10 dígitos, provincia 01-24 o 30, tercer dígito menor a 6 y verificador módulo 10.</summary>
    public static bool CedulaValida(string cedula)
    {
        if (!Regex.IsMatch(cedula, "^\\d{10}$"))
        {
            return false;
        }
        int provincia = int.Parse(cedula[..2], CultureInfo.InvariantCulture);
        if ((provincia < 1 || provincia > 24) && provincia != 30)
        {
            return false;
        }
        if (cedula[2] - '0' >= 6)
        {
            return false;
        }
        int suma = 0;
        for (int i = 0; i < 9; i++)
        {
            int digito = (cedula[i] - '0') * (i % 2 == 0 ? 2 : 1);
            suma += digito > 9 ? digito - 9 : digito;
        }
        return (10 - suma % 10) % 10 == cedula[9] - '0';
    }

    private static string Nombre(string? valor, string campo)
    {
        string v = Espacios.Replace(SeguridadUtil.Texto(valor), " ").ToUpper(new CultureInfo("es-EC"));
        if (v.Length < 2 || v.Length > 60)
        {
            throw new ReglaNegocioException(campo + " son obligatorios (de 2 a 60 caracteres).");
        }
        if (!FormatoNombre.IsMatch(v))
        {
            throw new ReglaNegocioException(campo + " solo admiten letras y espacios.");
        }
        return v;
    }

    private async Task<Empleado> BuscarAsync(string codigoEmpleado)
    {
        string codigo = SeguridadUtil.Texto(codigoEmpleado).ToUpperInvariant();
        return await db.Empleados.FirstOrDefaultAsync(e => e.CodigoEmpleado == codigo)
            ?? throw new ReglaNegocioException("El empleado no existe.");
    }

    private static string NombreEstado(string estado) => estado == Activo ? "Activo" : "Inactivo";
}
