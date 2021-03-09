using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial interface IBaseModel
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// uid
        /// </summary>
        public Guid Uid { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 第一次并不会引发删除，只是隐藏，第二次会真正的删除数据
        /// 当IsDelete为false时，删除会将IsDelete值改为true，当IsDelete值为true时将删除数据
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        public string OperatorCare { get; set; }
        /// <summary>
        /// 更新操作员的名称
        /// </summary>
        public string OperatorUp { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisable { get; set; }
    }
}
