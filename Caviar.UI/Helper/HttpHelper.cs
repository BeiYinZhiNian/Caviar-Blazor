using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using Caviar.Models.SystemData;
using Caviar.UI.Shared;
using AntDesign;

namespace Caviar.UI.Helper
{
    public partial class HttpHelper
    {
        NotificationService _notificationService;
        public HttpHelper(HttpClient http, NotificationService _notice)
        {
            Http = http;
            _notificationService = _notice;
        }
        public HttpClient Http { get; }
        public async Task<ResultMsg<T>> GetJson<T>(string address, EventCallback eventCallback = default)
        {
            var result = await HttpRequest<T>(address,"get",default,eventCallback);
            return result;
        }

        public async Task<ResultMsg> GetJson(string address, EventCallback eventCallback = default)
        {
            var result = await HttpRequest<object>(address, "get", default, eventCallback);
            return result;
        }

        public async Task<ResultMsg<T>> PostJson<T>(string address,T data, EventCallback eventCallback = default)
        {
            var result = await HttpRequest(address, "post", data, eventCallback);
            return result;
        }

        public async Task<ResultMsg> PostJson(string address, string data, EventCallback eventCallback = default)
        {
            var result = await HttpRequest<object>(address, "post", data, eventCallback);
            return result;
        }

        async Task<ResultMsg<T>> HttpRequest<T>(string address,string model,T data, EventCallback eventCallback)
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
                    var response = await Http.PostAsJsonAsync(address,data);
                    result = await response.Content.ReadFromJsonAsync<ResultMsg<T>>();
                }
            }
            catch(Exception e)
            {
                result = new ResultMsg<T>()
                {
                    Title = "请求失败，请检查网络",
                    Type = address,
                    Detail = e.Message,
                    Status = 400,
                };
            }
            mainLayoutStyle.Loading = false;
            await eventCallback.InvokeAsync(mainLayoutStyle);
            if (result.Status != 200)
            {
                string msg = "";
                if (!string.IsNullOrEmpty(result.Detail))
                {
                    msg += "错误详细信息：" + result.Detail;
                }
                if (result.Errors!=null && result.Errors.Count!=0)
                {
                    msg += "错误提示：";
                    foreach (var item in result.Errors)
                    {
                        msg += "<br>" + item.Key + ":" ;
                        foreach (var value in item.Value)
                        {
                            msg +=  value + "<br>";
                        }
                    }
                }
                _notificationService.Open(new NotificationConfig()
                {
                    Message = result.Title,
                    Description = msg,
                    NotificationType = NotificationType.Error
                });
            }
            return result;
        }
    }
}