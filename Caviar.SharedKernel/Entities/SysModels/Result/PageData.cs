using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel
{

    public class PageData<T>
    {
        public PageData()
        {

        }

        public PageData(List<T> rows)
        {
            Rows = rows;
        }

        /// <summary>
        /// 数据
        /// </summary>
        public List<T> Rows { get; set; }
        /// <summary>
        /// 总计
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
