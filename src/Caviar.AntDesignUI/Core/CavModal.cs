// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using OneOf;

namespace Caviar.AntDesignUI.Core
{
    public class CavModal
    {
        readonly ModalService _modal;
        readonly ILanguageService _languageService;
        readonly MessageService _messageService;
        public CavModal(ILanguageService languageService, ModalService modalService, MessageService messageService)
        {
            _languageService = languageService;
            _modal = modalService;
            _messageService = messageService;
        }

        public async Task<ModalRef> Create(CavModalOptions modalOptions)
        {
            var modelHandle = new ModalHandle(_languageService, _modal, _messageService);
            return await modelHandle.Create(modalOptions);
        }

        public RenderFragment Render(string url, string title, IEnumerable<KeyValuePair<string, object>> paramenter)
        {
            var modelHandle = new ModalHandle(_languageService, _modal, _messageService);
            return modelHandle.Render(url, title, paramenter);
        }


        protected class ModalHandle
        {
            readonly ModalService _modal;
            readonly ILanguageService _languageService;
            readonly MessageService _messageService;
            ModalRef _modalRef;
            Action _onOK;
            Func<Task<bool>> _funcValidate;
            ITableTemplate _tableTemplate;

            public ModalHandle(ILanguageService languageService, ModalService modalService, MessageService messageService)
            {
                _languageService = languageService;
                _modal = modalService;
                _messageService = messageService;
            }

            /// <summary>
            /// 根据page url动态创建Modal
            /// </summary>
            /// <param name="modalOptions"></param>
            /// <returns></returns>
            public async Task<ModalRef> Create(CavModalOptions modalOptions)
            {
                if (modalOptions.BodyStyle == null) modalOptions.BodyStyle = "";
                modalOptions.BodyStyle += $"height:{modalOptions.Height}px;";
                if (modalOptions.IsOverflow) modalOptions.BodyStyle += "overflow-y: auto;";
                RenderFragment render = modalOptions.Render;
                if (render == null)
                {
                    render = Render(modalOptions.Url, modalOptions.Title, modalOptions.Paramenter);
                }
                ModalOptions options = new ModalOptions()
                {
                    OnOk = HandleOk,
                    OnCancel = HandleCancel,
                    MaskClosable = false,
                    Width = modalOptions.Width,
                    Content = render,
                    Style = modalOptions.Style,
                    Title = modalOptions.Title,
                    BodyStyle = modalOptions.BodyStyle,
                    Visible = true,
                    OkText = _languageService[$"{CurrencyConstant.Page}.{CurrencyConstant.Confirm}"],
                    CancelText = _languageService[$"{CurrencyConstant.Page}.{CurrencyConstant.Cancel}"],
                    DestroyOnClose = true,
                    Footer = modalOptions.Footer
                };
                if (!string.IsNullOrEmpty(modalOptions.Title))
                {
                    options.Draggable = true;
                }
                _onOK = modalOptions.ActionOK;
                _funcValidate = modalOptions.FuncValidate;
                _modalRef = await _modal.CreateModalAsync(options);
                return _modalRef;
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
                        var componentType = (Type)item.GetObjValue("Handler");
                        var index = 0;
                        builder.OpenElement(index++, "div");
                        builder.OpenComponent(index++, componentType);
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
                _messageService.Error(_languageService[$"{CurrencyConstant.Page}.{CurrencyConstant.ComponentErrorMsg}"].Replace("{title}", title).Replace("{url}", url));
            };


            void SetComponent(object e)
            {
                if (e is ITableTemplate)
                {
                    _tableTemplate = (ITableTemplate)e;
                }
            }

            private async Task HandleOk(MouseEventArgs e)
            {
                _modalRef.Config.ConfirmLoading = true;
                var res = true;
                if (_tableTemplate != null)
                {
                    res = await _tableTemplate.Validate();
                }
                if (_funcValidate != null)
                {
                    res = await _funcValidate.Invoke();
                }
                _modalRef.Config.ConfirmLoading = false;
                _modalRef.Config.Visible = !res;
                if (res)
                {
                    _onOK?.Invoke();
                }
            }

            private Task HandleCancel(MouseEventArgs e)
            {
                _modalRef.Config.Visible = false;
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

        public Func<Task<bool>> FuncValidate { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 容器样式
        /// </summary>
        public string BodyStyle { get; set; }
        /// <summary>
        /// model样式
        /// </summary>
        public string Style { get; set; }
        public bool IsOverflow { get; set; } = true;

        public OneOf<string, double> Width { get; set; } = "500px";
        public double Height { get; set; } = 400;
        public RenderFragment Render { get; set; }
        internal static readonly RenderFragment DefaultFooter = delegate (RenderTreeBuilder builder)
        {
            builder.OpenComponent<ModalFooter>(0);
            builder.CloseComponent();
        };
        public OneOf<string, RenderFragment>? Footer { get; set; } = DefaultFooter;
    }
}
