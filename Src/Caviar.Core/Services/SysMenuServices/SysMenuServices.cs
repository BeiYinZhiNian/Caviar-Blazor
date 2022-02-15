using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    public partial class SysMenuServices: EasyBaseServices<SysMenu>
    {
        public SysMenuServices(IAppDbContext appDbContext):base(appDbContext)
        {

        }
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
            if (url == null) throw new Exception("url不能为null");
            if (url[0] == '/' && url.Count() > 1) url = url.Substring(1);
            var entity = await GetEntity(u => u.Url.ToLower() == url.ToLower(), true);
            if (entity == null) throw new Exception($"未找到{url}所需资源");
            var apiList = await GetEntity(u => u.ControllerName == entity.ControllerName);
            if(controllerList != null)
            {
                foreach (var item in controllerList)
                {
                    if (item == "") continue;
                    var otherApi = await GetEntity(u => u.ControllerName == item);
                    apiList.AddRange(otherApi);
                }
            }
            
            return apiList;
        }

        public async Task DeleteEntityAll(List<SysMenu> menus)
        {
            await base.DeleteEntity(menus);
        }

        public override async Task<bool> DeleteEntity(SysMenu menus)
        {
            List<SysMenu> menuList = await GetEntity(u => u.ParentId == menus.Id);
            if (menuList != null && menuList.Count > 1)
            {
                foreach (var item in menuList)
                {
                    if (item.ParentId == menus.Id)
                    {
                        item.ParentId = menus.ParentId;
                    }
                }
                await base.UpdateEntity(menuList);
            }
            return await base.DeleteEntity(menus);
        }
    }
}
