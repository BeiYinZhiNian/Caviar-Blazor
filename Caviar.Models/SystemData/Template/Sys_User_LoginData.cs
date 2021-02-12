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
    public partial class Sys_User_LoginData : Sys_BaseModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "请输入您的用户名")]
        [DisplayName("用户名")]
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Required(ErrorMessage = "请输入您的密码")]
        [DisplayName("密码")]
        public string Password { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [Required(ErrorMessage = "请输入您的手机号码")]
        [DisplayName("手机号码")]
        public string PhoneNumber { get; set; }
    }
}
