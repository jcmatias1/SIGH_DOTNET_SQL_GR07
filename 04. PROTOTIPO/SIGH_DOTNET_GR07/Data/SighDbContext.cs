// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Subsistema de Seguridad
using Microsoft.EntityFrameworkCore;
using SIGH_DOTNET_GR07.Models;

namespace SIGH_DOTNET_GR07.Data;

/// <summary>
/// Contexto de Entity Framework Core sobre la base bd_monster_gr07 de SQL Server.
/// La base se crea con el script BD_MONSTER_GR07_SQLSERVER.sql (no se usan migraciones).
/// </summary>
public class SighDbContext(DbContextOptions<SighDbContext> options) : DbContext(options)
{
    public DbSet<Estado> Estados => Set<Estado>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Perfil> Perfiles => Set<Perfil>();
    public DbSet<Sistema> Sistemas => Set<Sistema>();
    public DbSet<Opcion> Opciones => Set<Opcion>();
    public DbSet<UsuPer> UsuPers => Set<UsuPer>();
    public DbSet<OpcPer> OpcPers => Set<OpcPer>();
    public DbSet<Sesion> Sesiones => Set<Sesion>();
    public DbSet<Auditoria> Auditorias => Set<Auditoria>();
    public DbSet<TokenRecuperacion> TokensRecuperacion => Set<TokenRecuperacion>();

    // Subsistema de Personal
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Cargo> Cargos => Set<Cargo>();
    public DbSet<Genero> Generos => Set<Genero>();
    public DbSet<EstadoCivil> EstadosCiviles => Set<EstadoCivil>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tablas intermedias con clave primaria compuesta
        modelBuilder.Entity<UsuPer>(e =>
        {
            e.HasKey(x => new { x.CodigoUsuario, x.CodigoPerfil });
            e.HasOne(x => x.Usuario).WithMany(u => u.Perfiles).HasForeignKey(x => x.CodigoUsuario);
            e.HasOne(x => x.Perfil).WithMany(p => p.Usuarios).HasForeignKey(x => x.CodigoPerfil);
        });

        modelBuilder.Entity<OpcPer>(e =>
        {
            e.HasKey(x => new { x.CodigoPerfil, x.CodigoOpcion });
            e.HasOne(x => x.Perfil).WithMany(p => p.Opciones).HasForeignKey(x => x.CodigoPerfil);
            e.HasOne(x => x.Opcion).WithMany().HasForeignKey(x => x.CodigoOpcion);
        });

        modelBuilder.Entity<TokenRecuperacion>().HasIndex(x => x.Token).IsUnique();

        // PECAR_CARGO: clave compuesta departamento + cargo, relacionada con PEDEP_DEPAR (FK_DEP_CAR)
        modelBuilder.Entity<Cargo>(e =>
        {
            e.HasKey(x => new { x.CodigoDepartamento, x.CodigoCargo });
            e.HasOne(x => x.Departamento).WithMany(d => d.Cargos).HasForeignKey(x => x.CodigoDepartamento);
        });

        // Igual que el modelo físico: ninguna llave foránea borra en cascada
        foreach (var llave in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()))
        {
            llave.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
