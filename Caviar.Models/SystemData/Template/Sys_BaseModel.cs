using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    /// <summary>
    /// 数据基础类
    /// </summary>
    partial class Sys_BaseModel
    {
        /// <summary>
        /// id
        /// </summary>
        [DisplayName("Id")]
        public int Id { get; set; }
        /// <summary>
        /// uid
        /// </summary>
        [DisplayName("Uid")]
        public string Uid { get; set; } = Guid.NewGuid().ToString();
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
        public bool IsDelete { get; set; }
        /// <summary>
        /// 操作员的名称
        /// </summary>
        [DisplayName("操作员")]
        public int UserName { get; set; }
    }
}
