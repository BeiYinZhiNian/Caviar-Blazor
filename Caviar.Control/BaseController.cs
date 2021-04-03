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
        BaseControllerModel _model;
        public BaseControllerModel Model
        {
            get
            {
                if (_model == null)
                {
                    _model = CaviarConfig.ApplicationServices.GetRequiredService<BaseControllerModel>();
                    if (_model.DataContext == null)
                    {
                        _model.DataContext = CaviarConfig.ApplicationServices.GetRequiredService<SysDataContext>();
                    }
                    if (_model.Logger == null)
                    {
                        _model.Logger = CaviarConfig.ApplicationServices.GetRequiredService<ILogger<BaseController>>();
                    }
                }
                return _model;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            //获取ip地址
            Model.Current_Ipaddress = context.HttpContext.GetUserIp();
            //获取完整Url
            Model.Current_AbsoluteUri = context.HttpContext.Request.GetAbsoluteUri();
            //获取请求路径
            Model.Current_Action = context.HttpContext.Request.Path.Value;

            var sysUserInfo = context.HttpContext.Session.Get<SysUserInfo>("SysUserInfo");
            if (sysUserInfo == null)
            {
                Model.SysUserInfo.SysUserLogin.UserName = CaviarConfig.NoLoginRole;
                //获取未登录角色
                var role = Model.DataContext.GetEntityAsync<SysRole>(u => u.RoleName == CaviarConfig.NoLoginRole);
                Model.SysUserInfo.SysRoles.AddRange(role);
                foreach (var item in Model.SysUserInfo.SysRoles)
                {
                    var menus = Model.DataContext.GetEntityAsync<SysRoleMenu>(u => u.RoleId == item.Id).FirstOrDefault();
                    if (menus == null) continue;
                    Model.SysUserInfo.SysPowerMenus.Add(menus.Menu);
                }
                context.HttpContext.Session.Set("SysUserInfo", Model.SysUserInfo);
            }
            var IsVerification = ActionVerification();
            if (!IsVerification)
            {
                context.Result = ResultForbidden();
                return;
            }

        }

        protected virtual T CreatEntity<T>() where T : class, IBaseModel
        {
            var entity = CaviarConfig.ApplicationServices.GetRequiredService<T>();
            entity.Model = Model;
            return entity;
        }

        protected virtual T CreatEntity<T>(int id) where T : class, IBaseModel
        {
            var entity = Model.DataContext.GetEntityAsync<T>(id).Result;
            if (entity != null) entity.Model = Model;
            return entity;
        }
        protected virtual T CreatEntity<T>(Guid guid) where T : class, IBaseModel
        {
            var entity = Model.DataContext.GetEntityAsync<T>(guid).Result;
            if (entity != null) entity.Model = Model;
            return entity;
        }
        protected virtual T CreatEntity<T>(Expression<Func<T, bool>> whereLambda) where T : class, IBaseModel
        {
            var entity = Model.DataContext.GetEntityAsync<T>(whereLambda).FirstOrDefault();
            if (entity != null) entity.Model = Model;
            return entity;
        }



        protected virtual IActionResult ResultForbidden()
        {
            return ResultError(403, "对不起，您没有该页面的访问权限！");
        }


        protected virtual bool ActionVerification()
        {
            if (CaviarConfig.IsDebug) return true;
            var menu = Model.SysUserInfo.SysPowerMenus.Where(u => u.Url == Model.Current_Action).FirstOrDefault();
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
