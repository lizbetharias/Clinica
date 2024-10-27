using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Models;

public partial class Diagnostico
{
    [Key]
    [Column("diagnosticoID")]
    public int DiagnosticoId { get; set; }

    [Column("descripcion")]
    [StringLength(250)]
    public string Descripcion { get; set; } = null!;

    [Column("observaciones")]
    [StringLength(250)]
    public string? Observaciones { get; set; }

    [InverseProperty("Diagnostico")]
    public virtual ICollection<Receta> Receta { get; set; } = new List<Receta>();
}
