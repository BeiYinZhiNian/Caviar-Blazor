using Caviar.Models;
using Caviar.Models.SystemData;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.ModelAction
{
    public class BaseModelAction<T,ViewT> : IBaseModelAction<T, ViewT> where T : class, IBaseModel,new()  where ViewT: class,T, new()
    {
        /// <summary>
        /// 数据实体
        /// </summary>
        public T Entity { get; set; }

        public IBaseControllerModel BC { get; set; }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> AddEntity()
        {
            var count = await BC.DC.AddEntityAsync(Entity);
            return count;
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> DeleteEntity()
        {
            var count = await BC.DC.DeleteEntityAsync(Entity);
            return count;
        }
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> UpdateEntity()
        {
            var count = await BC.DC.UpdateEntityAsync(Entity);
            return count;
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<PageData<ViewT>> GetPages(Expression<Func<T, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true)
        {
            var pages = await BC.DC.GetPageAsync(where, u => u.Number, pageIndex, pageSize, isOrder, isNoTracking);
            var list = ModelToViewModel(pages.Rows);
            PageData<ViewT> viewPage = new PageData<ViewT>(list);
            return viewPage;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteEntity(List<T> menus)
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
        /// <summary>
        /// 通用模糊查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual async Task<List<ViewT>> FuzzyQuery(ViewQuery query,List<SysModelFields> fields)
        {
            var assemblyList = CommonHelper.GetAssembly();
            Type type = null;
            foreach (var item in assemblyList)
            {
                type = item.GetTypes().SingleOrDefault(u => u.Name.ToLower() == typeof(T).Name.ToLower());
                if (type != null) break;
            }
            if (type == null) return default;
            List<SqlParameter> parameters = new List<SqlParameter>();
            var queryField = "";
            if (query.QueryData.Count > 0)
            {
                queryField = "and (";
                var i = 0;
                foreach (var item in query.QueryData)
                {
                    var field = fields.FirstOrDefault(u => u.TypeName == item.Key);
                    if (field == null) return default;
                    queryField += $" {item.Key} LIKE @{item.Key}Query ";
                    parameters.Add(new SqlParameter($"@{item.Key}Query", "%" + item.Value + "%"));
                    i++;
                    if (i < query.QueryData.Count)
                    {
                        queryField += " and ";
                    }
                }
                queryField += ")";
            }
            var from = CommonHelper.GetCavBaseType(type)?.Name;
            string sql = $"select top(20)* from {from} where IsDelete=0 " + queryField;
            if (query.StartTime != null)
            {
                sql += $" and CreatTime>=@StartTime ";
                parameters.Add(new SqlParameter("@StartTime", query.StartTime));
            }
            if (query.EndTime != null)
            {
                sql += $" and CreatTime<=@EndTime ";
                parameters.Add(new SqlParameter("@EndTime", query.EndTime));
            }
            var data = BC.DC.SqlQuery(sql, parameters.ToArray());
            var model = data.ToList<T>(type);
            return ModelToViewModel(model);
        }

        /// <summary>
        /// 数据转换
        /// 需要达到一个model转为viewModel效果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual List<ViewT> ModelToViewModel(List<T> model)
        {
            model.AToB(out List<ViewT> outModel);
            return outModel;
        }
    }
}
