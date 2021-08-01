using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models
{
    [DisplayName("部门")]
    public partial class SysUserGroup:SysBaseModel
    {
        [Required(ErrorMessage = "请输入您的用户名")]
        [DisplayName("名称")]
        [StringLength(50, ErrorMessage = "用户名请不要超过{1}个字符")]
        public string Name { get; set; }

        [DisplayName("父id")]
        public int ParentId { get; set; }
    }
}
