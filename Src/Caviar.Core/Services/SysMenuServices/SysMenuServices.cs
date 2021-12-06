using Caviar.SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services.SysMenuServices
{
    public partial class SysMenuServices: EasyBaseServices<SysMenu>
    {
        /// <summary>
        /// 获取当前菜单
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysMenu>> GetMenuBar()
        {
            var menus = await GetAllAsync();
            return menus;
        }

        public async Task<List<SysMenu>> GetApiList(string url,string[] controllerList)
        {
            var entity = await GetEntity(u => u.Url == url,true);
            var apiList = await GetEntity(u => u.ControllerName == entity.ControllerName);
            if(controllerList != null)
            {
                foreach (var item in controllerList)
                {
                    var otherApi = await GetEntity(u => u.ControllerName == item);
                    apiList.AddRange(otherApi);
                }
            }
            
            return apiList;
        }
    }
}
