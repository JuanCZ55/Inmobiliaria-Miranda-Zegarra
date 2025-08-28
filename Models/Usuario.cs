using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
  public class Usuario
  {
    [Key]
    [Display(Name = "Codigo")]
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio")]
    public string Observaciones { get; set; } = string.Empty;

    [Required(ErrorMessage = "La constraseña es obligatorio")]
    public string? Clave { get; set; } = string.Empty;


    [Required(ErrorMessage = "El rol es obligatorio")]
    public string? Rol { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = string.Empty;

    // Falta el avatar

    public int Estado { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
  }
}
