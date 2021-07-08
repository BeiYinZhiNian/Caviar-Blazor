using AntDesign;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages
{
    public partial class DataComponentBase<ViewT> : CavComponentBase,ITableTemplate where ViewT : class, new()
    {
        #region 参数
        [Parameter]
        public ViewT DataSource { get; set; } = new ViewT();

        [Parameter]
        public string Url { get; set; }

        [Parameter]
        public string SuccMsg { get; set; } = "操作成功";
        #endregion

        protected override Task OnInitializedAsync()
        {
            if(DataSource is IBaseModel)
            {
                ((IBaseModel)DataSource).Number = "999";
            }
            return base.OnInitializedAsync();
        }


        #region 回调
        public Form<ViewT> _meunForm;
        /// <summary>
        /// 提交前本地校验
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> Validate()
        {
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
            var result = await Http.PostJson(Url, DataSource);
            if (result.Status == 200)
            {
                Message.Success(SuccMsg);
                return true;
            }
            return false;
        }
        #endregion
    }
}
