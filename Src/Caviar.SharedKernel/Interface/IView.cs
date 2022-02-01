using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public interface IView<T> where T : IUseEntity
    {
        public T Entity { get; set; }
    }

    public class BaseView<T>: IView<T> where T : IUseEntity
    {
        public virtual T Entity { get; set; }
    }
}
