
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

        [Inject]
        HttpHelper http { get; set; }
        public async Task Test()
        {
            Console.WriteLine("开始请求");
            if (http == null)
            {
                Console.WriteLine("请求失败");
            }
            var data = await http.GetJson<List<SysPowerMenu>>("Menu/GetLeftSideMenus");
        }
    }


}