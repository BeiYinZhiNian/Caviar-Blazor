using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    public class UserGroupServices : EasyBaseServices<SysUserGroup, SysUserGroupView>
    {
        public UserGroupServices(IAppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<PageData<SysUserGroupView>> GetPageAsync(Expression<Func<SysUserGroup, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true)
        {
            var pages = await AppDbContext.GetPageAsync(where,u=>u.Number, pageIndex, pageSize, isOrder, isNoTracking);
            var pageViews = ToView(pages);
            pageViews.Rows = pageViews.Rows.ListToTree();
            return pageViews;
        }
    }
}
