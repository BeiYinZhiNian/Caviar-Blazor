
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
        HttpHelper http { get; set; }
        public async Task Test()
        {
            Console.WriteLine("开始请求");
            if (http == null)
            {
                Console.WriteLine("请求失败");
            }
            //await http.GetJson("Menu/GetLeftSideMenus", LayoutStyleCallBack);
            SysUserLogin sysUserLogin = new SysUserLogin() 
            {
                UserName = "admin",
                Password = CommonHelper.SHA256EncryptString("123456"),
            };
            await http.PostJson("User/Login", sysUserLogin, LayoutStyleCallBack);
        }
    }


}