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
        /// 模糊查询
        /// 使用权限验证sql的正确性
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FuzzyQuery(ViewQuery query)
        {
            if (string.IsNullOrEmpty(query.QueryObj)) return ResultError("查询对象不可为空");
            var action = CreateModel<PermissionAction>();
            var fields = await action.GetFieldsData(CavAssembly, query.QueryObj);
            if(fields==null) return ResultError("没有对该对象的查询权限");
            var assemblyList = CommonHelper.GetAssembly();
            Type type = null;
            foreach (var item in assemblyList)
            {
                type = item.GetTypes().SingleOrDefault(u => u.Name.ToLower() == query.QueryObj.ToLower());
                if (type != null) break;
            }
            if (type == null) return ResultError("没有对该对象的查询权限");
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("@queryStr", "%" + query.QueryStr + "%"),
            };
            var queryField = "";
            if (query.QueryField != null && query.QueryField.Count > 0)
            {
                queryField = "and (";
                for (int i = 0; i < query.QueryField.Count; i++)
                {
                    var field = fields.FirstOrDefault(u => u.TypeName == query.QueryField[i]);
                    if (field == null) return ResultError("查询字段错误");
                    queryField += $" {query.QueryField[i]} LIKE @queryStr ";
                    var index = i + 1;
                    if (index < query.QueryField.Count)
                    {
                        queryField += " or ";
                    }
                }
                queryField += ")";
            }
            var from = CommonHelper.GetCavBaseType(type)?.Name;
            string sql = $"select top(20)* from {from} where IsDelete=0 " + queryField;
            if (query.StartTime != null)
            {
                sql += $" and CreatTime>=@StartTime ";
                parameters.Add(new SqlParameter("@StartTime", query.StartTime));
            }
            if (query.EndTime != null)
            {
                sql += $" and CreatTime<=@EndTime ";
                parameters.Add(new SqlParameter("@EndTime", query.EndTime));
            }
            var data = BC.DC.SqlQuery(sql, parameters.ToArray());
            ResultMsg.Data = data.ToList(type);
            return ResultOK();
        }

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
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.WebUIPath + "/Pages/";
                }
                else if (keyName.Contains("View.cs"))
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.ModelsPath + "/ViewModels/";
                }
                else if(keyName.Contains("Action.cs"))
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.WebApiPath + "/ModelAction/";
                }
                else if (keyName.Contains("Controller.cs"))
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.WebApiPath + "/Controllers/";
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


        private async Task<int> CreateButton(string menuName, string outName, int parentId, bool isTree = false)
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
            if (isTree)
            {
                AddButton.Url = $"{outName}/Move";
            }
            await AddMenu(AddButton);
            return parentId;
        }


        private async Task<SysMenu> AddMenu(SysMenu menu)
        {
            SysMenu entity = null;
            if (menu.MenuType != MenuType.Button)
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
