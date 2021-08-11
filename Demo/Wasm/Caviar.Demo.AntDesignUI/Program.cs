using Caviar.AntDesignUI;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Caviar.Demo.AntDesignUI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<Caviar.AntDesignUI.App>("#app");
            var ServerUrl = "http://localhost:5215/api/";
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(ServerUrl) });
            builder.Services.AddCaviar(new Type[] { typeof(Program) });
            await builder.Build().RunAsync();
        }
    }
}
