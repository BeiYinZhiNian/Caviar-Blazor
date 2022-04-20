using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Shared
{
    public class CavEnumSelect<TEnum> : EnumSelect<TEnum>
    {
        [Inject]
        ILanguageService LanguageService { get; set; }
        protected override string GetLabel(TEnum item)
        {
            return LanguageService[$"{CurrencyConstant.Enum}.{item}"];
        }
    }
}
