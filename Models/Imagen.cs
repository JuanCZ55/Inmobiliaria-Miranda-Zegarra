using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inmobiliaria.Models
{
    public class Imagen
    {
        [Key]
        [Display(Name = "Código")]
        public int IdImagen { get; set; }

        [Display(Name = "Inmueble")]
        public int IdInmueble { get; set; }

        [ForeignKey(nameof(IdInmueble))]
        [BindNever]
        public Inmueble? Inmueble { get; set; }

        [Display(Name = "Url")]
        public string Url { get; set; } = string.Empty;

        [Display(Name = "Tipo")]
        public int Tipo { get; set; } = 0; // 1 = portada, 2 = galeria
    }
}
