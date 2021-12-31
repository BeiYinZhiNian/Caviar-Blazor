using Caviar.AntDesignUI;
using Caviar.AntDesignUI.Helper;
using Caviar.Demo.Wasm;
using Caviar.SharedKernel.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;



namespace Caviar.Demo.Wasm
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddScoped<IAuthService, WasmAuthService>();
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>();
            builder.Services.AddSingleton<IAuthorizationService, DefaultAuthorizationService>();
            var baseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/");
            baseAddress = new Uri("http://localhost:5215/api/");
            builder.Services.AddScoped(sp =>
            {
                return new HttpClient() { BaseAddress = baseAddress };
            });
            builder.Services.AddAdminCaviar(new Type[] { typeof(Program) });
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
