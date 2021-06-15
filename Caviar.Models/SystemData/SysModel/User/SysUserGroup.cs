using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    [DisplayName("用户组")]
    public partial class SysUserGroup:SysBaseModel
    {
        [DisplayName("名称")]
        public string Name { get; set; }

        [DisplayName("父id")]
        public int ParentId { get; set; }


    }
}
