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

        public RenderFragment Render(string url, string title, IEnumerable<KeyValuePair<string, object>> paramenter)
        {
            var modelHandle = new ModalHandle(UserConfig, Modal, MessageService);
            return modelHandle.Render(url, title, paramenter);
        }

        protected class ModalHandle
        {
            ModalService Modal;
            UserConfig UserConfig;
            MessageService MessageService;
            ModalRef modalRef;
            Action OnOK;
            ITableTemplate TableTemplate;

            public ModalHandle(UserConfig userConfig, ModalService modalService, MessageService messageService)
            {
                UserConfig = userConfig;
                Modal = modalService;
                MessageService = messageService;
            }

            /// <summary>
            /// 根据page url动态创建Modal
            /// </summary>
            /// <param name="modalOptions"></param>
            /// <returns></returns>
            public async Task<ModalRef> Create(CavModalOptions modalOptions)
            {
                ModalOptions options = new ModalOptions()
                {
                    OnOk = HandleOk,
                    OnCancel = HandleCancel,
                    MaskClosable = false,
                    Content = Render(modalOptions.Url, modalOptions.Title, modalOptions.Paramenter),
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
                OnOK = modalOptions.ActionOK;
                modalRef = await Modal.CreateModalAsync(options);
                return modalRef;
            }

            public RenderFragment Render(string url, string title, IEnumerable<KeyValuePair<string, object>> paramenter) => builder =>
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

            
            void SetComponent(object e)
            {
                TableTemplate = (ITableTemplate)e;
            }

            private async Task HandleOk(MouseEventArgs e)
            {
                modalRef.Config.ConfirmLoading = true;
                var res = true;
                if (TableTemplate != null)
                {
                    res = await TableTemplate.Validate();
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
        /// 成功执行后回调
        /// </summary>
        public Action ActionOK { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 容器样式
        /// </summary>
        public string BodyStyle { get; set; } = "overflow-y: auto;height: 400px;";
    }
}
