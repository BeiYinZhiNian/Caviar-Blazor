// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Caviar.SharedKernel.Entities;

namespace Caviar.SharedKernel.Common
{
    public interface IInteractor
    {
        /// <summary>
        /// 周期事件id
        /// </summary>
        public Guid TraceId { get; set; }

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
        public string UserName { get; }

        public ApplicationUser UserInfo { get; set; }

        /// <summary>
        /// 当前用户所有角色
        /// </summary>
        public IList<ApplicationRole> ApplicationRoles { get; set; }

        /// <summary>
        /// 当前用户所有可用url集合
        /// </summary>
        public List<string> PermissionUrls { get; set; }
    }
}
