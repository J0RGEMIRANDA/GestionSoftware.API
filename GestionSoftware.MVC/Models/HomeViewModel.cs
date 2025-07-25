namespace GestionSoftware.MVC.Models
{
    public class HomeViewModel
    {
        public int TotalProyectos { get; set; }
        public int TotalTareas { get; set; }
        public int TotalUsuarios { get; set; }
        public int TareasPendientes { get; set; }
        public int TareasEnProgreso { get; set; }
        public int TareasCompletadas { get; set; }
    }
}