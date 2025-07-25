using Dapper;
using GestionSoftware.API.Data;
using GestionSoftware.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionSoftware.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProyectosController : ControllerBase
    {
        private readonly GestionProyectosSoftwareContext _context;

        public ProyectosController(GestionProyectosSoftwareContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using var connection = _context.CreateConnection();
            return Ok(await connection.QueryAsync<Proyecto>("SELECT * FROM Proyectos"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using var connection = _context.CreateConnection();
            var proyecto = await connection.QueryFirstOrDefaultAsync<Proyecto>(
                "SELECT * FROM Proyectos WHERE Id = @Id", new { Id = id });
            return proyecto == null ? NotFound() : Ok(proyecto);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Proyecto p)
        {
            using var connection = _context.CreateConnection();
            var sql = "INSERT INTO Proyectos (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";
            await connection.ExecuteAsync(sql, p);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, Proyecto p)
        {
            using var connection = _context.CreateConnection();
            p.Id = id;
            var sql = "UPDATE Proyectos SET Nombre=@Nombre, Descripcion=@Descripcion WHERE Id=@Id";
            await connection.ExecuteAsync(sql, p);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Proyectos WHERE Id = @Id", new { Id = id });
            return Ok();
        }
    }
}
