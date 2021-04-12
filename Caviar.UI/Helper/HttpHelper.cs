using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using Caviar.Models.SystemData;
using Caviar.UI.Shared;

namespace Caviar.UI.Helper
{
    public partial class HttpHelper
    {

        public HttpHelper(HttpClient http)
        {
            Http = http;
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
                    result = response.Content.ReadFromJsonAsync<ResultMsg<T>>().Result;
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
            return result;
        }
    }
}