using Caviar.SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Base.Entitys
{
    public class UserEntity : SysBaseEntity, IUser
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string HeadPortrait { get; set; }
        public string PasswordHash { get; set; }
        public int UserGroupId { get; set; }
    }
}
