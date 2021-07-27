using Caviar.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Permission
{
    /// <summary>
    /// 字段权限操作类
    /// </summary>
    public partial class PermissionAction
    {
        /// <summary>
        /// 获取所有模型
        /// </summary>
        /// <param name="isView"></param>
        /// <returns></returns>
        public ResultMsg<List<ViewModelFields>> GetModels(bool isView)
        {
            List<ViewModelFields> viewModels = new List<ViewModelFields>();
            var types = CommonlyHelper.GetModelList(isView);
            foreach (var item in types)
            {
                var displayName = item.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                viewModels.Add(new ViewModelFields() { TypeName = item.Name, DisplayName = displayName, FullName = item.FullName.Replace("." + item.Name, "") });
            }
            return Ok(viewModels);
        }

        /// <summary>
        /// 获取权限下字段
        /// </summary>
        /// <returns></returns>
        public ResultMsg<List<SysModelFields>> GetPermissionFields(List<SysPermission> permissions)
        {
            //获取字段权限
            var permission = permissions.Where(u => u.PermissionType == PermissionType.Field);
            List<SysModelFields> fields = new List<SysModelFields>();
            foreach (var item in BC.SysModelFields)
            {
                if (permission.FirstOrDefault(u => u.PermissionId == item.Id) != null)
                {
                    fields.Add(item);
                }
            }
            return Ok(fields);
        }
        /// <summary>
        /// 获取角色字段展示
        /// IsPermission = true 时表示拥有该字段权限
        /// 管理员拥有全部字段权限，但是如果不设置权限，意味着不使用
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="roleId">当id为0时获取当前角色的拥有的字段</param>
        /// <returns></returns>
        public async Task<ResultMsg<List<ViewModelFields>>> GetRoleFields(string fullName,int roleId = 0)
        {
            if (string.IsNullOrEmpty(fullName)) return Error<List<ViewModelFields>>("模型名称不能为空");
            IEnumerable<SysPermission> permission;
            if (roleId == 0)
            {
                permission = BC.UserData.Permissions.Where(u => u.PermissionType == PermissionType.Field);
            }
            else{
                //当id!=0时，是设置其他角色的权限
                permission = await BC.DbContext.GetEntityAsync<SysPermission>(u => u.IdentityId == roleId && u.PermissionType == PermissionType.Field && u.PermissionIdentity == PermissionIdentity.Role);
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
            return Ok(viewFields);
        }
        /// <summary>
        /// 设置角色字段
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="roleId"></param>
        /// <param name="modelFields"></param>
        /// <returns></returns>
        public async Task<ResultMsg> SetRoleFields(string fullName, int roleId, List<ViewModelFields> modelFields)
        {
            if (string.IsNullOrEmpty(fullName) || roleId == 0) return Error("设置角色字段失败，请检查模型名称或角色id");
            var permission = await BC.DbContext.GetEntityAsync<SysPermission>(u => u.IdentityId == roleId && u.PermissionType == PermissionType.Field && u.PermissionIdentity == PermissionIdentity.Role);
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
                var count = await BC.DbContext.UpdateEntityAsync(field);
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
                        await BC.DbContext.AddEntityAsync(perm);
                    }
                }
                else
                {
                    //删除授权
                    if (perm != null)
                    {
                        await BC.DbContext.DeleteEntityAsync(perm, IsDelete: true);
                    }
                }
            }
            return Ok();
        }

        /// <summary>
        /// 获取字段其他信息
        /// </summary>
        /// <param name="CavAssembly"></param>
        /// <param name="modelName"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<ResultMsg<List<ViewModelFields>>> GetFieldsData(ICodeGeneration CavAssembly, string modelName, int roleId = 0)
        {
            if (string.IsNullOrEmpty(modelName)) return Error<List<ViewModelFields>>("请输入需要获取的数据名称");
            var fields = await GetRoleFields(modelName, roleId);
            var modelFields = CavAssembly.GetViewModelHeaders(modelName);//其他信息
            var viewFields = new List<ViewModelFields>();
            var isAdmin = roleId != 0 && BC.IsAdmin;
            foreach (var item in modelFields)
            {
                var field = fields.Data.FirstOrDefault(u => u.FullName == item.FullName && u.TypeName == item.TypeName);
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
            return Ok(viewFields);
        }
    }
}
