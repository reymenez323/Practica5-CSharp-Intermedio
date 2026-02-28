using System.ComponentModel.DataAnnotations;

namespace Practica5.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Correo { get; set; } = string.Empty;

    public DateTime FechaDeNacimiento { get; set; }
}