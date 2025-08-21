namespace Inmobiliaria.Models
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;

  public class Propietario
  {
    [Key]
    [Display(Name = "Codigo")]
    public int IdPropietario { get; set; }

    [Required(ErrorMessage = "El DNI es obligatorio")]
    [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener solo numeros y entre 7 y 8 digitos")]
    public string Dni { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
    [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "El nombre solo puede contener letras, espacios y sin tildes")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres")]
    [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "El apellido solo puede contener letras, espacios y sin tildes")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El telefono es obligatorio")]
    [StringLength(20, ErrorMessage = "El telefono no puede superar los 20 caracteres")]
    [RegularExpression(@"^\+?\d{8,15}$", ErrorMessage = "El telefono debe tener solo numeros y puede empezar con +, entre 8 y 15 digitos")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [RegularExpression(
          @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
          ErrorMessage = "El correo no tiene un formato válido, pedrosanchez@gmail.com."
      )]
    [StringLength(100, ErrorMessage = "El email no puede superar los 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La direccion es obligatoria")]
    [RegularExpression(@"^(?=.*[A-Za-z])[\w\s,./-]+$", ErrorMessage = "La direccion puede ser como: Av. Libertador 2500 / Piso 3 - Calle Falsa S/N - Ruta 3 km 45")]
    [StringLength(100, ErrorMessage = "La direccion no puede superar los 100 caracteres")]
    public string Direccion { get; set; } = string.Empty;

    public int Estado { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
  }
}
