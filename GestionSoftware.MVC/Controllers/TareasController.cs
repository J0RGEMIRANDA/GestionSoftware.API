using GestionSoftware.API.Models;
using HarmonySound.API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            // REMOVER: SetJwtTokenFromSession();
        }

        private void SetJwtTokenFromSession()
        {
            Console.WriteLine($"=== SetJwtTokenFromSession() - {this.GetType().Name} ===");
            
            if (HttpContext?.Session != null)
            {
                var token = HttpContext.Session.GetString("JwtToken");
                Console.WriteLine($"Token en sesión: {!string.IsNullOrEmpty(token)}");
                
                if (!string.IsNullOrEmpty(token))
                {
                    // CRÍTICO: Usar el token global compartido
                    GlobalCrudSettings.SharedJwtToken = token;
                    Console.WriteLine($"Token configurado globalmente");
                }
                else
                {
                    Console.WriteLine("⚠️ NO hay token en la sesión");
                }
            }
            else
            {
                Console.WriteLine("⚠️ HttpContext o Session es null");
            }
        }

        // GET: TareasController
        public ActionResult Index()
        {
            try
            {
                SetJwtTokenFromSession();
                var data = Crud<Tarea>.GetAll();
                return View(data);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: TareasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tarea data)
        {
            try
            {
                SetJwtTokenFromSession();
                Crud<Tarea>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex) 
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError("", "Error al crear la tarea: " + ex.Message);
                return View(data);
            }
        }

        // GET: TareasController/Edit/5
        public ActionResult Edit(int id)
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

        // POST: TareasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Tarea data)
        {
            try
            {
                SetJwtTokenFromSession();
                Crud<Tarea>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex) 
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError("", "Error al editar la tarea: " + ex.Message);
                return View(data);
            }
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
    }
}
