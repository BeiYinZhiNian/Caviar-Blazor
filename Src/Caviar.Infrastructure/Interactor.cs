using Caviar.Core;
using Caviar.Core.Interface;
using Caviar.SharedKernel;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure
{
    public partial class Interactor
    {
        /// <summary>
        /// 数据上下文
        /// </summary>
        public IAppDbContext DbContext => HttpContext.RequestServices.GetRequiredService<IAppDbContext>();
        /// <summary>
        /// 计时器
        /// </summary>
        public Stopwatch Stopwatch { get; set; } = new Stopwatch();
        /// <summary>
        /// 当前访问菜单
        /// </summary>
        public ViewMenu CurrentMenu { get; set; }
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
        public List<ViewMenu> SysMenus { get; set; }

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

        public virtual bool IsAdmin { get; }
        public UserData UserData { get; set; } = new UserData();
        public UserToken UserToken { get; set; } = new UserToken();

    }
}
