using Caviar.SharedKernel;
using Caviar.SharedKernel.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Exceptions
{
    public class ResultException : Exception
    {
        public int StatusCode { get; set; }
        public ResultMsg ResultMsg { get; set; }
        public ResultException(int statusCode,string errorMsg,ResultMsg resultMsg = null) : base(errorMsg)
        {
            StatusCode = statusCode;
            ResultMsg = resultMsg;
        }
    }
}
