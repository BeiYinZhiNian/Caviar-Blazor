using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Shared
{
    public partial class CavPopover
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        UserConfig UserConfig { get; set; }
        [Inject]
        CavModal CavModal { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }

        private string Runtime => System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier;
        bool IsSmallScreen = true;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var clientWidth = await JSRuntime.InvokeAsync<int>("getClientWidth");
            if (clientWidth < 576 != IsSmallScreen)
            {
                IsSmallScreen = clientWidth < 576;
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
        }


    }
}
