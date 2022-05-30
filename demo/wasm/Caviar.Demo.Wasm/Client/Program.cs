using Caviar.AntDesignUI;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseAddress = new Uri(builder.HostEnvironment.BaseAddress + CurrencyConstant.Api);
builder.Services.AddScoped(sp =>
{
    return new HttpClient() { BaseAddress = baseAddress };
});
builder.AddCavWasm();
builder.Services.AddAdminCaviar(new Type[] { typeof(Program) });
await builder.Build().RunAsync();
