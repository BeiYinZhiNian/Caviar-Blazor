// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;
using System.Threading.Tasks;
using Caviar.SharedKernel.Entities.View;

namespace Caviar.AntDesignUI.Pages.UserGroup
{
    public partial class SysUserGroupIndex
    {
        protected override void OnInitialized()
        {
            TableOptions.TreeChildren = u => u.Children;
            base.OnInitialized();
        }

        protected override Task<List<SysUserGroupView>> GetPages(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            // 当使用树形组件时，需要获取全部数据
            // 也可改成GetAll
            return base.GetPages(pageIndex, Config.MaxPageSize, isOrder);
        }
    }
}
