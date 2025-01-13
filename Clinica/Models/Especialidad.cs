using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Models;

public partial class Especialidad
{
    [Key]
    [Column("especialidadID")]
    public int EspecialidadId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? Detalle { get; set; }

    [InverseProperty("Especialidad")]
    public virtual ICollection<Historial> Historial { get; set; } = new List<Historial>();

    [InverseProperty("Especialidad")]
    public virtual ICollection<Paciente> Paciente { get; set; } = new List<Paciente>();
}

