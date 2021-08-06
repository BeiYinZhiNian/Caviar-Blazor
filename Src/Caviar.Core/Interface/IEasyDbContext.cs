using Caviar.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Interface
{
    public interface IEasyDbContext<T>: IAppDbContext where T : class, IBaseEntity, new()
    {
        public Task<List<T>> GetAllAsync(bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false);

        public Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> where, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false);

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false);

        public Task<List<T>> GetEntityAsync(Expression<Func<T, bool>> where, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false);
    }
}
