using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class SysRole:SysBaseModel
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required(ErrorMessage = "请输入您的角色名称")]
        [DisplayName("角色名名称")]
        [StringLength(50, ErrorMessage = "角色名称请不要超过{1}个字符")]
        public string RoleName { get; set; }

        [Display(Name = "用户")]
        public virtual List<SysRoleLogin> UserRoles { get; set; }

    }
}
