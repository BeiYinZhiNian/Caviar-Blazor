using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    [Inject]
    public class ResultMsg
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        
    }
    [Inject]
    public class ResultMsg<T>:ResultMsg
    {
        public T Data { get; set; }
    }
}
