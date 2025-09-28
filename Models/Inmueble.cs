using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inmobiliaria.Models
{
    public class Inmueble
    {
        [Key]
        [Display(Name = "Código")]
        public int IdInmueble { get; set; }

        [Display(Name = "Propietario")]
        [Required(ErrorMessage = "El campo Propietario es obligatorio.")]
        public int IdPropietario { get; set; }

        [ForeignKey(nameof(IdPropietario))]
        [BindNever]
        public Propietario? Propietario { get; set; }

        [Required(ErrorMessage = "El campo Tipo de Inmueble es obligatorio.")]
        [Display(Name = "Tipo de Inmueble")]
        public int IdTipoInmueble { get; set; }

        [ForeignKey(nameof(IdTipoInmueble))]
        [BindNever]
        public TipoInmueble? TipoInmueble { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(255, ErrorMessage = "La dirección no puede superar los 255 caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El uso es obligatorio.")]
        [Range(1, 2, ErrorMessage = "El uso debe ser 1 (Residencial) o 2 (Comercial).")]
        public int Uso { get; set; }

        [Required(ErrorMessage = "La cantidad de ambientes es obligatoria.")]
        [Display(Name = "Cantidad de Ambientes")]
        public int CantidadAmbientes { get; set; }

        [Required(ErrorMessage = "La longitud es obligatoria.")]
        public string Longitud { get; set; } = string.Empty;

        [Required(ErrorMessage = "La latitud es obligatoria.")]
        public string Latitud { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(
            0.01,
            double.MaxValue,
            ErrorMessage = "El precio debe ser un número positivo y válido."
        )]
        public decimal Precio { get; set; }

        [StringLength(255, ErrorMessage = "La descripción no puede superar los 255 caracteres.")]
        public string? Descripcion { get; set; } = " -";

        [Range(1, 2, ErrorMessage = "El estado debe ser 1 o 2")]
        public int Estado { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public IFormFile? FilePortada { get; set; }

        public List<IFormFile>? FileGaleria { get; set; }

        public List<Imagen>? listImagenes { get; set; } = new List<Imagen>();

        public List<int>? EliminarIDs { get; set; }
    }
}
