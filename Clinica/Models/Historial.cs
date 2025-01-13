using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Models;
public partial class Historial
{
    [Key]
    [Column("historialID")]
    public int HistorialId { get; set; }

    public DateTime Fecha { get; set; }

    [ForeignKey("Paciente")]
    [Column("pacienteID")]
    public int? PacienteId { get; set; }

    [ForeignKey("Especialidad")]
    [Column("especialidadID")]
    public int? EspecialidadId { get; set; }

    [InverseProperty("Historial")]
    public virtual Paciente? Paciente { get; set; }

    [InverseProperty("Historial")]
    public virtual Especialidad? Especialidad { get; set; }
}
