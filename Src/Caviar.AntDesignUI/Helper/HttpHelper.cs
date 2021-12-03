using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System;
using AntDesign;
using System.Collections.Generic;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Http;
using Caviar.SharedKernel;

namespace Caviar.AntDesignUI.Helper
{
    public partial class HttpHelper
    {
        NotificationService _notificationService;
        NavigationManager _navigationManager;
        MessageService _message;
        IJSRuntime _jSRuntime;
        public string TokenName => CurrencyConstant.Authorization;
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
                Http.DefaultRequestHeaders.TryGetValues(TokenName, out IEnumerable<string> values);
                if (Http.DefaultRequestHeaders.Contains(TokenName))
                {
                    Http.DefaultRequestHeaders.Remove(TokenName);
                }
                if (!string.IsNullOrEmpty(cookie))
                {
                    Http.DefaultRequestHeaders.Add(TokenName,CurrencyConstant.JWT + cookie);
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
            try
            {
                HttpResponseMessage responseMessage;
                if (model.ToLower() == "get")
                {
                    responseMessage = await Http.GetAsync(address);
                }
                else if(model.ToLower() == "post")
                {
                    responseMessage = await Http.PostAsJsonAsync(address, data);
                }
                else
                {
                    throw new Exception("暂不支持的请求方法");
                }
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //当这里解析失败，又找不到具体原因，试试使用Newtonsoft.json进行解析
                    result = await responseMessage.Content.ReadFromJsonAsync<ResultMsg<T>>();
                }
                else
                {
                    result = new ResultMsg<T>()
                    {
                        Title = "请求失败:" + responseMessage.ReasonPhrase,
                        Status = (int)responseMessage.StatusCode,
                    };
                }
            }
            catch(Exception e)
            {
                result = new ResultMsg<T>()
                {
                    Title = "请求失败，发生请求错误",
                    Detail = e.Message,
                    Status = 500,
                };
            }
            Response(result);
            return result;
        }
        
        public async void Response(ResultMsg result)
        {
            switch (result.Status)
            {
                case StatusCodes.Status200OK://正确响应
                    break;
                case StatusCodes.Status307TemporaryRedirect://重定向专用
                    _navigationManager.NavigateTo(result.Title);
                    break;
                case StatusCodes.Status401Unauthorized://退出登录
                    await _jSRuntime.InvokeVoidAsync("delCookie", Config.CookieName);
                    IsSetCookie = false;
                    _navigationManager.NavigateTo(result.Title);
                    break;
                case StatusCodes.Status404NotFound:
                    _navigationManager.NavigateTo("/Exception/404");
                    break;
                case StatusCodes.Status500InternalServerError://发生严重错误
                default:
                    string msg = "";
                    if (!string.IsNullOrEmpty(result.Detail))
                    {
                        msg += "错误详细信息：" + result.Detail + "<br>";
                    }
                    if (!string.IsNullOrEmpty(result.Url))
                    {
                        msg += $"<a target='_Blank' href='{result.Url}'>点击查看解决办法</a><br>";
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


    }
}