using AntDesign;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.Shared
{
    partial class NavBreadcrumb
    {
        [Parameter]
        public MenuItem BreadcrumbItemNav 
        {
            get {
                return _breadcrumbItemNav;
            }
            set {
                _breadcrumbItemNav = value;
                CreatBreadcrumbItemNav(value);
            } 
        }

        MenuItem _breadcrumbItemNav;
        List<string> BreadcrumbItemArr { get; set; }

        string _homeTitle = "首页";

        void CreatBreadcrumbItemNav(MenuItem menuItem)
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
