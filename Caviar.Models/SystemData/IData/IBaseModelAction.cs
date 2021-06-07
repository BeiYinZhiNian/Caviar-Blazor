using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public interface IBaseModelAction<T, ViewT>:IActionModel, IDIinjectAtteribute where T : class, IBaseModel
    {
        /// <summary>
        /// 数据实体，当前台post中带有ViewT模型，会自动注入到实体中
        /// </summary>
        public T Entity { get; set; }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <returns></returns>
        public Task<int> AddEntity();
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <returns></returns>
        public Task<int> DeleteEntity();
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <returns></returns>
        public Task<int> UpdateEntity();
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public Task<PageData<ViewT>> GetPages(Expression<Func<T, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public Task<int> DeleteEntity(List<T> menus);
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public Task<int> UpdateEntity(List<SysMenu> menus);
        /// <summary>
        /// 数据转换
        /// 需要达到一个model转为viewModel效果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ViewT> ModelToViewModel(List<T> model);
    }
}
