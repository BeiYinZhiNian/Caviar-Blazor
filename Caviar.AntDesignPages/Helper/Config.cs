using AntDesign;
using Caviar.Models.SystemData;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Helper
{
    public static class Config
    {
        public static string CookieName { get; set; } = "token";

        public static List<Assembly> AdditionalAssemblies;

        public static IServiceCollection AddCaviar(this IServiceCollection services, Type[] assemblies)
        {
            services.AddScoped(typeof(HttpHelper));
            services.AddAntDesign();
            services.AddSingleton(typeof(UserConfig));
            services.AddSingleton<UserToken>();
            services.AddSingleton<CavModal>();
            services.AddSingleton<ModalService>();
            services.AddSingleton<MessageService>();
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
