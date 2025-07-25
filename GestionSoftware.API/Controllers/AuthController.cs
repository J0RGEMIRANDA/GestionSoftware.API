using Microsoft.AspNetCore.Mvc;
using Dapper;
using GestionSoftware.API.Data;
using GestionSoftware.API.Models;
using GestionSoftware.API.Services;
using System.Security.Cryptography;
using System.Text;

namespace GestionSoftware.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GestionProyectosSoftwareContext _context;
        private readonly JwtService _jwtService;

        public AuthController(GestionProyectosSoftwareContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            
            try
            {
                if (request == null)
                {
                    return BadRequest(new { mensaje = "Datos de login requeridos" });
                }

                using var connection = _context.CreateConnection();
                
           
                var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                    "SELECT * FROM Usuarios WHERE Correo = @Correo", 
                    new { Correo = request.Correo });

                if (usuario != null)
                {
                }

                if (usuario == null)
                {
                    return Unauthorized(new { mensaje = "Credenciales inválidas" });
                }

        
                var contrasenaHash = HashPassword(request.Contrasena);
                
                if (usuario.ContrasenaHash != contrasenaHash)
                {
                    return Unauthorized(new { mensaje = "Credenciales inválidas" });
                }

      

                var token = _jwtService.GenerarToken(usuario);
                var expiracion = DateTime.UtcNow.AddMinutes(60);

                var response = new LoginResponse
                {
                    Token = token,
                    Expiracion = expiracion,
                    Usuario = usuario
                };

                return Ok(response);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Usuario usuario)
        {

            
            try
            {
                using var connection = _context.CreateConnection();
                
             
                var usuarioExistente = await connection.QueryFirstOrDefaultAsync<Usuario>(
                    "SELECT * FROM Usuarios WHERE Correo = @Correo", 
                    new { Correo = usuario.Correo });

                if (usuarioExistente != null)
                {

                    return BadRequest(new { mensaje = "El correo ya está registrado" });
                }

         
                var hashOriginal = usuario.ContrasenaHash;
                usuario.ContrasenaHash = HashPassword(usuario.ContrasenaHash);
 
         
                var sql = "INSERT INTO Usuarios (Nombre, Correo, ContrasenaHash) VALUES (@Nombre, @Correo, @ContrasenaHash)";
                await connection.ExecuteAsync(sql, usuario);

              
                return Ok(new { mensaje = "Usuario registrado exitosamente" });
            }
            catch (Exception ex)
            {
      
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}