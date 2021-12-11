using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Caviar.Demo.AntDesignUI.Pages
{
    partial class Index
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        private async Task LogUsername()
        {
            var authState = await authenticationStateTask;
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                Console.WriteLine($"{user.Identity.Name} is authenticated.");
            }
            else
            {
                Console.WriteLine("The user is NOT authenticated.");
            }
        }
    }
}