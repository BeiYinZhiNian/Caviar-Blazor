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
            var count = await BC.DC.AddEntityAsync(this);
            return count;
        }

        public virtual async Task<int> DeleteEntity()
        {
            var count = await BC.DC.DeleteEntityAsync(this);
            return count;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteEntity(List<SysPowerMenu> menus)
        {
            var count = await BC.DC.DeleteEntityAsync(menus);
            return count;
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateEntity(List<SysPowerMenu> menus)
        {
            var count = await BC.DC.UpdateEntityAsync(menus);
            return count;
        }

        public virtual List<SysPowerMenu> GetEntitys(Expression<Func<SysPowerMenu, bool>> where)
        {
            var menus = BC.DC.GetEntityAsync(where).OrderBy(u => u.Id).ToList();
            return menus;
        }

        public virtual List<ViewPowerMenu> GetViewMenus(Expression<Func<SysPowerMenu, bool>> where)
        {
            var menus = GetEntitys(where);
            return ModelToView(menus);
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


        public void ViewToModel(ViewPowerMenu data, List<ViewPowerMenu> menus)
        {
            foreach (var item in data.Children)
            {
                menus.Add(item);
                if (item.Children != null && item.Children.Count != 0)
                {
                    ViewToModel(item, menus);
                }
            }
        }
    }
}