// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace Caviar.SharedKernel.Entities
{
    public interface ILanguageService
    {
        CultureInfo CurrentCulture { get; }

        JObject Resources { get; }

        event EventHandler<CultureInfo> LanguageChanged;

        void SetLanguage(CultureInfo culture);

        void SetLanguage(string cultureName);

        void SetLanguage(ValueTask<string> cultureName);

        string this[string name] { get; }

        /// <summary>
        /// 获取支持的语言列表
        /// </summary>
        /// <returns></returns>
        List<(string CultureName, string ResourceName)> GetLanguageList();
    }
}
