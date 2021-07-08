using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using Caviar.Models.SystemData;
using Caviar.AntDesignPages.Shared;
using AntDesign;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Web;
using Microsoft.JSInterop;

namespace Caviar.AntDesignPages.Helper
{
    public partial class HttpHelper
    {
        NotificationService _notificationService;
        NavigationManager _navigationManager;
        MessageService _message;
        IJSRuntime _jSRuntime;
        public string TokenName => "UsreToken";
        public HttpHelper(HttpClient http, NotificationService _notice,NavigationManager navigationManager, MessageService message,IJSRuntime JsRuntime)
        {
            Http = http;
            _notificationService = _notice;
            _navigationManager = navigationManager;
            _message = message;
            _jSRuntime = JsRuntime;
        }

        static object cookiesOb = new object();
        /// <summary>
        /// 设置为false 用于更新cookies
        /// </summary>
        public bool IsSetCookie { get; set; } = false;
        async Task SetCookies()
        {
            if (IsSetCookie) return;
            var cookie = await GetToken();
            //这里为什么要用双锁呢，被逼的~
            lock (cookiesOb)
            {
                if (IsSetCookie) return;
                Http.DefaultRequestHeaders.TryGetValues(TokenName, out IEnumerable<string>? values);
                if (Http.DefaultRequestHeaders.Contains(TokenName))
                {
                    Http.DefaultRequestHeaders.Remove(TokenName);
                }
                if (!string.IsNullOrEmpty(cookie))
                {
                    Http.DefaultRequestHeaders.Add(TokenName, cookie);
                    IsSetCookie = true;
                }
            }
            
        }

        public async Task<string> GetToken()
        {
            var cookie = await _jSRuntime.InvokeAsync<string>("getCookie", Config.CookieName);
            return cookie;
        }


        public HttpClient Http { get; }
        public async Task<ResultMsg<T>> GetJson<T>(string address, EventCallback eventCallback = default)
        {
            var result = await HttpRequest<T,T>(address,"get",default,eventCallback);
            return result;
        }

        public async Task<ResultMsg> GetJson(string address, EventCallback eventCallback = default)
        {
            var result = await HttpRequest<object,object>(address, "get", default, eventCallback);
            return result;
        }

        public async Task<ResultMsg<T>> PostJson<K, T>(string address,K data, EventCallback eventCallback = default)
        {
            var result = await HttpRequest<K,T>(address, "post", data, eventCallback);
            return result;
        }

        public async Task<ResultMsg> PostJson<K>(string address, K data, EventCallback eventCallback = default)
        {
            var result = await HttpRequest<K, object>(address, "post", data, eventCallback);
            return result;
        }

        async Task<ResultMsg<T>> HttpRequest<K,T>(string address,string model, K data, EventCallback eventCallback)
        {
            await SetCookies();
            ResultMsg<T> result = default;
            var mainLayoutStyle = new MainLayoutStyle() { Loading = true };
            await eventCallback.InvokeAsync(mainLayoutStyle);
            try
            {
                if (model.ToLower() == "get")
                {
                    result = await Http.GetFromJsonAsync<ResultMsg<T>>(address);
                }
                else if(model.ToLower() == "post")
                {
                    var response = await Http.PostAsJsonAsync(address, data);
                    result = await response.Content.ReadFromJsonAsync<ResultMsg<T>>();
                }
                else
                {
                    throw new Exception("暂不支持的请求方法");
                }
            }
            catch(Exception e)
            {
                result = new ResultMsg<T>()
                {
                    Title = "请求失败，发生请求错误",
                    Type = "http://www.baidu.com/s?wd=" + HttpUtility.UrlEncode(e.Message),
                    Detail = e.Message,
                    Status = 500,
                };
            }
            mainLayoutStyle.Loading = false;
            await eventCallback.InvokeAsync(mainLayoutStyle);
            Response(result);
            Tips(result);
            return result;
        }
        
        public async void Response(ResultMsg result)
        {
            switch (result.Status)
            {
                case 200://正确响应
                    break;
                case 302://重定向专用
                    _navigationManager.NavigateTo(result.Type);
                    break;
                case 401://退出登录
                    await _jSRuntime.InvokeVoidAsync("delCookie", Config.CookieName);
                    IsSetCookie = false;
                    _navigationManager.NavigateTo(result.Type);
                    break;
                case 403://只用于提示
                case 406:
                    break;
                case 404:
                    _navigationManager.NavigateTo("/Exception/404");
                    break;
                case 500://发生严重错误
                default:
                    string msg = "";
                    if (!string.IsNullOrEmpty(result.Detail))
                    {
                        msg += "错误详细信息：" + result.Detail + "<br>";
                    }
                    if (result.Errors != null && result.Errors.Count != 0)
                    {
                        msg += "错误提示：";
                        foreach (var item in result.Errors)
                        {
                            msg += "<br>" + item.Key + ":";
                            foreach (var value in item.Value)
                            {
                                msg += value + "<br>";
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(result.Type))
                    {
                        msg += $"<a target='_Blank' href='{result.Type}'>点击查看解决办法</a><br>";
                    }
                    await _notificationService.Open(new NotificationConfig()
                    {
                        Message = result.Title,
                        Description = msg,
                        NotificationType = NotificationType.Error
                    });
                    break;
            }
        }


        void Tips(ResultMsg result)
        {
            if (result.IsTips && result.Status!=200)
            {
                _message.Warn($"{result.Title} 状态码:{result.Status}");
            }
        }


    }
}