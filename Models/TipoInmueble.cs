using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class TipoInmueble
    {
        [Key]
        public int IdTipoInmueble { get; set; } = 0;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Relación inversa: un tipo puede tener muchos inmuebles
        public ICollection<Inmueble>? Inmuebles { get; set; }
    }
}
