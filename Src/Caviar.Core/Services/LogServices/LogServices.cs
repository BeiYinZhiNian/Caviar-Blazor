using Caviar.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    public class LogServices : DbServices
    {
        public LogServices(IAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
