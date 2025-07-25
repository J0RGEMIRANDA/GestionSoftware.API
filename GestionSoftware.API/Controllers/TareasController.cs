using Dapper;
using GestionSoftware.API.Data;
using GestionSoftware.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionSoftware.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly GestionProyectosSoftwareContext _context;

        public TareasController(GestionProyectosSoftwareContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using var connection = _context.CreateConnection();
            var tareas = await connection.QueryAsync<Tarea>("SELECT * FROM Tareas");
            return Ok(tareas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using var connection = _context.CreateConnection();
            var tarea = await connection.QueryFirstOrDefaultAsync<Tarea>(
                "SELECT * FROM Tareas WHERE Id = @Id", new { Id = @id });
            return tarea == null ? NotFound() : Ok(tarea);
        }

        [HttpGet("porProyecto/{proyectoId}")]
        public async Task<IActionResult> GetPorProyecto(int proyectoId)
        {
            using var connection = _context.CreateConnection();
            var tareas = await connection.QueryAsync<Tarea>(
                "SELECT * FROM Tareas WHERE ProyectoId = @ProyectoId", new { ProyectoId = @proyectoId });
            return Ok(tareas);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Tarea t)
        {
            using var connection = _context.CreateConnection();
            var sql = @"INSERT INTO Tareas 
                    (Titulo, Descripcion, Estado, Prioridad, FechaVencimiento, UsuarioId, ProyectoId)
                    VALUES (@Titulo, @Descripcion, @Estado, @Prioridad, @FechaVencimiento, @UsuarioId, @ProyectoId)";
            await connection.ExecuteAsync(sql, t);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, Tarea t)
        {
            using var connection = _context.CreateConnection();
            t.Id = id;
            var sql = @"UPDATE Tareas SET 
                    Titulo=@Titulo, Descripcion=@Descripcion, Estado=@Estado, Prioridad=@Prioridad, 
                    FechaVencimiento=@FechaVencimiento, UsuarioId=@UsuarioId, ProyectoId=@ProyectoId
                    WHERE Id=@Id";
            await connection.ExecuteAsync(sql, t);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Tareas WHERE Id = @Id", new { Id = @id });
            return Ok();
        }
    }
}
