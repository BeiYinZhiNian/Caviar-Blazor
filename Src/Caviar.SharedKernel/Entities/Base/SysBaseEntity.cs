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

    public partial class SysBaseEntity : IBaseEntity
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public virtual int Id { get; set; }
    }
    /// <summary>
    /// 数据基础类
    /// </summary>
    public partial class SysUseEntity:IUseEntity
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public virtual int Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreatTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 根据配置确定删除后是否保留条目
        /// </summary>
        public virtual bool IsDelete { get; set; } = false;
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public virtual string OperatorCare { get; set; }
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public virtual string OperatorUp { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(300, ErrorMessage = "LengthErrorMsg")]
        public virtual string Remark { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public virtual bool IsDisable { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public virtual string Number { get; set; } = "999";
        /// <summary>
        /// 数据权限
        /// </summary>
        public virtual int DataId { get; set; }
    }
}
