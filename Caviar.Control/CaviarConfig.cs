using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Caviar.Control
{
    public static class CaviarConfig
    {
        public static SqlConfig SqlConfig { get; set; }

        public static IConfiguration Configuration { get; set; }

        public static string NoLoginRole { get; set; }
        public static string SysAdminRole { get; set; }

        public static IServiceCollection AddCaviar(this IServiceCollection services,SqlConfig sqlConfig,IConfiguration configuration)
        {
            SqlConfig = sqlConfig;
            services.AddDbContext<DataContext>();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddScoped<SysDataContext>();
            Configuration = configuration;

            var caviarDynamicConfig = new CaviarDynamicConfig();
            caviarDynamicConfig.AddIBaseModel(services);
            caviarDynamicConfig.AddInject(services);

            NoLoginRole = Configuration["Caviar:Role:NoLoginRole"];
            if (string.IsNullOrEmpty(NoLoginRole)) NoLoginRole = "未登录用户";
            SysAdminRole = Configuration["Caviar:Role:SysAdminRole"];
            if (string.IsNullOrEmpty(SysAdminRole)) NoLoginRole = "管理员";
            return services;
        }

        


        public static IServiceProvider ApplicationServices { get; set; }

        public static IApplicationBuilder UserCaviar(this IApplicationBuilder app)
        {
            ApplicationServices = app.ApplicationServices;
            app.UseSession();
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

        #region session扩展
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
        #endregion



        class CaviarDynamicConfig
        {
            /// <summary>
            /// 使用加载器技术
            /// </summary>
            /// <returns></returns>
            List<System.Reflection.Assembly> GetAssembly()
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .Where(u => !u.FullName.Contains("Microsoft"))//排除微软类库
                    .Where(u => !u.FullName.Contains("System"))//排除系统类库
                    .ToList();
            }
            /// <summary>
            /// 遍历所有的类，筛选实现IService接口的类，并过判断是否是类,并按照注解方式自动注入类
            /// 自动注入所有继承IBaseModel接口的类，注入类型为AddTransient
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
                        .Where(u => u.GetInterfaces().Contains(typeof(IBaseModel)))
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
                            .Where(a => a.GetCustomAttributes(true).Select(t => t.GetType()).Contains(typeof(InjectAttribute)) && !a.IsAbstract)
                            //判断是否是类
                            .Where(u => u.IsClass)
                            //转换成list
                            .ToList()
                            //循环,并添注入
                            .ForEach(t =>
                            {
                                var inject = (InjectAttribute)t.GetCustomAttributes(true).FirstOrDefault(d => d.GetType() == typeof(InjectAttribute));
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
