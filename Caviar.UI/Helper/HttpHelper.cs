using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using Caviar.Models.SystemData;

namespace Caviar.UI.Helper
{
    public class HttpHelper
    {

        public HttpHelper(IConfiguration configuration, HttpClient http)
        {
            Configuration = configuration;
            Http = http;
            Console.WriteLine("注入：" + Http.BaseAddress);
        }
        IConfiguration Configuration;
        public HttpClient Http { get; }
        public async Task<T> GetJson<T>(string address)
        {
            var result = await HttpGet<T>(address);
            return result.data;
        }

        public async Task GetJson(string address)
        {
            var result = await HttpGet(address);
            return;
        }

        async Task<ResultMsg<T>> HttpGet<T>(string address)
        {
            var result = await Http.GetFromJsonAsync<ResultMsg<T>>(address);
            Console.WriteLine("请求消息：" + result.Title);
            Console.WriteLine("请求状态：" + result.Status);
            return result;
        }
        async Task<ResultMsg> HttpGet(string address)
        {
            var result = await Http.GetFromJsonAsync<ResultMsg>(address);
            Console.WriteLine("请求消息：" + result.Title);
            Console.WriteLine("请求状态：" + result.Status);
            return result;
        }
    }
}