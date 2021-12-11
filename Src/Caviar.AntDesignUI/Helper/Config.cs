using AntDesign;
using Blazored.LocalStorage;
using Caviar.AntDesignUI.Helper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace Caviar.AntDesignUI
{
    public static class Config
    {
        public static string CookieName { get; set; } = "token";

        public static List<Assembly> AdditionalAssemblies;

        public static IServiceCollection AddCaviar(this IServiceCollection services, Type[] assemblies)
        {
            services.AddAntDesign();
            services.AddScoped<HttpHelper>();
            services.AddScoped<CavModal>();
            services.AddSingleton<UserConfig>();
            services.AddSingleton<ModalService>();
            services.AddSingleton<MessageService>();
            services.AddScoped<IPrismHighlighter, PrismHighlighter>();
            services.AddBlazoredLocalStorage();
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            services.AddScoped<IAuthService, AuthService>();
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
