using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;

namespace Caviar.Infrastructure
{
    public partial class Interactor
    {
        /// <summary>
        /// 计时器
        /// </summary>
        public Stopwatch Stopwatch { get; set; } = new Stopwatch();
        /// <summary>
        /// 当前访问菜单
        /// </summary>
        public SysMenuView CurrentMenu { get; set; }
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
        /// 全局缓存
        /// 字段列表
        /// </summary>
        public List<ViewFields> SysModelFields { get; set; }
        /// <summary>
        /// 全局缓存
        /// 菜单列表
        /// </summary>
        public List<SysMenuView> SysMenus { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public IDictionary<string, object> ActionArguments { get; set; }



        public virtual bool IsAdmin { get; }

    }
}
