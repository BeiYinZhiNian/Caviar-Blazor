using Caviar.SharedKernel.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.View
{
    public class RowCallbackData<T>
    {
        /// <summary>
        /// 点击的菜单
        /// </summary>
        public SysMenuView Menu { get; set; }
        /// <summary>
        /// 回调数据
        /// </summary>
        public T Data { get; set; }
    }
}
