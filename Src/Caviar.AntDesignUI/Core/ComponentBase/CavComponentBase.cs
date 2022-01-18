using AntDesign;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Core
{
    public partial class CavComponentBase : ComponentBase
    {
        #region 属性注入
        /// <summary>
        /// HttpClient
        /// </summary>
        [Inject]
        protected HttpService HttpService { get; set; }
        /// <summary>
        /// 全局提示
        /// </summary>
        [Inject]
        protected MessageService MessageService { get; set; }
        /// <summary>
        /// 导航管理器
        /// </summary>
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// API组
        /// </summary>
        [Parameter]
        public List<SysMenuView> APIList { get; set; } = new List<SysMenuView>();
        /// <summary>
        /// url读取器
        /// </summary>
        protected UrlAccessor Url { get; set; }
        /// <summary>
        /// 需要获取url的控制器集合
        /// </summary>
        public List<string> ControllerList = new List<string>();
        /// <summary>
        /// 加载等待
        /// </summary>
        public bool Loading = false;
        /// <summary>
        /// 当前url
        /// </summary>
        [Parameter]
        public string CurrentUrl { get; set; }

        [Inject]
        public UserConfig UserConfig { get; set; }

        public ILanguageService LanguageService => UserConfig.LanguageService;
        #endregion

        /// <summary>
        /// 获取API
        /// 获取该页面下的API
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<List<SysMenuView>> GetApiList()
        {
            var splicing = "";
            foreach (var item in ControllerList)
            {
                splicing += item + "|";
            }
            var uri = new Uri(NavigationManager.Uri);
            var result = await HttpService.GetJson<List<SysMenuView>>($"{UrlConfig.GetApiList}?url={uri.LocalPath}&splicing={splicing}");
            if (result.Status != HttpStatusCode.OK) return null;
            return result.Data;
        }

        protected override void OnParametersSet()
        {
            UserConfig.RefreshCurrentPage = Refresh;
            base.OnParametersSet();
        }

        protected override async Task OnInitializedAsync()
        {
            Loading = true;
            if (string.IsNullOrEmpty(CurrentUrl))
            {
                CurrentUrl = NavigationManager.Uri.Replace(NavigationManager.BaseUri, "");
            }
            APIList = await GetApiList();
            Url = new UrlAccessor(APIList);
            Loading = false;
            await base.OnInitializedAsync();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <returns></returns>
        public virtual async void Refresh()
        {
            await OnInitializedAsync();
            StateHasChanged();
        }

        public RadioOption<T>[] GetRadioOptions<T>()
        {
            var enumDic = CommonHelper.GetEnumModelHeader<T>(typeof(T), LanguageService);
            List<RadioOption<T>> radioOptions = new List<RadioOption<T>>();
            foreach (var item in enumDic)
            {
                radioOptions.Add(new RadioOption<T>() { Value = item.Key, Label = item.Value });
            }
            return radioOptions.ToArray();
        }
    }

    public class UrlAccessor
    {
        public UrlAccessor(List<SysMenuView> apiList)
        {
            APIList = apiList;
        }

        public List<SysMenuView> APIList { get; set; }


        public string this[string name] { 
            get 
            {
                var url = APIList?.FirstOrDefault(u => u.Entity.Key.ToLower() == name.ToLower())?.Entity.Url;
                return url; 
            } 
        }

        public string this[string name, string controller]
        {
            get
            {
                var url = APIList?.SingleOrDefault(u => u.Entity.Key.ToLower() == name.ToLower() && u.Entity.ControllerName.ToLower() == controller.ToLower())?.Entity.Url;
                return url;
            }
        }
    }
}
