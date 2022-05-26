// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

namespace Caviar.SharedKernel.Entities.View
{
    public class RowCallbackData<T>
    {
        /// <summary>
        /// 点击的菜单
        /// </summary>
        public SysMenuView Menu { get; set; }
        /// <summary>
        /// 回调数据
        /// </summary>
        public T Data { get; set; }
    }
}
