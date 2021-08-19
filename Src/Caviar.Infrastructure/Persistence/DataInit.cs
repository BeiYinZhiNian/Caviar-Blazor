using Caviar.Core;
using Caviar.Core.Interface;
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
        static object obj = new object();
        IAppDbContext DbContext { get; set; }
        public DataInit(IAppDbContext dbContext)
        {
            if (Configure.HasDataInit) return;
            DbContext = dbContext;
            lock (obj)
            {
                if (Configure.HasDataInit) return;
                Configure.HasDataInit = true;
                fieldsInit().Wait();
            }
        }

        private async Task fieldsInit()
        {
            var fields = FieldScannerServices.GetApplicationFields().Select(u => u.Entity);
            var dataBaseFields = await DbContext.GetAllAsync<SysFields>(isDataPermissions: false);
            foreach (var sysField in fields)
            {
                foreach (var dataBaseField in dataBaseFields)
                {
                    if (dataBaseField.FieldName == sysField.FieldName && dataBaseField.FullName == sysField.FullName && dataBaseField.BaseFullName == sysField.BaseFullName)
                    {
                        sysField.Id = dataBaseField.Id;
                        dataBaseField.Id = 0;
                    }
                }
            }
            var addFields = fields.Where(u => u.Id == 0).ToList();
            var deleteFields = dataBaseFields.Where(u => u.Id != 0).ToList();
            await DbContext.DeleteEntityAsync(deleteFields, IsDelete: true);
            await DbContext.AddEntityAsync(addFields);
        }

    }
}
