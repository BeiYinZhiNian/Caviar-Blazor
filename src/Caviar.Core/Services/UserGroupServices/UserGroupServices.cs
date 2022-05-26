// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;

namespace Caviar.Core.Services
{
    public class UserGroupServices : EasyBaseServices<SysUserGroup, SysUserGroupView>
    {
        public UserGroupServices(IAppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<PageData<SysUserGroupView>> GetPageAsync(Expression<Func<SysUserGroup, bool>> where, int pageIndex, int pageSize, bool isOrder = true)
        {
            var pages = await AppDbContext.GetPageAsync(where, u => u.Number, pageIndex, pageSize, isOrder);
            var pageViews = ToView(pages);
            pageViews.Rows = pageViews.Rows.ListToTree();
            return pageViews;
        }
    }
}
