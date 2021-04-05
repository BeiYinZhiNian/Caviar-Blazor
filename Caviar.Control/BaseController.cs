using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Renci.SshNet.Messages.Authentication;
using System.Linq.Expressions;

namespace Caviar.Control
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public partial class BaseController : Controller
    {
        IBaseControllerModel _controllerModel;
        public IBaseControllerModel ControllerModel
        {
            get
            {
                if (_controllerModel == null)
                {
                    _controllerModel = CaviarConfig.ApplicationServices.GetRequiredService<BaseControllerModel>();
                }
                return _controllerModel;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            //获取ip地址
            ControllerModel.Current_Ipaddress = context.HttpContext.GetUserIp();
            //获取完整Url
            ControllerModel.Current_AbsoluteUri = context.HttpContext.Request.GetAbsoluteUri();
            //获取请求路径
            ControllerModel.Current_Action = context.HttpContext.Request.Path.Value;
            //设置请求上下文
            ControllerModel.HttpContext = context.HttpContext;
            if (!ControllerModel.SysUserInfo.IsInit)
            {
                var session = HttpContext.Session.Get<SysUserInfo>(CaviarConfig.SessionUserInfoName);
                if (session == null)
                {
                    ControllerModel.SysUserInfo.IsInit = true;
                    ControllerModel.SysUserInfo.SysUserLogin.UserName = CaviarConfig.NoLoginRole;
                    //获取未登录角色
                    var role = ControllerModel.DataContext.GetEntityAsync<SysRole>(u => u.RoleName == CaviarConfig.NoLoginRole);
                    ControllerModel.SysUserInfo.SysRoles.AddRange(role);
                    foreach (var item in ControllerModel.SysUserInfo.SysRoles)
                    {
                        var menus = ControllerModel.DataContext.GetEntityAsync<SysRoleMenu>(u => u.RoleId == item.Id).FirstOrDefault();
                        if (menus == null) continue;
                        ControllerModel.SysUserInfo.SysPowerMenus.Add(menus.Menu);
                    }
                    HttpContext.Session.Set(CaviarConfig.SessionUserInfoName, ControllerModel.SysUserInfo);
                }
                else
                {
                    ControllerModel.SysUserInfo = session;
                }
                ControllerModel.SysUserInfo.IsInit = true;
            }
            var IsVerification = ActionVerification();
            if (!IsVerification)
            {
                context.Result = ResultForbidden();
                return;
            }

        }

        protected virtual T CreateModel<T>() where T : class, IBaseModel
        {
            var entity = CaviarConfig.ApplicationServices.GetRequiredService<T>();
            return entity;
        }

        protected virtual T CreateModel<T>(int id) where T : class, IBaseModel
        {
            var entity = ControllerModel.DataContext.GetEntityAsync<T>(id).Result;
            return entity;
        }
        protected virtual T CreateModel<T>(Guid guid) where T : class, IBaseModel
        {
            var entity = ControllerModel.DataContext.GetEntityAsync<T>(guid).Result;
            return entity;
        }
        protected virtual T CreateModel<T>(Expression<Func<T, bool>> whereLambda) where T : class, IBaseModel
        {
            var entity = ControllerModel.DataContext.GetEntityAsync<T>(whereLambda).FirstOrDefault();
            return entity;
        }



        protected virtual IActionResult ResultForbidden()
        {
            return ResultError(403, "对不起，您没有该页面的访问权限！");
        }


        protected virtual bool ActionVerification()
        {
            if (CaviarConfig.IsDebug) return true;
            var menu = ControllerModel.SysUserInfo.SysPowerMenus.Where(u => u.Url == ControllerModel.Current_Action).FirstOrDefault();
            if (menu == null)
            {
                return false;
            }
            return true;
        }


        #region 消息回复
        private ResultMsg _resultMsg;
        protected ResultMsg ResultMsg 
        {
            get 
            {
                if (_resultMsg == null)
                {
                    _resultMsg = CaviarConfig.ApplicationServices.GetRequiredService<ResultMsg>();
                }
                return _resultMsg;
            }
        }

        protected virtual IActionResult ResultOK()
        {
            return Ok(ResultMsg);
        }

        protected virtual IActionResult ResultOK(string title)
        {
            ResultMsg.Title = title;
            return Ok(ResultMsg);
        }

        protected virtual IActionResult ResultOK(ResultMsg resultMsg)
        {
            return Ok(resultMsg);
        }



        protected virtual IActionResult ResultError(int status, string title, string detail)
        {
            ResultMsg.Status = status;
            ResultMsg.Title = title;
            ResultMsg.Detail = detail;
            return StatusCode(status, ResultMsg);
        }
        protected virtual IActionResult ResultError(int status, string title, string detail, IDictionary<string, string[]> errors)
        {
            ResultMsg.Status = status;
            ResultMsg.Title = title;
            ResultMsg.Detail = detail;
            ResultMsg.Errors = errors;
            return StatusCode(status, ResultMsg);
        }

        protected virtual IActionResult ResultError(int status, string title)
        {
            ResultMsg.Status = status;
            ResultMsg.Title = title;
            return StatusCode(status, ResultMsg);
        }

        protected virtual IActionResult ResultError(ResultMsg resultMsg)
        {
            return StatusCode(resultMsg.Status, resultMsg);
        }
        #endregion


    }
}
