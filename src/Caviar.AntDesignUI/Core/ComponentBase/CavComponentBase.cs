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
    public partial class CavComponentBase : ComponentBase,IReuseTabsPage
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
        public List<SysMenuView> APIList { get; set; }
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
        public bool Loading { get; set; }
        /// <summary>
        /// 页面最大数量
        /// </summary>
        public int MaxPageSize { get; set; } = 9999;
        /// <summary>
        /// 当前控制器
        /// </summary>
        [Parameter]
        public string ControllerName 
        {
            get
            {
                return _controllerName;
            }
            set 
            {
                if (value.Contains('/')) // 带有url进行分类
                {
                    var split = value.Split('/');
                    if (split.Length > 0)
                    {
                        _controllerName = split[0];
                    }
                }
                else
                {
                    _controllerName = value;
                }
            }
        }

        [Parameter]
        public string SubmitUrl { get; set; }

        string _currentUrl;

        string CurrentUrl { 
            get 
            { 
                if(_currentUrl == null)
                {
                    _currentUrl = NavigationManager.Uri;
                }
                return _currentUrl;
            } 
        }

        string _controllerName;

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
            if (!string.IsNullOrEmpty(ControllerName)) // 不需要注入url
            {
                var result = await HttpService.GetJson<List<SysMenuView>>($"{UrlConfig.GetApis}?controllerName={ControllerName}&splicing={splicing}");
                if (result.Status != HttpStatusCode.OK) return null;
                return result.Data;
            }
            return new List<SysMenuView>();
        }

        protected override void OnParametersSet()
        {
            UserConfig.RefreshCurrentPage = Refresh;
            base.OnParametersSet();
        }

        protected override async Task OnInitializedAsync()
        {
            if (ControllerName == null)
            {
                ControllerName = CurrentUrl.Replace(NavigationManager.BaseUri, "");
            }
            if (string.IsNullOrEmpty(SubmitUrl))
            {
                var uri = new Uri(NavigationManager.Uri);
                SubmitUrl = uri.LocalPath.Substring(1);
            }
            APIList = await GetApiList();
            Url = new UrlAccessor(APIList);
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

        public virtual RenderFragment GetPageTitle() => builder =>
        {
            var uri = new Uri(CurrentUrl);
            var absolutePath = uri.AbsolutePath;
            if (absolutePath[0] == '/') absolutePath = absolutePath[1..];
            var menu = UserConfig.Menus.FirstOrDefault(u => u.Entity.Url == absolutePath);
            if(menu != null)
            {
                var index = 0;
                builder.OpenElement(index++, "div");
                if (!string.IsNullOrEmpty(menu.Entity.Icon))
                {
                    IEnumerable<KeyValuePair<string, object>> paramenter = new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("Type",menu.Entity.Icon)
                    };

                    builder.OpenComponent(index++, typeof(Icon));
                    builder.AddMultipleAttributes(index++, paramenter);
                    builder.CloseComponent();
                }
                builder.AddMarkupContent(index++, LanguageService[$"{CurrencyConstant.Menu}.{menu.Entity.Key}"]);
                builder.CloseElement();
            }
            else
            {
                var index = 0;
                builder.OpenElement(index++, "div");
                builder.AddMarkupContent(index++, absolutePath);
                builder.CloseElement();
            }
        };
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
                var url = APIList?.FirstOrDefault(u => u.Entity.Key.ToLower() == name.ToLower() && u.Entity.ControllerName.ToLower() == controller.ToLower())?.Entity.Url;
                return url;
            }
        }
    }
}
