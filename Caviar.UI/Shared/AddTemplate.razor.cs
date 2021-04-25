using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.UI.Shared
{
    public partial class AddTemplate<TData>
    {

        public TData model { get; set; }


        [Parameter]
        public List<ViewModelHeader> ViewModelHeader { get; set; }
    }
}
