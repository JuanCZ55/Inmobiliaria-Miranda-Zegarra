using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inmobiliaria.Models
{
  public class Contrato
  {
    [Key]
    [Display(Name = "Codigo")]
    public int IdContrato { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatorio")]
    public DateTime FechaDesde { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatorio")]
    public DateTime FechaHasta { get; set; }

    public DateTime? FechaFin { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio")]
    public decimal MontoMensual { get; set; }

    public decimal Multa { get; set; }

    public int Estado { get; set; } = 1;

    [Required(ErrorMessage = "El Inquilino es obligatorio")]
    public int IdInquilino { get; set; }
    [ForeignKey(nameof(IdInquilino))]
    [BindNever]
    public Inquilino? Inquilino { get; set; }

    [Required(ErrorMessage = "El Inmueble es obligatorio")]
    public int IdInmueble { get; set; }
    [ForeignKey(nameof(IdInmueble))]
    [BindNever]
    public Inmueble? Inmueble { get; set; }

/*
    public int IdUsuarioCreador { get; set; }
    [ForeignKey(nameof(IdUsuarioCreador))]
    [BindNever]
    public Usuario? UsuarioCreador { get; set; }
    
    public int IdUsuarioFinalizador { get; set; }
    [ForeignKey(nameof(IdUsuarioCreador))]
    [BindNever]
    public Usuario? UsuarioFinalizador { get; set; }
*/

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
  }
}
