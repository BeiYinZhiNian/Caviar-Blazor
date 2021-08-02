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
    [DisplayName("用户")]
    public partial class SysUser : SysBaseEntity
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
        [StringLength(65, ErrorMessage = "密码请不要超过{1}个字符")]
        [DisplayName("密码")]
        public string Password { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [RegularExpression("^$|^[1][3-9]\\d{9}$", ErrorMessage = "请输入正确的手机号")]
        [StringLength(11, ErrorMessage = "手机号码请不要超过{1}个字符")]
        [DisplayName("手机号码")]
        public string PhoneNumber { get; set; }
        [RegularExpression("^[A-Za-z0-9\\u4e00-\\u9fa5]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "请输入正确邮箱")]
        [StringLength(50, ErrorMessage = "邮箱地址请不要超过{1}个字符")]
        [DisplayName("邮箱地址")]
        public string EmailNumber { get; set; }
        /// <summary>
        /// 部门or用户组
        /// </summary>
        [DisplayName("部门")]
        public int? UserGroupId { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [DisplayName("头像")]
        [StringLength(1024, ErrorMessage = "头像地址请不要超过{1}个字符")]
        public string HeadPortrait { get; set; }
    }
}
