using GestionSoftware.API.Models;
using GestionSoftware.MVC.Models;
using HarmonySound.API.Consumer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionSoftware.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7103";
        }

        [HttpGet]
        public ActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                
                var loginRequest = new LoginRequest
                {
                    Correo = model.Correo,
                    Contrasena = model.Contrasena
                };



               
                var loginResponse = Crud<LoginResponse>.PostAuth<LoginResponse>(
                    $"{_apiBaseUrl}/api/Auth/login", 
                    loginRequest
                );

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                   
                    
                    GlobalCrudSettings.SharedJwtToken = loginResponse.Token;
                    
                    
                    
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, loginResponse.Usuario.Id.ToString()),
                        new Claim(ClaimTypes.Name, loginResponse.Usuario.Nombre),
                        new Claim(ClaimTypes.Email, loginResponse.Usuario.Correo),
                        new Claim("JwtToken", loginResponse.Token),
                        new Claim("TokenExpiration", loginResponse.Expiracion.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = loginResponse.Expiracion
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    
                    HttpContext.Session.SetString("JwtToken", loginResponse.Token);
                    
                   
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Credenciales inválidas");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                
                var usuario = new Usuario
                {
                    Nombre = model.Nombre,
                    Correo = model.Correo,
                    ContrasenaHash = model.Contrasena 
                };

               
                var response = Crud<object>.PostAuth<object>(
                    $"{_apiBaseUrl}/api/Auth/register", 
                    usuario
                );

                TempData["SuccessMessage"] = "Usuario registrado exitosamente. Puede iniciar sesión.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("JwtToken");
            Crud<object>.JwtToken = null;
            return RedirectToAction("Login");
        }

        public ActionResult AccessDenied()
        {
            return View();
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
    }
}
