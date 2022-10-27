// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using CacheManager.Core;
using Caviar.Core.Interface;
using Caviar.Core.Services;
using Caviar.Infrastructure.API;
using Caviar.Infrastructure.Persistence;
using Caviar.SharedKernel.Common;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
            _ = services.AddTransient(sp =>
            {
                var handler = sp.GetService<HttpClientHandler>();
                HttpClient client = new HttpClient(handler);
                var httpContextAccessor = sp.GetService<IHttpContextAccessor>();
                var config = sp.GetService<CaviarConfig>();
                var httpContext = httpContextAccessor?.HttpContext;
                var interactor = new Interactor(httpContextAccessor);
                client.DefaultRequestHeaders.Add(CurrencyConstant.UserAgent, interactor.Browser);
                client.DefaultRequestHeaders.Add(CurrencyConstant.XForwardedFor, interactor.Current_Ipaddress);
                var uri = GetServerUri();
                if (uri != null)
                {
                    client.BaseAddress = new Uri($"{config.Urls.Replace("0.0.0.0", "localhost")}/{CurrencyConstant.Api}");
                }
                return client;
            });

            services.AddScoped<IInteractor, Interactor>();
            return services;
        }

        private static Uri GetServerUri()
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
            var cacheManager = CacheFactory.Build("StartedCache", settings =>
            {
                settings.
                WithMicrosoftMemoryCacheHandle("caviar")
                .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromMinutes(3));
            });
            services.AddSingleton(cacheManager);
            services.AddSingleton<CaviarConfig>(); // 配置文件
            services.AddScoped<IAuthService, ServerAuthService>();
            services.AddScoped<ILanguageService, InAssemblyLanguageService>();
            services.AddScoped<Interactor>();
            services.AddScoped<IAppDbContext, ApplicationDbContext>();
            services.AddScoped(typeof(IEasyBaseServices<,>), typeof(EasyBaseServices<,>));
            var injection = new AutomaticInjection();
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

        public static void ReadConfig<T>(string appsettingPath, T anonymousTypeObject)
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

    internal class AutomaticInjection
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
