using GestionSoftware.API.Models;
using GestionSoftware.MVC.Models;
using HarmonySound.API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GestionSoftware.MVC.Controllers
{
    // Remover [Authorize] temporalmente para permitir acceso sin login
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7103";
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel();
            
            // Solo intentar obtener datos si el usuario está autenticado
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    // Configurar token desde la sesión
                    var token = HttpContext.Session.GetString("JwtToken");
                    if (!string.IsNullOrEmpty(token))
                    {
                        Console.WriteLine($"=== HOME DEBUG ===");
                        Console.WriteLine($"Token recuperado de sesión: {!string.IsNullOrEmpty(token)}");
                        
                        // CRÍTICO: Configurar el token globalmente para TODOS los tipos
                        GlobalCrudSettings.SharedJwtToken = token;
                        
                        Console.WriteLine($"Token configurado globalmente: {!string.IsNullOrEmpty(GlobalCrudSettings.SharedJwtToken)}");

                        // Configurar endpoints
                        Crud<Proyecto>.EndPoint = $"{_apiBaseUrl}/api/Proyectos";
                        Crud<Tarea>.EndPoint = $"{_apiBaseUrl}/api/Tareas";
                        Crud<Usuario>.EndPoint = $"{_apiBaseUrl}/api/Usuarios";

                        var proyectos = Crud<Proyecto>.GetAll() ?? new List<Proyecto>();
                        var tareas = Crud<Tarea>.GetAll() ?? new List<Tarea>();
                        var usuarios = Crud<Usuario>.GetAll() ?? new List<Usuario>();

                        viewModel.TotalProyectos = proyectos.Count;
                        viewModel.TotalTareas = tareas.Count;
                        viewModel.TotalUsuarios = usuarios.Count;
                        viewModel.TareasPendientes = tareas.Count(t => t.Estado == "Pendiente");
                        viewModel.TareasEnProgreso = tareas.Count(t => t.Estado == "En Progreso");
                        viewModel.TareasCompletadas = tareas.Count(t => t.Estado == "Completada");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ No hay token en la sesión - HomeController");
                        SetDefaultValues(viewModel);
                    }
                }
                catch (Exception ex)
                {
                    // Si hay error, usar valores por defecto
                    Console.WriteLine($"Error conectando con API: {ex.Message}");
                    SetDefaultValues(viewModel);
                }
            }
            else
            {
                // Usuario no autenticado, mostrar valores por defecto
                SetDefaultValues(viewModel);
            }

            return View(viewModel);
        }

        private void SetDefaultValues(HomeViewModel viewModel)
        {
            viewModel.TotalProyectos = 0;
            viewModel.TotalTareas = 0;
            viewModel.TotalUsuarios = 0;
            viewModel.TareasPendientes = 0;
            viewModel.TareasEnProgreso = 0;
            viewModel.TareasCompletadas = 0;
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
