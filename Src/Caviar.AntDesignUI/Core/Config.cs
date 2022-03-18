using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace Caviar.AntDesignUI
{
    public static class Config
    {
        /// <summary>
        /// 是否为server模式
        /// </summary>
        public static bool IsServer { get; set; } = true;
        public static bool IsDebug { get; set; } = true;
        /// <summary>
        /// 是否处理过iframeMessage
        /// </summary>
        public static bool IsHandleIframeMessage { get; set; }

        public static List<Assembly> AdditionalAssemblies;

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
            services.AddScoped<HttpService>();
            services.AddScoped<CavModal>();
            services.AddScoped<UserConfig>();
            services.AddScoped<ModalService>();
            services.AddScoped<MessageService>();
            services.AddScoped<ILanguageService, InAssemblyLanguageService>();
            services.AddScoped<HostAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<HostAuthenticationStateProvider>());
            services.AddScoped<IPrismHighlighter, PrismHighlighter>();
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
}
