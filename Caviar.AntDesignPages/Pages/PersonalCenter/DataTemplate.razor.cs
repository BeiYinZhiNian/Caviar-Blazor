using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.PersonalCenter
{
    public partial class DataTemplate
    {
        protected override async Task OnInitializedAsync()
        {
            var password = "123456";//创建的初始密码为123456，修改时候也提交这个密码，字段权限会自动过滤掉
            DataSource.Password = CommonHelper.SHA256EncryptString(password);//设置默认密码
            await base.OnInitializedAsync();
        }
    }
}
