using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Caviar.Control
{
    public static class CaviarConfig
    {
        public static SqlConfig SqlConfig { get; private set; }

        public static IConfiguration Configuration { get; private set; }

        public static Guid NoLoginRoleGuid { get; private set; }
        public static Guid SysAdminRoleGuid { get; private set; }
        static Guid TokenKey;
        public static string SessionUserInfoName { get; private set; } = "SysUserInfo";

        public static bool IsDebug { get; set; }

        public static IServiceCollection AddCaviar(this IServiceCollection services, SqlConfig sqlConfig, IConfiguration configuration)
        {
            SqlConfig = sqlConfig;
            services.AddDbContext<DataContext>();
            services.AddScoped<IDataContext, SysDataContext>();
            services.AddSingleton<IMemoryCache, MemoryCache>();
            Configuration = configuration;

            var caviarDynamicConfig = new CaviarDynamicConfig();
            caviarDynamicConfig.AddIBaseModel(services);
            caviarDynamicConfig.AddInject(services);
            LoadAppsettings();
            return services;
        }

        static void LoadAppsettings()
        {
            var appsettingPath = "appsettings.json";
            string appsettings = "{}";
            if (File.Exists(appsettingPath))
            {
                appsettings = File.ReadAllText(appsettingPath);
            }
            var json = JObject.Parse(appsettings);
            PaseAppsettingsJson(ref json);

            IsDebug = bool.Parse(json["Caviar"]["IsDebug"].ToString());
            var guid = json["Caviar"]["Role"]["NoLoginRole"].ToString();
            NoLoginRoleGuid = Guid.Parse(guid);
            SysAdminRoleGuid = Guid.Parse(json["Caviar"]["Role"]["SysAdminRole"].ToString());
            TokenKey = Guid.Parse(json["Caviar"]["TokenKey"].ToString());
            var paseJson = json.ToString();
            File.WriteAllText(appsettingPath, paseJson);
        }

        static void PaseAppsettingsJson(ref JObject json)
        {
            if (json["Caviar"] == null) json["Caviar"] = new JObject();
            if (json["Caviar"]["IsDebug"] == null) json["Caviar"]["IsDebug"] = false;
            if (json["Caviar"]["Role"] == null) json["Caviar"]["Role"] = new JObject();
            if (json["Caviar"]["Role"]["NoLoginRole"] == null) json["Caviar"]["Role"]["NoLoginRole"] = Guid.NewGuid();
            if (json["Caviar"]["Role"]["SysAdminRole"] == null) json["Caviar"]["Role"]["SysAdminRole"] = Guid.NewGuid();
            if (json["Caviar"]["TokenKey"] == null) json["Caviar"]["TokenKey"] = Guid.NewGuid();
        }


        public static IServiceProvider ApplicationServices { get; set; }

        public static IApplicationBuilder UserCaviar(this IApplicationBuilder app)
        {
            ApplicationServices = app.ApplicationServices;
            return app;
        }

        public static string GetUserToken(UserToken userToken)
        {
            return CommonHelper.SHA256EncryptString(userToken.Id + userToken.UserName + userToken.Uid.ToString() + userToken.CreateTime + TokenKey.ToString());
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

        private static List<Assembly> _assemblies;

        /// <summary>
        /// 使用加载器技术
        /// </summary>
        /// <returns></returns>
        public static List<Assembly> GetAssembly()
        {
            if (_assemblies == null)
            {
                _assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(u => !u.FullName.Contains("Microsoft"))//排除微软类库
                .Where(u => !u.FullName.Contains("System"))//排除系统类库
                .Where(u => !u.FullName.Contains("Newtonsoft"))//排除Newtonsoft.json
                .Where(u => !u.FullName.Contains("Swagger"))//排除Swagger
                .Where(u => !u.FullName.Contains("EntityFrameworkCore"))//排除EntityFrameworkCore
                .ToList();
            }
            return _assemblies;
        }

        #region IBaseModelExtend扩展




        #endregion


        class CaviarDynamicConfig
        {
            
            /// <summary>
            /// 遍历所有的类，筛选实现IService接口的类，并过判断是否是类,并按照注解方式自动注入类
            /// 自动注入所有继承IDIinjectAtteribute接口的类
            /// </summary>
            public void AddIBaseModel(IServiceCollection services)
            {
                GetAssembly()
                    //遍历查找
                    .ForEach((t =>
                    {
                        //获取所有对象
                        t.GetTypes()
                            //查找是否包含IService接口的类
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
                GetAssembly()
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



    public class SqlConfig
    {
        public string Connections { get; set; }

        public DBTypeEnum DBTypeEnum { get; set; }
    }
}
