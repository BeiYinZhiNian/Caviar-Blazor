using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Core
{
    public interface ITableTemplate
    {
        [Parameter]
        public string SubmitUrl { get; set; }
        public Task<bool> Validate();
    }
}
