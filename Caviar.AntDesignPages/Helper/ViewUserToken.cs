using Caviar.Models.SystemData;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Caviar.AntDesignPages.Helper
{
    public partial class ViewUserToken:UserToken
    {
        IJSRuntime _JSRuntime;
        public ViewUserToken(IJSRuntime JsRuntime)
        {
            _JSRuntime = JsRuntime;
            GetUserToken();
        }

        public async Task GetUserToken()
        {
            var cookie = await _JSRuntime.InvokeAsync<string>("getCookie", Config.CookieName);
            if (!string.IsNullOrEmpty(cookie))
            {
                string base64 = HttpUtility.UrlDecode(cookie);
                string json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                var token = JsonSerializer.Deserialize<UserToken>(json);
                this.AutoAssign(token);
            }
        } 
    }
}
