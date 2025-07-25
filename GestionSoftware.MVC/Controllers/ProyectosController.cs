using GestionSoftware.API.Models;
using HarmonySound.API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionSoftware.MVC.Controllers
{
    [Authorize]
    public class ProyectosController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public ProyectosController(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7103";
            Crud<Proyecto>.EndPoint = $"{_apiBaseUrl}/api/Proyectos";
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

        public ActionResult Index()
        {
            try
            {
                SetJwtTokenFromSession();
                var data = Crud<Proyecto>.GetAll();
                return View(data);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View(new List<Proyecto>());
            }
        }

        // GET: ProyectosController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                SetJwtTokenFromSession();
                var data = Crud<Proyecto>.GetById(id);
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

        // GET: ProyectosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProyectosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Proyecto data)
        {
            try
            {
                SetJwtTokenFromSession();
                Crud<Proyecto>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex) 
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError("", "Error al crear el proyecto: " + ex.Message);
                return View(data);
            }
        }

        // GET: ProyectosController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                SetJwtTokenFromSession();
                var data = Crud<Proyecto>.GetById(id);
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

        // POST: ProyectosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Proyecto data)
        {
            try
            {
                SetJwtTokenFromSession();
                Crud<Proyecto>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                return View(data);
            }
        }

        // GET: ProyectosController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                SetJwtTokenFromSession();
                var data = Crud<Proyecto>.GetById(id);
                return View(data);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                return View();
            }
        }

        // POST: ProyectosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Proyecto data)
        {
            try
            {
                SetJwtTokenFromSession();
                Crud<Proyecto>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("401"))
                {
                    return RedirectToAction("Login", "Account");
                }
                return View();
            }
        }
    }
}
