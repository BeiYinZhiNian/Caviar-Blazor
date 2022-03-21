using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.MyUserDetails
{
    public partial class Index
    {
        [Inject]
        HttpService HttpService { get; set; }
        [Inject]
        CavModal CavModal { get; set; }
        [Inject]
        UserConfig UserConfig { get; set; }

        string Roles { get; set; }

        UserDetails _user;
        protected override async Task OnInitializedAsync()
        {
            var result = await HttpService.GetJson<UserDetails>(UrlConfig.MyDetails);
            if(result.Status == System.Net.HttpStatusCode.OK)
            {
                _user = result.Data;
                foreach (var item in _user.Roles)
                {
                    Roles += item + ";";
                }
            }
            await base.OnInitializedAsync();
        }
    }
}
