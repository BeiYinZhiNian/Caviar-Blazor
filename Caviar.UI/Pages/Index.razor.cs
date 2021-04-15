
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
using Caviar.UI.Helper;
using Microsoft.AspNetCore.Components;

namespace Caviar.UI.Pages
{
    partial class Index
    {

        [CascadingParameter]
        public EventCallback LayoutStyleCallBack { get; set; }

        [Inject]
        HttpHelper Http { get; set; }
        [Inject]
        UserState UserState { get; set; }
        public async Task Test()
        {

            UserState.Id += 1;
            Console.WriteLine(UserState.Id);

        }
    }


}