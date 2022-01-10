using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Core
{
    public class CavNavigationManager : NavigationManager
    {
        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            base.NavigateToCore(uri, forceLoad);
        }
    }
}
