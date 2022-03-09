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
        /// 查询条数
        /// </summary>
        public int Number { get; set; } = 100;
        /// <summary>
        /// 查询的数据
        /// key 字段名称
        /// value 查询字符串
        /// </summary>
        public List<QueryModel> QueryModels { get; set; }
    }

    public class QueryModel
    {
        /// <summary>
        /// 查询类型
        /// </summary>
        public QuerType QuerTypes { get; set; }
        /// <summary>
        /// 向上拼接
        /// </summary>
        public QuerySplicing QuerySplicings { get; set; }
        /// <summary>
        /// 查询字段
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 查询数据
        /// </summary>
        public string Value { get; set; }
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
            /// 小于等于
            /// </summary>
            LessThanOrEqual,
            /// <summary>
            /// 大于
            /// </summary>
            GreaterThan,
            /// <summary>
            /// 大于等于
            /// </summary>
            GreaterThanOrEqual,
            /// <summary>
            /// 不等于
            /// </summary>
            NotEqual,
            /// <summary>
            /// 包含
            /// </summary>
            Contains,
        }
    }


}
