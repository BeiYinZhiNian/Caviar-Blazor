using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public static class CurrencyConstant
    {
        /// <summary>
        /// 附件映射路径
        /// </summary>
        public const string Enclosure = "/Enclosure";
        /// <summary>
        /// 头像文件夹名称
        /// </summary>
        public const string HeadPortrait = "headPortrait";
        /// <summary>
        /// url名称
        /// </summary>
        public const string CavModelUrl = "Url";
        
        
    }

    public static class HttpState
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        public const int OK = 200;
        /// <summary>
        /// 请求失败
        /// </summary>
        public const int Error = 406;
        /// <summary>
        /// 重定向
        /// </summary>
        public const int Redirect = 302;
        /// <summary>
        /// 未授权，需要登录
        /// </summary>
        public const int Unauthorized = 401;
        /// <summary>
        /// 无权限
        /// </summary>
        public const int NotPermission = 403;
        /// <summary>
        /// 无页面
        /// </summary>
        public const int NotFound = 404;
        /// <summary>
        /// 服务器内部错误
        /// </summary>
        public const int InternaError = 500;
    }
}
