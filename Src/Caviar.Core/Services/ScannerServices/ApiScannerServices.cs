using Caviar.SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services.ScannerServices
{
    public static class ApiScannerServices
    {
        /// <summary>
        /// 获取所有控制器集合
        /// </summary>
        /// <param name="baseController">继承的控制器</param>
        /// <param name="excludeController">需要排除的控制器</param>
        /// <returns></returns>
        public static List<Type> GetAllController(Type baseController,List<Type> excludeController)
        {
            
            List<Type> controllerList = new List<Type>();
            CommonHelper.GetAssembly().ForEach(u =>
            {
                var type = u.GetTypes().Where(u => u.ContainBaseClass(baseController)!=null).ToList();
                foreach (var type_item in type)
                {
                    bool isExclude = false;
                    excludeController.ForEach(u => {
                        if (type_item == u)
                        {
                            isExclude = true;
                        }
                        }
                    );
                    if (isExclude) continue;
                    controllerList.Add(type_item);
                }
            });
            return controllerList;
        }
        /// <summary>
        /// 获取控制器下所有的Http方法
        /// </summary>
        /// <param name="controllerList"></param>
        /// <param name="HttpAttribute"></param>
        /// <returns></returns>
        public static List<SysMenu> GetAllMethods(List<Type> controllerList, Type HttpAttribute)
        {
            List<SysMenu> ApiList = new List<SysMenu>();
            foreach (var item in controllerList)
            {
                var info = item.GetMethods();
                foreach (var info_item in info)
                {
                    var attrs = info_item.GetCustomAttributes(HttpAttribute, false);
                    var attrsList = (IList<object>)attrs;
                    if (attrsList.Count == 0) continue;
                    var dispLayName = info_item.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                    foreach (var attrs_item in attrsList)
                    {
                        var httpMethods = (List<string>)attrs_item.GetObjValue("HttpMethods");
                        foreach (var httpMethod_item in httpMethods)
                        {
                            SysMenu menu = new SysMenu()
                            {
                                MenuType = MenuType.API,
                                HttpMethods = httpMethod_item,
                                Url = $"{item.Name.Replace("Controller", "")}/{info_item.Name}",
                                Key = dispLayName != null ? dispLayName : $"{info_item.Name}",
                                ControllerName = item.Name.Replace("Controller", ""),
                                TargetType = TargetType.Callback
                            };
                            ApiList.Add(menu);
                        }

                    }
                }
            }
            return ApiList;
        }
    }
}
