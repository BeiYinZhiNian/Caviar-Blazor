using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Helper
{
    public class UserConfig
    {
        public Router Router;

        IEnumerable _routes;
        public IEnumerable Routes()
        {
            if (_routes == null)
            {
                var routes = Router.GetObjValue("Routes");
                _routes = (IEnumerable)routes.GetObjValue("Routes");
            }
            return _routes;
        }
        /// <summary>
        /// 手风琴模式
        /// </summary>
        public bool Accordion { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// 是否table页
        /// </summary>
        public bool IsTable { get; set; }

    }
}
