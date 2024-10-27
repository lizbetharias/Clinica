using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Models;

public partial class Receta
{
    [Key]
    [Column("recetaID")]
    public int RecetaId { get; set; }

    [Column("fecha", TypeName = "datetime")]
    public DateTime Fecha { get; set; }

    [Column("pacienteID")]
    public int? PacienteId { get; set; }

    public int IdUsuario { get; set; }

    [Column("diagnosticoID")]
    public int? DiagnosticoId { get; set; }

    [ForeignKey("DiagnosticoId")]
    [InverseProperty("Receta")]
    public virtual Diagnostico? Diagnostico { get; set; }

    [ForeignKey("IdUsuario")]
    [InverseProperty("Receta")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    [ForeignKey("PacienteId")]
    [InverseProperty("Receta")]
    public virtual Paciente? Paciente { get; set; }

    [InverseProperty("Receta")]
    public virtual ICollection<RecetaMedicamento> RecetaMedicamento { get; set; } = new List<RecetaMedicamento>();
}
