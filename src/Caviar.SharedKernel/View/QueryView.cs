// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Caviar.SharedKernel.Entities.View
{
    public class QueryView
    {
        /// <summary>
        /// 查询条数
        /// </summary>
        public uint Number { get; set; } = 20;
        /// <summary>
        /// 查询的数据
        /// </summary>
        public Dictionary<Guid, QueryModel> QueryModels { get; set; }
    }

    public class ComponentStatus
    {
        public bool AndOr { get; set; } = true;
        public FieldsView Field { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class QueryModel
    {
        /// <summary>
        /// 用于描述组件状态
        /// </summary>
        [JsonIgnore]
        public ComponentStatus ComponentStatus { get; set; }
        /// <summary>
        /// 查询类型
        /// </summary>
        public QuerType QuerTypes { get; set; }
        /// <summary>
        /// 向上拼接
        /// </summary>
        public QuerySplicing QuerySplicings { get; set; }
        /// <summary>
        /// 查询字段
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 查询数据
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 是否未枚举
        /// </summary>
        public bool IsEnum { get; set; }
        public enum QuerySplicing
        {
            And,
            Or,
        }
        public enum QuerType
        {
            /// <summary>
            /// 等于
            /// </summary>
            Equal,
            /// <summary>
            /// 小于
            /// </summary>
            LessThan,
            /// <summary>
            /// 小于等于
            /// </summary>
            LessThanOrEqual,
            /// <summary>
            /// 大于
            /// </summary>
            GreaterThan,
            /// <summary>
            /// 大于等于
            /// </summary>
            GreaterThanOrEqual,
            /// <summary>
            /// 不等于
            /// </summary>
            NotEqual,
            /// <summary>
            /// 包含
            /// </summary>
            Contains,
        }
    }


}
