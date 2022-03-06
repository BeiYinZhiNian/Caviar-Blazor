using AntDesign;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Caviar.SharedKernel.Entities.View;
using Caviar.SharedKernel.Entities;

namespace Caviar.AntDesignUI.Core
{
    public partial class DataComponentBase<ViewT,T> : CavComponentBase,ITableTemplate where ViewT:IView<T>,new() where T: IUseEntity,new()
    {
        #region 参数
        [Parameter]
        public ViewT DataSource { get; set; } = new ViewT()
        {
            Entity = new T()
        };
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

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
            var result = await HttpService.PostJson(SubmitUrl, DataSource);
            if (result.Status == HttpStatusCode.OK)
            {
                _ = MessageService.Success(result.Title);
                return true;
            }
            return false;
        }

        #endregion
    }
}