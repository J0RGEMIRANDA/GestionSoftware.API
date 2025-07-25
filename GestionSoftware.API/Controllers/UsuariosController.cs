using Dapper;
using GestionSoftware.API.Data;
using GestionSoftware.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionSoftware.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Agregar esta línea
    public class UsuariosController : ControllerBase
    {
        private readonly GestionProyectosSoftwareContext _context;

        public UsuariosController(GestionProyectosSoftwareContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            using var connection = _context.CreateConnection();
            var usuarios = await connection.QueryAsync<Usuario>("SELECT * FROM Usuarios");
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            using var connection = _context.CreateConnection();
            var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                "SELECT * FROM Usuarios WHERE Id = @Id", new { Id = id });
            return usuario == null ? NotFound() : Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Usuario u)
        {
            using var connection = _context.CreateConnection();
            var sql = "INSERT INTO Usuarios (Nombre, Correo, ContrasenaHash) VALUES (@Nombre, @Correo, @ContrasenaHash)";
            await connection.ExecuteAsync(sql, u);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, Usuario u)
        {
            using var connection = _context.CreateConnection();
            u.Id = id;
            var sql = "UPDATE Usuarios SET Nombre=@Nombre, Correo=@Correo, ContrasenaHash=@ContrasenaHash WHERE Id=@Id";
            await connection.ExecuteAsync(sql, u);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Usuarios WHERE Id = @Id", new { Id = id });
            return Ok();
        }
    }
}
