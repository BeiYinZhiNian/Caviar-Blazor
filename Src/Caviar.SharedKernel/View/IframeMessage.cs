using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.View
{
    /// <summary>
    /// iframe与框架通信类
    /// </summary>
    public class IframeMessage
    {
        /// <summary>
        /// 调用的方法
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 传输的数据
        /// </summary>
        public object Data { get; set; }
    }
}
