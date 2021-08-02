using AntDesign;
using Caviar.AntDesignUI.Helper;
using Caviar.SharedKernel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.PersonalCenter
{
    public partial class Index
    {
        public ViewUser UserData { get; set; }
        [Inject]
        public HttpHelper Http { get; set; }
        [Inject]
        CavModal CavModal { get; set; }
        void OnClick(string icon)
        {
            Console.WriteLine($"icon {icon} is clicked");
        }

        async void UpdateUserData()
        {
            Dictionary<string,object> paramenter = new Dictionary<string, object>();
            //因为引用类型，这里进行一次转换，相当于深度复制
            //否则更改内容然后取消，列表会发生改变
            UserData.AToB(out ViewUser dataSource);
            paramenter.Add("DataSource", dataSource);
            paramenter.Add(CurrencyConstant.CavModelUrl, "User/MyDetails");
            await CavModal.Create("User/MyDetailsUpdate", "修改信息", HandleOk, paramenter);

        }

        async void HandleOk()
        {
            await OnInitializedAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var result = await Http.GetJson<ViewUser>("User/MyDetails");
            if (result.Status != HttpState.OK) return;
            UserData = result.Data;
            StateHasChanged();
        }


    }
}
