using Caviar.Core.Interface;
using Caviar.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Builder;
using Caviar.Core.Services;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Caviar.Infrastructure.API;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Caviar.SharedKernel.Entities;

namespace Caviar.Infrastructure
{
    public static class Configure
    {
        public static bool HasDataInit { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }
        private static IServerAddressesFeature ServerAddressesFeature { get; set; }
        public static IServiceCollection AddCaviarServer(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient(sp =>
            {
                var env = sp.GetService<IWebHostEnvironment>();
                var httpContextAccessor = sp.GetService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor?.HttpContext;
                var cookies = httpContext.Request.Cookies;
                var cookieContainer = new System.Net.CookieContainer();
                var uri = GetServerUri();
                var domain = uri == null ? httpContext.Request.Host.Host : uri.Host;
                foreach (var c in cookies)
                {
                    try
                    {
                        cookieContainer.Add(new System.Net.Cookie(c.Key, c.Value) { Domain = "localhost" });
                    }
                    catch
                    {
                        //无效cookies过滤
                    }
                }
                
                var handler = new HttpClientHandler { CookieContainer = cookieContainer };
                return handler;
            });
            services.AddTransient(sp =>
            {
                var handler = sp.GetService<HttpClientHandler>();
                HttpClient client = new HttpClient(handler);
                var httpContextAccessor = sp.GetService<IHttpContextAccessor>();
                var config = sp.GetService<CaviarConfig>();
                var httpContext = httpContextAccessor?.HttpContext;
                var ip = CommonHelper.GetUserIp(httpContext);
                client.DefaultRequestHeaders.Add(CurrencyConstant.UserAgent, CurrencyConstant.DefaultUserAgent);
                client.DefaultRequestHeaders.Add(CurrencyConstant.XForwardedFor, ip);
                var uri = GetServerUri();
                if (uri != null)
                {
                    client.BaseAddress = new Uri($"{config.Urls}/{CurrencyConstant.Api}");
                }
                return client;
            });
            return services;
        }

        static Uri GetServerUri()
        {
            if (ServerAddressesFeature?.Addresses == null
                 || ServerAddressesFeature.Addresses.Count == 0)
            {
                return null;
            }
            var insideIIS = Environment.GetEnvironmentVariable("APP_POOL_ID") is string;

            var address = ServerAddressesFeature.Addresses
                .FirstOrDefault(a => a.StartsWith($"http{(insideIIS ? "s" : "")}:"))
                ?? ServerAddressesFeature.Addresses.First();

            var uri = new Uri(address);
            return uri;
        }

        public static IServiceCollection AddCaviar(this IServiceCollection services)
        {
            services.AddSingleton<CaviarConfig>(); // 配置文件
            services.AddScoped<IAuthService, ServerAuthService>();
            services.AddScoped<ILanguageService, InAssemblyLanguageService>();
            services.AddScoped<Interactor>();
            services.AddScoped<IAppDbContext, ApplicationDbContext>();
            services.AddScoped(typeof(IEasyBaseServices<,>),typeof(EasyBaseServices<,>));
            AutomaticInjection injection = new AutomaticInjection();
            injection.AddIBaseModel(services);
            injection.AddInject(services);
            return services;
        }

        public static IApplicationBuilder UseCaviar(this IApplicationBuilder app)
        {
            ServerAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            ServiceProvider = app.ApplicationServices;
            var caviarConfig = ServiceProvider.GetService<CaviarConfig>();
            HasDataInit = true;
            new SysDataInit(app.ApplicationServices).StartInit().Wait(); // 先进行数据初始化，然后获取配置文件
            HasDataInit = false;
            ReadConfig("appsettings.json", caviarConfig);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseGlobalExceptionHandling();
            return app;
        }

        public static void ReadConfig<T>(string appsettingPath,T anonymousTypeObject)
        {
            string appsettings = "{}";
            if (File.Exists(appsettingPath))
            {
                appsettings = File.ReadAllText(appsettingPath);
            }
            JsonConvert.PopulateObject(appsettings, anonymousTypeObject);
        }

        /// <summary>
        /// 自定义用户表和角色表
        /// 后面很多情况暂时不支持自定义用户和角色表
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TRole"></typeparam>
        /// <param name="services"></param>
        public static IdentityBuilder AddCaviarDbContext<TUser, TRole>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TUser : IdentityUser<int>
            where TRole : IdentityRole<int>
        {
            services.AddDbContext<SysDbContext<TUser, TRole, int>>(optionsAction, contextLifetime, optionsLifetime);
            var identityBuilder = services.AddIdentity<TUser, TRole>()
                    .AddEntityFrameworkStores<SysDbContext<TUser, TRole, int>>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();

            services.AddScoped<IDbContext, SysDbContext<TUser, TRole, int>>();
            return identityBuilder;
        }

        /// <summary>
        /// 自定义用户表和角色表
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TRole"></typeparam>
        /// <param name="services"></param>
        public static IdentityBuilder AddCaviarDbContext(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            var identityBuilder = services.AddCaviarDbContext<ApplicationUser, ApplicationRole>(optionsAction, contextLifetime, optionsLifetime);
            return identityBuilder;
        }

    }

    class AutomaticInjection
    {

        /// <summary>
        /// 遍历所有的类，筛选实现IService接口的类，并过判断是否是类,并按照注解方式自动注入类
        /// 自动注入所有继承IDIinjectAtteribute接口的类
        /// </summary>
        public void AddIBaseModel(IServiceCollection services)
        {
            //获取所有对象
            CommonHelper.GetAllTypes()
            //查找是否包含IDIinjectAtteribute接口的类
            .Where(u => u.GetInterfaces().Contains(typeof(IDIinjectAtteribute)))
            //判断是否是类
            .Where(u => u.IsClass)
            //转换成list
            .ToList()
            //循环,并添注入
            .ForEach(t =>
            {
                services.AddTransient(t);
            });

        }
        /// <summary>
        /// 自动注入所有带有Inject特性类
        /// </summary>
        /// <param name="services"></param>
        public void AddInject(IServiceCollection services)
        {
            //获取所有对象
            CommonHelper.GetAllTypes()
            //查找是否包含DI特性并且查看是否是抽象类
            .Where(a => a.GetCustomAttributes(true).Select(t => t.GetType()).Contains(typeof(DIInjectAttribute)) && !a.IsAbstract)
            //判断是否是类
            .Where(u => u.IsClass)
            //转换成list
            .ToList()
            //循环,并添注入
            .ForEach(t =>
            {
                var inject = (DIInjectAttribute)t.GetCustomAttributes(true).FirstOrDefault(d => d.GetType() == typeof(DIInjectAttribute));
                switch (inject.InjectType)
                {
                    case InjectType.SINGLETON:
                        services.AddSingleton(t);
                        break;
                    case InjectType.SCOPED:
                        services.AddScoped(t);
                        break;
                    case InjectType.TRANSIENT:
                        services.AddTransient(t);
                        break;
                }
            });
        }
    }
}
