using GestionSoftware.API.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionSoftware.MVC.Models
{
    public class TareaViewModel
    {
        public Tarea Tarea { get; set; } = new Tarea();
        public List<SelectListItem> Proyectos { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Usuarios { get; set; } = new List<SelectListItem>();
        

        public int ProyectoSeleccionado { get; set; }
        public int UsuarioSeleccionado { get; set; }
    }
}