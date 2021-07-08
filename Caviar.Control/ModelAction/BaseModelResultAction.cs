using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.ModelAction
{
    public partial class BaseModelAction <T, ViewT>
    {
        #region 成功请求
        public ResultMsg Ok()
        {
            return new ResultMsg();
        }
        public ResultMsg Ok(string title)
        {
            return new ResultMsg() { Title = title};
        }
        public ResultMsg<K> Ok<K>(string title,K data)
        {
            return new ResultMsg<K>() { Title = title,Data = data };
        }
        public ResultMsg<K> Ok<K>(K data)
        {
            return new ResultMsg<K>() { Data = data };
        }

        #endregion

        #region 失败请求
        public ResultMsg Error(string title)
        {
            return new ResultMsg() { Status = 406, Title = title };
        }
        public ResultMsg Error(string title,string detail)
        {
            return new ResultMsg() { Status = 406, Title = title ,Detail = detail};
        }
        public ResultMsg Error(string title, string detail, string uri)
        {
            return new ResultMsg() { Status = 406, Detail = detail, Title = title, Uri = uri };
        }
        public ResultMsg Error(string title, string detail,IDictionary<string,string> errors)
        {
            return new ResultMsg() { Status = 406, Title = title, Detail = detail ,Errors = errors};
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
            return new ResultMsg() { Status = 302, Title = title, Uri= uri };
        }
        /// <summary>
        /// 没有操作权限
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public virtual ResultMsg NoPermission(string title)
        {
            return new ResultMsg() { Title = title, Status = 403 };
        }

        public virtual ResultMsg Unauthorized(string title)
        {
            return new ResultMsg() { Title = title, Status = 401 };
        }
        #endregion
    }
}
