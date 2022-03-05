using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public class UserDetails
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string UserGroupName { get; set; }

        public IList<string> Roles { get; set; }
        public string Remark { get; set; }
    }
}
