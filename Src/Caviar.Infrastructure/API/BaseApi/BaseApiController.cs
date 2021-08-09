using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Caviar.Core;
using Caviar.Core.Services;
using Caviar.SharedKernel;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Caviar.Core.Services.BaseSdk;

namespace Caviar.Infrastructure.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseApiController<TSdk,T> : Controller where TSdk:IBaseSdk<T> where T:class,IBaseEntity,new()
    {
        TSdk sdk;
        TSdk Sdk { 
            get 
            {
                if (sdk == null)
                {
                    sdk = HttpContext.RequestServices.GetService<TSdk>();
                }
                return sdk; 
            }
            set 
            {
                sdk = value;
            } 
        }

        Interactor _interactor;
        protected Interactor Interactor
        {
            get
            {
                if (_interactor == null)
                {
                    _interactor = HttpContext.RequestServices.GetService<Interactor>();
                }
                return _interactor;
            }
        }

        private DataCheckSdk DataCheckSdk = new DataCheckSdk();

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
        }

        [HttpPost]
        public IActionResult CreateEntity(T entity)
        {
            Sdk.CreateEntity(entity);
            return Ok();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            var result = context.Result;
            var resultMsg = DataCheckSdk.ResultHandle(result);
            if (resultMsg != null)
            {
                context.Result = Ok(resultMsg);
            }
        }


    }
}
