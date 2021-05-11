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

namespace Caviar.AntDesignPages.Pages.SystemPages.Menu
{
    public partial class Index
    {
        [Inject]
        HttpHelper Http { get; set; }
        [Inject]
        MessageService Message { get; set; }
        [Inject]
        UserConfigHelper UserConfig { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        public List<ViewMenu> DataSource { get; set; }
        public int Total { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        List<ViewModelHeader> ViewModelHeaders { get; set; }
        List<ViewMenu> Buttons { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await GetMenus();//获取数据源
            ViewModelHeaders = await GetViewModelNames();//获取表头
            Buttons = await GetButtons();//获取按钮
        }

        async Task GetMenus()
        {
            var result = await Http.GetJson<PageData<ViewMenu>>("Menu/GetPages?pageSize=100");
            if (result.Status != 200) return;
            if (result.Data != null)
            {
                DataSource = result.Data.Rows;
                Total = result.Data.Total;
                PageIndex = result.Data.PageIndex;
                PageSize = result.Data.PageSize;
            }
        }

        async Task<List<ViewMenu>> GetButtons()
        {
            string url = NavigationManager.Uri.Replace(NavigationManager.BaseUri,"");
            var result = await Http.GetJson<List<ViewMenu>>("Menu/GetButtons?url=" + url);
            if (result.Status != 200) return new List<ViewMenu>();
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
        public async void HandleOk(ViewMenu e)
        {
            switch (e.MenuName)
            {
                case "新增":
                    await OnInitializedAsync();
                    StateHasChanged();
                    break;
                default:
                    break;
            }
            
        }

        void RowCallback(RowCallbackData<ViewMenu> row)
        {
            switch (row.Menu.MenuName)
            {
                case "删除":
                    Delete(row.Data);
                    break;
                default:
                    break;
            }
        }

        async void Delete(ViewMenu data)
        {
            if(data.Children!=null && data.Children.Count > 0)
            {
                var confirm = await ShowConfirm(data.MenuName, data.Children.Count);
                if (confirm == ConfirmResult.Abort)//全部删除
                {

                    var result = await Http.PostJson<ViewMenu, object>("Menu/DeleteAllEntity", data);
                    if (result.Status == 200)
                    {
                        Message.Success("删除成功");
                    }

                }
                else if(confirm == ConfirmResult.Retry)//移到上层
                {
                    var result = await Http.PostJson<ViewMenu, object>("Menu/MoveEntity", data);
                    if (result.Status == 200)
                    {
                        Message.Success("删除成功");
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
                var result = await Http.PostJson<ViewMenu, object>("Menu/MoveEntity", data);
                if (result.Status == 200)
                {
                    Message.Success("删除成功");
                }
            }
            await OnInitializedAsync();
            StateHasChanged();
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
