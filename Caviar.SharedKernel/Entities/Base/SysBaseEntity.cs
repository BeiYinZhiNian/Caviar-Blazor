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
        public int Id { get; set; }
        /// <summary>
        /// uid
        /// </summary>
        [DisplayName("Uid")]
        public Guid Uid { get; set; } = Guid.NewGuid();
        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        public DateTime CreatTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改时间
        /// </summary>
        [DisplayName("修改时间")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 根据配置确定删除后是否保留条目
        /// </summary>
        [DisplayName("是否删除")]
        public bool IsDelete { get; set; } = false;
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [DisplayName("创建操作员")]
        [StringLength(50, ErrorMessage = "请不要超过{1}个字符")]
        public string OperatorCare { get; set; }
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [DisplayName("修改操作员")]
        [StringLength(50, ErrorMessage = "请不要超过{1}个字符")]
        public string OperatorUp { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        [StringLength(300, ErrorMessage = "请不要超过{1}个字符")]
        public string Remark { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        [DisplayName("是否禁用")]
        public bool IsDisable { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        [DisplayName("编号")]
        [StringLength(50, ErrorMessage = "编号请不要超过{1}个字符")]
        public string Number { get; set; } = "999";
        /// <summary>
        /// 数据权限
        /// </summary>
        [DisplayName("数据权限")]
        public int? DataId { get; set; }
    }
}
