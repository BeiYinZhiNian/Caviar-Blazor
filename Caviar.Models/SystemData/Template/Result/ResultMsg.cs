using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{

    public class ResultMsg
    {
        public int Code { get; set; } = 200;
        public string Msg { get; set; } = "操作完成";
        
    }

    public class ResultMsg<T>:ResultMsg
    {
        public T Data { get; set; }
    }
}
