﻿// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Core
{
    public class BootstrapDynamicComponent
    {
        /// <summary>
        /// 获得/设置 组件参数集合
        /// </summary>
        private IEnumerable<KeyValuePair<string, object>> Parameters { get; set; }

        /// <summary>
        /// 获得/设置 组件类型
        /// </summary>
        public Type ComponentType { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="parameters">TCom 组件所需要的参数集合</param>
        public BootstrapDynamicComponent(Type componentType, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            ComponentType = componentType;
            Parameters = parameters;
        }

        /// <summary>
        /// 创建自定义组件方法
        /// </summary>
        /// <typeparam name="TCom"></typeparam>
        /// <param name="parameters">TCom 组件所需要的参数集合</param>
        /// <returns></returns>
        public static BootstrapDynamicComponent CreateComponent<TCom>(IEnumerable<KeyValuePair<string, object>> parameters) where TCom : IComponent => new(typeof(TCom), parameters);

        /// <summary>
        /// 创建自定义组件方法
        /// </summary>
        /// <typeparam name="TCom"></typeparam>
        /// <returns></returns>
        public static BootstrapDynamicComponent CreateComponent<TCom>() where TCom : IComponent => CreateComponent<TCom>(Enumerable.Empty<KeyValuePair<string, object>>());

        /// <summary>
        /// 创建组件实例并渲染
        /// </summary>
        /// <returns></returns>
        public RenderFragment Render() => builder =>
        {
            var index = 0;
            builder.OpenComponent(index++, ComponentType);
            if (Parameters.Any())
            {
                builder.AddMultipleAttributes(index++, Parameters);
            }
            builder.CloseComponent();
        };
    }
}
