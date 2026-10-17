using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario o correo electrónico es obligatorio.")]
        [Display(Name = "Nombre de usuario o correo electrónico")]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }

        [Display(Name = "URL de retorno")]
        // Corregido: Se agrega el '?' para que el framework entienda que puede ser nulo u opcional
        public string? ReturnUrl { get; set; }
    }
}
