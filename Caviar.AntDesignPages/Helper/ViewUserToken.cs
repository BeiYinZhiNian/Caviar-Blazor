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

        public ViewUserToken()
        {

        }

        public async Task GetUserToken()
        {
            var cookie = await _JSRuntime.InvokeAsync<string>("getCookie", Config.CookieName);
            if (!string.IsNullOrEmpty(cookie))
            {
                var json = CommonHelper.UrlBase64Handle(cookie);
                var token = JsonSerializer.Deserialize<UserToken>(json);
                this.AutoAssign(token);
            }
        } 
    }
}
