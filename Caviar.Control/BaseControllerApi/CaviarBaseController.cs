using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Caviar.Control.Permission;

namespace Caviar.Control
{
    public partial class CaviarBaseController
    {
        protected IAssemblyDynamicCreation CavAssembly => BC.HttpContext.RequestServices.GetService<IAssemblyDynamicCreation>();


        #region API
        /// <summary>
        /// 代码生成
        /// </summary>
        /// <param name="generate"></param>
        /// <param name="isPerview">是否预览</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CodeFileGenerate(CodeGenerateData generate,bool isPerview = true)
        {
            if (generate == null) return ResultError("必要参数不可为空");
            var data = CavAssembly.CodeGenerate(generate, BC.UserName);
            if(isPerview)
            {
                ResultMsg.Data = CavAssembly.CodeGenerate(generate, BC.UserName);
                return ResultOK();
            }
            bool isCover = generate.Config?.SingleOrDefault(u => u == "覆盖") == null ? false : true;
            bool isCreateMenu = generate.Config?.SingleOrDefault(u => u == "创建按钮") == null ? false : true;
            foreach (var item in data)
            {
                string outName = "";
                string path = "";
                string keyName = item.KeyName.Replace("(name)", "");
                if (keyName.Contains(".razor"))
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.WebUI.Path + "/Pages/";
                }
                else if (keyName.Contains("View.cs"))
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.Models.Path + "/ViewModels/";
                }
                else if(keyName.Contains("Action.cs"))
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.WebAPI.Path + "/ModelAction/";
                }
                else if (keyName.Contains("Controller.cs"))
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.WebAPI.Path + "/Controllers/";
                }
                if (path == "") continue;
                path += generate.OutName + "/";
                outName = path + item.TabName;
                CavAssembly.WriteCodeFile(path, outName, item.Content, isCover);
            }
            if (isCreateMenu)
            {
                await CreateButton(generate.ModelName, generate.OutName, 0);
            }
            return ResultOK();
        }

        /// <summary>
        /// 创建基础按钮
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="outName"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        protected virtual async Task<int> CreateButton(string menuName, string outName, int parentId)
        {
            SysMenu menu = new SysMenu()
            {
                MenuName = menuName + "管理",
                TargetType = TargetType.CurrentPage,
                MenuType = MenuType.Menu,
                Url = $"{outName}/Index",
                Icon = "border-outer",
                ParentId = parentId,
                Number = "999"
            };
            menu = await AddMenu(menu);
            parentId = menu.Id;
            SysMenu AddButton = new SysMenu()
            {
                MenuType = MenuType.Button,
                TargetType = TargetType.EjectPage,
                MenuName = "新增",
                ButtonPosition = ButtonPosition.Default,
                Url = $"{outName}/Add",
                Icon = "appstore-add",
                ParentId = parentId,
                IsDoubleTrue = false,
                Number = "999"
            };
            await AddMenu(AddButton);
            AddButton = new SysMenu()
            {
                MenuType = MenuType.Button,
                Url = $"{outName}/Update",
                MenuName = "修改",
                TargetType = TargetType.EjectPage,
                ButtonPosition = ButtonPosition.Row,
                Icon = "edit",
                ParentId = parentId,
                Number = "999"
            };
            await AddMenu(AddButton);
            AddButton = new SysMenu()
            {
                MenuType = MenuType.Button,
                MenuName = "删除",
                ButtonPosition = ButtonPosition.Row,
                Url = $"{outName}/Delete",
                TargetType = TargetType.Callback,
                Icon = "delete",
                ParentId = parentId,
                IsDoubleTrue = true,
                Number = "9999"
            };
            await AddMenu(AddButton);
            AddButton = new SysMenu()
            {
                MenuType = MenuType.API,
                MenuName = "查询",
                Url = $"{outName}/FuzzyQuery",
                ParentId = parentId,
                Number = "999"
            };
            await AddMenu(AddButton);
            AddButton = new SysMenu()
            {
                MenuType = MenuType.API,
                MenuName = "获取字段",
                Url = $"{outName}/GetFields",
                ParentId = parentId,
                Number = "999"
            };
            await AddMenu(AddButton);
            return parentId;
        }

        /// <summary>
        /// 判断按钮是否可以生成，防止重复
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        protected virtual async Task<SysMenu> AddMenu(SysMenu menu)
        {
            SysMenu entity = null;
            if (!string.IsNullOrEmpty(menu.Url))
            {
                entity = await BC.DC.GetSingleEntityAsync<SysMenu>(u => u.Url == menu.Url);
            }
            else if (menu.MenuType != MenuType.Button)
            {
                entity = await BC.DC.GetSingleEntityAsync<SysMenu>(u => u.MenuName == menu.MenuName);
            }
            else
            {
                entity = await BC.DC.GetSingleEntityAsync<SysMenu>(u => u.MenuName == menu.MenuName && u.ParentId == menu.ParentId);
            }
            var count = 0;
            if (entity == null)
            {
                count = await BC.DC.AddEntityAsync(menu);
                entity = menu;
            }
            return entity;
        }
        #endregion
    }
}
