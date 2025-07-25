﻿using GestionSoftware.API.Models;
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
                ViewBag.ErrorMessage = "Error al cargar usuarios";
                return View(new List<Usuario>());
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
                ModelState.AddModelError("", "Error al crear usuario");
                return View(data);
            }
        }

    }
}
