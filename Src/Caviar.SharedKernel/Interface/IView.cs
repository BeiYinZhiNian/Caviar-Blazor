using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel
{
    public interface IView<T> where T : IBaseEntity
    {
        public T Entity { get; set; }
    }

    public class BaseView<T>: IView<T> where T : IBaseEntity
    {
        public virtual T Entity { get; set; }
    }
}
