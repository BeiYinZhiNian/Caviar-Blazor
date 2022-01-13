using Caviar.SharedKernel.Entities;
using System;

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
