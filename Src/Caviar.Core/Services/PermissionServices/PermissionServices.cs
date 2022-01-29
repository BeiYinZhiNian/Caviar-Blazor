using Caviar.SharedKernel.Entities.View;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Caviar.SharedKernel.Entities;
using System.Security.Claims;

namespace Caviar.Core.Services
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
        public async Task<List<ViewFields>> GetFields(List<ViewFields> fields,IEnumerable<Claim> claims,string fieldName,string fullName, int roleId = 0)
        {
            if (string.IsNullOrEmpty(fieldName)) throw new System.Exception("请输入需要获取的数据名称");
            var sysFields = await DbContext.GetEntityAsync<SysFields>(u => u.FullName == fullName);
            var type = PermissionType.Field.ToString();
            foreach (var item in fields)
            {
                var field = sysFields.FirstOrDefault(u => u.FieldName == item.Entity.FieldName && u.FullName == item.Entity.FullName);
                var value = CommonHelper.GetClaimValue(item.Entity);
                var claim = claims.SingleOrDefault(u=> u.Type == type && u.Value == value);
                if(claim != null) item.IsPermission = true;
                item.Entity = field;
            }
            return fields;
        }
    }
}
