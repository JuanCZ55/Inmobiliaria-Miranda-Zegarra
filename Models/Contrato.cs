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
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatorio")]
        public DateTime FechaFinalizacion { get; set; }

        public DateTime? FechaCancelacion { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser un valor positivo.")]
        public decimal Monto { get; set; }

        public decimal? Multa { get; set; }

        [Required(ErrorMessage = "El tipo de contrato es obligatorio")]
        [Range(1, 2, ErrorMessage = "Seleccione Pago Total o Pagos Mensuales.")]
        public int Tipo { get; set; }

        public int Estado { get; set; } = 1;

        [Required(ErrorMessage = "El Inquilino es obligatorio")]
        [Display(Name = "Inquilino")]
        public int IdInquilino { get; set; }

        [ForeignKey(nameof(IdInquilino))]
        [BindNever]
        [Display(Name = "Inquilino")]
        public Inquilino? Inquilino { get; set; }

        [Required(ErrorMessage = "El Inmueble es obligatorio")]
        [Display(Name = "Inmueble")]
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

        // Validación personalizada
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaInicio >= FechaFinalizacion)
            {
                yield return new ValidationResult(
                    "La fecha de inicio debe ser menor que la fecha de finalización",
                    new[] { nameof(FechaInicio), nameof(FechaFinalizacion) }
                );
            }
        }

        public bool Validate()
        {
            return FechaInicio >= FechaFinalizacion;
        }
    }
}
