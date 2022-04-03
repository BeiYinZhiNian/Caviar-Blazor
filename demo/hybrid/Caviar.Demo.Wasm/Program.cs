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
        /// serverģʽ��wasmģʽ������ʼ��
        /// </summary>
        public static void PublicInit()
        {
#if DEBUG
            // �˴�Ϊ����wasmģʽ�ĵ���״̬
            Config.IsDebug = true;
#endif
        }
    }
}
