using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Caviar.SharedKernel.Entities
{
    public partial class Interactor
    {
        public Interactor(IHttpContextAccessor context)
        {
            //获取ip地址
            Current_Ipaddress = CommonHelper.GetUserIp(context.HttpContext);
            //获取完整Url
            Current_AbsoluteUri = GetAbsoluteUri(context.HttpContext.Request);
            //获取请求路径
            Current_Action = context.HttpContext.Request.Path.Value;
            //请求上下文
            HttpContext = context.HttpContext;
            //浏览器信息
            Browser = context.HttpContext.Request.Headers[CurrencyConstant.UserAgent];
            //请求方式
            Method = context.HttpContext.Request.Method;
        }

        public Interactor()
        {

        }

        /// <summary>
        /// 获取请求的完整地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected string GetAbsoluteUri(HttpRequest request)
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
        /// <summary>
        /// 周期事件id
        /// </summary>
        public Guid TraceId { get; set; } = Guid.NewGuid();
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
        public List<FieldsView> SysModelFields { get; set; }
        /// <summary>
        /// 全局缓存
        /// 菜单列表
        /// </summary>
        public List<SysMenuView> SysMenus { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public IDictionary<string, object> ActionArguments { get; set; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string Browser { get; set; }
        /// <summary>
        /// 请求方式
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 用户名称
        /// 当前用户信息不为null时，取自User
        /// 当未登录用户进行操作时，需要自定义用户名称,可以自主指定
        /// </summary>
        public string UserName => UserInfo?.UserName;
        /// <summary>
        /// 当前用户详细信息
        /// </summary>
        public ApplicationUser UserInfo { get; set; }
        /// <summary>
        /// 当前用户所有角色
        /// </summary>
        public IList<ApplicationRole> ApplicationRoles { get; set; }
    }
}
