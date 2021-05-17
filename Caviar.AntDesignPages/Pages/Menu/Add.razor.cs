using AntDesign;
using Caviar.AntDesignPages.Helper;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.Menu
{
    public partial class Add : ITableTemplate
    {
        [Inject]
        HttpHelper Http { get; set; }

        DataTemplate Template { get; set; }

        public async Task<bool> Submit()
        {
            return await Template.Submit();
        }
    }
}
