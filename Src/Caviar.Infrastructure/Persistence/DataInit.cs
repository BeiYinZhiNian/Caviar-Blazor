using Caviar.SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.Persistence
{
    /// <summary>
    /// 同步系统与数据库的数据
    /// </summary>
    public class DataInit
    {
        Interactor Interactor { get; set; }
        public DataInit(Interactor interactor)
        {
            fieldsInit();
        }

        private void fieldsInit()
        {
            var fields = FieldScanner.GetApplicationFields();
            var dataBaseFields = Interactor.DbContext.GetAllAsync<SysFields>(isDataPermissions: false);
        }

    }
}
