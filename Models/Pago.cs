using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inmobiliaria.Models
{
  public class Pago
  {
    [Key]
    [Display(Name = "Codigo")]
    public int IdPago { get; set; }

    [Required(ErrorMessage = "El número de pago es obligatorio")]
    public int numeroPago { get; set; }

    [Required(ErrorMessage = "Fecha de pago es obligatoria")]
    public DateTime FechaPago { get; set; }

    [Required(ErrorMessage = "El concepto es obligatorio")]
    public string Concepto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El monto de pago es obligatorio")]
    public decimal Monto { get; set; }

    public int Estado { get; set; } = 1;

    public int IdContrato { get; set; }
    [ForeignKey(nameof(IdContrato))]
    [BindNever]
    public Contrato? contrato { get; set; }

/*
    public int IdUsuario { get; set; }
    [ForeignKey(nameof(IdUsuario))]
    [BindNever]
    public Usuario? user { get; set; }
*/

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
  }
}
