using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models
{
    public interface ITree<T>
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 父id
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 孩子节点
        /// </summary>
        public List<T> Children { get; set; }
    }
}
