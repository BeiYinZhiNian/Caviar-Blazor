using AntDesign;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;

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
        public CavModal(UserConfig userConfig,ModalService modalService, MessageService messageService)
        {
            UserConfig = userConfig;
            Modal = modalService;
            MessageService = messageService;
        }

        public async Task<ModalRef> Create(CavModalOptions modalOptions)
        {
            var modelHandle = new ModalHandle(UserConfig, Modal, MessageService);
            return await modelHandle.Create(modalOptions);
        }

        protected class ModalHandle
        {
            public string ModalStyle { get; set; } = "overflow-y: auto;height: 400px;";
            ModalService Modal;
            UserConfig UserConfig;
            MessageService MessageService;
            public ModalHandle(UserConfig userConfig, ModalService modalService, MessageService messageService)
            {
                UserConfig = userConfig;
                Modal = modalService;
                MessageService = messageService;
            }

            ModalRef modalRef;
            Action OnOK;
            public async Task<ModalRef> Create(CavModalOptions modalOptions)
            {
                if(modalOptions.Content == null)
                {
                    modalOptions.Content = Render(modalOptions.Url, modalOptions.Title, modalOptions.Paramenter);
                }
                ModalOptions options = new ModalOptions()
                {
                    OnOk = HandleOk,
                    OnCancel = HandleCancel,
                    MaskClosable = false,
                    Content = modalOptions.Content,
                    Title = modalOptions.Title,
                    BodyStyle = modalOptions.BodyStyle,
                    Visible = true,
                    OkText = @UserConfig.LanguageService[$"{CurrencyConstant.Page}.{CurrencyConstant.Confirm}"],
                    CancelText = @UserConfig.LanguageService[$"{CurrencyConstant.Page}.{CurrencyConstant.Cancel}"],
                    DestroyOnClose = true,
                };
                if (!string.IsNullOrEmpty(modalOptions.Title))
                {
                    options.Draggable = true;
                }
                this.OnOK = modalOptions.ActionOK;
                modalRef = await Modal.CreateModalAsync(options);
                return modalRef;
            }

            RenderFragment Render(string url, string title, IEnumerable<KeyValuePair<string, object>> paramenter) => builder =>
            {
                if (url[0] == '/') url = url[1..];
                var routes = UserConfig.Routes();
                foreach (var item in routes)
                {
                    var page = (string)item.GetObjValue("Template").GetObjValue("TemplateText");
                    if (page.ToLower() == url.ToLower())
                    {
                        var ComponentType = (Type)item.GetObjValue("Handler");
                        var index = 0;
                        builder.OpenElement(index++, "div");
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
                MessageService.Error(UserConfig.LanguageService[$"{CurrencyConstant.Page}.{CurrencyConstant.ComponentErrorMsg}"].Replace("{title}", title).Replace("{url}", url));
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

    public class CavModalOptions
    {
        /// <summary>
        /// 需要传入的参数
        /// </summary>
        public Dictionary<string, object> Paramenter { get; set; }
        /// <summary>
        /// url地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 传入组件，提供组件则不解析url和传入的参数
        /// </summary>
        public RenderFragment Content { get; set; }
        /// <summary>
        /// 点击回调
        /// </summary>
        public Action ActionOK { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        public string BodyStyle { get; set; } = "overflow-y: auto;height: 400px;";
    }
}
