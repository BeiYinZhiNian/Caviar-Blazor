// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using AntDesign;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;

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
