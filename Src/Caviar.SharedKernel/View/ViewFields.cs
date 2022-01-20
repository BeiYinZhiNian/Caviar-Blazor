using Caviar.SharedKernel.Entities.View;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities.View
{
    public partial class ViewFields : BaseView<SysFields>
    {
        /// <summary>
        /// 类型名称如string,int等
        /// </summary>
        public string EntityType { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 是否为枚举
        /// </summary>
        public bool IsEnum { get; set; }
        /// <summary>
        /// 实体命名空间
        /// </summary>
        public string EntityNamespace { get; set; }
        /// <summary>
        /// 枚举 值-名称
        /// </summary>
        public Dictionary<int,string> EnumValueName { get; set; }
        /// <summary>
        /// 是否授权
        /// </summary>
        public bool IsPermission { get; set; }

    }
}
