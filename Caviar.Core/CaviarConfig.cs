using Caviar.SharedKernel;
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
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.HttpOverrides;

namespace Caviar.Core
{
    public static class CaviarConfig
    {
        public static SqlConfig SqlConfig { get; private set; }
        public static EnclosureConfig EnclosureConfig { get; private set; }
        public static IConfiguration Configuration { get; private set; }
        public static TokenConfig TokenConfig { get; private set; }
        public static Guid NoLoginRoleGuid { get; private set; }
        public static Guid SysAdminRoleGuid { get; private set; }
        public static Guid UserAdminGuid { get; private set; }
        public static CodeFileGenerate WebUI { get; set; }
        public static CodeFileGenerate WebAPI { get; set; }
        public static CodeFileGenerate Models { get; set; }
        public static CavUrl CavUrl { get; set; }
        public static bool IsDebug { get; set; }
        /// <summary>
        /// 是否为严格模式
        /// </summary>
        public static bool IsStrict { get; set; }

        public static IServiceCollection AddCaviar(this IServiceCollection services, SqlConfig sqlConfig, IConfiguration configuration)
        {
            SqlConfig = sqlConfig;
            services.AddDbContext<SysDbContext>();
            services.AddScoped<IAppDbContext, AppDbContext>();
            services.AddScoped<IInteractor, AppInteractor>();
            services.AddScoped<SysLogAction>();

            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<ICodeGeneration, CodeGeneration>();
            
            Configuration = configuration;

            var caviarDynamicConfig = new CaviarDynamicConfig();
            caviarDynamicConfig.AddIBaseModel(services);
            caviarDynamicConfig.AddInject(services);
            TokenConfig = new TokenConfig();
            WebUI = new CodeFileGenerate();
            WebAPI = new CodeFileGenerate();
            Models = new CodeFileGenerate();
            EnclosureConfig = new EnclosureConfig();
            CavUrl = new CavUrl();
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
            IsStrict = bool.Parse(json["Caviar"]["IsStrict"].ToString());
            var guid = json["Caviar"]["Uid"]["NoLoginRole"].ToString();
            NoLoginRoleGuid = Guid.Parse(guid);
            SysAdminRoleGuid = Guid.Parse(json["Caviar"]["Uid"]["SysAdminRole"].ToString());
            UserAdminGuid = Guid.Parse(json["Caviar"]["Uid"]["UserAdmin"].ToString());
            TokenConfig.Key = Guid.Parse(json["Caviar"]["Token"]["Key"].ToString());
            TokenConfig.Duration = int.Parse(json["Caviar"]["Token"]["Duration"].ToString());
            WebUI.Path = json["Caviar"]["WebUI"]["Path"].ToString();
            WebUI.Namespace = json["Caviar"]["WebUI"]["namespace"].ToString();
            Models.Path = json["Caviar"]["Models"]["Path"].ToString();
            Models.Namespace = json["Caviar"]["Models"]["namespace"].ToString();
            WebAPI.Path = json["Caviar"]["WebApi"]["Path"].ToString();
            WebAPI.Namespace = json["Caviar"]["WebApi"]["namespace"].ToString();
            WebAPI.Base = json["Caviar"]["WebApi"]["BaseController"].ToString();
            SqlConfig.SqlFilePath = json["Caviar"]["SqlFile"]["SqlPath"].ToString();
            EnclosureConfig.Path = json["Caviar"]["Enclosure"]["Path"].ToString();
            EnclosureConfig.Size = int.Parse(json["Caviar"]["Enclosure"]["Size"].ToString());
            EnclosureConfig.CurrentDirectory = Directory.GetCurrentDirectory();
            CavUrl.UserLogin = json["Caviar"]["Url"]["UserLogin"].ToString();
            var paseJson = json.ToString();
            File.WriteAllText(appsettingPath, paseJson);
        }

