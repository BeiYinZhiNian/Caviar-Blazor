using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Helper
{
    public class RequestData<T>
    {
        public RequestData(T entity)
        {
            Entity = entity;
        }
        public T Entity { get; set; }
    }
}
