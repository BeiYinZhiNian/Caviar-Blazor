using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    [DIInject]
    public class ResultMsg
    {
        /// <summary>
        /// HTTP 状态代码
        /// </summary>
        public int Status { get; set; } = 200;
        /// <summary>
        /// 问题类型的简短、可读的摘要
        /// </summary>
        public string Title { get; set; } = "操作完成";
        /// <summary>
        /// 标识问题类型的 URI 引用
        /// </summary>
        public string Type { get; set; } = "";

        public string TraceId { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// 此问题特定的可读说明。
        /// </summary>
        public string Detail { get; set; } = "";
        /// <summary>
        /// 获取与此实例关联的验证错误
        /// </summary>
        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
        /// <summary>
        /// 用于存放附加数据
        /// </summary>
        public object Data { get; set; }
    }

    public class ResultMsg<T> : ResultMsg
    {
        public new T Data { get; set; }
    }
}
