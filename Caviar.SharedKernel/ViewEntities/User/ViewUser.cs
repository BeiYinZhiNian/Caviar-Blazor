using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel
{
    public partial class ViewUser
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [DisplayName("部门")]
        public string UserGroupName { get; set; }
    }
}
