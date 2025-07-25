using System.ComponentModel.DataAnnotations;

namespace GestionSoftware.MVC.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Ingrese un correo v�lido")]
        [Display(Name = "Correo Electr�nico")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contrase�a es requerida")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contrase�a")]
        public string Contrasena { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contrase�a")]
        [Compare("Contrasena", ErrorMessage = "Las contrase�as no coinciden.")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}