// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;

namespace Caviar.SharedKernel.Entities
{
    /// <summary>
    /// 自动注入
    /// 允许被继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class DIInjectAttribute : Attribute
    {
        public DIInjectAttribute(InjectType injectType)
        {
            this.InjectType = injectType;
        }
        public DIInjectAttribute()
        {
            InjectType = InjectType.TRANSIENT;
        }
        public InjectType InjectType { get; }

    }
    /// <summary>
    /// 注入类型选择
    /// </summary>
    public enum InjectType
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        SINGLETON,
        /// <summary>
        /// 瞬时
        /// </summary>
        TRANSIENT,
        /// <summary>
        /// 单次
        /// </summary>
        SCOPED
    }
}
