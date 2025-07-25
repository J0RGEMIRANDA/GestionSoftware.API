namespace GestionSoftware.API.Models
{
    public class Tarea
    {   public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public string Prioridad { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int UsuarioId { get; set; }
        public int ProyectoId { get; set; }
        public Usuario? Usuario { get; set; }
        public Proyecto? Proyecto { get; set; }

    }
}
