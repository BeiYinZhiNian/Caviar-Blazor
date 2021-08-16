using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.Persistence
{
    /// <summary>
    /// 同步系统与数据库的数据
    /// </summary>
    public class DataInit
    {
        IAppDbContext DbContext { get; set; }
        public DataInit(IAppDbContext dbContext)
        {
            if (Configure.HasDataInit) return;
            DbContext = dbContext;
            fieldsInit().Wait();
            Configure.HasDataInit = true;
        }

        private async Task fieldsInit()
        {
            var fields = FieldScanner.GetApplicationFields();
            var dataBaseFields = await DbContext.GetAllAsync<SysFields>(isDataPermissions: false);
        }

    }
}
