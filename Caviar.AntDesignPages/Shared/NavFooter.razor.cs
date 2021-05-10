using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Shared
{
    partial class NavFooter
    {
        [Parameter]
        public string Style { get; set; } = "text-align: center;";
    }
}
