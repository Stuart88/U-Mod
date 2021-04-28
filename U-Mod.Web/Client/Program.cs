using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BlazorPro.BlazorSize;
using Blazor.Analytics;

namespace U_Mod.Web.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");

            builder.Services
                 .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons()
                 .AddSingleton(new HttpClient
                 {
                     BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
                 })
                 .AddScoped<ResizeListener>()
                 .AddScoped<IMediaQueryService, MediaQueryService>()
                 .AddGoogleAnalytics("G-616RTS520B");

            var host = builder.Build();

            await host.RunAsync();
        }
    }
}