        static void PaseAppsettingsJson(ref JObject json)
        {
            if (json["Caviar"] == null) json["Caviar"] = new JObject();
            if (json["Caviar"]["IsDebug"] == null) json["Caviar"]["IsDebug"] = false;
            if (json["Caviar"]["IsStrict"] == null) json["Caviar"]["IsStrict"] = false;
            if (json["Caviar"]["Uid"] == null) json["Caviar"]["Uid"] = new JObject();
            if (json["Caviar"]["Uid"]["NoLoginRole"] == null) json["Caviar"]["Uid"]["NoLoginRole"] = Guid.NewGuid();
            if (json["Caviar"]["Uid"]["SysAdminRole"] == null) json["Caviar"]["Uid"]["SysAdminRole"] = Guid.NewGuid();
            if (json["Caviar"]["Uid"]["UserAdmin"] == null) json["Caviar"]["Uid"]["UserAdmin"] = Guid.NewGuid();
            if (json["Caviar"]["Token"] == null) json["Caviar"]["Token"] = new JObject();
            if (json["Caviar"]["Token"]["Duration"] == null) json["Caviar"]["Token"]["Duration"] = 60 * 2;
            if (json["Caviar"]["Token"]["Key"] == null) json["Caviar"]["Token"]["Key"] = Guid.NewGuid();
            if (json["Caviar"]["WebUI"] == null) json["Caviar"]["WebUI"] = new JObject();
            if (json["Caviar"]["WebUI"]["Path"] == null) json["Caviar"]["WebUI"]["Path"] = "../Caviar.Demo.AntDesignUI/Template/";
            if (json["Caviar"]["WebUI"]["namespace"] == null) json["Caviar"]["WebUI"]["namespace"] = "Caviar.Demo.AntDesignUI.Pages";
            if (json["Caviar"]["Models"] == null) json["Caviar"]["Models"] = new JObject();
            if (json["Caviar"]["Models"]["Path"] == null) json["Caviar"]["Models"]["Path"] = "../Caviar.Demo.Models/Template/";
            if (json["Caviar"]["Models"]["namespace"] == null) json["Caviar"]["Models"]["namespace"] = "Caviar.Demo.Models";
            if (json["Caviar"]["WebApi"] == null) json["Caviar"]["WebApi"] = new JObject();
            if (json["Caviar"]["WebApi"]["Path"] == null) json["Caviar"]["WebApi"]["Path"] = "/Template/";
            if (json["Caviar"]["WebApi"]["namespace"] == null) json["Caviar"]["WebApi"]["namespace"] = "Caviar.Demo.WebAPI";
            if (json["Caviar"]["WebApi"]["BaseController"] == null) json["Caviar"]["WebApi"]["BaseController"] = "TemplateBaseController<{OutName}Action,{EntityName},{ViewOutName}>";
            if (json["Caviar"]["SqlFile"]==null) json["Caviar"]["SqlFile"] = new JObject();
            if (json["Caviar"]["SqlFile"]["SqlPath"] == null) json["Caviar"]["SqlFile"]["SqlPath"] = "/SqlFile/CaviarSqlServer.sql";
            if (json["Caviar"]["Enclosure"] == null) json["Caviar"]["Enclosure"] = new JObject();
            if (json["Caviar"]["Enclosure"]["Path"] == null) json["Caviar"]["Enclosure"]["Path"] = "wwwroot/Enclosure";
            if (json["Caviar"]["Enclosure"]["Size"] == null) json["Caviar"]["Enclosure"]["Size"] = 3;
            if (json["Caviar"]["Url"] == null) json["Caviar"]["Url"] = new JObject();
            if (json["Caviar"]["Url"]["UserLogin"] == null) json["Caviar"]["Url"]["UserLogin"] = "User/Login";
        }


        public static IServiceProvider ApplicationServices { get; set; }

        public static IApplicationBuilder UserCaviar(this IApplicationBuilder app)
        {
            ApplicationServices = app.ApplicationServices;
            var path = EnclosureConfig.CurrentDirectory + "/" + EnclosureConfig.Path;//物理路径
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(path),
                RequestPath = CurrencyConstant.Enclosure
            });
            app.UseCustomExceptionMiddleware();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            return app;
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
                CommonlyHelper.GetAssembly()
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
                CommonlyHelper.GetAssembly()
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

    public class CavUrl
    {
        public string UserLogin { get; set; }
    }
    public class EnclosureConfig
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 目录
        /// </summary>
        public string CurrentDirectory { get; set; }
    }

    public class TokenConfig
    {
        /// <summary>
        /// 到期时间
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// token密钥
        /// **重要**
        /// </summary>
        public Guid Key { get; set; }
    }
    /// <summary>
    /// 代码生成器配置
    /// </summary>
    public class CodeFileGenerate
    {
        /// <summary>
        /// 代码存放路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace{ get;set;}
        /// <summary>
        /// 父类如果需要
        /// </summary>
        public string Base { get; set; }
    }


    public class SqlConfig
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Connections { get; set; }
        /// <summary>
        /// 初始化脚本
        /// </summary>
        public string SqlFilePath { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBTypeEnum DBTypeEnum { get; set; }
    }
}
