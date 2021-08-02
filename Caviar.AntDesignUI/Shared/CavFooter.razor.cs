using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Shared
{
    partial class CavFooter
    {
        [Parameter]
        public string Style { get; set; } = "text-align: center;";
    }
}
