using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.Shared
{
    partial class NavMenu
    {
        [Parameter]
        public bool InlineCollapsed { get; set; }
        [Parameter]
        public MenuItem BreadcrumbItemNav { get; set; }
        [Parameter]
        public EventCallback<MenuItem> BreadcrumbItemNavChanged { get; set; }

        public string[] OpenKeysNav { get; set; } = Array.Empty<string>();

        

        public async void OnMenuItemClickedNav(MenuItem menuItem)
        {
            BreadcrumbItemNav = menuItem;
            if (BreadcrumbItemNavChanged.HasDelegate)
            {
                await BreadcrumbItemNavChanged.InvokeAsync(BreadcrumbItemNav);
            }
        }


    }


}
