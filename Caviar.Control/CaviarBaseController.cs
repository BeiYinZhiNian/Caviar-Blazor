using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using Caviar.Models;
using Microsoft.Extensions.Primitives;
using System.Web;
using Caviar.Control.Role;
using System.Threading.Tasks;
using Caviar.Control.Permission;
using Caviar.Control.Menu;
using System.Linq;
using System.Reflection;

namespace Caviar.Control
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public partial class CaviarBaseController : Controller
    {
        IBaseControllerModel _controllerModel;
        protected IBaseControllerModel BC
        {
            get
            {
                if (_controllerModel == null)
                {
                    _controllerModel = HttpContext.RequestServices.GetService<IBaseControllerModel>();
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
            BC.Current_Ipaddress = context.HttpContext.GetUserIp();
            //获取完整Url
            BC.Current_AbsoluteUri = context.HttpContext.Request.GetAbsoluteUri();
            //获取请求路径
            BC.Current_Action = context.HttpContext.Request.Path.Value;
            //请求上下文
            BC.HttpContext = HttpContext;
            //请求参数
            BC.ActionArguments = context.ActionArguments;
            if (HttpContext.Request.Headers.TryGetValue("UsreToken", out StringValues value))
            {
                string base64url = value[0];
                string base64 = HttpUtility.UrlDecode(base64url);
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
                BC.UserToken = userToken;
            }
            GetPermission();
            ResultMsg.ModelFields = BC.UserData.ModelFields;
            var IsVerification = ActionVerification();
            if (!IsVerification)
            {
                if (BC.IsLogin)
                {
                    context.Result = ResultForbidden("对不起，您还没有获得该权限");
                }
                else
                {
                    context.Result = ResultUnauthorized("请您先登录");
                }
                return;
            }
        }
        /// <summary>
        /// 获取用户角色和权限
        /// 可以做缓存，未做
        /// </summary>
        /// <returns></returns>
        void GetPermission()
        {
            BC.SysModelFields = BC.DC.GetAllAsync<SysModelFields>().Result;
            var roleAction = CreateModel<RoleAction>();
            BC.UserData.Roles = roleAction.GetCurrentRoles().Result;
            var permissionAction = CreateModel<PermissionAction>();
            BC.UserData.Permissions = permissionAction.GetCurrentRolePermissions(BC.UserData.Roles).Result;
            BC.UserData.ModelFields = permissionAction.GetRoleFields();
            var menuAction = CreateModel<MenuAction>();
            BC.UserData.Menus = menuAction.GetPermissionMenu(BC.UserData.Permissions).Result;
        } 

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            stopwatch.Stop();
            //日志记录，这里应该想一个更好的办法
            
        }
        
        #region 创建模型
        protected virtual T CreateModel<T>() where T : class, IActionModel
        {
            var entity = BC.HttpContext.RequestServices.GetRequiredService<T>();
            entity.BC = BC;
            return entity;
        }
        #endregion



        protected virtual bool ActionVerification()
        {
            if (CaviarConfig.IsDebug) return true;
            var url = BC.Current_Action.Replace("/api/", "").ToLower();
            var menu = BC.UserData.Menus.FirstOrDefault(u => !string.IsNullOrEmpty(u.Url) && u.Url.ToLower() == url);
            if (menu != null) return true;
            return false;
        }

        #region  日志消息
        protected void LoggerMsg<T>(string msg, string action = "", LogLevel logLevel = LogLevel.Information, bool IsSucc = true)
        {
            BC.GetLogger<T>().LogInformation($"用户：{BC.UserName} 访问地址：{BC.Current_AbsoluteUri} 访问ip：{BC.Current_Ipaddress} 执行时间：{stopwatch.Elapsed} 执行结果：{IsSucc} 执行消息：{msg}");
        }
        #endregion


        #region 消息回复
        private CavResultMsg _resultMsg;
        protected CavResultMsg ResultMsg
        {
            get
            {
                if (_resultMsg == null)
                {
                    _resultMsg = HttpContext.RequestServices.GetRequiredService<CavResultMsg>();
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

        protected virtual IActionResult ResultFound(string title,string url)
        {
            ResultMsg.Status = 302;
            ResultMsg.Title = title;
            ResultMsg.Type = url;
            return ResultError(ResultMsg);

        }
        /// <summary>
        /// 返回（无权限）
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
            ResultMsg.Status = 401;
            ResultMsg.Title = title;
            ResultMsg.Type = "/User/Login";//此处后续读取数据库
            return ResultError(ResultMsg);
        }

        protected virtual IActionResult ResultError(string title, string detail)
        {
            return ResultError(406, title, detail);
        }
        protected virtual IActionResult ResultError(string title)
        {
            return ResultError(406, title);
        }
        #endregion

        protected class CavResultMsg : ResultMsg
        {
            /// <summary>
            /// 用于存放附加数据
            /// </summary>
            public new object Data { get { return _data; } set { Filter(value); } }

            public List<SysModelFields> ModelFields { private get; set; }

            private object _data;
            /// <summary>
            /// 过滤字段
            /// 以后用反射进行过滤
            /// </summary>
            /// <param name="data"></param>
            private void Filter(object data)
            {
                _data = data;
                if (_data == null) return;
                var type = _data.GetType();
                ArgumentsModel(type,_data);
            }

            /// <summary>
            /// 遍历类型中所有包含IBaseModel的子类
            /// 过滤模型
            /// </summary>
            /// <param name="type"></param>
            private void ArgumentsModel(Type type,object data)
            {
                if (!type.IsClass)//排除非类
                {
                    return;
                }
                else if (type == typeof(string))//排除字符串（特殊类）
                {
                    return;
                }
                bool isBaseModel;
                isBaseModel = type.GetInterfaces().Contains(typeof(IBaseModel));
                if (isBaseModel)
                {
                    if (data == null) return;
                    //去过滤参数
                    ArgumentsFields(type, data);
                }
                else if (type.GetInterfaces().Contains(typeof(System.Collections.ICollection)))
                {
                    var list = (System.Collections.IEnumerable)data;
                    foreach (var dataItem in list)
                    {
                        ArgumentsModel(dataItem.GetType(), dataItem);
                    }
                }
                else
                {
                    foreach (PropertyInfo sp in type.GetProperties())//获得类型的属性字段
                    {
                        var properType = sp.PropertyType;
                        var value = sp.GetValue(data, null);
                        ArgumentsModel(properType, value);
                    }
                }

            }

            /// <summary>
            /// 过滤参数
            /// </summary>
            /// <param name="type"></param>
            /// <param name="data"></param>
            private void ArgumentsFields(Type type,object data)
            {
                var baseType = CommonHelper.GetCavBaseType(type);
                if (baseType == null) return;
                foreach (PropertyInfo sp in baseType.GetProperties())//获得类型的属性字段
                {
                    if (sp.Name.ToLower() == "id") continue;//忽略id字段
                    if (sp.Name.ToLower() == "uid") continue;//忽略uid字段
                    var field = ModelFields.FirstOrDefault(u => u.BaseTypeName == baseType.Name && sp.Name == u.TypeName);
                    if (field == null)
                    {
                        try
                        {
                            sp.SetValue(data, default, null);//设置为默认字段
                        }
                        catch
                        {
                            //忽略该错误,并记录到错误日志
                        }
                    }
                }

            }
        }

    }


}
