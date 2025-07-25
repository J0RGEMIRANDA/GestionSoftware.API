using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace HarmonySound.API.Consumer
{
    public static class Crud<T>
    {
        public static string EndPoint { get; set; }
        
        // CAMBIO CRÍTICO: Usar una propiedad estática global compartida
        public static string JwtToken 
        { 
            get => GlobalCrudSettings.SharedJwtToken; 
            set => GlobalCrudSettings.SharedJwtToken = value; 
        }

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            
            // DEBUGGING: Mostrar el estado del token
            Console.WriteLine($"=== CRUD DEBUG ===");
            Console.WriteLine($"EndPoint: {EndPoint}");
            Console.WriteLine($"JwtToken presente: {!string.IsNullOrEmpty(JwtToken)}");
            if (!string.IsNullOrEmpty(JwtToken))
            {
                Console.WriteLine($"Token (primeros 50 chars): {JwtToken.Substring(0, Math.Min(50, JwtToken.Length))}...");
                client.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtToken);
                Console.WriteLine("Authorization header agregado");
            }
            else
            {
                Console.WriteLine("⚠️ NO HAY TOKEN JWT - La petición fallará");
            }
            
            return client;
        }

        public static List<T> GetAll()
        {
            Console.WriteLine($"=== GetAll() llamado para {typeof(T).Name} ===");
            using (var client = CreateHttpClient())
            {
                var response = client.GetAsync(EndPoint).Result;
                Console.WriteLine($"Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<List<T>>(json);
                    Console.WriteLine($"✅ GetAll exitoso - {result?.Count ?? 0} elementos");
                    return result;
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"❌ GetAll falló: {response.StatusCode} - {errorContent}");
                    throw new Exception($"Error: {response.StatusCode} - {errorContent}");
                }
            }
        }

        public static T GetById(int id)
        {
            Console.WriteLine($"=== GetById({id}) llamado para {typeof(T).Name} ===");
            using (var client = CreateHttpClient())
            {
                var response = client.GetAsync($"{EndPoint}/{id}").Result;
                Console.WriteLine($"Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<T>(json);
                    Console.WriteLine($"✅ GetById exitoso");
                    return result;
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"❌ GetById falló: {response.StatusCode} - {errorContent}");
                    throw new Exception($"Error: {response.StatusCode} - {errorContent}");
                }
            }
        }

        public static T Create(T item)
        {
            Console.WriteLine($"=== Create() llamado para {typeof(T).Name} ===");
            using (var client = CreateHttpClient())
            {
                var response = client.PostAsync(
                        EndPoint,
                        new StringContent(
                            JsonConvert.SerializeObject(item),
                            Encoding.UTF8,
                            "application/json"
                        )
                    ).Result;

                Console.WriteLine($"Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<T>(json);
                    Console.WriteLine($"✅ Create exitoso");
                    return result;
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"❌ Create falló: {response.StatusCode} - {errorContent}");
                    throw new Exception($"Error: {response.StatusCode} - {errorContent}");
                }
            }
        }

        public static bool Update(int id, T item)
        {
            Console.WriteLine($"=== Update({id}) llamado para {typeof(T).Name} ===");
            using (var client = CreateHttpClient())
            {
                var response = client.PutAsync(
                        $"{EndPoint}/{id}",
                        new StringContent(
                            JsonConvert.SerializeObject(item),
                            Encoding.UTF8,
                            "application/json"
                        )
                    ).Result;

                Console.WriteLine($"Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Update exitoso");
                    return true;
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"❌ Update falló: {response.StatusCode} - {errorContent}");
                    throw new Exception($"Error: {response.StatusCode} - {errorContent}");
                }
            }
        }

        public static bool Delete(int id)
        {
            Console.WriteLine($"=== Delete({id}) llamado para {typeof(T).Name} ===");
            using (var client = CreateHttpClient())
            {
                var response = client.DeleteAsync($"{EndPoint}/{id}").Result;
                Console.WriteLine($"Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Delete exitoso");
                    return true;
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"❌ Delete falló: {response.StatusCode} - {errorContent}");
                    throw new Exception($"Error: {response.StatusCode} - {errorContent}");
                }
            }
        }

        // Métodos específicos para autenticación
        public static TResult PostAuth<TResult>(string endpoint, object data)
        {
            Console.WriteLine($"=== PostAuth() llamado ===");
            Console.WriteLine($"Endpoint: {endpoint}");
            
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(
                        endpoint,
                        new StringContent(
                            JsonConvert.SerializeObject(data),
                            Encoding.UTF8,
                            "application/json"
                        )
                    ).Result;

                Console.WriteLine($"Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<TResult>(json);
                    Console.WriteLine($"✅ PostAuth exitoso");
                    return result;
                }
                else
                {
                    var error = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"❌ PostAuth falló: {response.StatusCode} - {error}");
                    throw new Exception($"Error {response.StatusCode}: {error}");
                }
            }
        }
    }

    // NUEVA CLASE: Para compartir el token globalmente
    public static class GlobalCrudSettings
    {
        public static string SharedJwtToken { get; set; }
    }
}

