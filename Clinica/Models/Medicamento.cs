using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Models;

public partial class Medicamento
{
    [Key]
    [Column("medicamentoID")]
    public int MedicamentoId { get; set; }

    [Column("nombre")]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [Column("dosis")]
    [StringLength(50)]
    public string? Dosis { get; set; }

    [Column("frecuencia")]
    [StringLength(50)]
    public string? Frecuencia { get; set; }

    [Column("viaAdministracion")]
    [StringLength(50)]
    public string? ViaAdministracion { get; set; }

    [InverseProperty("Medicamento")]
    public virtual ICollection<RecetaMedicamento> RecetaMedicamento { get; set; } = new List<RecetaMedicamento>();
}
