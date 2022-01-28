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
    }

    public class CaviarClaim
    {
        public CaviarClaim(Claim claim)
        {
            Type = claim.Type;
            Value = claim.Value;
        }
        public CaviarClaim()
        {

        }
        public string Type { get; set; }

        public string Value { get; set; }
    }
}
