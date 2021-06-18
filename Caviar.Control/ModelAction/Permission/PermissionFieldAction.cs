using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.Permission
{
    /// <summary>
    /// 字段权限操作类
    /// </summary>
    public partial class PermissionAction
    {
        
        /// <summary>
        /// 获取角色字段
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<List<SysModelFields>> GetRoleFields(string fullName,int roleId = 0)
        {
            if (string.IsNullOrEmpty(fullName)) return null;
            IEnumerable<SysPermission> permission;
            if (roleId == 0)
            {
                permission = BC.UserData.Permissions.Where(u => u.PermissionType == PermissionType.Field);
            }
            else{
                permission = await BC.DC.GetEntityAsync<SysPermission>(u => u.IdentityId == roleId && u.PermissionType == PermissionType.Field && u.PermissionIdentity == PermissionIdentity.Role);
            }
            var fields = BC.SysModelFields.Where(u => u.FullName == fullName).ToList();
            foreach (var item in fields)
            {
                if (permission.FirstOrDefault(u => u.PermissionId == item.Id)!=null)
                {
                    item.IsDisable = false;
                }
            }
            return fields;
        }
        /// <summary>
        /// 设置角色字段
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="roleId"></param>
        /// <param name="modelFields"></param>
        /// <returns></returns>
        public async Task SetRoleFields(string fullName, int roleId, List<SysModelFields> modelFields)
        {
            if (string.IsNullOrEmpty(fullName) || roleId == 0) return;
            var permission = await BC.DC.GetEntityAsync<SysPermission>(u => u.IdentityId == roleId && u.PermissionType == PermissionType.Field && u.PermissionIdentity == PermissionIdentity.Role);
            var fields = BC.SysModelFields.Where(u => u.FullName == fullName);
            foreach (var item in modelFields)
            {
                var field = fields.FirstOrDefault(u => u.TypeName == item.TypeName);
                if (field == null) continue;
                var perm = permission.FirstOrDefault(u => u.PermissionType == PermissionType.Field && u.PermissionId == field.Id);
                field.Width = item.Width;
                field.Number = item.Number;
                field.IsPanel = item.IsPanel;
                if (!string.IsNullOrEmpty(item.DisplayName))
                {
                    field.DisplayName = item.DisplayName;
                }
                await BC.DC.UpdateEntityAsync(field);
                if (item.IsDisable)
                {
                    if (perm != null)
                    {
                        await BC.DC.DeleteEntityAsync(perm,IsDelete:true);
                    }

                }
                else
                {
                    if (perm == null)
                    {
                        perm = new SysPermission()
                        {
                            PermissionType = PermissionType.Field,
                            PermissionId = field.Id,
                            IdentityId = roleId,
                            PermissionIdentity = PermissionIdentity.Role
                        };
                        await BC.DC.AddEntityAsync(perm);
                    }
                }
            }
        }


        public async Task<List<ViewModelFields>> GetFieldsData(IAssemblyDynamicCreation CavAssembly, string modelName, int roleId = 0)
        {
            if (string.IsNullOrEmpty(modelName)) return null;
            var fields = await GetRoleFields(modelName, roleId);
            var modelFields = CavAssembly.GetViewModelHeaders(modelName);//其他信息
            var viewFields = new List<ViewModelFields>();
            var isAdmin = roleId != 0 && BC.IsAdmin;
            foreach (var item in modelFields)
            {
                var field = fields.FirstOrDefault(u => u.FullName == item.FullName && u.TypeName == item.TypeName);
                if (field != null && (!field.IsDisable || isAdmin))
                {
                    item.IsDisable = field.IsDisable;
                    item.Width = field.Width;
                    item.IsPanel = field.IsPanel;
                    item.Number = field.Number;
                    if (!string.IsNullOrEmpty(field.DisplayName))
                    {
                        item.DisplayName = field.DisplayName;
                    }
                    viewFields.Add(item);
                }
            }
            viewFields = viewFields.OrderBy(u => u.Number).ToList();
            return viewFields;
        }
    }
}
