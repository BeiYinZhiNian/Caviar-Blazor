// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Caviar.Core.Interface;
using Caviar.Core.Services;
using Caviar.SharedKernel.Common;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Caviar.Infrastructure.API.BaseApi
{
    [Route($"{CurrencyConstant.Api}[controller]/[action]")]
    [ApiController]
    public class BaseApiController : Controller
    {
        /// <summary>
        /// 数据互动
        /// </summary>
        private IInteractor _interactor;
        /// <summary>
        /// 语言服务
        /// </summary>
        private ILanguageService _languageService;
        /// <summary>
        /// 用户服务
        /// </summary>
        private UserServices _userServices;
        /// <summary>
        /// 权限服务
        /// </summary>
        private PermissionServices _permissionServices;
        /// <summary>
        /// 角色服务
        /// </summary>
        private RoleServices _roleServices;
        private LogServices<BaseApiController> _logServices;
        /// <summary>
        /// 服务配置
        /// </summary>
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
            _languageService = CreateService<ILanguageService>();
            _languageService.SetLanguage(culture);
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _interactor = CreateService<IInteractor>();
            _userServices = CreateService<UserServices>();
            _roleServices = CreateService<RoleServices>();
            _logServices = CreateService<LogServices<BaseApiController>>();
            _permissionServices = CreateService<PermissionServices>();
            _caviarConfig = CreateService<CaviarConfig>();
            _interactor.Stopwatch.Start();
            if (!User.Identity.IsAuthenticated && _caviarConfig.TouristVisit)
            {
                _interactor.UserInfo = await _userServices.GetUserInfoAsync(CurrencyConstant.TouristUser);
            }
            else
            {
                if (User.Identity.IsAuthenticated)
                {
                    _interactor.UserInfo = await _userServices.GetUserInfoAsync(User.Identity.Name);
                }
            }
            var roles = await _userServices.GetRolesAsync(_interactor.UserInfo);
            _interactor.ApplicationRoles = await _roleServices.GetRoles(roles);
            //请求参数
            _interactor.ActionArguments = context.ActionArguments;
            _logServices.Infro("请求开始");
            if (_interactor.Method == "POST")
            {
                var actionArguments = context.ActionArguments;
                var postData = JsonConvert.SerializeObject(actionArguments);
                var log = _logServices.CreateLog("post请求数据", LogLevel.Information, postData);
                _logServices.Log(log);
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
            //获取所有角色id
            var roleIds = _interactor.ApplicationRoles.Select(u => u.Id).ToList();
            var menuPermission = _permissionServices.GetPermissionsAsync(roleIds, u => u.PermissionType == (int)PermissionType.RoleMenus).Result;
            _interactor.PermissionUrls = _permissionServices.GetPermissionsAsync(menuPermission);
            var url = _interactor.Current_Action.Remove(0, CurrencyConstant.Api.Length + 1);
            if (IgnoreUrl.Contains(url)) return true;
            return _interactor.PermissionUrls.Contains(url);
        }

        protected virtual void UrlUnauthorized(ActionExecutingContext context)
        {
            // 未登录或者是游客身份
            if (User.Identity.IsAuthenticated || _caviarConfig.TouristVisit)
            {
                var msg = _languageService[$"{CurrencyConstant.ExceptionMessage}.{CurrencyConstant.Unauthorized}"];
                context.Result = Ok(HttpStatusCode.Unauthorized, _interactor.Current_Action + msg);
                _logServices.Infro("url访问未授权");
            }
            else
            {
                var msg = _languageService[$"{CurrencyConstant.ExceptionMessage}.{CurrencyConstant.LoginExpiration}"];
                context.Result = Ok(HttpStatusCode.RedirectMethod, msg, UrlConfig.Login);
                _logServices.Infro("用户登录过期");
            }
        }



        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var dbContext = CreateService<IDbContext>();
            // 取消所有实体跟踪
            dbContext.DetachAll();
            var result = context.Result;
            var resultScanner = CreateService<ResultScannerServices>();
            var roleIds = _interactor.ApplicationRoles.Select(u => u.Id).ToList();
            //赋值字段权限
            resultScanner.PermissionFieldss = _permissionServices.GetPermissionsAsync(roleIds, u => u.PermissionType == (int)PermissionType.RoleFields).Result;
            var resultMsg = resultScanner.ResultHandle(result);
            if (resultMsg != null)
            {
                resultMsg.TraceId = _interactor.TraceId.ToString();
                resultMsg.Title = _languageService[$"{CurrencyConstant.ResuleMsg}.{resultMsg.Title}"];
                ModificationTips(resultMsg);
                context.Result = base.Ok(resultMsg);
            }
            base.OnActionExecuted(context);
            _interactor.Stopwatch.Stop();
            var timeSpan = _interactor.Stopwatch.Elapsed;
            if (resultMsg != null)
            {
                var log = _logServices.CreateLog(resultMsg.Title, LogLevel.Information, elapsed: timeSpan.TotalMilliseconds, status: resultMsg.Status);
                _logServices.Log(log);
            }
            else
            {
                //自定义返回
                var statusCode = result.GetObjValue("StatusCode");
                var code = statusCode == null ? HttpStatusCode.OK : (HttpStatusCode)statusCode;
                var log = _logServices.CreateLog("自定义返回", LogLevel.Information, elapsed: timeSpan.TotalMilliseconds, status: code);
                _logServices.Log(log);
            }
            _logServices.Infro("请求结束");
        }

        protected virtual IActionResult Ok(HttpStatusCode status = HttpStatusCode.OK, string title = "Succeeded", string url = null, Dictionary<string, string> detail = null, object data = null)
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
            resultMsg.Title = _languageService[$"{CurrencyConstant.ResuleMsg}.Title.{resultMsg.Title}"];
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
        protected virtual async Task<List<FieldsView>> GetFields<T>() where T : IUseEntity
        {
            var permissionServices = CreateService<RoleFieldServices>();
            var fieldName = typeof(T).Name;
            var fullName = typeof(T).FullName;
            var fields = FieldScannerServices.GetClassFields(fieldName, fullName, _languageService);
            var user = await _userServices.GetCurrentUserInfoAsync();
            var roles = await _userServices.GetRoleIdsAsync(user);
            fields = await permissionServices.GetRoleFields(fields, fullName, roles);
            fields = fields.OrderBy(u => u.Entity.Number).ToList();
            return fields;
        }

    }


    public class EasyBaseApiController<Vm, T> : BaseApiController where T : class, IUseEntity, new() where Vm : class, IView<T>, new()
    {
        IEasyBaseServices<T, Vm> _service;
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
