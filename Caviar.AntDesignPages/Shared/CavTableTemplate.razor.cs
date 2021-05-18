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
    public partial class CavTableTemplate<TData>
    {
        [Parameter]
        public List<TData> DataSource { get; set; }
        [Parameter]
        public int Total { get; set; }
        [Parameter]
        public int PageIndex { get; set; }
        [Parameter]
        public int PageSize { get; set; }

        [Parameter]
        public List<ViewMenu> Buttons { get; set; }

        [Parameter]
        public string ModelHeaderName { get; set; }
        [Parameter]
        public Func<TData, IEnumerable<TData>> TreeChildren { get; set; } = _ => Enumerable.Empty<TData>();
        [Parameter]
        public List<ViewModelHeader> ViewModelHeader { get; set; }
        [Inject]
        HttpHelper Http { get; set; }
        [Parameter]
        public EventCallback<PaginationEventArgs> PageIndexChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Query.QueryObj = GetBaseType(typeof(TData))?.Name;

            if (!string.IsNullOrEmpty(ModelHeaderName))
            {
                var modelNameList = await Http.GetJson<List<ViewModelHeader>>("CaviarBase/GetModelHeader?name=" + ModelHeaderName);
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
        ViewMenu CurrentMenu { get; set; }
        void ButtonClick(ViewMenu menu, TData data)
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
                            ModalVisible = true;
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


        #region Modal
        [Inject]
        UserConfigHelper UserConfig { get; set; }
        [Parameter]
        public string ModalUrl { get;set; }
        [Parameter]
        public IEnumerable<KeyValuePair<string, object?>> ModalParamenter { get; set; }
        string UpUrl = "";
        RenderFragment UpRenderFragment;
        RenderFragment CreateDynamicComponent()
        {
            if (ModalUrl == null) ModalUrl = "";
            //if (UpUrl == ModalUrl)
            //{
            //    return UpRenderFragment;
            //}
            //UpUrl = ModalUrl;
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

                     var ComponentType = (Type)item.GetObjValue("Handler");
                     var index = 0;
                     builder.OpenComponent(index++, ComponentType);
                     if (ModalParamenter!=null && ModalParamenter.Any())
                     {
                         builder.AddMultipleAttributes(index++, ModalParamenter);
                     }
                     builder.AddComponentReferenceCapture(index++, SetComponent);
                     builder.CloseComponent();
                     return;
                 }
             }
         };



        void SetComponent(object e)
        {
            menuAdd = (ITableTemplate)e;
        }
        [Parameter]
        public string ModalTitle { get; set; }
        [Parameter]
        public bool ModalVisible { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        ITableTemplate menuAdd;
        [Parameter]
        public EventCallback<ViewMenu> HandleOkCallback { get; set; }
        [Parameter]
        public EventCallback<ViewMenu> HandleCancelCallback { get; set; }
        private async void HandleOk(MouseEventArgs e)
        {
            var res = true;
            if (menuAdd != null)
            {
                res = await menuAdd.Submit();
            }
            ModalVisible = !res;
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
            ModalVisible = false;
            if (HandleCancelCallback.HasDelegate)
            {
                await HandleCancelCallback.InvokeAsync(CurrentMenu);
            }
        }

        private async Task AfterClose()
        {
            ModalParamenter = null;
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
            var result = await Http.PostJson<ViewQuery, List<TData>>("CaviarBase/FuzzyQuery", Query);
            DataSource = result.Data;
            StateHasChanged();
        }
        #endregion
    }
}
