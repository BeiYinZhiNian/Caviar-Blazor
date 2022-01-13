using AntDesign;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Core
{
    public class CavModal
    {
        ModalService Modal;
        UserConfig UserConfig;
        MessageService MessageService;
        public string ModalStyle { get; set; } = "overflow-y: auto;height: 400px;";
        public CavModal(UserConfig userConfig,ModalService modalService, MessageService messageService)
        {
            UserConfig = userConfig;
            Modal = modalService;
            MessageService = messageService;
        }
        ModalRef modalRef;
        Action OnOK;
        public async Task<ModalRef> Create(string url,string title ,Action OnOK = null, Dictionary<string,object> paramenter = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (!paramenter.ContainsKey(CurrencyConstant.CavModelUrl))
            {
                paramenter.Add(CurrencyConstant.CavModelUrl, url);//不提供url时候默认url一致
            }
            ModalOptions options = new ModalOptions()
            {
                OnOk = HandleOk,
                OnCancel = HandleCancel,
                MaskClosable = false,
                Content = Render(url, title, paramenter),
                Title = title,
                Visible = true,
                OkText = "确定",
                CancelText = "取消",
                DestroyOnClose = true
            };
            if (!string.IsNullOrEmpty(title))
            {
                options.Draggable = true;
            }
            this.OnOK = OnOK;
            modalRef = await Modal.CreateModalAsync(options);
            return modalRef;
        }

        RenderFragment Render(string url,string title, IEnumerable<KeyValuePair<string, object>> paramenter) => builder =>
        {
            var routes = UserConfig.Routes();
            foreach (var item in routes)
            {
                var page = (string)item.GetObjValue("Template").GetObjValue("TemplateText");
                if (page.ToLower() == url.ToLower())
                {
                    var ComponentType = (Type)item.GetObjValue("Handler");
                    var index = 0;
                    builder.OpenElement(index++, "div");
                    builder.AddAttribute(index++, "style", ModalStyle);
                    builder.OpenComponent(index++, ComponentType);
                    if (paramenter != null && paramenter.Any())
                    {
                        builder.AddMultipleAttributes(index++, paramenter);
                    }
                    builder.AddComponentReferenceCapture(index++, SetComponent);
                    builder.CloseComponent();
                    builder.CloseElement();
                    return;
                }
            }
            MessageService.Error($"未找到{title}组件，请检查url地址：{url}");
        };

        ITableTemplate menuAdd;

        void SetComponent(object e)
        {
            menuAdd = (ITableTemplate)e;
        }

        private async Task HandleOk(MouseEventArgs e)
        {
            modalRef.Config.ConfirmLoading = true;
            var res = true;
            if (menuAdd != null)
            {
                res = await menuAdd.Validate();
            }
            modalRef.Config.ConfirmLoading = false;
            modalRef.Config.Visible = !res;
            if (res)
            {
                OnOK?.Invoke();
            }
        }

        private Task HandleCancel(MouseEventArgs e)
        {
            modalRef.Config.Visible = false;
            return Task.CompletedTask;
        }
    }
}
