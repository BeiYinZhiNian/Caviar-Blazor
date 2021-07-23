using AntDesign;
using Caviar.AntDesignPages.Helper;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
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

        public List<ViewUserGroup> ViewUserGroups { get; set; }
        #endregion

        [Inject]
        public ViewUserToken UserToken { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (DataSource is IBaseModel)
            {
                var data = (IBaseModel)DataSource;
                var result = await Http.GetJson<List<ViewUserGroup>>("Permission/GetPermissionGroup");
                if (result.Status == HttpState.OK)
                {
                    ViewUserGroups = result.Data;
                }
                var list = ViewUserGroups?.ListToTree();
                if (data.Id == 0)
                {
                    data.Number = "999";
                    data.DataId = UserToken.UserGroupId == 0 ? null : UserToken.UserGroupId;
                }
                var userGroup = list?.FirstOrDefault(u => u.Id == data.DataId);
                if (userGroup != null) UserGroupName = userGroup.Name;
                await base.OnInitializedAsync();
            }
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
            var result = await Http.PostJson(Url, DataSource);
            if (result.Status == HttpState.OK)
            {
                Message.Success(SuccMsg);
                return true;
            }
            return false;
        }

        public string UserGroupName = "请选择部门";
        public void OnUserGroupCancel()
        {
            UserGroupName = "请选择部门";
            ((IBaseModel)DataSource).DataId = null;
        }

        public void OnUserGroupSelect(TreeEventArgs<ViewUserGroup> args)
        {
            UserGroupName = args.Node.Title;
            ((IBaseModel)DataSource).DataId = int.Parse(args.Node.Key);
        }
        #endregion
    }
}
