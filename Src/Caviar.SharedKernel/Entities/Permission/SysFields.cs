using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public class SysFields: SysBaseEntity, IBaseEntity
    {
        /// <summary>
        /// 类型名称，判断是否为同一属性的唯一条件
        /// 修改后需要重新设置，或者同步设置
        /// </summary>
        [Required(ErrorMessage = "RequiredErrorMsg")]
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string FieldName { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string DisplayName { get; set; }
        /// <summary>
        /// 获取类型的完全限定名，包括其命名空间，但不包括其名称空间
        /// </summary>
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string FullName { get; set; }
        /// <summary>
        /// 判断父类的唯一条件
        /// </summary>
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string BaseFullName { get; set; }
        /// <summary>
        /// 字段需要的宽度
        /// </summary>
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string TableWidth { get; set; }
        /// <summary>
        /// 字段需要的高度
        /// </summary>
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string TableHeight { get; set; }
        /// <summary>
        /// 是否显示在看板
        /// </summary>
        public bool IsPanel { get; set; } = true;
        /// <summary>
        /// 字段长度
        /// </summary>
        public int? FieldLen { get; set; }
    }
}
