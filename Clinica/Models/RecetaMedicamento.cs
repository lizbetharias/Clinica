using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Models;

[PrimaryKey("RecetaId", "MedicamentoId")]
public partial class RecetaMedicamento
{
    [Key]
    [Column("recetaID")]
    public int RecetaId { get; set; }

    [Key]
    [Column("medicamentoID")]
    public int MedicamentoId { get; set; }

    [Column("cantidad")]
    public int? Cantidad { get; set; }

    [Column("instrucciones")]
    [StringLength(250)]
    public string? Instrucciones { get; set; }

    [ForeignKey("MedicamentoId")]
    [InverseProperty("RecetaMedicamento")]
    public virtual Medicamento Medicamento { get; set; } = null!;

    [ForeignKey("RecetaId")]
    [InverseProperty("RecetaMedicamento")]
    public virtual Receta Receta { get; set; } = null!;
}
