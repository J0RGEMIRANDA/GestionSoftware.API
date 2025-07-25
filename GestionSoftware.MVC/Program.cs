using Microsoft.AspNetCore.Authentication.Cookies;
using HarmonySound.API.Consumer;

namespace GestionSoftware.MVC
{
    public class Program
    { 
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configurar URLs fijas
            builder.WebHost.UseUrls("https://localhost:7002", "http://localhost:5002");

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configurar autenticación con cookies
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.SlidingExpiration = true;
                });

            // Configurar sesiones
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configurar endpoints para Crud
            var configuration = app.Services.GetRequiredService<IConfiguration>();
            var apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7103";
            
            Crud<object>.EndPoint = $"{apiBaseUrl}/api";

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            Console.WriteLine($"MVC ejecutándose en: https://localhost:7002");
            Console.WriteLine($"API debe estar en: https://localhost:7103");

            app.Run();
        }
    }
}
