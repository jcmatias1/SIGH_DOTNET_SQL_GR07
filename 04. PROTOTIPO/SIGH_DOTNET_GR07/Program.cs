// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Punto de entrada: registra servicios, seguridad y la conexión a SQL Server.
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using SIGH_DOTNET_GR07.Data;
using SIGH_DOTNET_GR07.Dtos;
using SIGH_DOTNET_GR07.Filters;
using SIGH_DOTNET_GR07.Services;

var builder = WebApplication.CreateBuilder(args);

// Parámetros del SIGH (appsettings.json)
builder.Services.Configure<SighOpciones>(builder.Configuration.GetSection("Sigh"));
builder.Services.Configure<CorreoOpciones>(builder.Configuration.GetSection("Correo"));
int minutosSesion = builder.Configuration.GetValue("Sigh:MinutosSesion", 15);

// Conexión a SQL Server con Entity Framework Core
builder.Services.AddDbContext<SighDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSQLServer")));

// Servicios de negocio (uno por solicitud) y registro de sesiones (uno para toda la aplicación)
builder.Services.AddScoped<AuditoriaServicio>();
builder.Services.AddScoped<AutenticacionServicio>();
builder.Services.AddScoped<MenuServicio>();
builder.Services.AddScoped<UsuarioServicio>();
builder.Services.AddScoped<PerfilServicio>();
builder.Services.AddScoped<RecuperacionPasswordServicio>();
builder.Services.AddScoped<DepartamentoServicio>();
builder.Services.AddScoped<CargoServicio>();
builder.Services.AddScoped<EmpleadoServicio>();
builder.Services.AddSingleton<RegistroSesiones>();
builder.Services.AddHostedService<CierreSesionesServicio>();

// Autenticación con cookie: la cookie solo viaja por HTTP (no la lee JavaScript) y no sale a otros sitios
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.Cookie.Name = SeguridadFiltro.NombreCookie;
        opciones.Cookie.HttpOnly = true;
        opciones.Cookie.SameSite = SameSiteMode.Strict;
        opciones.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        opciones.LoginPath = "/login";
        opciones.LogoutPath = "/logout";
        // El cierre por inactividad lo controla RegistroSesiones; la cookie solo pone un tope de 8 horas.
        opciones.ExpireTimeSpan = TimeSpan.FromHours(8);
        opciones.SlidingExpiration = true;
    });

// MVC con vistas Razor; el filtro de seguridad protege todas las acciones
builder.Services.AddControllersWithViews(opciones =>
{
    opciones.Filters.Add<SeguridadFiltro>();
    opciones.Filters.Add<ErroresApiFiltro>();
});

// Si el JSON enviado no se puede leer, se responde con el mismo formato { "mensaje": "..." }
builder.Services.Configure<ApiBehaviorOptions>(opciones =>
{
    opciones.InvalidModelStateResponseFactory = _ =>
        new BadRequestObjectResult(new MensajeDto("Los datos enviados no tienen el formato correcto."));
});

// Las vistas muestran tildes y eñes tal cual (sin convertirlas en entidades &#xE1;)
builder.Services.Configure<WebEncoderOptions>(opciones =>
    opciones.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

var app = builder.Build();

// Crea la base y sus tablas si no existen y carga los datos iniciales (ya no es obligatorio ejecutar los .sql)
using (var alcance = app.Services.CreateScope())
{
    var db = alcance.ServiceProvider.GetRequiredService<SighDbContext>();
    DatosIniciales.Inicializar(db, app.Logger);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/login");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("[SIGH] Aplicación lista. Sesión por inactividad: {Minutos} minutos", minutosSesion);
app.Run();
