using Caviar.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.UserGroup
{
    public partial class UserGroupAction
    {
        /// <summary>
        /// 获取指定用户的用户组
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isDataPermissions">是否开启数据权限，默认开启</param>
        /// <returns></returns>
        public async Task<ResultMsg<SysUserGroup>> GetUserGroup(int userId,bool isDataPermissions = true)
        {
            var user = await Interactor.DbContext.GetSingleEntityAsync<SysUser>(u => u.Id == userId,isDataPermissions:isDataPermissions);
            if (user == null) return Error<SysUserGroup>("不存在该角色");
            var group = await Interactor.DbContext.GetSingleEntityAsync<SysUserGroup>(u => u.Id == user.UserGroupId, isDataPermissions: isDataPermissions);
            return Ok(group);
        }
        /// <summary>
        /// 获取指定用户组下的所有用户组
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="isDataPermissions">是否开启数据权限，默认开启</param>
        /// <returns></returns>
        public async Task<ResultMsg<List<SysUserGroup>>> GetSubordinateUserGroup(int userGroupId, bool isDataPermissions = true)
        {
            var user = await Interactor.DbContext.GetEntityAsync<SysUserGroup>(u => u.ParentId == userGroupId, isDataPermissions: isDataPermissions);
            if (user != null && user.Count!=0)
            {
                foreach (var item in user)
                {
                    var result = await GetSubordinateUserGroup(item.Id, isDataPermissions);
                    if (result.Status != HttpState.OK) continue;
                    if(result.Data!=null && result.Data.Count != 0)
                    {
                        user.AddRange(result.Data);
                    }
                }
            }
            return Ok(user);
        }
    }
}
