using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.UI.Shared
{
    partial class EmptyLayout
    {
        [Parameter]
        public string Style { get; set; }

        [Inject]
        IConfiguration Configuration { get; set; }

        public string BackgroundImage { get; set; }

        protected override void OnInitialized()
        {
            BackgroundImage = Configuration["Background:Image"];
            Style = $"min-height:100vh;background-image: url({BackgroundImage});";
            base.OnInitialized();
        }
    }
}
