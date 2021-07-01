using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.User
{
    public partial class Index
    {
        protected override async Task RowCallback(RowCallbackData<ViewUser> row)
        {
            switch (row.Menu.MenuName)
            {
                case "删除":
                    row.Data.Password = CommonHelper.SHA256EncryptString("123456");//密码不能为空，所以构建一个初始密码
                    await Delete(row.Menu.Url, row.Data);
                    break;
            }
        }
    }
}
