using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Caviar.Models.SystemData;
/// <summary>
/// 生成者：未登录用户
/// 生成时间：2021/5/31 11:39:35
/// 代码由代码生成器自动生成，更改的代码可能被进行替换
/// 可在上层目录使用partial关键字进行扩展
/// 菜单模型操作器
/// </summary>
namespace Caviar.Control.Menu
{
    [DisplayName("菜单方法")]
    public partial class MenuAction : SysMenu
    {
        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> AddEntity()
        {
            var count = await BC.DC.AddEntityAsync(this);
            return count;
        }
        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> DeleteEntity()
        {
            var count = await BC.DC.DeleteEntityAsync(this);
            return count;
        }
        /// <summary>
        /// 修改菜单
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
        public virtual async Task<PageData<ViewMenu>> GetPages(Expression<Func<SysMenu, bool>> where,int pageIndex,int pageSize, bool isOrder = true, bool isNoTracking = false)
        {
            var pages = await BC.DC.GetPageAsync(where, u => u.Number, pageIndex, pageSize, isOrder, isNoTracking);
            var ViewPages = ModelToViewModel(pages);
            return ViewPages;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteEntity(List<SysMenu> menus)
        {
            var count = await BC.DC.DeleteEntityAsync(menus);
            return count;
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateEntity(List<SysMenu> menus)
        {
            var count = await BC.DC.UpdateEntityAsync(menus);
            return count;
        }

        partial void PartialModelToViewModel(ref bool isContinue, PageData<SysMenu> model,ref PageData<ViewMenu> outModel);

        /// <summary>
        /// 魔法转换
        /// 需要达到一个model转为viewModel效果
        /// 交给你去处理，如果你不需要处理，则自动处理，我称之为魔法转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        private PageData<ViewMenu> ModelToViewModel(PageData<SysMenu> model)
        {
            
            bool isContinue = true;
            PageData<ViewMenu> outModel = null;
            PartialModelToViewModel(ref isContinue, model,ref outModel);
            if (!isContinue) return outModel;
            outModel = CommonHelper.AToB<PageData<ViewMenu>, PageData<SysMenu>>(model);
            return outModel;
        }
    }
}