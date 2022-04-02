using Caviar.AntDesignUI;
using Caviar.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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

        public void ConfigureServices(IServiceCollection services)
        {
            //身份验证配置
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);

                options.User.RequireUniqueEmail = true;
            });
            //cookies配置
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            });
            //cookies验证配置
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

            Wasm.Program.PublicInit();
            //客户端配置
            services.AddCaviarServer();
            services.AddAdminCaviar(new Type[] { typeof(Program),typeof(Wasm.Program) });

            //服务端配置
            services.AddCaviar();
            services.AddCaviarDbContext(options =>
            //选择使用mysql或者sqlserver，其他的数据库只要efcore支持，本框架都支持
            options.UseSqlServer(
            Configuration.GetConnectionString("DefaultConnection")
            //options.UseMySQL(
            //Configuration.GetConnectionString("DefaultConnection")
            , b => b.MigrationsAssembly("Caviar.Demo.Hybrid")));
            //跨域
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            //控制器
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }

            app.UseBlazorFrameworkFiles();
            
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCaviar();
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
