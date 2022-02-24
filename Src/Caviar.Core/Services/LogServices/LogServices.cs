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
    public class LogServices : EasyBaseServices<SysLog,SysLogView>
    {
        public LogServices(IAppDbContext dbContext) : base(dbContext)
        {

        }


    }
}
