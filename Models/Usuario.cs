using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models
{
  public class Usuario
  {
    [Key]
    [Display(Name = "Codigo")]
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
    [RegularExpression(
        @"^[A-Za-z\s]+$",
        ErrorMessage = "El nombre solo puede contener letras, espacios y sin tildes"
    )]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres")]
    [RegularExpression(
        @"^[A-Za-z\s]+$",
        ErrorMessage = "El apellido solo puede contener letras, espacios y sin tildes"
    )]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El genero es obligatorio")]
    [Range(1, 2, ErrorMessage = "Seleccione Masculino o Femenino")]
    public int Genero { get; set; } = 1;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [RegularExpression(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        ErrorMessage = "El correo no tiene un formato válido, pedrosanchez@gmail.com."
    )]
    [StringLength(100, ErrorMessage = "El email no puede superar los 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener un mínimo de 8 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es obligatorio")]
    [Range(1, 2, ErrorMessage = "Seleccione Administrador o Empleado")]
    public int Rol { get; set; } = 1;

    public string AvatarURL { get; set; } = string.Empty;

    public IFormFile? AvatarFile { get; set; }

    public int Estado { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  }
}
