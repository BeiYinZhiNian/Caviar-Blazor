using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 生成者：未登录用户
/// 生成时间：2021/5/11 17:57:45
/// 代码由代码生成器自动生成，更改的代码可能被进行替换
/// 可在上层目录使用partial关键字进行扩展
/// 系统菜单模型操作器
/// </summary>
namespace Caviar.Control
{
    public partial class MenuAction : SysPowerMenu
    {
        /// <summary>
        /// 添加系统菜单
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> AddEntity()
        {
            var count = await BC.DC.AddEntityAsync(this);
            return count;
        }
        /// <summary>
        /// 删除系统菜单
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> DeleteEntity()
        {
            var count = await BC.DC.DeleteEntityAsync(this);
            return count;
        }
        /// <summary>
        /// 修改系统菜单
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> UpdateEntity()
        {
            var count = await BC.DC.UpdateEntityAsync(this);
            return count;
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<PageData<ViewMenu>> GetPages(Expression<Func<SysPowerMenu, bool>> where,int pageIndex,int pageSize, bool isOrder = true, bool isNoTracking = false)
        {
            var pages = await BC.DC.GetPageAsync(where, u => u.Id, pageIndex, pageSize, isOrder, isNoTracking);
            var ViewPages = ModelToViewModel(pages);
            return ViewPages;
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

        protected delegate T Transformation<T,K>(K model);

        protected event Transformation<PageData<ViewMenu>, PageData<SysPowerMenu>> TransformationEvent;

        /// <summary>
        /// 魔法转换
        /// 需要达到一个model转为viewModel效果
        /// 交给你去处理，如果你不需要处理，则自动处理，我称之为魔法转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        private PageData<ViewMenu> ModelToViewModel(PageData<SysPowerMenu> model)
        {
            
            var viewModel = TransformationEvent?.Invoke(model);
            if (viewModel == null)
            {
                viewModel = CommonHelper.AToB<PageData<ViewMenu>, PageData<SysPowerMenu>>(model);
            }
            return viewModel;
        }
    }
}