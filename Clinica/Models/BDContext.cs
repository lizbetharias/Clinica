using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Clinica.Models;

namespace Clinica.Models;

public partial class BDContext : DbContext
{
    public BDContext()
    {
    }

    public BDContext(DbContextOptions<BDContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Diagnostico> Diagnostico { get; set; }

    public virtual DbSet<Examen> Examen { get; set; }

    public virtual DbSet<Medicamento> Medicamento { get; set; }

    public virtual DbSet<Paciente> Paciente { get; set; }

    public virtual DbSet<Receta> Receta { get; set; }

    public virtual DbSet<RecetaMedicamento> RecetaMedicamento { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }
    public virtual DbSet<Historial> Historial { get; set; }

    public DbSet<Clinica.Models.Especialidad> Especialidad { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Diagnostico>(entity =>
        {
            entity.HasKey(e => e.DiagnosticoId).HasName("PK__Diagnost__9667CF5E513C8833");
        });

        modelBuilder.Entity<Examen>(entity =>
        {
            entity.HasKey(e => e.ExamenId).HasName("PK__Examen__FA90F7CD1E5E2A63");

            entity.HasOne(d => d.Paciente).WithMany(p => p.Examen).HasConstraintName("FK__Examen__paciente__37A5467C");
        });

        modelBuilder.Entity<Medicamento>(entity =>
        {
            entity.HasKey(e => e.MedicamentoId).HasName("PK__Medicame__F2F6F6071B88927C");
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.PacienteId).HasName("PK__Paciente__0AB98B2E47195522");
        });

        modelBuilder.Entity<Receta>(entity =>
        {
            entity.HasKey(e => e.RecetaId).HasName("PK__Receta__B9B1CB1D93E4B678");

            entity.HasOne(d => d.Diagnostico).WithMany(p => p.Receta).HasConstraintName("FK__Receta__diagnost__2F10007B");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Receta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK1_Usuario_Receta");

            entity.HasOne(d => d.Paciente).WithMany(p => p.Receta).HasConstraintName("FK__Receta__paciente__2D27B809");
        });

        modelBuilder.Entity<RecetaMedicamento>(entity =>
        {
            entity.HasKey(e => new { e.RecetaId, e.MedicamentoId }).HasName("PK__RecetaMe__D69EA47D7E552770");

            entity.HasOne(d => d.Medicamento).WithMany(p => p.RecetaMedicamento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RecetaMed__medic__34C8D9D1");

            entity.HasOne(d => d.Receta).WithMany(p => p.RecetaMedicamento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RecetaMed__recet__33D4B598");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rol__3214EC078DA851DB");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuario__3214EC0710839E6C");

            entity.Property(e => e.Password).IsFixedLength();

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK1_Rol_Usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    //Este es RECETA MEDICAMENTO y lo voy a modificar es ON MODEL CREATING PARTIAL
    // partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    static void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecetaMedicamento>(entity =>
        {
            // Definir recetamedicamentoID como clave primaria
            entity.HasKey(e => e.recetamedicamentoID);

            // Configurar las relaciones con las tablas Receta y Medicamento
            entity.HasOne(e => e.Receta)
                .WithMany(r => r.RecetaMedicamento)
                .HasForeignKey(e => e.RecetaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Medicamento)
                .WithMany(m => m.RecetaMedicamento)
                .HasForeignKey(e => e.MedicamentoId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }





}

