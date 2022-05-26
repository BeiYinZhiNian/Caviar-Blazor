// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;

namespace Caviar.SharedKernel.Entities.View
{
    public partial class FieldsView : BaseView<SysFields>
    {
        /// <summary>
        /// 类型名称如string,int等
        /// </summary>
        public string EntityType { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 是否为枚举
        /// </summary>
        public bool IsEnum { get; set; }
        /// <summary>
        /// 实体命名空间
        /// </summary>
        public string EntityNamespace { get; set; }
        /// <summary>
        /// 枚举 值-名称
        /// </summary>
        public Dictionary<int, string> EnumValueName { get; set; }
        /// <summary>
        /// 是否授权
        /// </summary>
        public bool IsPermission { get; set; }

    }
}
