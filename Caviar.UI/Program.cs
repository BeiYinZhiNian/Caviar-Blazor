using Caviar.UI;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Caviar.UI.Helper;

namespace Caviar
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            ConfigureServices(builder);

            await builder.Build().RunAsync();
        }

        public static void ConfigureServices(WebAssemblyHostBuilder builder)
        {
            builder.RootComponents.Add<App>("#app");
            var ServerUrl = builder.Configuration["Caviar:ServerUrl"];
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(ServerUrl) });
            builder.Services.AddScoped(typeof(HttpHelper));
            builder.Services.AddAntDesign();
            builder.Services.AddSingleton<UserState>();
        }
    }
}
