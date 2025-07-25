using GestionSoftware.API.Models;
using GestionSoftware.MVC.Models;
using HarmonySound.API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GestionSoftware.MVC.Controllers
{
    [Authorize] 
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
            

            if (User.Identity.IsAuthenticated)
            {
                try
                {

                    var token = HttpContext.Session.GetString("JwtToken");
                    if (!string.IsNullOrEmpty(token))
                    {

                        

                        GlobalCrudSettings.SharedJwtToken = token;
                        
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
                        SetDefaultValues(viewModel);
                    }
                }
                catch (Exception ex)
                {
                    SetDefaultValues(viewModel);
                }
            }
            else
            {
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
