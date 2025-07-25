using GestionSoftware.API.Models;
using HarmonySound.API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionSoftware.MVC.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public UsuariosController(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7103";
            Crud<Usuario>.EndPoint = $"{_apiBaseUrl}/api/Usuarios";
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

        // GET: UsuariosController
        public ActionResult Index()
        {
            try
            {
                SetJwtTokenFromSession();
                var data = Crud<Usuario>.GetAll();
                return View(data);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View(new List<Usuario>());
            }
        }

        // GET: UsuariosController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                SetJwtTokenFromSession();
                var data = Crud<Usuario>.GetById(id);
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

        // GET: UsuariosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuariosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario data)
        {
            try
            {
                SetJwtTokenFromSession();
                Crud<Usuario>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex) 
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError("", "An error occurred while creating the user: " + ex.Message);
                return View(data);
            }
        }

        // GET: UsuariosController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                SetJwtTokenFromSession();
                var data = Crud<Usuario>.GetById(id);
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

        // POST: UsuariosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Usuario data)
        {
            try
            {
                SetJwtTokenFromSession();
                Crud<Usuario>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex) 
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError("", "An error occurred while editing the user: " + ex.Message);
                return View(data);
            }
        }

        // GET: UsuariosController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                SetJwtTokenFromSession();
                var data = Crud<Usuario>.GetById(id);
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

        // POST: UsuariosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Usuario data)
        {
            try
            {
                SetJwtTokenFromSession();
                Crud<Usuario>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex) 
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError("", "An error occurred while deleting the user: " + ex.Message);
                return View();
            }
        }
    }
}
