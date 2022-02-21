using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System;
using AntDesign;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Net;
using Caviar.SharedKernel.Entities.View;
using Caviar.SharedKernel.Entities;

namespace Caviar.AntDesignUI.Core
{
    public partial class HttpService
    {
        NotificationService _notificationService;
        NavigationManager _navigationManager;
        MessageService _message;
        IJSRuntime _jSRuntime;
        public HttpService(HttpClient http, 
            NotificationService _notice,
            NavigationManager navigationManager, 
            MessageService message,
            IJSRuntime JsRuntime)
        {
            HttpClient = http;
            _notificationService = _notice;
            _navigationManager = navigationManager;
            _message = message;
            _jSRuntime = JsRuntime;
        }

        public HttpClient HttpClient { get; }
        public async Task<ResultMsg<T>> GetJson<T>(string address)
        {
            var result = await HttpRequest<T,T>(address,"get",default);
            return result;
        }

        public async Task<ResultMsg> GetJson(string address)
        {
            var result = await HttpRequest<object,object>(address, "get", default);
            return result;
        }

        public async Task<ResultMsg<T>> PostJson<K, T>(string address,K data)
        {
            var result = await HttpRequest<K,T>(address, "post", data);
            return result;
        }

        public async Task<ResultMsg> PostJson<K>(string address, K data)
        {
            var result = await HttpRequest<K, object>(address, "post", data);
            return result;
        }

        async Task<ResultMsg<T>> HttpRequest<K,T>(string address,string model, K data)
        {
            ResultMsg<T> result;
            try
            {
                HttpResponseMessage responseMessage;
                if (model.ToLower() == "get")
                {
                    responseMessage = await HttpClient.GetAsync(address);
                }
                else if(model.ToLower() == "post")
                {
                    responseMessage = await HttpClient.PostAsJsonAsync(address, data);
                }
                else
                {
                    throw new Exception("暂不支持的请求方法");
                }
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //当这里解析失败，又找不到具体原因，试试使用Newtonsoft.json进行解析
                    var resultMsg = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<ResultMsg<T>>(resultMsg);
                }
                else
                {
                    result = new ResultMsg<T>()
                    {
                        Title = "请求失败:" + responseMessage.ReasonPhrase,
                        Status = responseMessage.StatusCode,
                    };
                }
            }
            catch(Exception e)
            {
                result = new ResultMsg<T>()
                {
                    Title = "请求失败，发生请求错误",
                    Status = HttpStatusCode.BadRequest,
                    Detail = new System.Collections.Generic.Dictionary<string, string>()
                    {
                        { "请求错误信息", e.Message}
                    }
                };
            }
            Response(result);
            return result;
        }
        
        public void Response(ResultMsg result)
        {
            switch (result.Status)
            {
                case HttpStatusCode.OK://正确响应
                    break;
                case HttpStatusCode.Redirect://重定向
                    _ = _message.Warning(result.Title);
                    _navigationManager.NavigateTo(result.Url);
                    break;
                case HttpStatusCode.RedirectMethod://强制重定向
                    _ = _message.Warning(result.Title);
                    _navigationManager.NavigateTo(_jSRuntime,result.Url);
                    break;
                case HttpStatusCode.Unauthorized://权限不足
                case HttpStatusCode.InternalServerError://发生严重错误
                default:
                    string msg = "";
                    if (result.Detail!=null && result.Detail.Count>0)
                    {
                        foreach (var item in result.Detail)
                        {
                            msg += $"{item.Key}：{item.Value}<br>";
                        }
                    }
                    if (!string.IsNullOrEmpty(result.Url))
                    {
                        msg += $"<a target='_Blank' href='{result.Url}'>点击查看解决办法</a><br>";
                    }
                    _ = _notificationService.Open(new NotificationConfig()
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