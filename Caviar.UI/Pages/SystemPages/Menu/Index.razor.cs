using AntDesign;
using Caviar.Models.SystemData;
using Caviar.UI.Helper;
using Caviar.UI.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.UI.Pages.SystemPages.Menu
{
    public partial class Index
    {
        [Inject]
        HttpHelper Http { get; set; }
        [Inject]
        MessageService _messageService { get; set; }
        [Inject]
        UserConfigHelper UserConfig { get; set; }
        public List<ViewPowerMenu> DataSource { get; set; }

        List<ViewModelHeader> ViewModelHeaders { get; set; }
        List<ViewPowerMenu> Buttons { get; set; }
        protected override async Task OnInitializedAsync()
        {
            DataSource = await GetPowerMenus();//获取数据源
            ViewModelHeaders = await GetViewModelNames();//获取表头
            Buttons = await GetPowerButtons();//获取按钮
        }

        async Task<List<ViewPowerMenu>> GetPowerMenus()
        {
            var result = await Http.GetJson<List<ViewPowerMenu>>("Menu/GetLeftSideMenus");
            if (result.Status != 200) return new List<ViewPowerMenu>();
            return result.Data;
        }

        async Task<List<ViewPowerMenu>> GetPowerButtons()
        {
            var result = await Http.GetJson<List<ViewPowerMenu>>("Menu/GetButtons?menuId=" + UserConfig.CurrentMenuId);
            if (result.Status != 200) return new List<ViewPowerMenu>();
            return result.Data;
        }

        async Task<List<ViewModelHeader>> GetViewModelNames()
        {
            var modelNameList = await Http.GetJson<List<ViewModelHeader>>("Base/GetModelHeader?name=SyspowerMenu");
            if (modelNameList.Status == 200)
            {
                var item = modelNameList.Data.SingleOrDefault(u => u.TypeName.ToLower() == "icon");
                if (item != null) item.ModelType = "icon";
                return modelNameList.Data;
            }
            return new List<ViewModelHeader>();
        }

        EventCallback<ViewPowerMenu> DeleteCallback => EventCallback.Factory.Create<ViewPowerMenu>(this, Delete);

        async void Delete(object e)
        {
            var data = (ViewPowerMenu)e;
            if(data.Children!=null && data.Children.Count > 0)
            {
                var confirm = await ShowConfirm(data.MenuName, data.Children.Count);
                if (confirm == ConfirmResult.Abort)//全部删除
                {

                    var result = await Http.PostJson<ViewPowerMenu, object>("Menu/DeleteAllEntity", data);
                    if (result.Status == 200)
                    {
                        _messageService.Success("删除成功");
                    }

                }
                else if(confirm == ConfirmResult.Retry)//移到上层
                {
                    var result = await Http.PostJson<ViewPowerMenu, object>("Menu/MoveEntity", data);
                    if (result.Status == 200)
                    {
                        _messageService.Success("删除成功");
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                //删除单条
                var result = await Http.PostJson<ViewPowerMenu, object>("Menu/MoveEntity", data);
                if (result.Status == 200)
                {
                    _messageService.Success("删除成功");
                }
            }
            await OnInitializedAsync();
            StateHasChanged();
        }
        [Inject]
        ConfirmService _confirmService { get; set; }
        private async Task<ConfirmResult> ShowConfirm(string menuName,int count)
        {
            return await _confirmService.Show(
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
