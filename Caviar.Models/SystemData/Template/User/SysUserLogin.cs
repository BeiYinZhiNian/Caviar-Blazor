using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    /// <summary>
    /// 系统用户
    /// </summary>
    public partial class SysUserLogin : SysBaseModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "请输入您的用户名")]
        [DisplayName("用户名")]
        [StringLength(50, ErrorMessage = "用户名请不要超过{1}个字符")]
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Required(ErrorMessage = "请输入您的密码")]
        [DisplayName("密码")]
        [StringLength(32, ErrorMessage = "密码请不要超过{1}个字符")]
        public string Password { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [Required(ErrorMessage = "请输入您的手机号码")]
        [RegularExpression("^[1][3-9]\\d{9}$", ErrorMessage = "请输入正确的手机号")]
        [StringLength(11, ErrorMessage = "手机号码请不要超过{1}个字符")]
        [DisplayName("手机号码")]
        public string PhoneNumber { get; set; }

        [Display(Name = "角色")]
        public List<SysRoleLogin> UserRoles { get; set; }
    }
}
