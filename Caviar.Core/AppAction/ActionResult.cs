using Caviar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.ModelAction
{
    public partial class ActionResult
    {

        #region 成功请求
        public ResultMsg Ok()
        {
            return new ResultMsg();
        }
        public ResultMsg Ok(string title)
        {
            return new ResultMsg() { Title = title };
        }
        public ResultMsg<K> Ok<K>(string title, K data)
        {
            return new ResultMsg<K>() { Title = title, Data = data };
        }
        public ResultMsg<K> Ok<K>(K data)
        {
            return new ResultMsg<K>() { Data = data };
        }

        #endregion

        #region 失败请求
        public ResultMsg Error(string title)
        {
            return new ResultMsg() { Status = HttpState.Error, Title = title, };
        }
        public ResultMsg Error(string title, string detail)
        {
            return new ResultMsg() { Status = HttpState.Error, Title = title, Detail = detail, };
        }

        public ResultMsg Error(string title, string detail, string uri)
        {
            return new ResultMsg() { Status = HttpState.Error, Detail = detail, Title = title, Uri = uri, };
        }

        public ResultMsg Error(string title, string detail, IDictionary<string, string> errors)
        {
            return new ResultMsg() { Status = HttpState.Error, Title = title, Detail = detail, Errors = errors, };
        }
        public ResultMsg<K> Error<K>(string title, K data = default)
        {
            return new ResultMsg<K>() { Status = HttpState.Error, Title = title, Data = data };
        }
        public ResultMsg<K> Error<K>(string title, string detail, K data = default)
        {
            return new ResultMsg<K>() { Status = HttpState.Error, Title = title, Detail = detail, Data = data };
        }

        public ResultMsg<K> Error<K>(string title, string detail, string uri, K data = default)
        {
            return new ResultMsg<K>() { Status = HttpState.Error, Detail = detail, Title = title, Uri = uri, Data = data };
        }

        public ResultMsg<K> Error<K>(string title, string detail, IDictionary<string, string> errors, K data = default)
        {
            return new ResultMsg<K>() { Status = HttpState.Error, Title = title, Detail = detail, Errors = errors, Data = data };
        }
        #endregion

        #region 其他
        /// <summary>
        /// 重定向
        /// </summary>
        /// <param name="title"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public virtual ResultMsg Redirect(string title, string uri)
        {
            return new ResultMsg() { Status = HttpState.Redirect, Title = title, Uri = uri };
        }
        /// <summary>
        /// 没有操作权限
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public virtual ResultMsg NoPermission(string title)
        {
            return new ResultMsg() { Title = title, Status = HttpState.NotPermission };
        }
        /// <summary>
        /// 没有授权，需要登录
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public virtual ResultMsg Unauthorized(string title,string uri)
        {
            return new ResultMsg() { Title = title, Status = HttpState.Unauthorized,Uri = uri };
        }
        #endregion
    }
}
