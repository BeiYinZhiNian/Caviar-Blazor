using Caviar.Core.Interface;
using Caviar.Infrastructure.Persistence;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Caviar.SharedKernel.Entities.Base;
using Microsoft.AspNetCore.Builder;
using Caviar.Core.Services;

namespace Caviar.Infrastructure
{
    public static class Configure
    {
        public static bool HasDataInit { get; set; }

        public static CaviarConfig CaviarConfig { get; set; }

        public static IServiceProvider ServiceProvider { get; set; }

        public static IServiceCollection AddCaviar(this IServiceCollection services)
        {
            services.AddScoped<ILanguageService, InAssemblyLanguageService>();
            services.AddScoped<Interactor>();
            services.AddScoped<IAppDbContext, ApplicationDbContext>();
            services.AddScoped(typeof(IEasyBaseServices<>),typeof(EasyBaseServices<>));
            AutomaticInjection injection = new AutomaticInjection();
            injection.AddIBaseModel(services);
            injection.AddInject(services);
            ReadConfig("appsettings.json");
            return services;
        }

        public static IApplicationBuilder UseCaviar(this IApplicationBuilder app)
        {
            ServiceProvider = app.ApplicationServices;
            new SysDataInit(app.ApplicationServices).StartInit().Wait();
            app.Use((context, next) =>
            {
                var idCookiaName = CurrencyConstant.LanguageHeader;
                if (!context.Request.Cookies.Any(c => c.Key == idCookiaName))
                {
                    var idCookieOptions = new CookieOptions
                    {
                        Path = "/",
                        Secure = true,
                        HttpOnly = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.Now.AddYears(100),
                    };
                    context.Response.Cookies.Append(
                        key: idCookiaName,
                        value: "zh-CN",
                        options: idCookieOptions);
                }
                return next();
            });
            return app;
        }

        public static void ReadConfig(string appsettingPath)
        {
            string appsettings = "{}";
            if (File.Exists(appsettingPath))
            {
                appsettings = File.ReadAllText(appsettingPath);
            }
            CaviarConfig = JsonConvert.DeserializeObject<CaviarConfig>(appsettings);
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


        /// <summary>
        /// 获取用户的ip地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUserIp(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
        /// <summary>
        /// 获取请求的完整地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetAbsoluteUri(this HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
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
            CommonHelper.GetAssembly()
                //遍历查找
                .ForEach((t =>
                {
                        //获取所有对象
                        t.GetTypes()
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
                }));

        }
        /// <summary>
        /// 自动注入所有带有Inject特性类
        /// </summary>
        /// <param name="services"></param>
        public void AddInject(IServiceCollection services)
        {
            CommonHelper.GetAssembly()
               //遍历查找
               .ForEach((t =>
               {
                       //获取所有对象
                       t.GetTypes()
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
               }));
        }
    }
}
