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




    }
}
