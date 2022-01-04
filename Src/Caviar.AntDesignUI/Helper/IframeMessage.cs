using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI
{
    /// <summary>
    /// iframe与框架通信类
    /// </summary>
    public class IframeMessage
    {
        private NavigationManager _navigationManager;

        public IframeMessage(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            Ref = DotNetObjectReference.Create(this);
        }
        /// <summary>
        /// 调用的方法
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 传输的数据
        /// </summary>
        public object Data { get; set; }

        public DotNetObjectReference<IframeMessage> Ref { get; set; }

        [JSInvokable]
        public void JsNavigation()
        {
            Console.WriteLine("test");
            //_navigationManager.NavigateTo(url);
        }

    }
}
