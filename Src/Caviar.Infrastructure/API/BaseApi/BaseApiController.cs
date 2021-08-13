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
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Caviar.Core.Interface;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities;

namespace Caviar.Infrastructure.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseApiController<Vm,T> : Controller where T:class,IBaseEntity,new() where Vm : IView<T>
    {
        IBaseSdk<T> sdk;
        IBaseSdk<T> Sdk { 
            get 
            {
                if (sdk == null)
                {
                    sdk = HttpContext.RequestServices.GetRequiredService<BaseSdk<T>>();
                }
                return sdk; 
            }
            set 
            {
                sdk = value;
            } 
        }

        protected Interactor Interactor;

        private ResultDataFilter DataFilter = new ResultDataFilter();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Interactor = HttpContext.RequestServices.GetRequiredService<Interactor>();
            Sdk.DbContext = HttpContext.RequestServices.GetRequiredService<IEasyDbContext<T>>();
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

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            var result = context.Result;
            var resultMsg = DataFilter.ResultHandle(result);
            if (resultMsg != null)
            {
                context.Result = Ok(resultMsg);
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> CreateEntity(Vm vm)
        {
            var id = await Sdk.CreateEntity(vm.Entity);
            return Ok(id);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UpdateEntity(Vm vm)
        {
            await Sdk.UpdateEntity(vm.Entity);
            return Ok();
        }



        [HttpPost]
        public virtual async Task<IActionResult> DeleteEntity(Vm vm)
        {
            await Sdk.DeleteEntity(vm.Entity);
            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetEntity(int id)
        {
            var entity = await Sdk.GetEntity(id);
            return Ok(entity);
        }
    }
}
