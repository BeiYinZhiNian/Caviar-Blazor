using Caviar.AntDesignUI;
using Caviar.AntDesignUI.Helper;
using Caviar.Infrastructure;
using Caviar.Infrastructure.API;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

namespace Caviar.Demo.Hybrid
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private IServerAddressesFeature ServerAddressesFeature { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);

                options.User.RequireUniqueEmail = true;
            });
            services
                .AddAuthentication(cfg =>
                {
                    cfg.DefaultScheme = IdentityConstants.ApplicationScheme;
                    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer()
                .AddCookie();
            // Add services to the container.
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddHttpContextAccessor();
            services.AddTransient(sp =>
            {
                var env = sp.GetService<IWebHostEnvironment>();
                var httpContextAccessor = sp.GetService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor?.HttpContext;
                var cookies = httpContext.Request.Cookies;
                var cookieContainer = new System.Net.CookieContainer();
                foreach (var c in cookies)
                {
                    cookieContainer.Add(new System.Net.Cookie(c.Key, c.Value) { Domain = httpContext.Request.Host.Host });
                }
                var user = httpContext.User.Identity.IsAuthenticated;
                var handler = new HttpClientHandler { CookieContainer = cookieContainer };
                if (env.IsDevelopment())
                {
                    handler.ServerCertificateCustomValidationCallback = (c, v, b, n) => { return true; };
                }
                return handler;
            });
            services.AddTransient(sp =>
            {
                var handler = sp.GetService<HttpClientHandler>();

                if (ServerAddressesFeature?.Addresses == null
                 || ServerAddressesFeature.Addresses.Count == 0)
                {
                    return new HttpClient(handler);
                }

                var insideIIS = Environment.GetEnvironmentVariable("APP_POOL_ID") is string;

                var address = ServerAddressesFeature.Addresses
                    .FirstOrDefault(a => a.StartsWith($"http{(insideIIS ? "s" : "")}:"))
                    ?? ServerAddressesFeature.Addresses.First();

                var uri = new Uri(address);

                return new HttpClient(handler) { BaseAddress = new Uri($"{uri.Scheme}://localhost:{uri.Port}/api/") };
            });
            services.AddAdminCaviar(new Type[] { typeof(Program) });
            services.AddScoped<IAuthService, ServerAuthService>();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            services.AddControllers();
            services.AddCaviar();
            services.AddCaviarDbContext(options =>
                options.UseSqlServer(
            Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Caviar.Demo.WebApi")));
            services.Configure<HybridOptions>(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ServerAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            // Configure the HTTP request pipeline.
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCaviar();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

        }
    }
}
