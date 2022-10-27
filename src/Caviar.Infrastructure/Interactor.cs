// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Caviar.SharedKernel.Common;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Http;

namespace Caviar.Infrastructure
{
    public partial class Interactor : IInteractor
    {
        public Interactor(IHttpContextAccessor context)
        {
            //获取ip地址
            Current_Ipaddress = GetUserIp(context.HttpContext);
            //获取完整Url
            Current_AbsoluteUri = GetAbsoluteUri(context.HttpContext?.Request);
            //获取请求路径
            Current_Action = context.HttpContext?.Request.Path.Value;
            //请求上下文
            HttpContext = context.HttpContext;
            //浏览器信息
            Browser = context.HttpContext?.Request.Headers[CurrencyConstant.UserAgent];
            //请求方式
            Method = context.HttpContext?.Request.Method;
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
            if (request == null) return null;
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
        /// 获取用户的ip地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string GetUserIp(HttpContext context)
        {
            if (context == null)
            {
                return null;
            }
            var ip = context.Request.Headers[CurrencyConstant.XForwardedFor].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
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

        /// <summary>
        /// 当前用户所有可用url集合
        /// </summary>
        public List<string> PermissionUrls { get; set; }

        ApplicationUser IInteractor.UserInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IList<ApplicationRole> IInteractor.ApplicationRoles { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
