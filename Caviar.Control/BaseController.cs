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
                    _model.DataContext = CaviarConfig.ApplicationServices.GetRequiredService<SysDataContext>();
                    _model.Logger = CaviarConfig.ApplicationServices.GetRequiredService<ILogger<BaseController>>();
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
                var role = Model.DataContext.GetEntity<SysRole>(u => u.RoleName == CaviarConfig.NoLoginRole);
                Model.SysUserInfo.SysRoles.AddRange(role);
                foreach (var item in Model.SysUserInfo.SysRoles)
                {
                    var menus = Model.DataContext.GetEntity<SysRoleMenu>(u => u.RoleId == item.Id).FirstOrDefault();
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

        protected virtual T CreatModel<T>() where T:IBaseModel
        {
            var icoModel = CaviarConfig.ApplicationServices.GetRequiredService<T>();
            icoModel.Model = Model;
            return icoModel;
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


        protected virtual IActionResult ResultOK()
        {
            var result = new ResultMsg();
            return Ok(result);
        }

        protected virtual IActionResult ResultOK(string msg)
        {
            var result = new ResultMsg() { Msg = msg };
            return Ok(result);
        }

        protected virtual IActionResult ResultOk<T>(T data)
        {
            var result = new ResultMsg<T>() { Data = data};
            return Ok(result);
        }

        protected virtual IActionResult ResultOk<T>(string msg,T data)
        {
            var result = new ResultMsg<T>() { Data = data,Msg = msg };
            return Ok(result);
        }

        protected virtual IActionResult ResultError<T>(int code,string msg, T data)
        {
            var result = new ResultMsg<T>() { Code = code,Data = data, Msg = msg };
            return StatusCode(code,result);
        }

        protected virtual IActionResult ResultError(int code, string msg)
        {
            var result = new ResultMsg() { Code = code, Msg = msg };
            return StatusCode(code, result);
        }
        #endregion


    }
}
