using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    [DisplayName("字段权限")]
    public class SysModelFields:SysBaseModel
    {
        /// <summary>
        /// 类型名称，判断是否为同一属性的唯一条件
        /// 修改后需要重新设置，或者同步设置
        /// </summary>
        [DisplayName("类型名称")]
        [StringLength(50, ErrorMessage = "类型名称请不要超过{1}个字符")]
        public string TypeName { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        [DisplayName("显示名称")]
        [StringLength(50, ErrorMessage = "显示名称请不要超过{1}个字符")]
        public string DisplayName { get; set; }
        /// <summary>
        /// 命名空间
        /// </summary>
        [DisplayName("命名空间")]
        [StringLength(50, ErrorMessage = "命名空间请不要超过{1}个字符")]
        public string FullName { get; set; }
        /// <summary>
        /// 判断父类的唯一条件
        /// </summary>
        [DisplayName("基类名称")]
        [StringLength(50, ErrorMessage = "命名空间请不要超过{1}个字符")]
        public string BaseTypeName { get; set; }
        /// <summary>
        /// 字段需要的宽度
        /// </summary>
        [DisplayName("表宽")]
        [StringLength(50, ErrorMessage = "宽度请不要超过{1}个字符")]
        public string Width { get; set; }
        /// <summary>
        /// 字段需要的高度
        /// </summary>
        [DisplayName("表高")]
        [StringLength(50, ErrorMessage = "字段需要的高度请不要超过{1}个字符")]
        public string Height { get; set; }
        /// <summary>
        /// 是否显示在看板
        /// </summary>
        [DisplayName("在看板显示")]
        public bool IsPanel { get; set; } = true;
        /// <summary>
        /// 字段长度
        /// </summary>
        public int? ValueLen { get; set; }
    }
}
