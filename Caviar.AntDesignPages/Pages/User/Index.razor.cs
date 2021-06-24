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
        partial void PratialRowCallback(ref bool isContinue, RowCallbackData<ViewUser> row)
        {
            switch (row.Menu.MenuName)
            {
                case "删除":
                    isContinue = false;//关闭部分方法的后续动作
                    row.Data.Password = CommonHelper.SHA256EncryptString("123456");//密码不能为空，所以构建一个初始密码
                    Delete(row.Menu.Url, row.Data);
                    break;
            }

        }
    }
}
