using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class ViewModelHeader
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DispLayName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string ModelType { get; set; }
        /// <summary>
        /// 字段需要的宽度
        /// </summary>
        public string Width { get; set; }
        /// <summary>
        /// 字段需要的高度
        /// </summary>
        public string Height { get; set; }
        /// <summary>
        /// 字段长度
        /// </summary>
        public int? ValueLen { get; set; }
        /// <summary>
        /// 是否为枚举
        /// </summary>
        public bool IsEnum { get; set; }
        /// <summary>
        /// 枚举 值-名称
        /// </summary>
        public Dictionary<int,string> EnumValueName { get; set; }
    }
}
