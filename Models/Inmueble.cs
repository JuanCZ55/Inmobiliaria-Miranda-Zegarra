using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inmobiliaria.Models
{
    public class Inmueble
    {
        [Key]
        [Display(Name = "Codigo")]
        public int IdInmueble { get; set; } = 0;

        [Required(ErrorMessage = "El campo Propietario es obligatorio.")]
        public int IdPropietario { get; set; }

        [ForeignKey(nameof(IdPropietario))]
        [BindNever]
        public Propietario? Propietario { get; set; }

        [Required(ErrorMessage = "El campo Tipo de Inmueble es obligatorio.")]
        public int IdTipoInmueble { get; set; } = 0;

        [ForeignKey(nameof(IdTipoInmueble))]
        public TipoInmueble? TipoInmueble { get; set; }

        [Required(ErrorMessage = "La direccion es obligatoria.")]
        [StringLength(255, ErrorMessage = "La direccion no puede superar los 255 caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El uso es obligatorio.")]
        [Range(1, 2, ErrorMessage = "Seleccione 1 (Residencial) o 2 (Comercial)")]
        public int Uso { get; set; }

        [Required(ErrorMessage = "La cantidad de ambientes es obligatoria.")]
        [Range(1, 20, ErrorMessage = "Debe tener al menos 1 ambiente.")]
        public int CantidadAmbientes { get; set; }

        [Required(ErrorMessage = "La longitud es obligatoria.")]
        [RegularExpression(
            @"^(\+|-)?(?:180(?:\.0+)?|1[0-7]\d(?:\.\d+)?|\d{1,2}(?:\.\d+)?)$",
            ErrorMessage = "La longitud no es válida."
        )]
        public string Longitud { get; set; } = string.Empty;

        [Required(ErrorMessage = "La latitud es obligatoria.")]
        [RegularExpression(
            @"^(\+|-)?(?:90(?:\.0+)?|[0-8]?\d(?:\.\d+)?)$",
            ErrorMessage = "La latitud no es válida."
        )]
        public string Latitud { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(
            0.01,
            double.MaxValue,
            ErrorMessage = "El precio debe ser un número positivo y válido."
        )]
        public decimal Precio { get; set; }

        [StringLength(255, ErrorMessage = "La descripcion no puede superar los 255 caracteres.")]
        public string? Descripcion { get; set; } = " -";

        public int Estado { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
