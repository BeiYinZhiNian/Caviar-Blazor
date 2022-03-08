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
        public List<QueryModel> QueryData { get; set; }
    }

    public class QueryModel
    {
        /// <summary>
        /// 查询类型
        /// </summary>
        public QuerType QuerType { get; set; }
        /// <summary>
        /// 向上拼接
        /// </summary>
        public QuerySplicing QuerySplicing { get; set; }
        /// <summary>
        /// 查询字段
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 查询数据
        /// </summary>
        public string Value { get; set; }
    }

    public enum QuerySplicing
    {
        And,
        Or,
    }
    public enum QuerType
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal,
        /// <summary>
        /// 小于
        /// </summary>
        LessThan,
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 不等于
        /// </summary>
        NotEqualTo,
        /// <summary>
        /// 包含
        /// </summary>
        Contain,
    }
}
