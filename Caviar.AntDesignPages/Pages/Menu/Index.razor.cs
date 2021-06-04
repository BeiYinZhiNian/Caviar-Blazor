using AntDesign;
using Caviar.Models.SystemData;
using Caviar.AntDesignPages.Helper;
using Caviar.AntDesignPages.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.Menu
{
    public partial class Index
    {

        partial void PratialGetPages(ref bool IsContinue, ref int pageIndex, ref int pageSize, ref bool isOrder)
        {
            pageSize = 100;
        }

        partial void PratialRowCallback(ref bool IsContinue, RowCallbackData<ViewMenu> row)
        {
            switch (row.Menu.MenuName)
            {
                case "删除":
                    IsContinue = false;
                    ConfirmDelete(row.Menu.Url, row.Data);
                    break;
                default:
                    break;
            }
        }

        async void ConfirmDelete(string uri,ViewMenu data)
        {
            var url = uri + "?IsDeleteAll=false";
            if (data.Children!=null && data.Children.Count > 0)
            {
                var confirm = await ShowConfirm(data.MenuName, data.Children.Count);
                if (confirm == ConfirmResult.Abort)//全部删除
                {
                    url = uri + "?IsDeleteAll=true";
                }
                else if(confirm == ConfirmResult.Ignore)
                {
                    return;
                }
            }
            var result = await Http.PostJson(url, data);
            if (result.Status == 200)
            {
                Message.Success("删除成功");
            }
            Refresh();
        }


        [Inject]
        ConfirmService Confirm { get; set; }
        private async Task<ConfirmResult> ShowConfirm(string menuName,int count)
        {
            return await Confirm.Show(
                $"检测到{menuName}菜单下,还有{count}条数据，请问如何处理？",
                "警告",
                ConfirmButtons.AbortRetryIgnore,
                ConfirmIcon.Warning,
                new ConfirmButtonOptions()
                {
                    Button1Props = new ButtonProps()
                    {
                        Type = ButtonType.Primary,
                        Danger = true,
                        ChildContent = "全部删除",
                    },
                    Button2Props = new ButtonProps()
                    {
                        Type = ButtonType.Primary,
                        ChildContent = "移到上层"
                    },
                    Button3Props = new ButtonProps()
                    {
                        ChildContent = "取消"
                    }
                }
                );
        }

    }
}
