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
            PublicInit();
            var host = builder.Build();
            await host.RunAsync();
        }
        /// <summary>
        /// server模式和wasm模式公共初始化
        /// </summary>
        public static void PublicInit()
        {
#if DEBUG
            // 此处为开启wasm模式的调试状态
            Config.IsDebug = true;
#endif
        }
    }
}
