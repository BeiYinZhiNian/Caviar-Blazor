using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Caviar.Core.Services;
using Caviar.Core.Interface;
using Caviar.SharedKernel;
using Caviar.Core;
using Caviar.Core.Services.PermissionServices;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace Caviar.Infrastructure.API.BaseApi
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseApiController: Controller
    {
        protected Interactor Interactor;

        private ResultDataFilter DataFilter = new ResultDataFilter();

        protected ILanguageService LanguageService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Interactor = HttpContext.RequestServices.GetRequiredService<Interactor>();
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
            
            var acceptLanguage = context.HttpContext.Request.Headers["Current-Language"];
            SetLanguage(acceptLanguage);
        }

        void SetLanguage(string acceptLanguage)
        {
            using (var serviceScope = Configure.ServiceProvider.CreateScope())
            {
                if (string.IsNullOrEmpty(acceptLanguage)) acceptLanguage = "zh-CN";
                var culture = CultureInfo.GetCultureInfo(acceptLanguage);
                LanguageService = serviceScope.ServiceProvider.GetRequiredService<ILanguageService>();
                LanguageService.SetLanguage(culture);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            var result = context.Result;
            var resultMsg = DataFilter.ResultHandle(result);
            if (resultMsg != null)
            {
                ModificationTips(resultMsg);
                context.Result = Ok(resultMsg);
            }
            Interactor.Stopwatch.Stop();
        }

        protected virtual void ModificationTips(ResultMsg resultMsg)
        {
            resultMsg.Title = LanguageService[$"SharedKernel.ResuleMsg.Title.{resultMsg.Title}"];
        }

        protected static T CreateService<T>() where T : new()
        {
            T service = new T();
            var propertyInfo = service.ContainProperty("DbContext",typeof(IAppDbContext));
            if (propertyInfo != null)
            {
                var serviceScope = Configure.ServiceProvider.CreateScope();
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<IAppDbContext>();
                propertyInfo.SetValue(service, dbContext,null);
            }
            
            return service;
        }

    }


    public class EasyBaseApiController<Vm, T>: BaseApiController where T : class, IBaseEntity, new() where Vm : IView<T>,new()
    {
        IEasyBaseServices<T> _service;
        protected IEasyBaseServices<T> Service
        {
            get
            {
                if (_service == null)
                {
                    _service = HttpContext.RequestServices.GetRequiredService<IEasyBaseServices<T>>();
                    _service.DbContext = HttpContext.RequestServices.GetRequiredService<IAppDbContext>();
                }
                return _service;
            }
            set
            {
                _service = value;
            }
        }

        /// <summary>
        /// 只能获取自身字段
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> GetFields()
        {
            var permissionServices = CreateService<PermissionServices>();
            var fieldName = typeof(T).Name;
            var fullName = typeof(T).FullName;
            var fields = FieldScannerServices.GetClassFields(fieldName, fullName);
            fields = await permissionServices.GetFields(fields, fieldName, fullName);
            foreach (var item in fields)
            {
                string key = null;
                if (string.IsNullOrEmpty(item.Entity.DisplayName))
                {
                    key = $"SharedKernel.EntitysName.{item.Entity.FieldName}";
                }
                else
                {
                    key = $"SharedKernel.EntitysName.{item.Entity.DisplayName}";
                }
                item.Entity.DisplayName = LanguageService[key];
            }
            return Ok(fields);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CreateEntity(Vm vm)
        {
            var id = await Service.CreateEntity(vm.Entity);
            return Ok(id);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UpdateEntity(Vm vm)
        {
            await Service.UpdateEntity(vm.Entity);
            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteEntity(Vm vm)
        {
            await Service.DeleteEntity(vm.Entity);
            return Ok();
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetEntity(int id)
        {
            var entity = await Service.GetEntity(id);
            var entityVm = ToView(entity);
            return Ok(entityVm);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, bool isOrder = true, bool isNoTracking = true)
        {
            var entity = await Service.GetPages(null, pageIndex, pageSize, isOrder, isNoTracking);
            var entityVm = ToView(entity);
            return Ok(entityVm);
        }

        protected virtual Vm ToView(T entity)
        {
            var vm = new Vm() {  Entity = entity};
            return vm;
        }

        protected virtual List<Vm> ToView(List<T> entitys)
        {
            if (entitys == null) return null;
            entitys = Sort(entitys);
            var vmList = new List<Vm>();
            foreach (var item in entitys)
            {
                var vm = new Vm(){ Entity = item };
                vmList.Add(vm);
            }
            return vmList;
        }

        protected virtual List<T> Sort(List<T> entitys)
        {
            return entitys.OrderBy(u => u.Number).ToList();
        }

        protected virtual PageData<Vm> ToView(PageData<T> page)
        {

            var pageVm = new PageData<Vm>()
            {
                Rows = ToView(page.Rows),
                PageIndex = page.PageIndex,
                PageSize = page.PageSize,
                Total = page.Total
            };
            return pageVm;
        }

        protected virtual List<T> ToEntity(List<Vm> vm)
        {
            if (vm == null) return null;
            var entitys = new List<T>();
            foreach (var item in vm)
            {
                entitys.Add(item.Entity);
            }
            return entitys;
        }
    }
}
