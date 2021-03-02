using Caviar.Models.SystemData.Template;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
namespace Caviar.Control
{
    public static class CaviarConfig
    {
        public static SqlConfig SqlConfig { get; set; }

        public static IServiceCollection AddCaviar(this IServiceCollection services,SqlConfig sqlConfig)
        {
            SqlConfig = sqlConfig;
            services.AddDbContext<DataContext>();
            services.AddDistributedMemoryCache();
            services.AddSession();
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

    }

    public class SqlConfig
    {
        public string Connections { get; set; }

        public DBTypeEnum DBTypeEnum { get; set; }
    }
}
