using Caviar.Models.SystemData.Template;
using Caviar.Models.SystemData.Template.User;
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
namespace Caviar.Control
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public partial class BaseController : Controller
    {
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
        protected Sys_User_Info Sys_User_Info { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //获取ip地址
            Base_Current_Ipaddress = context.HttpContext.GetUserIp();
            //获取完整Url
            Base_Current_AbsoluteUri = context.HttpContext.Request.GetAbsoluteUri();
            //获取请求路径
            Base_Current_Action = context.HttpContext.Request.Path.Value;

            Sys_User_Info sys_User_Info = context.HttpContext.Session.Get<Sys_User_Info>("Sys_User_Info");

            
            OnInfoVerification();
        }


        /// <summary>
        /// 信息验证
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnInfoVerification()
        {
            
            return true;
        }
        /// <summary>
        /// 验证完毕
        /// </summary>
        protected virtual void OnInfoOk()
        {

        }

    }
}
