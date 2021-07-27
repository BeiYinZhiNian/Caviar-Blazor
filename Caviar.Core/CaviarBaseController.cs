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
        protected IInteractor _Interactor
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
        protected ICodeGeneration CodeGeneration => _Interactor.HttpContext.RequestServices.GetService<ICodeGeneration>();

        CaviarBaseAction BaseAction => new CaviarBaseAction() { Interactor = _Interactor };
        SysLogAction LoginAction => HttpContext.RequestServices.GetService<SysLogAction>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _Interactor.Stopwatch.Start();
            base.OnActionExecuting(context);
            //获取ip地址
            _Interactor.Current_Ipaddress = context.HttpContext.GetUserIp();
            //获取完整Url
            _Interactor.Current_AbsoluteUri = context.HttpContext.Request.GetAbsoluteUri();
            //获取请求路径
            _Interactor.Current_Action = context.HttpContext.Request.Path.Value;
            //请求上下文
            _Interactor.HttpContext = HttpContext;
            //请求参数
            _Interactor.ActionArguments = context.ActionArguments;
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
            _Interactor.SysModelFields = _Interactor.DbContext.GetAllAsync<SysModelFields>().Result;
            _Interactor.SysMenus = _Interactor.DbContext.GetAllAsync<SysMenu>().Result;
            var roleAction = CreateModel<RoleAction>();
            var userGroup = CreateModel<UserGroupAction>();
            _Interactor.UserData.UserGroup = userGroup.GetUserGroup(_Interactor.Id).Result.Data;
            _Interactor.UserData.Roles = roleAction.GetUserRoles(_Interactor.Id, _Interactor.UserData.UserGroup?.Id).Result.Data;
            var permissionAction = CreateModel<PermissionAction>();
            var rolePermission = permissionAction.GetRolePermissions(_Interactor.UserData.Roles).Result;
            var userPermission = permissionAction.GetUserPermissions(_Interactor.UserToken.Id).Result;
            _Interactor.UserData.Permissions = new List<SysPermission>();
            _Interactor.UserData.Permissions.AddRange(rolePermission.Data);
            _Interactor.UserData.Permissions.AddRange(userPermission.Data);
            _Interactor.UserData.ModelFields = permissionAction.GetPermissionFields(_Interactor.UserData.Permissions).Data;
            _Interactor.UserData.Menus = permissionAction.GetPermissionMenu(_Interactor.UserData.Permissions).Data;
        } 

        public override OkObjectResult Ok(object value)
        {
            _Interactor.Stopwatch.Stop();
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
            var entity = _Interactor.HttpContext.RequestServices.GetRequiredService<T>();
            entity.Interactor = _Interactor;
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
