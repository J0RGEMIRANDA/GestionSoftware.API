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
 
        public static string JwtToken 
        { 
            get => GlobalCrudSettings.SharedJwtToken; 
            set => GlobalCrudSettings.SharedJwtToken = value; 
        }

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            
            if (!string.IsNullOrEmpty(JwtToken))
            {
                client.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtToken);

            }
            else
            {
            }
            
            return client;
        }

        public static List<T> GetAll()
        {
    
            using (var client = CreateHttpClient())
            {
                var response = client.GetAsync(EndPoint).Result;
                
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<List<T>>(json);
           
                    return result;
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
          
                    throw new Exception($"Error: {response.StatusCode} - {errorContent}");
                }
            }
        }

        public static T GetById(int id)
        {
     
            using (var client = CreateHttpClient())
            {
                var response = client.GetAsync($"{EndPoint}/{id}").Result;
                
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<T>(json);
           
                    return result;
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;

                    throw new Exception($"Error: {response.StatusCode} - {errorContent}");
                }
            }
        }

        public static T Create(T item)
        {
           
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
                
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<T>(json);
           
                    return result;
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
                 
                    throw new Exception($"Error: {response.StatusCode} - {errorContent}");
                }
            }
        }

        public static bool Update(int id, T item)
        {
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

                
                if (response.IsSuccessStatusCode)
                {
                
                    return true;
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
                  
                    throw new Exception($"Error: {response.StatusCode} - {errorContent}");
                }
            }
        }

        public static bool Delete(int id)
        {

            using (var client = CreateHttpClient())
            {
                var response = client.DeleteAsync($"{EndPoint}/{id}").Result;
                
                
                if (response.IsSuccessStatusCode)
                {
                    
                    return true;
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
                    throw new Exception($"Error: {response.StatusCode} - {errorContent}");
                }
            }
        }

        public static TResult PostAuth<TResult>(string endpoint, object data)
        {
           
            
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

                
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<TResult>(json);
                   
                    return result;
                }
                else
                {
                    var error = response.Content.ReadAsStringAsync().Result;
                   
                    throw new Exception($"Error {response.StatusCode}: {error}");
                }
            }
        }
    }

    public static class GlobalCrudSettings
    {
        public static string SharedJwtToken { get; set; }
    }
}

