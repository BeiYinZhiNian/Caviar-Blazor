using AntDesign;
using Caviar.Models.SystemData;
using Caviar.AntDesignPages.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Shared
{
    public partial class TableTemplate<TData>
    {
        [Parameter]
        public List<TData> DataSource { get; set; }

        [Parameter]
        public List<ViewPowerMenu> Buttons { get; set; }

        [Parameter]
        public string ModelHeaderName { get; set; }
        [Parameter]
        public Func<TData, IEnumerable<TData>> TreeChildren { get; set; } = _ => Enumerable.Empty<TData>();
        [Parameter]
        public List<ViewModelHeader> ViewModelHeader { get; set; }
        [Inject]
        HttpHelper Http { get; set; }
        

        protected override async Task OnInitializedAsync()
        {
            Query.QueryObj = GetBaseType(typeof(TData))?.Name;

            if (!string.IsNullOrEmpty(ModelHeaderName))
            {
                var modelNameList = await Http.GetJson<List<ViewModelHeader>>("Base/GetModelHeader?name=" + ModelHeaderName);
                if (modelNameList.Status == 200)
                {
                    ViewModelHeader = modelNameList.Data;
                }
            }
            
        }

        Type GetBaseType(Type type)
        {
            var baseType = type.BaseType;
            if (baseType == null)
            {
                return null;
            }
            else if(baseType.Name.ToLower() == "SysBaseModel".ToLower())
            {
                return type;
            }
            else
            {
                return GetBaseType(baseType);
            }
        }

        [Parameter]
        public EventCallback<RowCallbackData<TData>> RowCallback { get; set; }

        async void RoleAction(RowCallbackData<TData> data)
        {
            if (RowCallback.HasDelegate)
            {
                await RowCallback.InvokeAsync(data);
            }
            
        }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        ViewPowerMenu CurrentMenu { get; set; }
        void ButtonClick(ViewPowerMenu menu, TData data)
        {
            CurrentMenu = menu;
            switch (menu.ButtonPosition)
            {
                case ButtonPosition.Header:
                    switch (menu.TargetType)
                    {
                        case TargetType.CurrentPage:
                            NavigationManager.NavigateTo(menu.Url);
                            break;
                        case TargetType.EjectPage:
                            ModalUrl = menu.Url;
                            ModalTitle = menu.MenuName;
                            _visible = true;
                            break;
                        case TargetType.NewLabel:
                            JSRuntime.InvokeVoidAsync("open", menu.Url, "_blank");
                            break;
                        case TargetType.Callback:
                            HandleOk(null);
                            break;
                        default:
                            break;
                    }
                    break;
                case ButtonPosition.Row:
                    RowCallbackData<TData> row = new RowCallbackData<TData>()
                    {
                        Menu = menu,
                        Data = data,
                    };
                    RoleAction(row);
                    break;
                default:
                    break;
            }
        }

        

        [Inject]
        UserConfigHelper UserConfig { get; set; }
        #region Modal
        string ModalUrl = "";
        string UpUrl = "";
        RenderFragment UpRenderFragment;
        RenderFragment CreateDynamicComponent()
        {
            if (ModalUrl == null) ModalUrl = "";
            if (UpUrl == ModalUrl)
            {
                return UpRenderFragment;
            }
            UpUrl = ModalUrl;
            UpRenderFragment = Render();
            return UpRenderFragment;
        }
        RenderFragment Render() => builder =>
         {
             var routes = UserConfig.Routes;
             foreach (var item in routes)
             {
                 var page = (string)item.GetObjValue("Template").GetObjValue("TemplateText");
                 if (page.ToLower() == ModalUrl.ToLower() || "/" + page.ToLower() == ModalUrl.ToLower())
                 {
                     var type = (Type)item.GetObjValue("Handler");
                     builder.OpenComponent(0, type);
                     builder.AddComponentReferenceCapture(1, SetComponent);
                     builder.CloseComponent();
                     return;
                 }
             }

         };

        void SetComponent(object e)
        {
            menuAdd = (ITableTemplate)e;
        }

        string ModalTitle = "";
        bool _visible = false;
        [Inject] public NavigationManager NavigationManager { get; set; }
        ITableTemplate menuAdd;
        [Parameter]
        public EventCallback<ViewPowerMenu> HandleOkCallback { get; set; }
        [Parameter]
        public EventCallback<ViewPowerMenu> HandleCancelCallback { get; set; }
        private async void HandleOk(MouseEventArgs e)
        {
            var res = true;
            if (menuAdd != null)
            {
                res = await menuAdd.Submit();
            }
            _visible = !res;
            if (res)
            {
                if (HandleOkCallback.HasDelegate)
                {
                    await HandleOkCallback.InvokeAsync(CurrentMenu);
                }
            }
        }

        private async void HandleCancel(MouseEventArgs e)
        {
            _visible = false;
            if (HandleCancelCallback.HasDelegate)
            {
                await HandleCancelCallback.InvokeAsync(CurrentMenu);
            }
        }
        #endregion

        #region 查询条件
        IEnumerable<string> _selectedValues;
        [Parameter]
        public bool IsOpenQuery { get; set; } = true;


        ViewQuery Query = new ViewQuery();

        void OnRangeChange(DateRangeChangedEventArgs args)
        {
            Query.StartTime = args.Dates[0];
            Query.EndTime = args.Dates[1];
        }


        public async void FuzzyQuery()
        {
            Query.QueryField = _selectedValues?.ToList();
            var result = await Http.PostJson<ViewQuery, List<TData>>("Base/FuzzyQuery", Query);
            DataSource = result.Data;
            StateHasChanged();
        }
        #endregion
    }
}
