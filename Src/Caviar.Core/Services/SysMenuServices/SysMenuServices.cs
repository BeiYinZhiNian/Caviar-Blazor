using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    public partial class SysMenuServices: EasyBaseServices<SysMenu>
    {
        public List<SysPermission> MenuPermissions {  set { MenuKeys = value.Select(u => u.Permission).ToList(); } }

        private List<string> MenuKeys;

        public SysMenuServices(IAppDbContext appDbContext):base(appDbContext)
        {

        }

        public override IQueryable<SysMenu> GetEntity(Expression<Func<SysMenu, bool>> where)
        {
            return base.GetEntity(where).Where(u=>MenuKeys.Contains(u.Key));
        }

        public override Task<List<SysMenu>> GetAllAsync()
        {
            return base.GetEntity(u => MenuKeys.Contains(u.Key)).ToListAsync();
        }

        public override async Task<SysMenu> SingleOrDefaultAsync(Expression<Func<SysMenu, bool>> where)
        {
            var entity = await base.SingleOrDefaultAsync(where);
            if (entity == null) return null;
            if(MenuKeys.Contains(entity.Key)) return entity;
            return null;
        }

        public override Task<SysMenu> GetEntity(int id)
        {
            return SingleOrDefaultAsync(u=>u.Id == id);
        }
        /// <summary>
        /// 获取当前菜单
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysMenu>> GetMenuBar()
        {
            if (MenuKeys == null) return new List<SysMenu>();
            var menus = await GetAllAsync();
            return menus;
        }

        public async Task<List<SysMenu>> GetApiList(string url,string[] controllerList)
        {
            if (url == null) throw new Exception("url不能为null");
            if (url[0] == '/' && url.Count() > 1) url = url.Substring(1);
            var entity = await SingleOrDefaultAsync(u => u.Url.ToLower() == url.ToLower());
            if (entity == null) throw new Exception($"未找到{url}所需资源");
            var apiList = await GetEntity(u => u.ControllerName == entity.ControllerName).ToListAsync();
            if(controllerList != null)
            {
                foreach (var item in controllerList)
                {
                    if (item == "") continue;
                    var otherApi = await GetEntity(u => u.ControllerName == item).ToListAsync();
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
            List<SysMenu> menuList = await GetEntity(u => u.ParentId == menus.Id).ToListAsync();
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
