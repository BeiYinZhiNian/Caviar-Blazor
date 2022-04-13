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
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }

        public string UserGroupName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(300)]
        public string HeadPortrait { get; set; }

        public IList<string> Roles { get; set; }
        /// <summary>
        /// 个性签名
        /// </summary>
        [StringLength(300)]
        public string Remark { get; set; }
    }
}
