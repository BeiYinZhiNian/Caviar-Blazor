using AntDesign;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

namespace Caviar.AntDesignUI.Pages.Menu
{
    public partial class Index
    {
        protected override Task<List<SysMenuView>> GetPages(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            pageSize = 100;
            return base.GetPages(pageIndex, pageSize, isOrder);
        }

        protected override async Task RowCallback(RowCallbackData<SysMenuView> row)
        {
            switch (row.Menu.Entity.Key)
            {
                case "DeleteEntity":
                    await ConfirmDelete(row.Menu.Entity.Url, row.Data);
                    break;
                default:
                    break;
            }
            UserConfig.RefreshMenuAction.Invoke();//更新菜单
            Refresh();
            return;
        }

        async Task ConfirmDelete(string url, SysMenuView data)
        {
            if (data.Children!=null && data.Children.Count > 0)
            {
                var confirm = await ShowConfirm(data.Entity.Key, data.Children.Count);
                if (confirm == ConfirmResult.Abort)//全部删除
                {
                    data.IsDeleteAll = true;
                }
                else if(confirm == ConfirmResult.Ignore)
                {
                    return;
                }
            }
            var result = await HttpService.PostJson(url, data);
            if (result.Status == HttpStatusCode.OK)
            {
                _ = MessageService.Success("删除成功");
            }
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
