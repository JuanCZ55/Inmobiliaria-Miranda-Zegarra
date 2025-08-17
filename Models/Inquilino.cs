using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models;

public class Inquilino
{
    [Key]
    [Display(Name = "Codigo")]
    public int IdInquilino { get; set; }
    [Required]
    public string Dni { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    public string Apellido { get; set; }
    [Required]
    public string Telefono { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Direccion { get; set; }
    public int Estado { get; set; } = 1;// 1=Activo, 0=Inactivo
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
