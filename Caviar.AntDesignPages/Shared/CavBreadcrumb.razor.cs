using AntDesign;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Shared
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

        string _homeTitle = "首页";

        void CreatBreadcrumbItemCav(MenuItem menuItem)
        {
            if (menuItem == null) return;
            Console.WriteLine(menuItem.RouterLink);
            var breadcrumbItemArr = new List<string>();
            var parent = menuItem.ParentMenu;
            while (parent != null)
            {
                breadcrumbItemArr.Insert(0, parent.Key);
                parent = parent.Parent;
            }
            if (menuItem.RouterLink != "/")
            {
                breadcrumbItemArr.Add(menuItem.Key);
            }
            else
            {
                _homeTitle = menuItem.Key;
            }
            BreadcrumbItemArr = breadcrumbItemArr;
        }

    }
}
