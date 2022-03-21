using Caviar.SharedKernel.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public interface IAuthService
    {
        Task<ResultMsg> Login(UserLogin userLogin, string returnUrl);
        Task<string> Logout();
        Task<CurrentUser> CurrentUserInfo();
    }
}
