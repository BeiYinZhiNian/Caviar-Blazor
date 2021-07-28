using Caviar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Caviar.Core.Role;
using System.Threading.Tasks;
using Caviar.Core.Permission;
using Caviar.Core.UserGroup;

namespace Caviar.Core
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public partial class CaviarBaseController : Controller
    {
        IInteractor _interactor;
        protected IInteractor Interactor
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
        protected ICodeGeneration CodeGeneration => Interactor.HttpContext.RequestServices.GetService<ICodeGeneration>();

        CaviarBaseAction BaseAction => new CaviarBaseAction() { Interactor = Interactor };
        SysLogAction LoginAction => HttpContext.RequestServices.GetService<SysLogAction>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Interactor.Stopwatch.Start();
            base.OnActionExecuting(context);
            //获取ip地址
            Interactor.Current_Ipaddress = context.HttpContext.GetUserIp();
            //获取完整Url
            Interactor.Current_AbsoluteUri = context.HttpContext.Request.GetAbsoluteUri();
            //获取请求路径
            Interactor.Current_Action = context.HttpContext.Request.Path.Value;
            //请求上下文
            Interactor.HttpContext = HttpContext;
            //请求参数
            Interactor.ActionArguments = context.ActionArguments;
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
            Interactor.SysModelFields = Interactor.DbContext.GetAllAsync<SysModelFields>().Result;
            Interactor.SysMenus = Interactor.DbContext.GetAllAsync<SysMenu>().Result;
            var roleAction = CreateModel<RoleAction>();
            var userGroup = CreateModel<UserGroupAction>();
            Interactor.UserData.UserGroup = userGroup.GetUserGroup(Interactor.Id,false).Result.Data;
            if (Interactor.UserData.UserGroup != null)
            {
                Interactor.UserData.SubordinateUserGroup = userGroup.GetSubordinateUserGroup(Interactor.UserData.UserGroup.Id, false).Result.Data;
            }
            Interactor.UserData.Roles = roleAction.GetUserRoles(Interactor.Id, Interactor.UserData.UserGroup?.Id).Result.Data;
            var permissionAction = CreateModel<PermissionAction>();
            var rolePermission = permissionAction.GetRolePermissions(Interactor.UserData.Roles).Result;
            var userPermission = permissionAction.GetUserPermissions(Interactor.UserToken.Id).Result;
            Interactor.UserData.Permissions = new List<SysPermission>();
            Interactor.UserData.Permissions.AddRange(rolePermission.Data);
            Interactor.UserData.Permissions.AddRange(userPermission.Data);
            Interactor.UserData.ModelFields = permissionAction.GetPermissionFields(Interactor.UserData.Permissions).Data;
            Interactor.UserData.Menus = permissionAction.GetPermissionMenu(Interactor.UserData.Permissions).Data;
        } 

        public override OkObjectResult Ok(object value)
        {
            Interactor.Stopwatch.Stop();
            if(!(value is ResultMsg))
            {
                throw new Exception(Interactor.Current_Action + "必须返回ResultMsg类");
            }
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
            var entity = Interactor.HttpContext.RequestServices.GetRequiredService<T>();
            entity.Interactor = Interactor;
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
