using Caviar.Demo.AntDesignUI;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Caviar.SharedKernel;
using Caviar.AntDesignUI.Helper;
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
            builder.RootComponents.Add<Caviar.AntDesignUI.App>("#app");
            var ServerUrl = "http://localhost:5215/api/";
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(ServerUrl) });
            builder.Services.AddCaviar(new Type[] { typeof(Program) });
        }
    }
}
