using GestionSoftware.API.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionSoftware.MVC.Models
{
    public class ReporteViewModel
    {

        public List<SelectListItem> Proyectos { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Estados { get; set; } = new List<SelectListItem>();
        

        public int? ProyectoSeleccionado { get; set; }
        public string? EstadoSeleccionado { get; set; }

        public List<Tarea> TareasFiltradas { get; set; } = new List<Tarea>();
        

        public int TotalTareas { get; set; }
        public int TareasPendientes { get; set; }
        public int TareasEnProgreso { get; set; }
        public int TareasCompletadas { get; set; }

        public Dictionary<int, string> NombresProyectos { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> NombresUsuarios { get; set; } = new Dictionary<int, string>();
    }
}