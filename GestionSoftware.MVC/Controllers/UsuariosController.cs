using GestionSoftware.API.Models;
using HarmonySound.API.Consumer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestionSoftware.MVC.Controllers
{
    public class UsuariosController : Controller
    {
        // GET: UsuariosController
        public ActionResult Index()
        {
            var data = Crud<Usuario>.GetAll();
            return View(data);
        }

        // GET: UsuariosController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Usuario>.GetById(id);
            return View(data);
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
                Crud<Usuario>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex) 
            {
                // Log the exception or handle it as needed
                ModelState.AddModelError("", "An error occurred while creating the user: " + ex.Message);
                return View(data);
            }
        }

        // GET: UsuariosController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Usuario>.GetById(id);
            return View(data);
        }

        // POST: UsuariosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Usuario data)
        {
            try
            { 
                Crud<Usuario>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex) 
            {
                // Log the exception or handle it as needed
                ModelState.AddModelError("", "An error occurred while editing the user: " + ex.Message);
                // Return the view with the current data to allow corrections
                return View(data);
            }
        }

        // GET: UsuariosController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Usuario>.GetById(id);
            return View(data);
        }

        // POST: UsuariosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Tarea data)
        {
            try
            {
                Crud<Usuario>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex) 
            {
                // Log the exception or handle it as needed
                ModelState.AddModelError("", "An error occurred while deleting the user: " + ex.Message);
                return View();
            }
        }
    }
}
