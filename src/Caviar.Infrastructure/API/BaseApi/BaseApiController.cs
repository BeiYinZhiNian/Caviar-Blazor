using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Caviar.Core.Services;
using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities.View;
using Caviar.Core;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Caviar.SharedKernel.Entities;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Caviar.Infrastructure.API.BaseApi
{
    [Route($"{CurrencyConstant.Api}[controller]/[action]")]
    [ApiController]
    public class BaseApiController: Controller
    {
        protected Interactor Interactor;

        protected ILanguageService LanguageService { get; set; }
        /// <summary>
        /// 用户服务
        /// </summary>
        protected UserServices UserServices { get; set; }
        /// <summary>
        /// 权限服务
        /// </summary>
        private PermissionServices PermissionServices { get; set; }
        protected RoleServices RoleServices { get; set; }
        private LogServices<BaseApiController> LogServices { get; set; }
        private CaviarConfig _caviarConfig;
        /// <summary>
        /// 忽略url权限
        /// </summary>
        protected List<string> IgnoreUrl => new List<string>() { 
            UrlConfig.CurrentUserInfo,
            UrlConfig.SignInActual,
            UrlConfig.Logout,
            UrlConfig.LogoutServer,
            UrlConfig.Login
        };

        void SetLanguage(string acceptLanguage)
        {
            if (string.IsNullOrEmpty(acceptLanguage)) acceptLanguage = CurrencyConstant.DefaultLanguage;
            var culture = CultureInfo.GetCultureInfo(acceptLanguage);
            LanguageService = CreateService<ILanguageService>();
            LanguageService.SetLanguage(culture);
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Interactor = CreateService<Interactor>();
            UserServices = CreateService<UserServices>();
            RoleServices = CreateService<RoleServices>();
            LogServices = CreateService<LogServices<BaseApiController>>();
            PermissionServices = CreateService<PermissionServices>();
            _caviarConfig = CreateService<CaviarConfig>();
            Interactor.Stopwatch.Start();
            if (!User.Identity.IsAuthenticated && _caviarConfig.TouristVisit)
            {
                Interactor.UserInfo = await UserServices.GetUserInfoAsync(CurrencyConstant.TouristUser);
            }
            else
            {
                if(User.Identity.IsAuthenticated)
                {
                    Interactor.UserInfo = await UserServices.GetUserInfoAsync(User.Identity.Name);
                }
            }
            var roles = await UserServices.GetRolesAsync(Interactor.UserInfo);
            Interactor.ApplicationRoles = await RoleServices.GetRoles(roles);
            //请求参数
            Interactor.ActionArguments = context.ActionArguments;
            LogServices.Infro("请求开始");
            if(Interactor.Method == "POST")
            {
                var actionArguments = context.ActionArguments;
                var postData = JsonConvert.SerializeObject(actionArguments);
                var log = LogServices.CreateLog("post请求数据",LogLevel.Information, postData);
                LogServices.Log(log);
            }
            //设置语言信息
            var acceptLanguage = context.HttpContext.Request.Cookies.SingleOrDefault(c => c.Key == CurrencyConstant.LanguageHeader).Value;
            SetLanguage(acceptLanguage);

            if (!UrlCheck())
            {
                UrlUnauthorized(context);
                return;
            }

            await base.OnActionExecutionAsync(context, next);
        }

        /// <summary>
        /// 检查url权限
        /// </summary>
        /// <returns></returns>
        protected virtual bool UrlCheck()
        {
            //设置url权限
            var roleIds = UserServices.GetRoleIdsAsync(Interactor.UserInfo).Result;
            var menuPermission = PermissionServices.GetPermissionsAsync(roleIds, u => u.PermissionType == (int)PermissionType.RoleMenus).Result;
            Interactor.PermissionUrls = PermissionServices.GetPermissionsAsync(menuPermission);
            var url = Interactor.Current_Action.Remove(0, CurrencyConstant.Api.Length + 1);
            if (IgnoreUrl.Contains(url)) return true;
            return Interactor.PermissionUrls.Contains(url);
        }

        protected virtual void UrlUnauthorized(ActionExecutingContext context)
        {
            // 未登录或者是游客身份
            if (User.Identity.IsAuthenticated || _caviarConfig.TouristVisit)
            {
                var msg = LanguageService[$"{CurrencyConstant.ExceptionMessage}.{CurrencyConstant.Unauthorized}"];
                context.Result = Ok(HttpStatusCode.Unauthorized, Interactor.Current_Action + msg);
                LogServices.Infro("url访问未授权");
            }
            else
            {
                var msg = LanguageService[$"{CurrencyConstant.ExceptionMessage}.{CurrencyConstant.LoginExpiration}"];
                context.Result = Ok(HttpStatusCode.RedirectMethod, msg,UrlConfig.Login);
                LogServices.Infro("用户登录过期");
            }
        }



        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var dbContext = CreateService<IDbContext>();
            // 取消所有实体跟踪
            dbContext.DetachAll();
            var result = context.Result;
            var resultScanner = CreateService<ResultScannerServices>();
            var roleIds = UserServices.GetRoleIdsAsync(Interactor.UserInfo).Result;
            //赋值字段权限
            resultScanner.PermissionFieldss = PermissionServices.GetPermissionsAsync(roleIds,u => u.PermissionType == (int)PermissionType.RoleFields).Result;
            var resultMsg = resultScanner.ResultHandle(result);
            if (resultMsg != null)
            {
                resultMsg.TraceId = Interactor.TraceId.ToString();
                resultMsg.Title = LanguageService[$"{CurrencyConstant.ResuleMsg}.{resultMsg.Title}"];
                ModificationTips(resultMsg);
                context.Result = base.Ok(resultMsg);
            }
            base.OnActionExecuted(context);
            Interactor.Stopwatch.Stop();
            var timeSpan = Interactor.Stopwatch.Elapsed;
            if (resultMsg != null)
            {
                var log = LogServices.CreateLog(resultMsg.Title, LogLevel.Information, elapsed: timeSpan.TotalMilliseconds, status: resultMsg.Status);
                LogServices.Log(log);
            }
            else
            {
                //自定义返回
                var StatusCode = result.GetObjValue("StatusCode");
                var code = StatusCode == null ? HttpStatusCode.OK : (HttpStatusCode)StatusCode;
                var log = LogServices.CreateLog("自定义返回", LogLevel.Information, elapsed: timeSpan.TotalMilliseconds,status: code);
                LogServices.Log(log);
            }
            LogServices.Infro("请求结束");
        }

        protected virtual IActionResult Ok(HttpStatusCode status = HttpStatusCode.OK,string title = "Succeeded",string url = null,Dictionary<string,string> detail = null,object data = null)
        {
            var resultMst = new ResultMsg<object>
            {
                Status = status,
                Title = title,
                Url = url,
                Detail = detail,
                Data = data
            };
            return Ok(resultMst);
        }

        protected virtual void ModificationTips(ResultMsg resultMsg)
        {
            resultMsg.Title = LanguageService[$"{CurrencyConstant.ResuleMsg}.Title.{resultMsg.Title}"];
        }

        protected virtual T CreateService<T>()
        {
            T service = HttpContext.RequestServices.GetRequiredService<T>();
            return service;
        }


        /// <summary>
        /// 只能获取自身字段
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        protected virtual async Task<List<FieldsView>> GetFields<T>() where T:IUseEntity
        {
            var permissionServices = CreateService<RoleFieldServices>();
            var fieldName = typeof(T).Name;
            var fullName = typeof(T).FullName;
            var fields = FieldScannerServices.GetClassFields(fieldName, fullName, LanguageService);
            var user = await UserServices.GetCurrentUserInfoAsync();
            var roles = await UserServices.GetRoleIdsAsync(user);
            fields = await permissionServices.GetRoleFields(fields, fullName, roles);
            fields = fields.OrderBy(u => u.Entity.Number).ToList();
            return fields;
        }

    }


    public class EasyBaseApiController<Vm, T>: BaseApiController where T : class, IUseEntity, new() where Vm : class,IView<T>,new()
    {
        IEasyBaseServices<T,Vm> _service;
        protected IEasyBaseServices<T, Vm> Service
        {
            get
            {
                if (_service == null)
                {
                    _service = CreateService<IEasyBaseServices<T, Vm>>();
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
            var fields = await GetFields<T>();
            return Ok(fields);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CreateEntity(Vm vm)
        {
            var id = await Service.AddEntityAsync(vm.Entity);
            return Ok(id);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UpdateEntity(Vm vm)
        {
            await Service.UpdateEntityAsync(vm.Entity);
            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteEntity(Vm vm)
        {
            await Service.DeleteEntityAsync(vm.Entity);
            return Ok();
        }
        
        [HttpPost]
        public virtual async Task<IActionResult> Query(QueryView query)
        {
            var page = await Service.QueryAsync(query);
            return Ok(page);
        }


        [HttpGet]
        public virtual async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            var pages = await Service.GetPageAsync(null, pageIndex, pageSize, isOrder);
            return Ok(pages);
        }
    }
}
