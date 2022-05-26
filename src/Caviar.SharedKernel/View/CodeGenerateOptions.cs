// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.ComponentModel;

namespace Caviar.SharedKernel.Entities.View
{
    public class CodeGenerateOptions
    {
        [DisplayName("IsGenerateIndex")]
        public bool IsGenerateIndex { get; set; } = true;
        [DisplayName("IsGenerateController")]
        public bool IsGenerateController { get; set; } = true;
        [DisplayName("IsGenerateDataTemplate")]
        public bool IsGenerateDataTemplate { get; set; } = true;
        [DisplayName("IsGenerateViewModel")]
        public bool IsGenerateViewModel { get; set; } = true;
        [DisplayName("实体")]
        public string EntitieName { get; set; }
        [DisplayName("命名空间")]
        public string FullName { get; set; }
        /// <summary>
        /// 是否覆盖
        /// </summary>
        [DisplayName("是否覆盖")]
        public bool IsCover { get; set; }
    }
}
