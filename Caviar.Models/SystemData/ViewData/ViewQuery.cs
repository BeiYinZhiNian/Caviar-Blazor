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
        /// 查询字符串
        /// </summary>
        public string QueryStr { get; set; }
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
        /// 查询字段集合
        /// </summary>
        public List<string> QueryField { get; set; }
    }
}
