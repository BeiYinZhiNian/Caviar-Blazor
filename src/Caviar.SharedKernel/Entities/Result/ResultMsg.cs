// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;
using System.Net;

namespace Caviar.SharedKernel.Entities
{
    [DIInject]
    public class ResultMsg
    {
        /// <summary>
        /// HTTP 状态代码
        /// </summary>
        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
        /// <summary>
        /// 问题类型的简短、可读的摘要
        /// 可根据不同的http状态码承载不同的信息
        /// </summary>
        public string Title { get; set; } = "Succeeded";
        /// <summary>
        /// 承载url地址
        /// </summary>
        public string Url { get; set; }

        public string TraceId { get; set; }
        /// <summary>
        /// 此问题特定的可读说明。
        /// </summary>
        public Dictionary<string, string> Detail { get; set; }
    }

    public class ResultMsg<T> : ResultMsg
    {
        public T Data { get; set; }

        public ResultMsg(T data)
        {
            Data = data;
        }

        public ResultMsg()
        {

        }
    }
}
