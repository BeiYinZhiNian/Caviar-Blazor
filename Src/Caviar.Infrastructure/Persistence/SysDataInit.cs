using Caviar.Core;
using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.Persistence
{
    /// <summary>
    /// 同步系统与数据库的数据
    /// </summary>
    public class SysDataInit
    {

        public async Task FieldsInit(IAppDbContext dbContext)
        {
            var fields = FieldScannerServices.GetApplicationFields().Select(u => u.Entity);
            var dataBaseFields = await dbContext.GetAllAsync<SysFields>(isDataPermissions: false);
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
            await dbContext.DeleteEntityAsync(deleteFields, IsDelete: true);
            await dbContext.AddEntityAsync(addFields);
        }

    }
}
