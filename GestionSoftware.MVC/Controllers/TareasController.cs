using GestionSoftware.API.Models;
using GestionSoftware.MVC.Models;
using HarmonySound.API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionSoftware.MVC.Controllers
{
    [Authorize]
    public class TareasController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public TareasController(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7103";
            Crud<Tarea>.EndPoint = $"{_apiBaseUrl}/api/Tareas";
        }

        private void SetJwtTokenFromSession()
        {
            
            if (HttpContext?.Session != null)
            {
                var token = HttpContext.Session.GetString("JwtToken");

                
                if (!string.IsNullOrEmpty(token))
                {
                    GlobalCrudSettings.SharedJwtToken = token;
                }
                else
                {
                }
            }
            else
            {
 
            }
        }

        private async Task<TareaViewModel> CargarListasAsync()
        {
            var viewModel = new TareaViewModel();
            
            try
            {
                SetJwtTokenFromSession();
                

                Crud<Proyecto>.EndPoint = $"{_apiBaseUrl}/api/Proyectos";
                Crud<Usuario>.EndPoint = $"{_apiBaseUrl}/api/Usuarios";
                

                var proyectos = Crud<Proyecto>.GetAll() ?? new List<Proyecto>();
                var usuarios = Crud<Usuario>.GetAll() ?? new List<Usuario>();
                

                viewModel.Proyectos = proyectos.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                }).ToList();
                
                viewModel.Usuarios = usuarios.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Nombre
                }).ToList();
                
               
                viewModel.Proyectos.Insert(0, new SelectListItem { Value = "", Text = "Seleccione un proyecto" });
                viewModel.Usuarios.Insert(0, new SelectListItem { Value = "", Text = "Seleccione un usuario" });

            }
            catch (Exception ex)
            {
                viewModel.Proyectos.Add(new SelectListItem { Value = "", Text = "Error cargando proyectos" });
                viewModel.Usuarios.Add(new SelectListItem { Value = "", Text = "Error cargando usuarios" });
            }
            
            return viewModel;
        }

        // GET: TareasController
        public ActionResult Index()
        {
            try
            {
                SetJwtTokenFromSession();
                
                // Configurar endpoints
                Crud<Proyecto>.EndPoint = $"{_apiBaseUrl}/api/Proyectos";
                Crud<Usuario>.EndPoint = $"{_apiBaseUrl}/api/Usuarios";
                
                // Obtener todas las listas
                var tareas = Crud<Tarea>.GetAll() ?? new List<Tarea>();
                var proyectos = Crud<Proyecto>.GetAll() ?? new List<Proyecto>();
                var usuarios = Crud<Usuario>.GetAll() ?? new List<Usuario>();
                
                // Crear diccionarios para lookup rápido
                ViewBag.Proyectos = proyectos.ToDictionary(p => p.Id, p => p.Nombre);
                ViewBag.Usuarios = usuarios.ToDictionary(u => u.Id, u => u.Nombre);
                
                return View(tareas);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                ViewBag.Proyectos = new Dictionary<int, string>();
                ViewBag.Usuarios = new Dictionary<int, string>();
                return View(new List<Tarea>());
            }
        }

        // GET: TareasController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                SetJwtTokenFromSession();
                var data = Crud<Tarea>.GetById(id);
                return View(data);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View();
            }
        }

        // GET: TareasController/Create
        public async Task<ActionResult> Create()
        {
            var viewModel = await CargarListasAsync();
            return View(viewModel);
        }

        // POST: TareasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TareaViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetJwtTokenFromSession();
                    
                   
                    var tarea = new Tarea
                    {
                        Titulo = model.Tarea.Titulo,
                        Descripcion = model.Tarea.Descripcion,
                        Estado = model.Tarea.Estado,
                        Prioridad = model.Tarea.Prioridad,
                        FechaVencimiento = model.Tarea.FechaVencimiento,
                        UsuarioId = model.UsuarioSeleccionado,
                        ProyectoId = model.ProyectoSeleccionado
                    };
                    
                    Crud<Tarea>.Create(tarea);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch(Exception ex) 
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError("", "Error al crear la tarea: " + ex.Message);
            }
            
   
            model = await CargarListasAsync();
            model.Tarea = model.Tarea; 
            return View(model);
        }

        // GET: TareasController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                SetJwtTokenFromSession();
                var tarea = Crud<Tarea>.GetById(id);
                
                var viewModel = await CargarListasAsync();
                viewModel.Tarea = tarea;
                viewModel.ProyectoSeleccionado = tarea.ProyectoId;
                viewModel.UsuarioSeleccionado = tarea.UsuarioId;
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TareasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, TareaViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetJwtTokenFromSession();
                    
                   
                    var tarea = new Tarea
                    {
                        Id = id,
                        Titulo = model.Tarea.Titulo,
                        Descripcion = model.Tarea.Descripcion,
                        Estado = model.Tarea.Estado,
                        Prioridad = model.Tarea.Prioridad,
                        FechaVencimiento = model.Tarea.FechaVencimiento,
                        UsuarioId = model.UsuarioSeleccionado,
                        ProyectoId = model.ProyectoSeleccionado
                    };
                    
                    Crud<Tarea>.Update(id, tarea);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch(Exception ex) 
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError("", "Error al editar la tarea: " + ex.Message);
            }
            
         
            model = await CargarListasAsync();
            model.Tarea.Id = id; 
            return View(model);
        }

        // GET: TareasController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                SetJwtTokenFromSession();
                var data = Crud<Tarea>.GetById(id);
                return View(data);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View();
            }
        }

        // POST: TareasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Tarea data)
        {
            try
            {
                SetJwtTokenFromSession();
                Crud<Tarea>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex) 
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError("", "Error al eliminar la tarea: " + ex.Message);
                return View();
            }
        }

        // GET: TareasController/Reporte
        public ActionResult Reporte(int? proyectoId, string estado)
        {
            try
            {
                SetJwtTokenFromSession();
                
            
                Crud<Proyecto>.EndPoint = $"{_apiBaseUrl}/api/Proyectos";
                Crud<Usuario>.EndPoint = $"{_apiBaseUrl}/api/Usuarios";
                
                
                var todasLasTareas = Crud<Tarea>.GetAll() ?? new List<Tarea>();
                var proyectos = Crud<Proyecto>.GetAll() ?? new List<Proyecto>();
                var usuarios = Crud<Usuario>.GetAll() ?? new List<Usuario>();
                
                
                var viewModel = new ReporteViewModel();
                
               
                viewModel.Proyectos = proyectos.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre,
                    Selected = proyectoId.HasValue && p.Id == proyectoId.Value
                }).ToList();
                
                viewModel.Proyectos.Insert(0, new SelectListItem 
                { 
                    Value = "", 
                    Text = "Todos los proyectos",
                    Selected = !proyectoId.HasValue
                });
                
                viewModel.Estados = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Todos los estados", Selected = string.IsNullOrEmpty(estado) },
                    new SelectListItem { Value = "Pendiente", Text = "Pendiente", Selected = estado == "Pendiente" },
                    new SelectListItem { Value = "En Progreso", Text = "En Progreso", Selected = estado == "En Progreso" },
                    new SelectListItem { Value = "Completada", Text = "Completada", Selected = estado == "Completada" }
                };
                
             
                var tareasFiltradas = todasLasTareas.AsQueryable();
                
                if (proyectoId.HasValue)
                {
                    tareasFiltradas = tareasFiltradas.Where(t => t.ProyectoId == proyectoId.Value);
                    viewModel.ProyectoSeleccionado = proyectoId.Value;
                }
                
                if (!string.IsNullOrEmpty(estado))
                {
                    tareasFiltradas = tareasFiltradas.Where(t => t.Estado == estado);
                    viewModel.EstadoSeleccionado = estado;
                }
                
                viewModel.TareasFiltradas = tareasFiltradas.ToList();
                
             
                viewModel.TotalTareas = viewModel.TareasFiltradas.Count;
                viewModel.TareasPendientes = viewModel.TareasFiltradas.Count(t => t.Estado == "Pendiente");
                viewModel.TareasEnProgreso = viewModel.TareasFiltradas.Count(t => t.Estado == "En Progreso");
                viewModel.TareasCompletadas = viewModel.TareasFiltradas.Count(t => t.Estado == "Completada");
                
               
                viewModel.NombresProyectos = proyectos.ToDictionary(p => p.Id, p => p.Nombre);
                viewModel.NombresUsuarios = usuarios.ToDictionary(u => u.Id, u => u.Nombre);
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ViewBag.ErrorMessage = "Error al generar el reporte";
                return View(new ReporteViewModel());
            }
        }
    }
}
