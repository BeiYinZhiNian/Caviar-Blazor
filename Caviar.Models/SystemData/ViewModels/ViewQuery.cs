using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public class ViewQuery
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 查询对象
        /// </summary>
        public string QueryObj { get; set; }
        /// <summary>
        /// 查询的数据
        /// key 字段名称
        /// value 查询字符串
        /// </summary>
        public Dictionary<string, string> QueryData { get; set; } = new Dictionary<string, string>();
    }
}
