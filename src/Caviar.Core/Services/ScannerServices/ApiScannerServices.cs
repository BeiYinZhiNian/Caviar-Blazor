// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;

namespace Caviar.Core.Services
{
    public static class ApiScannerServices
    {

        public static async Task<int> CreateInitApi(IDbContext dbContext, CodeGenerateOptions codeGenerateOptions)
        {
            var menus = CreateInitMenus(codeGenerateOptions);
            var set = dbContext.Set<SysMenu>(); //使用原生dbcontext跳过权限控制
            var buttons = new List<SysMenu>();
            foreach (var item in menus)
            {
                if (set.SingleOrDefault(u => u.Url == item.Url) == null)
                {
                    set.Add(item);
                    buttons.Add(item);
                }
            }
            await dbContext.SaveChangesAsync();
            var subMenu = menus.Single(u => u.MenuName + "/Index" == u.Url);//查找父id
            buttons = UpdateInitButtons(buttons, subMenu.Id);
            var sysManagement = set.Single(u => u.MenuName == CurrencyConstant.SysManagementKey);
            subMenu.ParentId = sysManagement.Id; //父id
            subMenu.MenuType = MenuType.Menu; // 改为菜单
            set.UpdateRange(buttons);
            await dbContext.SaveChangesAsync();
            return buttons.Count;
        }
        /// <summary>
        /// 获取所有控制器集合
        /// </summary>
        /// <param name="baseController">继承的控制器</param>
        /// <param name="excludeController">需要排除的控制器</param>
        /// <returns></returns>
        public static List<Type> GetAllController(Type baseController, List<Type> excludeController)
        {
            List<Type> controllerList = new List<Type>();
            var type = CommonHelper.GetAllTypes().Where(u => u.ContainBaseClass(baseController) != null).ToList();
            foreach (var type_item in type)
            {
                bool isExclude = false;
                excludeController.ForEach(u =>
                {
                    if (type_item == u)
                    {
                        isExclude = true;
                    }
                }
                );
                if (isExclude) continue;
                controllerList.Add(type_item);
            }
            return controllerList;
        }
        /// <summary>
        /// 获取控制器下所有的Http方法
        /// </summary>
        /// <param name="controllerList"></param>
        /// <param name="httpAttribute"></param>
        /// <returns></returns>
        public static List<SysMenu> GetAllMethods(List<Type> controllerList, Type httpAttribute)
        {
            List<SysMenu> apiList = new List<SysMenu>();
            foreach (var item in controllerList)
            {
                var info = item.GetMethods();
                foreach (var info_item in info)
                {
                    var attrs = info_item.GetCustomAttributes(httpAttribute, false);
                    var attrsList = (IList<object>)attrs;
                    if (attrsList.Count == 0) continue;
                    var dispLayName = info_item.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                    foreach (var attrs_item in attrsList)
                    {
                        var httpMethods = (List<string>)attrs_item.GetObjValue("HttpMethods");
                        foreach (var httpMethod_item in httpMethods)
                        {
                            var fieldName = item.Name.Replace("Controller", "");
                            apiList.Add(CreatInitMenu(httpMethod_item, fieldName, info_item.Name));
                        }

                    }
                }
            }
            return apiList;
        }

        private static SysMenu CreatInitMenu(string httpMethod_item, string fieldName, string actionName, string key = null)
        {
            SysMenu menu = new SysMenu()
            {
                MenuType = MenuType.API,
                HttpMethods = httpMethod_item,
                Url = $"{fieldName}/{actionName}",
                MenuName = key ?? actionName,
                TargetType = TargetType.Callback
            };
            return menu;
        }

        public static List<SysMenu> CreateInitMenus(CodeGenerateOptions codeGenerateOptions)
        {
            List<SysMenu> apiList = new List<SysMenu>();
            apiList.Add(CreatInitMenu("GET", codeGenerateOptions.EntitieName, CurrencyConstant.Index, codeGenerateOptions.EntitieName));
            apiList.Add(CreatInitMenu("GET", codeGenerateOptions.EntitieName, CurrencyConstant.GetEntityKey));
            apiList.Add(CreatInitMenu("GET", codeGenerateOptions.EntitieName, CurrencyConstant.GetFieldsKey));
            apiList.Add(CreatInitMenu("POST", codeGenerateOptions.EntitieName, CurrencyConstant.DeleteEntityKey));
            apiList.Add(CreatInitMenu("POST", codeGenerateOptions.EntitieName, CurrencyConstant.UpdateEntityKey));
            apiList.Add(CreatInitMenu("POST", codeGenerateOptions.EntitieName, CurrencyConstant.CreateEntityKey));
            return apiList;
        }

        public static List<SysMenu> UpdateInitButtons(List<SysMenu> menuList, int parentId)
        {
            foreach (var menu_item in menuList)
            {
                menu_item.ParentId = parentId;
                switch (menu_item.MenuName)
                {
                    case CurrencyConstant.CreateEntityKey:
                        menu_item.MenuType = MenuType.Button;
                        menu_item.Icon = "appstore-add";
                        menu_item.TargetType = TargetType.EjectPage;
                        menu_item.Number = "997";
                        break;
                    case CurrencyConstant.UpdateEntityKey:
                        menu_item.MenuType = MenuType.Button;
                        menu_item.ButtonPosition = ButtonPosition.Row;
                        menu_item.Icon = "edit";
                        menu_item.TargetType = TargetType.EjectPage;
                        menu_item.Number = "998";
                        break;
                    case CurrencyConstant.DeleteEntityKey:
                        menu_item.MenuType = MenuType.Button;
                        menu_item.ButtonPosition = ButtonPosition.Row;
                        menu_item.IsDoubleTrue = true;
                        menu_item.Icon = "delete";
                        menu_item.TargetType = TargetType.Callback;
                        menu_item.Number = "999";
                        break;
                    default:
                        break;
                }
            }
            return menuList;
        }

    }
}
