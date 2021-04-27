using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using U_Mod.Shared.Models;

namespace U_Mod.Client.Helpers
{
    public class HttpPostHelper<T> : HttpRequestMessage
    {
        public HttpPostHelper(string uri, T obj): base(HttpMethod.Post, uri)
        {
            this.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.Content = new StringContent(JsonSerializer.Serialize<T>(obj), Encoding.UTF8, "application/json");
        }

        public async Task<BasicHttpResponse<TF>> SendAsync<TF>(HttpClient http)
        {
            using HttpResponseMessage resp = await http.SendAsync(this);

            return await resp.Content.ReadFromJsonAsync<BasicHttpResponse<TF>>();
        }
    }
}
