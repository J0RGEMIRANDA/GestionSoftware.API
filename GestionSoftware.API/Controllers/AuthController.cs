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
            Console.WriteLine("=== INICIO LOGIN ===");
            Console.WriteLine($"Correo recibido: {request?.Correo}");
            Console.WriteLine($"Contraseña recibida: {request?.Contrasena}");
            
            try
            {
                if (request == null)
                {
                    Console.WriteLine("Request es null");
                    return BadRequest(new { mensaje = "Datos de login requeridos" });
                }

                using var connection = _context.CreateConnection();
                Console.WriteLine("Conexión a BD creada");
                
                // Buscar usuario por correo
                var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                    "SELECT * FROM Usuarios WHERE Correo = @Correo", 
                    new { Correo = request.Correo });

                Console.WriteLine($"Usuario encontrado: {usuario != null}");
                if (usuario != null)
                {
                    Console.WriteLine($"ID: {usuario.Id}, Nombre: {usuario.Nombre}");
                }

                if (usuario == null)
                {
                    Console.WriteLine("Usuario no encontrado");
                    return Unauthorized(new { mensaje = "Credenciales inválidas" });
                }

                // Verificar contraseña
                var contrasenaHash = HashPassword(request.Contrasena);
                Console.WriteLine($"Hash calculado: {contrasenaHash}");
                Console.WriteLine($"Hash almacenado: {usuario.ContrasenaHash}");
                Console.WriteLine($"Hashes coinciden: {usuario.ContrasenaHash == contrasenaHash}");
                
                if (usuario.ContrasenaHash != contrasenaHash)
                {
                    Console.WriteLine("Contraseña incorrecta");
                    return Unauthorized(new { mensaje = "Credenciales inválidas" });
                }

                // Generar token JWT
                Console.WriteLine("Generando token JWT...");
                var token = _jwtService.GenerarToken(usuario);
                var expiracion = DateTime.UtcNow.AddMinutes(60);

                Console.WriteLine($"Token generado exitosamente");
                Console.WriteLine($"Token: {token.Substring(0, 50)}..."); // Solo primeros 50 caracteres

                var response = new LoginResponse
                {
                    Token = token,
                    Expiracion = expiracion,
                    Usuario = usuario
                };

                Console.WriteLine("=== LOGIN EXITOSO ===");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR EN LOGIN: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            Console.WriteLine("=== INICIO REGISTER ===");
            Console.WriteLine($"Registrando usuario: {usuario?.Correo}");
            
            try
            {
                using var connection = _context.CreateConnection();
                
                // Verificar si el correo ya existe
                var usuarioExistente = await connection.QueryFirstOrDefaultAsync<Usuario>(
                    "SELECT * FROM Usuarios WHERE Correo = @Correo", 
                    new { Correo = usuario.Correo });

                if (usuarioExistente != null)
                {
                    Console.WriteLine("Correo ya existe");
                    return BadRequest(new { mensaje = "El correo ya está registrado" });
                }

                // Hashear la contraseña
                var hashOriginal = usuario.ContrasenaHash;
                usuario.ContrasenaHash = HashPassword(usuario.ContrasenaHash);
                
                Console.WriteLine($"Contraseña original: {hashOriginal}");
                Console.WriteLine($"Hash generado: {usuario.ContrasenaHash}");

                // Insertar usuario
                var sql = "INSERT INTO Usuarios (Nombre, Correo, ContrasenaHash) VALUES (@Nombre, @Correo, @ContrasenaHash)";
                await connection.ExecuteAsync(sql, usuario);

                Console.WriteLine("Usuario registrado exitosamente");
                return Ok(new { mensaje = "Usuario registrado exitosamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR EN REGISTER: {ex.Message}");
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