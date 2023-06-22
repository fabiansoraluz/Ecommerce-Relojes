using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorEcommerce.Server.Modelos;

// Clase DbContext que representa el contexto de la base de datos
public partial class DbtiendaBlazorContext : DbContext
{
    // Constructor sin parámetros
    public DbtiendaBlazorContext()
    {
    }

    // Constructor que recibe opciones de configuración del contexto de la base de datos
    public DbtiendaBlazorContext(DbContextOptions<DbtiendaBlazorContext> options)
        : base(options)
    {
    }

    // Propiedades virtuales que representan las tablas de la base de datos
    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<DetalleVenta> DetalleVenta { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Venta> Venta { get; set; }

    // Método de configuración de opciones del DbContext
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    // Método de configuración del modelo de datos de la base de datos
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de la entidad "Categoria"
        modelBuilder.Entity<Categoria>(entity =>
        {
            // Configuración de la clave primaria de la tabla "Categoria"
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__A3C02A10E6DCE8EE");

            // Configuración de las propiedades de la entidad "Categoria"
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        // Configuración de la entidad "DetalleVenta"
        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            // Configuración de la clave primaria de la tabla "DetalleVenta"
            entity.HasKey(e => e.IdDetalleVenta).HasName("PK__DetalleV__AAA5CEC2D008617A");

            // Configuración de las propiedades de la entidad "DetalleVenta"
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            // Configuración de las relaciones con otras entidades
            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK__DetalleVe__IdPro__286302EC");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.IdVenta)
                .HasConstraintName("FK__DetalleVe__IdVen__276EDEB3");
        });

        // Configuración de la entidad "Persona"
        modelBuilder.Entity<Persona>(entity =>
        {
            // Configuración de la clave primaria de la tabla "Persona"
            entity.HasKey(e => e.IdPersona).HasName("PK__Persona__2EC8D2AC277522E5");

            // Configuración de las propiedades de la entidad "Persona"
            entity.ToTable("Persona");

            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Rol)
                .HasMaxLength(50)
                .IsUnicode(false);
        });


        // Configuración de la entidad "Producto"
        modelBuilder.Entity<Producto>(entity =>
        {
            // Configuración de la clave primaria de la tabla "Producto"
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__09889210A276D374");

            // Configuración de las propiedades de la entidad "Producto"
            entity.ToTable("Producto");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Imagen).IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PrecioOferta).HasColumnType("decimal(10, 2)");

            // Configuración de las relaciones con otras entidades
            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK__Producto__IdCate__1367E606");
        });

        // Configuración de la entidad "Venta"
        modelBuilder.Entity<Venta>(entity =>
        {
            // Configuración de la clave primaria de la tabla "Venta"
            entity.HasKey(e => e.IdVenta).HasName("PK__Venta__BC1240BD5E47FA0D");

            // Configuración de las propiedades de la entidad "Venta"
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            // Configuración de las relaciones con otras entidades
            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdPersona)
                .HasConstraintName("FK__Venta__IdPersona__239E4DCF");
        });

        // Llamada a un método parcial de configuración adicional del modelo
        OnModelCreatingPartial(modelBuilder);
    }

    // Este método parcial se utiliza para agregar configuraciones adicionales al modelo de datos en el contexto de la base de datos.
    // Su implementación completa se espera que esté en otra parte del código.
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
