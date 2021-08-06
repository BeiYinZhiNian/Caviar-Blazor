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
        public int Status { get; set; } = HttpState.OK;
        /// <summary>
        /// 问题类型的简短、可读的摘要
        /// </summary>
        public string Title { get; set; } = "操作完成";
        /// <summary>
        /// 标识问题类型的 URI 引用
        /// </summary>
        public string Uri { get; set; } = "";

        public string TraceId { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// 此问题特定的可读说明。
        /// </summary>
        public string Detail { get; set; } = "";
        /// <summary>
        /// 当状态不为200时是否提示Title
        /// </summary>
        public bool IsTips { get; set; } = true;
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
