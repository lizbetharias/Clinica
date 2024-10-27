using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Models;

public partial class Examen
{
    [Key]
    [Column("examenID")]
    public int ExamenId { get; set; }

    [Column("tipo")]
    [StringLength(50)]
    public string Tipo { get; set; } = null!;

    [Column("resultados")]
    [StringLength(250)]
    public string? Resultados { get; set; }

    [Column("pacienteID")]
    public int? PacienteId { get; set; }

    [ForeignKey("PacienteId")]
    [InverseProperty("Examen")]
    public virtual Paciente? Paciente { get; set; }
}
