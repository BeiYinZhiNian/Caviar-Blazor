﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.Shared
{
    partial class NavMenu
    {
        [Parameter]
        public bool InlineCollapsed { get; set; }
    }
}
