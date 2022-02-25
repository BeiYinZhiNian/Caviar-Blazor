using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    public class LogDataServices : EasyBaseServices<SysLog, SysLogView>
    {
        public LogDataServices(IAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
