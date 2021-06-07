using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
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
        /// 筛选器上下文
        /// </summary>
        public ActionExecutingContext Context { get; set; }
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
        /// 角色表
        /// </summary>
        public List<SysRole> Roles { get; set; }
        /// <summary>
        /// 权限表
        /// </summary>
        public List<SysPermission> Permissions { get; set; }
        /// <summary>
        /// 菜单表
        /// </summary>
        public List<SysMenu> Menus { get; set; }
        /// <summary>
        /// 缓存
        /// </summary>
        public IMemoryCache Cache { get; }
        /// <summary>
        /// 是否为管理员
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
