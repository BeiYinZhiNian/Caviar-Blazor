using Caviar.Models.SystemData;
using Caviar.UI.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.UI.Shared
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
            if (!string.IsNullOrEmpty(ModelHeaderName))
            {
                var modelNameList = await Http.GetJson<List<ViewModelHeader>>("Base/GetModelHeader?name=" + ModelHeaderName);
                if (modelNameList.Status == 200)
                {
                    ViewModelHeader = modelNameList.Data;
                }
            }
            
        }

        [Parameter]
        public EventCallback<TData> RowCallback { get; set; }

        async void RoleAction(TData data)
        {
            if (RowCallback.HasDelegate)
            {
                await RowCallback.InvokeAsync(data);
            }
            
        }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        void ButtonClick(ViewPowerMenu menu, TData data)
        {
            switch (menu.ButtonPosition)
            {
                case ButtonPosition.Header:
                    switch (menu.TargetType)
                    {
                        case TargetType.CurrentPage:
                            NavigationManager.NavigateTo(menu.Url);
                            break;
                        case TargetType.EjectPage:
                            _visible = true;

                            break;
                        case TargetType.NewLabel:
                            JSRuntime.InvokeAsync<object>("open", menu.Url, "_blank");
                            break;
                        default:
                            break;
                    }
                    break;
                case ButtonPosition.Row:
                    RoleAction(data);
                    break;
                default:
                    break;
            }
        }




        #region Modal
        RenderFragment CreateDynamicComponent() => builder =>
        {
            builder.OpenComponent(0, typeof(Program));
            builder.AddComponentReferenceCapture(1, SetComponent);
            builder.CloseComponent();
        };

        void SetComponent(object e)
        {
            menuAdd = (ITableTemplate)e;
        }

        string title = "";
        bool _visible = false;
        [Inject] public NavigationManager NavigationManager { get; set; }
        ITableTemplate menuAdd;

        private async void HandleOk(MouseEventArgs e)
        {
            var res = await menuAdd.Submit();
            _visible = !res;
            await OnInitializedAsync();
            StateHasChanged();
        }

        private void HandleCancel(MouseEventArgs e)
        {
            _visible = false;
        }
        #endregion
    }
}
