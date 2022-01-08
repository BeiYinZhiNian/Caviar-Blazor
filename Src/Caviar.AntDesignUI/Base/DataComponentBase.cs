using AntDesign;
using Caviar.AntDesignUI.Helper;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities.View;
using System.Net;

namespace Caviar.AntDesignUI
{
    public partial class DataComponentBase<ViewT,T> : CavComponentBase,ITableTemplate where ViewT:IView<T>,new() where T: IBaseEntity,new()
    {
        #region 参数
        [Parameter]
        public ViewT DataSource { get; set; } = new ViewT()
        {
            Entity = new T()
        };


        [Parameter]
        public string SuccMsg { get; set; } = "操作成功";
        #endregion


        #region 回调
        public Form<ViewT> _meunForm;
        /// <summary>
        /// 提交前本地校验
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> Validate()
        {
            if (_meunForm == null)//当_meunForm为null时，不进行表单验证
            {
                return await FormSubmit();
            }
            //数据效验
            if (_meunForm.Validate())
            {
                return await FormSubmit();
            }
            return false;
        }
        /// <summary>
        /// 开始表单提交
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> FormSubmit()
        {
            var result = await HttpService.PostJson(CurrentUrl, DataSource);
            if (result.Status == HttpStatusCode.OK)
            {
                await MessageService.Success(SuccMsg);
                return true;
            }
            return false;
        }

        #endregion
    }
}