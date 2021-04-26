using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace U_Mod.Shared.Helpers
{


    public static class HttpExtentions
    {

        public static void AddDefaultRequestHeader(this HttpClient http, string usernameEmail, string password)
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("AuthHeader", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{usernameEmail}:{password}")));
        }

        public static async Task<TResponse> FetchAsync<TResponse>(this HttpClient http, string uri, bool useCamelCasePolicy = true)
        {
            using HttpResponseMessage resp = await http.GetAsync($"{uri}");

            http.DefaultRequestHeaders.Clear();

            string json = await resp.Content.ReadAsStringAsync();

            var responseObject = JsonSerializer.Deserialize<TResponse>(json, new JsonSerializerOptions { PropertyNamingPolicy = useCamelCasePolicy ? JsonNamingPolicy.CamelCase : null });

            return responseObject;
        }

        public static async Task<TResponse> FetchAsync<TResponse>(this HttpClient http, string uri, string authName, string authPass)
        {
            http.AddDefaultRequestHeader(authName, authPass);

            return await http.FetchAsync<TResponse>(uri);
        }

        public static async Task<TResponse> SendAsync<TResponse, TRequest>(this HttpClient http, string uri, TRequest item, string authName, string authPass)
        {
            http.AddDefaultRequestHeader(authName, authPass);

            return await http.SendAsync<TResponse, TRequest>(uri, item);
        }

        public static async Task<TResponse> SendAsync<TResponse, TRequest>(this HttpClient http, string uri, TRequest item)
        {
            HttpPostHelper<TRequest> request = new HttpPostHelper<TRequest>(uri, item);
            request.Method = HttpMethod.Post;

            using HttpResponseMessage resp = await http.SendAsync(request);

            http.DefaultRequestHeaders.Clear();

            string json = await resp.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TResponse>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }

    public class HttpPostHelper<T> : HttpRequestMessage
    {
        public HttpPostHelper(string uri, T obj) : base(HttpMethod.Post, $"{uri}")
        {
            Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Content = new StringContent(JsonSerializer.Serialize<T>(obj), Encoding.UTF8, "application/json");
        }
    }
}
