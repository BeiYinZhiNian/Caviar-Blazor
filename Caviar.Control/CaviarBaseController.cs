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


        Stopwatch stopwatch = new Stopwatch();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            stopwatch.Start();
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

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            stopwatch.Stop();
            //日志记录，这里应该想一个更好的办法
            var statusCode = context.HttpContext.Response.StatusCode;
            if (statusCode != HttpState.OK)
            {
                LoggerMsg("", LogLevel.Error, statusCode, true);
            }
        }

        public override OkObjectResult Ok(object value)
        {
            ResultMsg result = (ResultMsg)value;
            if (result.Status == HttpState.OK)
            {
                LoggerMsg(result.Title, LogLevel.Information, result.Status, true);
            }
            else
            {
                LoggerMsg(result.Title, LogLevel.Warning, result.Status, true);
            }
            var data = value.GetObjValue("data");
            if (data != null)
            {
                BaseAction.ArgumentsModel(data.GetType(),data);
            }
            return base.Ok(value);
        }

        #region 创建模型
        protected virtual T CreateModel<T>() where T : class, IActionModel
        {
            var entity = BC.HttpContext.RequestServices.GetRequiredService<T>();
            entity.BC = BC;
            return entity;
        }
        #endregion



        #region  日志消息
        protected void LoggerMsg(string msg, LogLevel logLevel = LogLevel.Information, int status = 200,bool IsAutomatic = false)
        {
            var log = new SysLog() 
            {
                UserName = BC.UserName,
                AbsoluteUri = BC.Current_AbsoluteUri,
                Ipaddress = BC.Current_Ipaddress,
                Elapsed = stopwatch.Elapsed.TotalSeconds,
                Status = status,
                LogLevel = (CavLogLevel)logLevel,
                Msg = msg,
                Method = BC.HttpContext.Request.Method,
                IsAutomatic = IsAutomatic
            };
            if (BC.HttpContext.Request.Headers.ContainsKey("User-Agent"))
            {
                log.Browser = BC.HttpContext.Request.Headers["User-Agent"];
            }
            if (BC.IsLogin)
            {
                log.UserId = BC.UserToken.Id;
            }
            var isAdd = FilterLog(log);
            if (!isAdd) return;
            var count = BC.DC.AddEntityAsync(log).Result;
        }

        protected bool FilterLog(SysLog log)
        {
            if ((int)log.LogLevel < 2)
            {
                return false;
            }
            else if (log.Method == "GET" && log.LogLevel == CavLogLevel.Information && log.Status == HttpState.OK && log.Elapsed < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

    }


}
