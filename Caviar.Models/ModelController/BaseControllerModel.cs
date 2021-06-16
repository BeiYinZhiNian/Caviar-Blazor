using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models
{
    [DIInject(InjectType.SCOPED)]
    public partial class BaseControllerModel : IBaseControllerModel
    {
        /// <summary>
        /// 数据上下文
        /// </summary>
        public IDataContext DC => HttpContext.RequestServices.GetRequiredService<IDataContext>();
        /// <summary>
        /// 获取日志记录
        /// </summary>
        public ILogger<T> GetLogger<T>() => HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        public IMemoryCache Cache => HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

        /// <summary>
        /// 当前请求路径
        /// </summary>
        public string Current_Action { get; set; }
        /// <summary>
        /// 当前请求ip地址
        /// </summary>
        public string Current_Ipaddress { get; set; }
        /// <summary>
        /// 当前请求的完整Url
        /// </summary>
        public string Current_AbsoluteUri { get; set; }

        public HttpContext HttpContext { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public IDictionary<string, object> ActionArguments { get; set; }

        public string UserName => UserToken.UserName;

        public int Id => UserToken.Id;
        public bool IsLogin
        {
            get
            {
                return Id > 0;
            }
        }

        public bool IsAdmin { get; }
        public UserData UserData { get; set; } = new UserData();
        public UserToken UserToken { get; set; } = new UserToken();

    }
}
