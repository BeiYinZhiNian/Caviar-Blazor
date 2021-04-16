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
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.Text;
using Caviar.Models;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Caching.Memory;

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
                    _controllerModel = HttpContext.RequestServices.GetService<BaseControllerModel>();
                }
                return _controllerModel;
            }
        }
        


        Stopwatch stopwatch = new Stopwatch();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            stopwatch.Start();
            base.OnActionExecuting(context);
            //获取ip地址
            ControllerModel.Current_Ipaddress = context.HttpContext.GetUserIp();
            //获取完整Url
            ControllerModel.Current_AbsoluteUri = context.HttpContext.Request.GetAbsoluteUri();
            //获取请求路径
            ControllerModel.Current_Action = context.HttpContext.Request.Path.Value;
            //请求上下文
            ControllerModel.HttpContext = HttpContext;

            if (HttpContext.Request.Headers.TryGetValue("UsreToken", out StringValues value))
            {
                string base64 = value[0];
                string json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                UserToken userToken = JsonConvert.DeserializeObject<UserToken>(json);
                var token = CaviarConfig.GetUserToken(userToken);
                if (token != userToken.Token)
                {
                    context.Result = ResultUnauthorized("您的登录已过期，请重新登录");
                    return;
                }
                var outTime = (userToken.CreateTime.AddMinutes(CaviarConfig.TokenDuration) - DateTime.Now);
                if (outTime.TotalSeconds <= 0)
                {
                    context.Result = ResultUnauthorized("您的登录已过期，请重新登录");
                    return;
                }
                ControllerModel.UserToken = userToken;
            }
            var IsVerification = ActionVerification();
            if (!IsVerification)
            {
                context.Result = ResultUnauthorized("对不起，您还没有获得改页面权限");
                return;
            }

            if (context.ActionArguments.Count > 0)
            {
                foreach (var ArgumentsItem in context.ActionArguments)
                {
                    if(ArgumentsItem.Value is IBaseModel)
                    {
                        ((IBaseModel)ArgumentsItem.Value).BaseControllerModel = ControllerModel;
                    }
                }
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            var actionResult = (ObjectResult)context.Result;
            var setting = new JsonSerializerSettings();
            setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            setting.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            var json = JsonConvert.SerializeObject(actionResult.Value, setting);
            var resultMsg = JsonConvert.DeserializeObject<ResultMsg>(json);
            stopwatch.Stop();
            LoggerMsg<BaseController>(resultMsg.Title, IsSucc: resultMsg.Status == 200);
        }

        [HttpGet]
        public IActionResult GetModelName(string name)
        {
            if (string.IsNullOrEmpty(name)) return ResultError(400,"请输入需要获取的数据名称");
            var assemblyList = CaviarConfig.GetAssembly();
            Type type = null;
            foreach (var item in assemblyList)
            {
                type = item.GetTypes().SingleOrDefault(u => u.Name.ToLower() == name.ToLower());
                if (type != null) break;
            }
            List<ViewModelName> viewModelNames = new List<ViewModelName>();
            if (type != null)
            {
                foreach (var item in type.GetRuntimeProperties())
                {
                    var typeName = item.PropertyType.Name;
                    var dispLayName = item.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                    viewModelNames.Add(new ViewModelName() { TypeName = item.Name, ModelType = typeName,DispLayName=dispLayName });
                }
            }
            ResultMsg.Data = viewModelNames;
            return ResultOK();
        }
        #region 创建模型
        protected virtual T CreateModel<T>() where T : class, IBaseModel
        {
            var entity = ControllerModel.HttpContext.RequestServices.GetRequiredService<T>();
            entity.BaseControllerModel = ControllerModel;
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
        #endregion



        protected virtual bool ActionVerification()
        {
            if (CaviarConfig.IsDebug) return true;

            return true;
        }

        #region  日志消息
        protected void LoggerMsg<T>(string msg, string action = "", LogLevel logLevel = LogLevel.Information, bool IsSucc = true)
        {
            ControllerModel.GetLogger<T>().LogInformation($"用户：{ControllerModel.UserName} 手机号：{ControllerModel.PhoneNumber} 访问地址：{ControllerModel.Current_AbsoluteUri} 访问ip：{ControllerModel.Current_Ipaddress} 执行时间：{stopwatch.Elapsed} 执行结果：{IsSucc} 执行消息：{msg}");
        }
        #endregion


        #region 消息回复
        private ResultMsg _resultMsg;
        protected ResultMsg ResultMsg
        {
            get
            {
                if (_resultMsg == null)
                {
                    _resultMsg = HttpContext.RequestServices.GetRequiredService<ResultMsg>();
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
            return Ok(ResultMsg);
        }
        protected virtual IActionResult ResultError(int status, string title, string detail, IDictionary<string, string[]> errors)
        {
            ResultMsg.Status = status;
            ResultMsg.Title = title;
            ResultMsg.Detail = detail;
            ResultMsg.Errors = errors;
            return Ok(ResultMsg);
        }

        protected virtual IActionResult ResultError(int status, string title)
        {
            ResultMsg.Status = status;
            ResultMsg.Title = title;
            return Ok(ResultMsg);
        }

        protected virtual IActionResult ResultError(ResultMsg resultMsg)
        {
            return Ok(resultMsg);
        }


        /// <summary>
        /// 返回此结果定位到403界面（无权限）
        /// </summary>
        /// <returns></returns>
        protected virtual IActionResult ResultForbidden(string title)
        {
            return ResultError(403, title);
        }
        /// <summary>
        /// 返回此结果定位到登录界面（登录失效）
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        protected virtual IActionResult ResultUnauthorized(string title)
        {
            return ResultError(401, title);
        }

        protected virtual IActionResult ResultErrorMsg(string title, string detail)
        {
            return ResultError(406, title, detail);
        }
        protected virtual IActionResult ResultErrorMsg(string title)
        {
            return ResultError(406, title);
        }
        #endregion


    }
}
