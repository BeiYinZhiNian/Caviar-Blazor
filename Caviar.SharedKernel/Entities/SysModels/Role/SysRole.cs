using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Caviar.SharedKernel
{
    /// <summary>
    /// 角色表
    /// </summary>
    [DisplayName("角色")]
    public partial class SysRole : SysBaseModel
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required(ErrorMessage = "请输入您的角色名称")]
        [DisplayName("角色名称")]
        [StringLength(50, ErrorMessage = "角色名称请不要超过{1}个字符")]
        public string RoleName { get; set; }

        [DisplayName("父id")]
        public int ParentId { get; set; }

    }
}
