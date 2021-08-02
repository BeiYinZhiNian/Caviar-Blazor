using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel
{
    public interface IBaseAction<ViewT>:IActionModel, IDIinjectAtteribute where ViewT : class, IView, new()
    {

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <returns></returns>
        public Task<ResultMsg> AddEntity(ViewT entity);
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <returns></returns>
        public Task<ResultMsg> DeleteEntity(ViewT entity);
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <returns></returns>
        public Task<ResultMsg> UpdateEntity(ViewT entity);
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public Task<ResultMsg<PageData<ViewT>>> GetPages(Expression<Func<ViewT, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public Task<ResultMsg> DeleteEntity(List<ViewT> menus);
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public Task<ResultMsg> UpdateEntity(List<ViewT> menus);
        /// <summary>
        /// 数据转换
        /// 需要达到一个model转为viewModel效果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ViewT> ToViewModel(List<ViewT> model);
        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public ResultMsg<PageData<ViewT>> FuzzyQuery(ViewQuery query);
    }
}
