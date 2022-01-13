using Caviar.SharedKernel.Entities.View;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Caviar.SharedKernel.Entities;

namespace Caviar.Core.Services.PermissionServices
{
    public class PermissionServices: DbServices
    {
        /// <summary>
        /// 获取字段其他信息
        /// </summary>
        /// <param name="CavAssembly"></param>
        /// <param name="className"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<List<ViewFields>> GetFields(List<ViewFields> fields,string fieldName,string fullName, int roleId = 0)
        {
            if (string.IsNullOrEmpty(fieldName)) throw new System.Exception("请输入需要获取的数据名称");
            var sysFields = await DbContext.GetEntityAsync<SysFields>(u => u.FullName == fullName);
            foreach (var item in fields)
            {
                var field = sysFields.FirstOrDefault(u => u.FieldName == item.Entity.FieldName && u.FullName == item.Entity.FullName);
                item.Entity = field;
            }
            return fields;
        }
    }
}
