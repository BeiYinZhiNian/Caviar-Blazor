using Caviar.UI;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
using Caviar.AntDesignPages.Helper;
namespace Caviar
{
    public class Program
    {
        public static string CookieName { get; set; } = "token";
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            ConfigureServices(builder);

            await builder.Build().RunAsync();
        }

        public static void ConfigureServices(WebAssemblyHostBuilder builder)
        {
            builder.RootComponents.Add<AntDesignPages.App>("#app");
            var ServerUrl = builder.Configuration["Caviar:ServerUrl"];
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(ServerUrl) });
            builder.Services.AddCaviar(new Type[] { typeof(Program) });
        }
    }
}
