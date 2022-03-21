using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public class ResultException: SystemException
    {
        public ResultMsg ResultMsg { get; set; }
        public ResultException(ResultMsg resultMsg)
        {
            ResultMsg = resultMsg;
        }
    }

    public class ResultException<T>: ResultException
    {
        public new ResultMsg<T> ResultMsg { get; set; }
        public ResultException(ResultMsg<T> resultMsg):base(resultMsg)
        {
            ResultMsg = resultMsg;
        }
    }
}
