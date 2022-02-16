using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public class NotificationException : Exception
    {
        public ResultMsg ResultMsg { get; private set; } = new ResultMsg() { 
            Status = System.Net.HttpStatusCode.NotImplemented,
            Title = CurrencyConstant.NotificationException
        };

        public NotificationException(string message)//指定错误消息
            : base(message)
        {
            ResultMsg.Title = message;
        }

        public NotificationException(ResultMsg message)//指定错误消息
            : base(message.Title)
        {
            ResultMsg = message;
        }

        //public NotificationException(string message, Exception inner)//指定错误消息和内部异常信息
        //    : base(message, inner)
        //{

        //}
    }
}
