using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using Caviar.Models.SystemData;
using Caviar.UI.Shared;
using AntDesign;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Web;

namespace Caviar.UI.Helper
{
    public partial class HttpHelper
    {
        NotificationService _notificationService;
        UserToken _userToken;
        NavigationManager _navigationManager;
        MessageService _message;
        public HttpHelper(HttpClient http, NotificationService _notice, UserToken userToken, NavigationManager navigationManager, MessageService message)
        {
            Http = http;
            _notificationService = _notice;
            _userToken = userToken;
            _navigationManager = navigationManager;
            _message = message;
            var tokenName = "UsreToken";
            Http.DefaultRequestHeaders.TryGetValues(tokenName, out IEnumerable<string>? values);
            if(Http.DefaultRequestHeaders.Contains(tokenName))
            {
                Http.DefaultRequestHeaders.Remove(tokenName);
            }
            if (userToken.Token != "" && userToken.Id > 0)
            {
                var json = JsonConvert.SerializeObject(userToken);
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
                Http.DefaultRequestHeaders.Add(tokenName, base64);
            }
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

        public async Task<ResultMsg> PostJson(string address, string data, EventCallback eventCallback = default)
        {
            var result = await HttpRequest<object, object>(address, "post", data, eventCallback);
            return result;
        }

        async Task<ResultMsg<T>> HttpRequest<K,T>(string address,string model, K data, EventCallback eventCallback)
        {
            ResultMsg<T> result = default;
            var mainLayoutStyle = new MainLayoutStyle() { Loading = true };
            await eventCallback.InvokeAsync(mainLayoutStyle);
            try
            {
                if (model.ToLower() == "get")
                {
                    result = await Http.GetFromJsonAsync<ResultMsg<T>>(address);
                }
                else
                {
                    var response = await Http.PostAsJsonAsync(address, data);
                    result = await response.Content.ReadFromJsonAsync<ResultMsg<T>>();
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
            switch (result.Status)
            {
                case 200:
                    break;
                case 401:
                    _navigationManager.NavigateTo("/User/Login");
                    _message.Warn(result.Title);
                    break;
                case 403:
                    _navigationManager.NavigateTo("/Exception/403");
                    _message.Warn(result.Title);
                    break;
                case 404:
                    _navigationManager.NavigateTo("/Exception/404");
                    _message.Warn(result.Title);
                    break;
                case 400:
                    _navigationManager.NavigateTo("/Exception/400");
                    _message.Warn(result.Title);
                    break;
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
                    #pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                    _notificationService.Open(new NotificationConfig()
                    {
                        Message = result.Title,
                        Description = msg,
                        NotificationType = NotificationType.Error
                    });
                    #pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                    break;
            }
            return result;
        }
    }
}