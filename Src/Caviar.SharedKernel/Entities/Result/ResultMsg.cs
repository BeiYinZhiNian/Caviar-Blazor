using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel
{
    [DIInject]
    public class ResultMsg
    {
        /// <summary>
        /// HTTP 状态代码
        /// </summary>
        public int Status { get; set; } = StatusCodes.Status200OK;
        /// <summary>
        /// 问题类型的简短、可读的摘要
        /// 可根据不同的http状态码承载不同的信息
        /// </summary>
        public string Title { get; set; } = "操作完成";
        /// <summary>
        /// 承载url地址
        /// </summary>
        public string Url { get; set; }

        public string TraceId { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// 此问题特定的可读说明。
        /// </summary>
        public string Detail { get; set; } = "";
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
