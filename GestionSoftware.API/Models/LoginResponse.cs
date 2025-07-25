namespace GestionSoftware.API.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}