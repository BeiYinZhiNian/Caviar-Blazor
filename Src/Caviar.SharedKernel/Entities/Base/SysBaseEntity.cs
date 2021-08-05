using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    /// <summary>
    /// 数据基础类
    /// </summary>
    public partial class SysBaseEntity
    {
        /// <summary>
        /// id
        /// </summary>
        [DisplayName("Id")]
        [Key]
        public virtual int Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("CreatTime")]
        public virtual DateTime CreatTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改时间
        /// </summary>
        [DisplayName("UpdateTime")]
        public virtual DateTime UpdateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 根据配置确定删除后是否保留条目
        /// </summary>
        [DisplayName("IsDelete")]
        public virtual bool IsDelete { get; set; } = false;
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [DisplayName("OperatorCare")]
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public virtual string OperatorCare { get; set; }
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [DisplayName("OperatorUp")]
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public virtual string OperatorUp { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("Remark")]
        [StringLength(300, ErrorMessage = "LengthErrorMsg")]
        public virtual string Remark { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        [DisplayName("IsDisable")]
        public virtual bool IsDisable { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        [DisplayName("Number")]
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public virtual string Number { get; set; } = "999";
        /// <summary>
        /// 数据权限
        /// </summary>
        [DisplayName("DataId")]
        public virtual int DataId { get; set; }
    }
}
