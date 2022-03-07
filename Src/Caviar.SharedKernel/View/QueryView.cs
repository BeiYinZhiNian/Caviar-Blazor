using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities.View
{
    public class QueryView
    {
        /// <summary>
        /// 高级搜索
        /// </summary>
        public bool AdvancedSearch { get; set; }
        /// <summary>
        /// 查询字符串
        /// </summary>
        public string SearchValue { get; set; }
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
        /// 查询条数
        /// </summary>
        public int Number { get; set; } = 20;
        /// <summary>
        /// 查询的数据
        /// key 字段名称
        /// value 查询字符串
        /// </summary>
        public Dictionary<string, string> QueryData { get; set; } = new Dictionary<string, string>();
    }
}
