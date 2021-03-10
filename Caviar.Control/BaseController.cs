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
        IDataContext _iDataContext;
        protected IDataContext IDataContext 
        {
            get 
            {
                if (_iDataContext == null)
                {
                    _iDataContext = CaviarConfig.ApplicationServices.GetRequiredService<SysDataContext>();
                }
                return _iDataContext; 
            }
        }

        ILogger<BaseController> _logger;
        protected ILogger<BaseController> Base_Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = CaviarConfig.ApplicationServices.GetRequiredService<ILogger<BaseController>>();
                }
                return _logger;
            }
            set { _logger = value; }
        }
        /// <summary>
        /// 当前请求路径
        /// </summary>
        protected string Base_Current_Action { get; private set; }
        /// <summary>
        /// 当前请求ip地址
        /// </summary>
        protected string Base_Current_Ipaddress { get; private set; }
        /// <summary>
        /// 当前请求的完整Url
        /// </summary>
        protected string Base_Current_AbsoluteUri { get; private set; }
        /// <summary>
        /// 当前用户信息
        /// </summary>
        protected SysUserInfo SysUserInfo { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            //获取ip地址
            Base_Current_Ipaddress = context.HttpContext.GetUserIp();
            //获取完整Url
            Base_Current_AbsoluteUri = context.HttpContext.Request.GetAbsoluteUri();
            //获取请求路径
            Base_Current_Action = context.HttpContext.Request.Path.Value;

            SysUserInfo = context.HttpContext.Session.Get<SysUserInfo>("SysUserInfo");
            if (SysUserInfo == null)
            {
                SysUserInfo = new SysUserInfo()
                {
                    SysUserLogin = new SysUserLogin()
                    {
                        UserName = CaviarConfig.NoLoginRole,
                    },
                    SysRoles = new List<SysRole>(),
                    SysPowerMenus = new List<SysPowerMenu>(),
                    IsLogin = false
                };
                //获取未登录角色
                var role = IDataContext.GetEntity<SysRole>(u => u.RoleName == CaviarConfig.NoLoginRole);
                SysUserInfo.SysRoles.AddRange(role);
                foreach (var item in SysUserInfo.SysRoles)
                {
                    var menus = IDataContext.GetEntity<SysRoleMenu>(u => u.RoleId == item.Id).FirstOrDefault();
                    //SysUserInfo.SysPowerMenus.Add(menus.Menu);
                }
            }
            var menu = SysUserInfo.SysPowerMenus.Where(u => u.Url == Base_Current_Action).FirstOrDefault();
            if (menu == null)
            {
                
                return;
            }

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

        protected virtual IActionResult Result400()
        {
            var result = new ResultMsg() { Code = 400,Msg = "操作失败，请检查重试" };
            return BadRequest(result);
        }


        protected virtual IActionResult Result400(string msg)
        {
            var result = new ResultMsg() { Msg = msg,Code = 400};
            return BadRequest(result);
        }

        protected virtual IActionResult Result400<T>(T data)
        {
            var result = new ResultMsg<T>{ Msg = "操作失败，请检查重试", Code = 400,Data = data };
            return BadRequest(result);
        }

        protected virtual IActionResult Result400<T>(string msg,T data)
        {
            var result = new ResultMsg<T> { Msg = msg, Code = 400, Data = data };
            return BadRequest(result);
        }

        #endregion


    }
}
