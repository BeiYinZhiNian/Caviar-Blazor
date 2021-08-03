using Caviar.Core;
using Caviar.SharedKernel;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure
{
    /// <summary>
    /// 互动接口
    /// 主要贯穿Controller与Services的数据交流
    /// </summary>
    public partial interface IInteractor
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        public IAppDbContext DbContext { get; }
        /// <summary>
        /// http上下文
        /// </summary>
        public HttpContext HttpContext { get; set; }
        /// <summary>
        /// 计时器
        /// </summary>
        public Stopwatch Stopwatch { get; set; }
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
        /// 当前访问菜单
        /// </summary>
        public ViewMenu CurrentMenu { get; set; }
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
        public List<ViewFields> SysModelFields { get; set; }
        /// <summary>
        /// 全局缓存
        /// 菜单列表
        /// </summary>
        public List<ViewMenu> SysMenus { get; set; }
    }
}
