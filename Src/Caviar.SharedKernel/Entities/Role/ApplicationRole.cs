using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Caviar.SharedKernel.Entities
{
    public class ApplicationRole : IdentityRole<int>, IUseEntity
    {
        [Required(ErrorMessage = "RequiredErrorMsg")]
        [StringLength(256, ErrorMessage = "LengthErrorMsg")]
        public override string Name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 根据配置确定删除后是否保留条目
        /// </summary>
        public bool IsDelete { get; set; } = false;
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [StringLength(256, ErrorMessage = "LengthErrorMsg")]
        public string OperatorCare { get; set; }
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [StringLength(256, ErrorMessage = "LengthErrorMsg")]
        public string OperatorUp { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(300, ErrorMessage = "LengthErrorMsg")]
        public string Remark { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisable { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string Number { get; set; } = "999";
        /// <summary>
        /// 数据权限
        /// </summary>
        public int DataId { get; set; }
        /// <summary>
        /// 数据范围
        /// </summary>
        public DataRange DataRange { get; set; }
        /// <summary>
        /// 范围集合"1;2;3;5"储存权限id
        /// </summary>
        public string DataList { get; set; }
    }
}
