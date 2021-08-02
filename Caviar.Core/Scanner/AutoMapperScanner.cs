using AutoMapper;
using Caviar.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Scanner
{
    public class AutoMapperScanner:Profile
    {
        public AutoMapperScanner()
        {
            var test = typeof(ViewUser);
            var test1 = CommonlyHelper.GetCavBaseType(test);
            CreateMap(test, test1);
        }
    }
}
