using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.UI.Shared
{
    partial class NavMenu
    {
        bool _inlineCollapsed;
        [Parameter]
        public bool InlineCollapsed
        {
            get { return _inlineCollapsed; }
            set
            {
                OnCollapsed(value);
                _inlineCollapsed = value;
            }
        }

        [Parameter]
        public MenuItem BreadcrumbItemNav { get; set; }
        [Parameter]
        public EventCallback<MenuItem> BreadcrumbItemNavChanged { get; set; }

        public string[] OpenKeysNav { get; set; } = Array.Empty<string>();

        string[] _openKeysNae;


        public async void OnMenuItemClickedNav(MenuItem menuItem)
        {
            BreadcrumbItemNav = menuItem;
            if (BreadcrumbItemNavChanged.HasDelegate)
            {
                await BreadcrumbItemNavChanged.InvokeAsync(BreadcrumbItemNav);
            }
        }

        /// <summary>
        /// 当收缩时候将打开的菜单关闭，防止出现第二菜单。
        /// </summary>
        /// <param name="collapsed"></param>
        public void OnCollapsed(bool collapsed)
        {
            if (collapsed == _inlineCollapsed) return;
            if (collapsed)
            {
                _openKeysNae = OpenKeysNav;
                OpenKeysNav = Array.Empty<string>();
            }
            else
            {
                OpenKeysNav = _openKeysNae;
            }
        }
    }


}
