using Caviar.Models.SystemData;
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
using Caviar.Control.Role;
using System.Threading.Tasks;
using Caviar.Control.Permission;
using Caviar.Control.Menu;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Caviar.Control
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public partial class CaviarBaseController : Controller
    {
        IBaseControllerModel _controllerModel;
        protected IBaseControllerModel BC
        {
            get
            {
                if (_controllerModel == null)
                {
                    _controllerModel = HttpContext.RequestServices.GetService<IBaseControllerModel>();
                }
                return _controllerModel;
            }
        }

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
            var IsInto = BaseAction.GetInto();
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
            BC.SysModelFields = BC.DC.GetAllAsync<SysModelFields>().Result;
            BC.SysMenus = BC.DC.GetAllAsync<SysMenu>().Result;
            var roleAction = CreateModel<RoleAction>();
            BC.UserData.Roles = roleAction.GetCurrentRoles().Result.Data;
            var permissionAction = CreateModel<PermissionAction>();
            var rolePermission = permissionAction.GetRolePermissions(BC.UserData.Roles).Result;
            var userPermission = permissionAction.GetUserPermissions(BC.UserToken.Id).Result;
            BC.UserData.Permissions = new List<SysPermission>();
            BC.UserData.Permissions.AddRange(rolePermission.Data);
            BC.UserData.Permissions.AddRange(userPermission.Data);
            BC.UserData.ModelFields = permissionAction.GetRoleFields().Data;
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
    }


}
