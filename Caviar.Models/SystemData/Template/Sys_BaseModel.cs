using System;
using System.Collections.Generic;
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
        public int Id { get; set; }
        /// <summary>
        /// uid
        /// </summary>
        public string Uid { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }
}
