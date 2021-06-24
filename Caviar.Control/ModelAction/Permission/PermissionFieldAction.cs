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
        public List<SysModelFields> GetRoleFields()
        {
            //获取字段权限
            var permission = BC.UserData.Permissions.Where(u => u.PermissionType == PermissionType.Field);
            List<SysModelFields> fields = new List<SysModelFields>();
            foreach (var item in BC.SysModelFields)
            {
                if (permission.FirstOrDefault(u => u.PermissionId == item.Id) != null)
                {
                    fields.Add(item);
                }
            }
            return fields;
        }
        /// <summary>
        /// 获取角色字段
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="roleId">当id为0时获取当前角色的拥有的字段</param>
        /// <returns></returns>
        public async Task<List<ViewModelFields>> GetRoleFields(string fullName,int roleId = 0)
        {
            if (string.IsNullOrEmpty(fullName)) return null;
            IEnumerable<SysPermission> permission;
            if (roleId == 0)
            {
                permission = BC.UserData.Permissions.Where(u => u.PermissionType == PermissionType.Field);
            }
            else{
                //当id!=0时，是设置其他角色的权限
                permission = await BC.DC.GetEntityAsync<SysPermission>(u => u.IdentityId == roleId && u.PermissionType == PermissionType.Field && u.PermissionIdentity == PermissionIdentity.Role);
            }
            List<SysModelFields> fields;
            if (BC.IsAdmin)
            {
                fields = BC.SysModelFields.Where(u => u.FullName == fullName).ToList();
            }
            else
            {
                fields = BC.UserData.ModelFields.Where(u => u.FullName == fullName).ToList();
            }
            fields.AToB(out List<ViewModelFields> viewFields);
            foreach (var item in viewFields)
            {
                if (permission.FirstOrDefault(u => u.PermissionId == item.Id)!=null)
                {
                    item.IsPermission = true;
                }
            }
            return viewFields;
        }
        /// <summary>
        /// 设置角色字段
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="roleId"></param>
        /// <param name="modelFields"></param>
        /// <returns></returns>
        public async Task SetRoleFields(string fullName, int roleId, List<ViewModelFields> modelFields)
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
                var count = await BC.DC.UpdateEntityAsync(field);
                if (item.IsPermission)
                {
                    //进行授权
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
                else
                {
                    //删除授权
                    if (perm != null)
                    {
                        await BC.DC.DeleteEntityAsync(perm, IsDelete: true);
                    }
                }
            }
        }

        /// <summary>
        /// 获取字段其他信息
        /// </summary>
        /// <param name="CavAssembly"></param>
        /// <param name="modelName"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
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
                if (field != null && (field.IsPermission || isAdmin))
                {
                    item.IsPermission = field.IsPermission;
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
