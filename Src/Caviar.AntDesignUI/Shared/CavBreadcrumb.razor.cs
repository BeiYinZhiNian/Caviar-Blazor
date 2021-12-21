using AntDesign;
using Caviar.AntDesignUI.Helper;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Shared
{
    partial class CavBreadcrumb
    {
        [Parameter]
        public MenuItem BreadcrumbItemCav
        {
            get {
                return _breadcrumbItemCav;
            }
            set {
                _breadcrumbItemCav = value;
                CreatBreadcrumbItemCav(value);
            } 
        }

        MenuItem _breadcrumbItemCav;
        List<string> BreadcrumbItemArr { get; set; }

        void CreatBreadcrumbItemCav(MenuItem menuItem)
        {
            if (menuItem == null) return;
            var breadcrumbItemArr = new List<string>();
            var parent = menuItem.ParentMenu;
            while (parent != null)
            {
                breadcrumbItemArr.Insert(0, parent.Key);
                parent = parent.Parent;
            }
            breadcrumbItemArr.Add(menuItem.Key);
            BreadcrumbItemArr = breadcrumbItemArr;
        }

    }
}
