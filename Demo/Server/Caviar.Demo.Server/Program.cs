using Caviar.AntDesignUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
var ServerUrl = "http://localhost:5215/api/";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(ServerUrl) });
builder.Services.AddCaviar(new Type[] { typeof(Program) });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
