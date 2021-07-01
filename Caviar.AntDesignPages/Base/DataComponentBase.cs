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
    public partial class DataComponentBase<ViewT> : CavComponentBase,ITableTemplate where ViewT : class, IViewMode, IBaseModel, new()
    {
        #region 参数
        [Parameter]
        public ViewT DataSource { get; set; } = new ViewT() { Number = "999" };

        [Parameter]
        public string Url { get; set; }

        [Parameter]
        public string SuccMsg { get; set; } = "操作成功";
        #endregion

        #region 回调
        public Form<ViewT> _meunForm;


        public async Task<bool> Submit()
        {
            //数据效验
            if (_meunForm.Validate())
            {
                return await FormSubmit();
            }
            return false;
        }

        public async Task<bool> FormSubmit()
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
