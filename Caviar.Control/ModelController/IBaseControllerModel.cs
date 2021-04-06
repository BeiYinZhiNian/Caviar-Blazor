using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control
{
    public interface IBaseControllerModel
    {
        public HttpContext HttpContext { get; set; }
        /// <summary>
        /// 数据上下文
        /// </summary>
        public IDataContext DataContext { get; }
        /// <summary>
        /// 日志记录
        /// </summary>
        public ILogger GetLogger<T>();
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

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public SysUserInfo SysUserInfo { get; set; }
    }
}
