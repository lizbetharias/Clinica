using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
     => optionsBuilder.UseSqlServer();

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

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
