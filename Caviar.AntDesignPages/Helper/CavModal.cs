using AntDesign;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Helper
{
    public class CavModal
    {
        ModalService Modal;
        UserConfigHelper UserConfig;

        public CavModal(UserConfigHelper userConfig,ModalService modalService)
        {
            UserConfig = userConfig;
            Modal = modalService;
        }
        ModalRef modalRef;
        Action OnOK;
        public async Task<ModalRef> Create(string url,string title ,Action OnOK = null, IEnumerable<KeyValuePair<string, object?>> paramenter = null)
        {
            ModalOptions options = new ModalOptions()
            {
                OnOk = HandleOk,
                OnCancel = HandleCancel,
                Content = Render(url, paramenter),
                Title = title,
                Visible = true,
                OkText = "确定",
                CancelText = "取消"
            };
            if (!string.IsNullOrEmpty(title))
            {
                options.Draggable = true;
            }
            this.OnOK = OnOK;
            modalRef = await Modal.CreateModalAsync(options);
            return modalRef;
        }

        RenderFragment Render(string url, IEnumerable<KeyValuePair<string, object?>> paramenter) => builder =>
        {
            var routes = UserConfig.Routes;
            foreach (var item in routes)
            {
                var page = (string)item.GetObjValue("Template").GetObjValue("TemplateText");
                if (page.ToLower() == url.ToLower() || "/" + page.ToLower() == url.ToLower())
                {

                    var ComponentType = (Type)item.GetObjValue("Handler");
                    var index = 0;
                    builder.OpenComponent(index++, ComponentType);
                    if (paramenter != null && paramenter.Any())
                    {
                        builder.AddMultipleAttributes(index++, paramenter);
                    }
                    builder.AddComponentReferenceCapture(index++, SetComponent);
                    builder.CloseComponent();
                    return;
                }
            }
        };

        ITableTemplate menuAdd;

        void SetComponent(object e)
        {
            menuAdd = (ITableTemplate)e;
        }

        private async Task HandleOk(MouseEventArgs e)
        {
            var res = true;
            if (menuAdd != null)
            {
                res = await menuAdd.Submit();
            }
            modalRef.Config.Visible = !res;
            if (res)
            {
                OnOK?.Invoke();
            }

        }

        private async Task HandleCancel(MouseEventArgs e)
        {
            modalRef.Config.Visible = false;

        }
    }
}
