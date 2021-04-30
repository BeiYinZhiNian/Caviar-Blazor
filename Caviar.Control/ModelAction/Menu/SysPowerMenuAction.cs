using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Caviar.Control
{
    public partial class SysPowerMenuAction : SysPowerMenu
    {
        public virtual async Task<int> AddEntity()
        {
            var count = await BaseControllerModel.DataContext.AddEntityAsync(this);
            return count;
        }

        public virtual async Task<int> DeleteEntity()
        {
            var count = await BaseControllerModel.DataContext.DeleteEntityAsync(this);
            return count;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteEntity(List<SysPowerMenu> menus)
        {
            var transaction = BaseControllerModel.DataContext.BeginTransaction();
            foreach (var item in menus)
            {
                await BaseControllerModel.DataContext.DeleteEntityAsync(item, false);
            }
            var count = await BaseControllerModel.DataContext.SaveChangesAsync();
            transaction.Commit();
            return count;
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateEntity(List<SysPowerMenu> menus)
        {
            var transaction = BaseControllerModel.DataContext.BeginTransaction();
            foreach (var item in menus)
            {
                await BaseControllerModel.DataContext.UpdateEntityAsync(item, false);
            }
            var count = await BaseControllerModel.DataContext.SaveChangesAsync();
            transaction.Commit();
            return count;
        }

        public virtual List<ViewPowerMenu> GetViewMenus(Expression<Func<SysPowerMenu, bool>> where)
        {
            var menus = GetMenus(where);
            return ModelToView(menus);
        }

        public virtual List<SysPowerMenu> GetMenus(Expression<Func<SysPowerMenu, bool>> where)
        {
            var menus = BaseControllerModel.DataContext.GetEntityAsync(where).OrderBy(u => u.Id).ToList();
            return menus;
        }


        protected virtual List<ViewPowerMenu> ModelToView(List<SysPowerMenu> menus)
        {
            //将获取到的sys转为view
            var viewMenus = new List<ViewPowerMenu>().ListAutoAssign(menus);
            var resultViewMenuList = new List<ViewPowerMenu>();
            foreach (var item in viewMenus)
            {
                if (item.UpLayerId == 0)
                {
                    resultViewMenuList.Add(item);
                }
                else
                {
                    viewMenus.SingleOrDefault(u => u.Id == item.UpLayerId)?.Children.Add(item);
                }
            }
            return resultViewMenuList;
        }


        public void RecursionGetMenu(ViewPowerMenu data, List<ViewPowerMenu> menus)
        {
            foreach (var item in data.Children)
            {
                menus.Add(item);
                if (item.Children != null && item.Children.Count != 0)
                {
                    RecursionGetMenu(item, menus);
                }
            }
        }
    }
}