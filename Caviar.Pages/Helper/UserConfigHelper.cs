using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.Pages.Helper
{
    public class UserConfigHelper
    {
        public int CurrentMenuId { get; set; }

        public Router Router { get; set; }


        IEnumerable _routes;
        public IEnumerable Routes
        {
            get
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
}
