using Caviar.AntDesignUI;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Caviar.Demo.Wasm
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            var baseAddress = new Uri(builder.HostEnvironment.BaseAddress + CurrencyConstant.Api);
            builder.Services.AddScoped(sp =>
            {
                return new HttpClient() { BaseAddress = baseAddress };
            });
            builder.AddCavWasm();
            builder.Services.AddAdminCaviar(new Type[] { typeof(Program) });
#if DEBUG
            // 此处为开启wasm模式的调试状态
            Config.IsDebug = true;
#endif
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
