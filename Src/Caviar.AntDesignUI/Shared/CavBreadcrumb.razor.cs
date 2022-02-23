using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Caviar.AntDesignUI.Shared
{
    partial class CavBreadcrumb
    {
        [Parameter]
        public string[] BreadcrumbItemArr { get; set; }
    }
}
