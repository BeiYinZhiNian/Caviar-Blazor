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
    public class LogDataServices : EasyBaseServices<SysLog, SysLogView>
    {
        public LogDataServices(IAppDbContext dbContext) : base(dbContext)
        {
        }

        public async override Task<PageData<SysLogView>> GetPageAsync(Expression<Func<SysLog, bool>> where, int pageIndex, int pageSize, bool isOrder = true)
        {
            var pages = await AppDbContext.GetPageAsync(where, u => u.CreatTime, pageIndex, pageSize, isOrder);
            return ToView(pages);
        }
    }
}
