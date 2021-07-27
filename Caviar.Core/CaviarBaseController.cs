using Caviar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Caviar.Models;
using Microsoft.Extensions.Primitives;
using System.Web;
using Caviar.Core.Role;
using System.Threading.Tasks;
using Caviar.Core.Permission;
using Caviar.Core.Menu;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Caviar.Core.UserGroup;

namespace Caviar.Core
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public partial class CaviarBaseController : Controller
    {
        IInteractor _interactor;
        protected IInteractor BC
        {
            get
            {
                if (_interactor == null)
                {
                    _interactor = HttpContext.RequestServices.GetService<IInteractor>();
                }
                return _interactor;
            }
        }
        protected ICodeGeneration CodeGeneration => BC.HttpContext.RequestServices.GetService<ICodeGeneration>();

        CaviarBaseAction BaseAction => new CaviarBaseAction() { BC = BC };
        SysLogAction LoginAction => HttpContext.RequestServices.GetService<SysLogAction>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            BC.Stopwatch.Start();
            base.OnActionExecuting(context);
            //获取ip地址
            BC.Current_Ipaddress = context.HttpContext.GetUserIp();
            //获取完整Url
            BC.Current_AbsoluteUri = context.HttpContext.Request.GetAbsoluteUri();
            //获取请求路径
            BC.Current_Action = context.HttpContext.Request.Path.Value;
            //请求上下文
            BC.HttpContext = HttpContext;
            //请求参数
            BC.ActionArguments = context.ActionArguments;
            var result = BaseAction.CheckUsreToken();
            if (result.Status != HttpState.OK)
            {
                context.Result = Ok(result);
                return;
            }
            GetPermission();
            var IsInto = BaseAction.CheckAPI();
            if (IsInto.Status!=HttpState.OK)
            {
                context.Result = Ok(IsInto);
                return;
            }
        }
        /// <summary>
        /// 获取用户角色和权限
        /// 可以做缓存，未做
        /// </summary>
        /// <returns></returns>
        void GetPermission()
        {
            BC.SysModelFields = BC.DbContext.GetAllAsync<SysModelFields>().Result;
            BC.SysMenus = BC.DbContext.GetAllAsync<SysMenu>().Result;
            var roleAction = CreateModel<RoleAction>();
            var userGroup = CreateModel<UserGroupAction>();
            BC.UserData.UserGroup = userGroup.GetUserGroup(BC.Id).Result.Data;
            BC.UserData.Roles = roleAction.GetUserRoles(BC.Id, BC.UserData.UserGroup?.Id).Result.Data;
            var permissionAction = CreateModel<PermissionAction>();
            var rolePermission = permissionAction.GetRolePermissions(BC.UserData.Roles).Result;
            var userPermission = permissionAction.GetUserPermissions(BC.UserToken.Id).Result;
            BC.UserData.Permissions = new List<SysPermission>();
            BC.UserData.Permissions.AddRange(rolePermission.Data);
            BC.UserData.Permissions.AddRange(userPermission.Data);
            BC.UserData.ModelFields = permissionAction.GetPermissionFields(BC.UserData.Permissions).Data;
            BC.UserData.Menus = permissionAction.GetPermissionMenu(BC.UserData.Permissions).Data;
        } 

        public override OkObjectResult Ok(object value)
        {
            BC.Stopwatch.Stop();
            ResultMsg result = (ResultMsg)value;
            if (result.Status == HttpState.OK)
            {
                LoginAction.LoggerMsg(result.Title, LogLevel.Information, result.Status, true);
            }
            else
            {
                LoginAction.LoggerMsg(result.Title, LogLevel.Warning, result.Status, true);
            }
            var data = value.GetObjValue("data");
            if (data != null)
            {
                BaseAction.ArgumentsModel(data.GetType(),data);
            }
            return base.Ok(value);
        }

        public override OkResult Ok()
        {
            throw new Exception("请勿调用此方法，需要为前端返回ResultMsg");
        }

        #region 创建模型
        protected virtual T CreateModel<T>() where T : class, IActionModel
        {
            var entity = BC.HttpContext.RequestServices.GetRequiredService<T>();
            entity.BC = BC;
            return entity;
        }
        #endregion

        #region API
        /// <summary>
        /// 代码生成
        /// </summary>
        /// <param name="generate"></param>
        /// <param name="isPerview">是否预览</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CodeFileGenerate(CodeGenerateData generate, bool isPerview = true)
        {
            var result = await BaseAction.CodeFileGenerate(generate, isPerview);
            return Ok(result);
        }
        #endregion
    }


}
