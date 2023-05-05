using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Shopee.Models;
using System.Data;
using System.Net.Http.Headers;

namespace Shopee.Controllers
{
    public class APICall<T> : Controller
    {
        private string Baseurl = "https://localhost:7105";

        public async void Put(string controllerName, T model, int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = null;
                Res = await client.PutAsJsonAsync<T>($"api/{controllerName}/{id}", model);
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    Console.WriteLine($"Put in {controllerName} failed");
                }
            }
        }

        public async void Post(string controllerName, T model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var Res = await client.PostAsJsonAsync<T>($"api/{controllerName}", model);
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    Console.WriteLine($"Post in {controllerName} failed");
                }
            }
        }

        public async Task<List<T>> Get<T>(string controllerName, int id = -1)
        {
            List<T> objList = new List<T>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = null;
                if (id >= 0)
                {
                    Res = await client.GetAsync($"api/{controllerName}/{id}");
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        T obj = JsonConvert.DeserializeObject<T>(Response);
                        return new List<T>() { obj };
                    }
                }
                else
                {
                    Res = await client.GetAsync($"api/{controllerName}");
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;

                        objList = JsonConvert.DeserializeObject<List<T>>(Response);
                        return objList;
                    }
                }
            }
            return objList;
        }

        public async void Delete(string controllerName, int id = -1)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = null;
                if (id >= 0)
                {
                    Res = await client.DeleteAsync($"api/{controllerName}/{id}");
                }
                else
                {
                    Res = await client.DeleteAsync($"api/{controllerName}");
                }
            }
        }

    }
}
