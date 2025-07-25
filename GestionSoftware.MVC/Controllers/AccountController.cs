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
                // Crear el objeto de login request
                var loginRequest = new LoginRequest
                {
                    Correo = model.Correo,
                    Contrasena = model.Contrasena
                };

                Console.WriteLine($"=== MVC LOGIN DEBUG ===");
                Console.WriteLine($"Llamando API: {_apiBaseUrl}/api/Auth/login");

                // Llamar a la API de autenticación
                var loginResponse = Crud<LoginResponse>.PostAuth<LoginResponse>(
                    $"{_apiBaseUrl}/api/Auth/login", 
                    loginRequest
                );

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    Console.WriteLine($"✅ Login exitoso - Token recibido");
                    Console.WriteLine($"Token: {loginResponse.Token.Substring(0, 50)}...");

                    // CRÍTICO: Configurar el token GLOBALMENTE
                    GlobalCrudSettings.SharedJwtToken = loginResponse.Token;
                    
                    Console.WriteLine($"Token configurado globalmente: {!string.IsNullOrEmpty(GlobalCrudSettings.SharedJwtToken)}");

                    // Crear claims para la autenticación local
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

                    // Guardar el token en la sesión
                    HttpContext.Session.SetString("JwtToken", loginResponse.Token);
                    
                    Console.WriteLine($"Token guardado en sesión: {HttpContext.Session.GetString("JwtToken") != null}");

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
                Console.WriteLine($"❌ Error en MVC Login: {ex.Message}");
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
                // Crear objeto usuario
                var usuario = new Usuario
                {
                    Nombre = model.Nombre,
                    Correo = model.Correo,
                    ContrasenaHash = model.Contrasena // Se hasheará en la API
                };

                // Llamar a la API de registro
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
            Crud<object>.JwtToken = null; // Limpiar el token
            return RedirectToAction("Login");
        }

        public ActionResult AccessDenied()
        {
            return View();
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
    }
}
