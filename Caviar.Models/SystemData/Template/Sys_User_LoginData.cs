using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    /// <summary>
    /// 系统用户
    /// </summary>
    public partial class Sys_LoginUserData
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
