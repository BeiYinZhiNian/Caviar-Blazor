using Caviar.Core;
using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Caviar.Infrastructure.Persistence
{
    /// <summary>
    /// 同步系统与数据库的数据
    /// </summary>
    public class SysDataInit
    {
        public SysDataInit()
        {

        }

        public async Task StartInit(IServiceProvider provider)
        {
            using (var serviceScope = provider.CreateScope())
            {
                var dbAppContext = serviceScope.ServiceProvider.GetRequiredService<IAppDbContext>();
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<IDbContext>();
                await DatabaseInit(dbContext);
                await FieldsInit(dbAppContext);
            }
        }
        /// <summary>
        /// 初始化系统字段
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        protected virtual async Task FieldsInit(IAppDbContext dbContext)
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
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        protected virtual Task<bool> DatabaseInit(IDbContext dbContext)
        {
            return dbContext.Database.EnsureCreatedAsync();
        }

    }
}
