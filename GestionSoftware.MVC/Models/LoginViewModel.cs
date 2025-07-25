using System.ComponentModel.DataAnnotations;

namespace GestionSoftware.MVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Ingrese un correo v�lido")]
        [Display(Name = "Correo Electr�nico")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contrase�a es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contrase�a")]
        public string Contrasena { get; set; } = string.Empty;
    }
}