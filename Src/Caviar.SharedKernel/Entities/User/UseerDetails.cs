using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public class UserDetails
    {
        [Required(ErrorMessage = "RequiredErrorMsg")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "RequiredErrorMsg")]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string UserGroupName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(300, ErrorMessage = "LengthErrorMsg")]
        public string HeadPortrait { get; set; }

        public IList<string> Roles { get; set; }
        /// <summary>
        /// 个性签名
        /// </summary>
        [StringLength(300, ErrorMessage = "LengthErrorMsg")]
        public string Remark { get; set; }
    }
}
