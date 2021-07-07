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
    public partial interface IBaseControllerModel
    {
        public HttpContext HttpContext { get; set; }
        /// <summary>
        /// 数据上下文
        /// </summary>
        public IDataContext DC { get; }
        /// <summary>
        /// 日志记录
        /// </summary>
        public ILogger<T> GetLogger<T>();
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
        /// 用户名
        /// </summary>
        public string UserName { get; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// 是否登录
        /// </summary>
        public bool IsLogin { get; }
        /// <summary>
        /// 前端令牌
        /// </summary>
        public UserToken UserToken { get; set; }
        /// <summary>
        /// 用户信息
        /// 用于缓存部分
        /// </summary>
        public UserData UserData { get; set; }
        /// <summary>
        /// 缓存
        /// </summary>
        public IMemoryCache Cache { get; }
        /// <summary>
        /// 是否为管理员
        /// </summary>
        public bool IsAdmin { get; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public IDictionary<string,object> ActionArguments { get; set; }
        /// <summary>
        /// 全局缓存
        /// 字段列表
        /// </summary>
        public List<SysModelFields> SysModelFields { get; set; }
        /// <summary>
        /// 全局缓存
        /// 菜单列表
        /// </summary>
        public List<SysMenu> SysMenus { get; set; }
    }
}
