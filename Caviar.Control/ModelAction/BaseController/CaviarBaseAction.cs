using Caviar.Models;
using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Caviar.Control.ModelAction;
using System.Text.Json;
using System.Reflection;
using Microsoft.Extensions.Primitives;
using System.Web;

namespace Caviar.Control
{
    public class CaviarBaseAction: BaseModelResultAction
    {
        public IBaseControllerModel BC { get; set; }

        public ResultMsg ResultMsg { get; set; } = new ResultMsg();

        /// <summary>
        /// 代码生成
        /// </summary>
        /// <param name="generate"></param>
        /// <param name="isPerview"></param>
        /// <returns></returns>
        public async Task<ResultMsg<List<TabItem>>> CodeFileGenerate(CodeGenerateData generate, bool isPerview = true)
        {
            IAssemblyDynamicCreation CavAssembly = BC.HttpContext.RequestServices.GetService<IAssemblyDynamicCreation>();
            if (generate == null) return Error<List<TabItem>>("必要参数不可为空");
            var data = CavAssembly.CodeGenerate(generate, BC.UserName);
            if (isPerview)
            {
                return Ok(data);
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
                else if (keyName.Contains("Action.cs"))
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
            return Ok(data);
        }

        /// <summary>
        /// 创建基础按钮
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="outName"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public virtual async Task<int> CreateButton(string menuName, string outName, int parentId)
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

        /// <summary>
        /// 检查用户token
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public ResultMsg CheckUsreToken()
        {
            if (BC.HttpContext.Request.Headers.TryGetValue(CurrencyConstant.Authorization, out StringValues value))
            {
                string Authorization = value[0].Replace(CurrencyConstant.JWT, "");
                var IsValidate = JwtHelper.Validate(Authorization);
                if (!IsValidate)
                {
                    return Error("您的登录已过期，请重新登录");
                }
                BC.UserToken = CommonHelper.GetJwtUserToken(Authorization);
            }
            //没有写带token应该在权限拦下不应该在检查时候
            //这样的好处是可以给未登录用户一些权限
            return Ok();
        }

        public virtual ResultMsg GetInto()
        {
            if (!ActionVerification())
            {
                if (BC.IsLogin)
                {
                    return NoPermission("对不起，您还没有获得该权限");
                }
                else
                {
                    return Unauthorized("请您先登录",CaviarConfig.CavUrl.UserLogin);
                }
            }
            return Ok();
        }


        /// <summary>
        /// 判断是否可以访问
        /// </summary>
        /// <returns></returns>
        protected virtual bool ActionVerification()
        {
            if (CaviarConfig.IsDebug) return true;
            var url = BC.Current_Action.Replace("/api/", "").ToLower();
            if (CaviarConfig.IsStrict)
            {
                var menu = BC.UserData.Menus.FirstOrDefault(u => !string.IsNullOrEmpty(u.Url) && u.Url.ToLower() == url);
                if (menu != null) return true;
                return false;
            }
            else
            {
                var menu = BC.UserData.Menus.FirstOrDefault(u => !string.IsNullOrEmpty(u.Url) && u.Url.ToLower() == url);
                if (menu == null) return true;//宽松模式，未加入权限api则不受限制
                menu = BC.UserData.Menus.FirstOrDefault(u => u.Url == menu.Url);//如果有限制，则向用户api查询
                if (menu != null) return true;
                return false;
            }
        }

        /// <summary>
        /// 遍历类型中所有包含IBaseModel的子类
        /// 过滤模型
        /// </summary>
        /// <param name="type"></param>
        public void ArgumentsModel(Type type, object data)
        {
            if (data == null) return;
            if (!type.IsClass)//排除非类
            {
                return;
            }
            else if (type == typeof(string))//排除字符串（特殊类）
            {
                return;
            }
            bool isBaseModel;
            isBaseModel = type.GetInterfaces().Contains(typeof(IBaseModel));
            if (isBaseModel)
            {
                if (data == null) return;
                //去过滤参数
                ArgumentsFields(type, data);
            }
            else if (type.GetInterfaces().Contains(typeof(System.Collections.ICollection)))
            {
                var list = (System.Collections.IEnumerable)data;
                foreach (var dataItem in list)
                {
                    ArgumentsModel(dataItem.GetType(), dataItem);
                }
            }
            else
            {
                foreach (PropertyInfo sp in type.GetProperties())//获得类型的属性字段
                {
                    var properType = sp.PropertyType;
                    var value = sp.GetValue(data, null);
                    ArgumentsModel(properType, value);
                }
            }

        }

        /// <summary>
        /// 过滤参数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        private void ArgumentsFields(Type type, object data)
        {
            var baseType = CommonHelper.GetCavBaseType(type);
            if (baseType == null) return;
            foreach (PropertyInfo sp in baseType.GetProperties())//获得类型的属性字段
            {
                if (sp.Name.ToLower() == "id") continue;//忽略id字段
                if (sp.Name.ToLower() == "uid") continue;//忽略uid字段
                var field = BC.UserData.ModelFields.FirstOrDefault(u => u.BaseTypeName == baseType.Name && sp.Name == u.TypeName);
                if (field == null)
                {
                    try
                    {
                        sp.SetValue(data, default, null);//设置为默认字段
                    }
                    catch
                    {
                        //忽略该错误,并记录到错误日志
                    }
                }
            }

        }
    }
}
