using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel
{
    public partial class UserPwd
    {
        [Required(ErrorMessage = "请输入您初始密码")]
        [DisplayName("初始密码")]
        public string OriginalPwd { get; set; }
        [Required(ErrorMessage = "请输入您新密码")]
        [DisplayName("新密码")]
        public string NewPwd { get; set; }
        [Required(ErrorMessage = "请输入您确认密码")]
        [DisplayName("确认密码")]
        public string SurePwd { get; set; }
    }
}
