using AntDesign;
using Blazored.LocalStorage;
using Caviar.AntDesignUI.Helper;
using Caviar.SharedKernel;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace Caviar.AntDesignUI
{
    public static class Config
    {

        public static List<Assembly> AdditionalAssemblies;

        public static PathList PathList = new PathList();

        public static string TokenName { get; } = "authToken";

        public static IServiceCollection AddAdminCaviar(this IServiceCollection services, Type[] assemblies)
        {
            services.AddAntDesign();
            services.AddScoped<HttpHelper>();
            services.AddScoped<CavModal>();
            services.AddScoped<UserConfig>();
            services.AddScoped<ModalService>();
            services.AddScoped<MessageService>();
            services.AddSingleton<ILanguageService, InAssemblyLanguageService>();
            services.AddScoped<HostAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<HostAuthenticationStateProvider>());
            services.AddScoped<IPrismHighlighter, PrismHighlighter>();
            services.AddBlazoredLocalStorage();
            services.AddAuthorizationCore();
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
        public string Login { get; set; } = "/ApplicationUser/Login";

        public string CurrentUserInfo { get; set; } = "/ApplicationUser/CurrentUserInfo";

        public string Logout { get; set; } = "/ApplicationUser/Logout";

        public string MyDetails { get; set; } = "/ApplicationUser/MyDetails";

        public string UpdatePwd { get; set; } = "/ApplicationUserUpdatePwd";

        public string GetApiList { get; set; } = "SysMenu/GetApiList";
    }
}
