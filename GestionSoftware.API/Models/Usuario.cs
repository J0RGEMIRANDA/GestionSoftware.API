﻿namespace GestionSoftware.API.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string ContrasenaHash { get; set; }
    }
}
