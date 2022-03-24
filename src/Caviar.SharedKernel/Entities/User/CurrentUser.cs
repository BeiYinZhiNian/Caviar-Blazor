using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities.User
{
    public class CurrentUser
    {
        public string UserName { get; set; }
        public bool IsAuthenticated { get; set; }
        public IEnumerable<CaviarClaim> Claims { get; set; }
        /// <summary>
        /// 是否为游客
        /// </summary>
        public bool TouristVisit { get; set; }
    }

    /// <summary>
    /// 防止递归循环
    /// </summary>
    public class CaviarClaim
    {
        public CaviarClaim(Claim claim)
        {
            Type = claim.Type;
            Value = claim.Value;
        }

        public CaviarClaim(string type,string value)
        {
            Type = type;
            Value = value;
        }

        public CaviarClaim()
        {

        }

        public string Type { get; set; }

        public string Value { get; set; }
    }
}
