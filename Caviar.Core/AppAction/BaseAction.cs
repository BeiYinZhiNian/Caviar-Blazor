using Caviar.SharedKernel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.ModelAction
{
    public partial class BaseAction<ViewT> : ActionResult, IBaseAction<ViewT>   where ViewT: class,IView, new()
    {
        public IInteractor Interactor { get; set; }

        public ResultMsg ResultMsg { get; set; } = new ResultMsg();
        public virtual async Task<ResultMsg> AddEntity(ViewT entity)
        {
            try
            {
                var count = await Interactor.DbContext.AddEntityAsync(entity);
                if (count > 0)
                {
                    return Ok();
                }
                throw new Exception("添加失败，添加结果：" + count);
            }
            catch(Exception e)
            {
                return Error("添加数据失败", e.Message);
            }
        }
       
        /// <summary>
        /// 删除指定实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultMsg> DeleteEntity(ViewT entity)
        {
            try
            {
                if (entity.Uid == CaviarConfig.SysAdminRoleGuid)
                {
                    return Error("不能删除管理员角色");
                }
                else if(entity.Uid == CaviarConfig.NoLoginRoleGuid)
                {
                    return Error("不能默认用户组");
                }
                else if (entity.Uid == CaviarConfig.UserAdminGuid)
                {
                    return Error("不能删除默认用户");
                }
                var count = await Interactor.DbContext.DeleteEntityAsync(entity);
                if (count > 0)
                {
                    return Ok();
                }
                return Error("删除失败，删除结果：" + count);
            }
            catch (Exception e)
            {
                return Error("删除数据失败", e.Message);
            }
        }
        
        /// <summary>
        /// 修改指定实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultMsg> UpdateEntity(ViewT entity)
        {
            try
            {
                var count = await Interactor.DbContext.UpdateEntityAsync(entity);
                if (count > 0)
                {
                    return Ok();
                }
                throw new Exception("修改失败，删除结果：" + count);
            }
            catch (Exception e)
            {
                return Error("修改数据失败", e.Message);
            }
        }
        
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultMsg<PageData<ViewT>>> GetPages(Expression<Func<ViewT, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true)
        {
            var pages = await Interactor.DbContext.GetPageAsync(where, u => u.Number, pageIndex, pageSize, isOrder, isNoTracking);
            return Ok(pages);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<ResultMsg> DeleteEntity(List<ViewT> menus)
        {
            try
            {
                var count = await Interactor.DbContext.DeleteEntityAsync(menus);
                if (count > 0)
                {
                    return Ok();
                }
                throw new Exception("删除失败，删除结果：" + count);
            }
            catch (Exception e)
            {
                return Error("删除数据失败", e.Message);
            }
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<ResultMsg> UpdateEntity(List<ViewT> menus)
        {
            try
            {
                var count = await Interactor.DbContext.UpdateEntityAsync(menus);
                if (count > 0)
                {
                    return Ok();
                }
                throw new Exception("修改失败，删除结果：" + count);
            }
            catch (Exception e)
            {
                return Error("修改数据失败", e.Message);
            }
        }
        /// <summary>
        /// 通用模糊查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual ResultMsg<PageData<ViewT>> FuzzyQuery(ViewQuery query)
        {
            var fields = Interactor.UserData.ModelFields.Where(u => u.BaseTypeName == typeof(ViewT).Name).ToList();
            if (fields == null) return Error<PageData<ViewT>>("没有对该对象的查询权限");
            var assemblyList = CommonlyHelper.GetAssembly();
            Type type = null;
            foreach (var item in assemblyList)
            {
                type = item.GetTypes().SingleOrDefault(u => u.Name.ToLower() == typeof(ViewT).Name.ToLower());
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
            var from = CommonlyHelper.GetCavBaseType(type)?.Name;
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
            var data = Interactor.DbContext.SqlQuery(sql, parameters.ToArray());
            var model = data.ToList<ViewT>(type);
            var pages = new PageData<ViewT>(model);
            pages.Total = model.Count;
            pages.PageSize = model.Count;
            return Ok(pages);
        }

        /// <summary>
        /// 数据转换
        /// 需要达到一个model转为viewModel效果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual List<ViewT> ToViewModel(List<ViewT> model)
        {
            model.AToB(out List<ViewT> outModel);
            return outModel;
        }

        public virtual ViewT ToViewModel(ViewT model)
        {
            var list = ToViewModel(new List<ViewT>() { model });
            return list.FirstOrDefault();
        }

        public virtual async Task<ResultMsg<ViewT>> GetEntity(Guid guid)
        {
            var entity = await Interactor.DbContext.GetEntityAsync<ViewT>(guid);
            var viewModel = ToViewModel(entity);
            return Ok(viewModel);
        }

        public virtual async Task<ResultMsg<ViewT>> GetEntity(int id)
        {
            var entity = await Interactor.DbContext.GetEntityAsync<ViewT>(id);
            var viewModel = ToViewModel(entity);
            return Ok(viewModel);
        }
    }
}
