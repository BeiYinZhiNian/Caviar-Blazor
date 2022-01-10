using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Shared
{
    public partial class CavDataTemplate<ViewT, T> where ViewT:IView<T>,new () where T: IBaseEntity,new ()
    {

    }
}
