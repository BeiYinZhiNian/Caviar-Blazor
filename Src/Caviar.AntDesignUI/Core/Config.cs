using AntDesign;
using Blazored.LocalStorage;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;


namespace Caviar.AntDesignUI
{
    public static class Config
    {
        /// <summary>
        /// 是否为server模式
        /// </summary>
        public static bool IsServer { get; set; } = true;

        public static List<Assembly> AdditionalAssemblies;

        public static PathList PathList = new PathList();

        public static WebAssemblyHostBuilder AddCavWasm(this WebAssemblyHostBuilder builder)
        {
            IsServer = false;
            builder.Services.AddScoped<IAuthService, WasmAuthService>();
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>();
            builder.Services.AddSingleton<IAuthorizationService, DefaultAuthorizationService>();
            return builder;
        }

        public static IServiceCollection AddAdminCaviar(this IServiceCollection services, Type[] assemblies)
        {
            services.AddAntDesign();
            services.AddScoped<HttpHelper>();
            services.AddScoped<CavModal>();
            services.AddScoped<UserConfig>();
            services.AddScoped<ModalService>();
            services.AddScoped<MessageService>();
            services.AddScoped<ILanguageService, InAssemblyLanguageService>();
            services.AddScoped<HostAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<HostAuthenticationStateProvider>());
            services.AddScoped<IPrismHighlighter, PrismHighlighter>();
            services.AddScoped<CavNavigationManager>();
            if (assemblies != null)
            {
                AdditionalAssemblies = new List<Assembly>();
                foreach (var item in assemblies)
                {
                    AdditionalAssemblies.Add(item.Assembly);
                }
            }
            return services;
        }
    }

    public class PathList
    {
        public string Home { get; set; } = "/";
        public string Login { get; set; } = "ApplicationUser/Login";

        

        public string Logout { get; set; } = "ApplicationUser/Logout";

        public string MyDetails { get; set; } = "ApplicationUser/MyDetails";

        public string UpdatePwd { get; set; } = "ApplicationUserUpdatePwd";


        public string CurrentUserInfo { get; set; } = "ApplicationUser/CurrentUserInfo";
        public string GetApiList { get; set; } = "SysMenu/GetApiList";

        public string SetCookieLanguage { get; set; } = "api/SysMenu/SetCookieLanguage";
    }
}
