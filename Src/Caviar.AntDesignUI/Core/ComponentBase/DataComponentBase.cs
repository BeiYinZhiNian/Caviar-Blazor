using AntDesign;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities.View;
using System.Net;

namespace Caviar.AntDesignUI.Core
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

        /// <summary>
        /// 模型字段
        /// </summary>
        protected List<ViewFields> ViewFields { get; set; } = new List<ViewFields>();
        #endregion

        /// <summary>
        /// 获取模型字段
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<List<ViewFields>> GetModelFields()
        {
            var result = await HttpService.GetJson<List<ViewFields>>(Url["GetFields"]);
            if (result.Status != HttpStatusCode.OK) return null;
            return result.Data;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var fieldsTask = GetModelFields();//获取模型字段
            ViewFields = await fieldsTask;
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