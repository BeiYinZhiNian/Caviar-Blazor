using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.View
{
    public partial class ViewFields : BaseView<SysFields>
    {
        /// <summary>
        /// 类型名称如string,int等
        /// </summary>
        public string EntityType { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 是否为枚举
        /// </summary>
        public bool IsEnum { get; set; }
        /// <summary>
        /// 枚举 值-名称
        /// </summary>
        public Dictionary<int,string> EnumValueName { get; set; }
        /// <summary>
        /// 是否授权
        /// </summary>
        [DisplayName("是否授权")]
        public bool IsPermission { get; set; }

    }
}
