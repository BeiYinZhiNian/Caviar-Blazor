using Caviar.Models;
using Caviar.Models.SystemData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Caviar.Core
{
    public class SysLogAction
    {
        private IBaseControllerModel BC;
        public SysLogAction(IBaseControllerModel bc)
        {
            BC = bc;
        }

        public virtual void LoggerMsg(string msg, LogLevel logLevel = LogLevel.Information, int status = 200, bool IsAutomatic = false,ILogger logger = null)
        {
            if (logger == null)
            {
                logger = BC.HttpContext.RequestServices.GetRequiredService<ILogger<SysLogAction>>();
            }
            var log = new SysLog()
            {
                UserName = BC.UserName,
                AbsoluteUri = BC.Current_Action,
                ControllerName = BC.CurrentMenu?.MenuName,
                Ipaddress = BC.Current_Ipaddress,
                Elapsed = BC.Stopwatch.Elapsed.TotalMilliseconds,
                Status = status,
                LogLevel = (CavLogLevel)logLevel,
                Msg = msg,
                Method = BC.HttpContext.Request.Method,
                IsAutomatic = IsAutomatic
            };
            logger.Log(logLevel, msg,log);
            if (BC.HttpContext.Request.Headers.ContainsKey("User-Agent"))
            {
                log.Browser = BC.HttpContext.Request.Headers["User-Agent"];
            }
            if (BC.IsLogin)
            {
                log.UserId = BC.UserToken.Id;
            }
            if (log.Method.ToUpper() == "POST")
            {
                var json = JsonSerializer.Serialize(BC.ActionArguments);
                log.PostData = json;
            }
            var isAdd = FilterLog(log);
            if (!isAdd) return;
            var count = BC.DC.AddEntityAsync(log).Result;
        }
        public virtual void LoggerMsg<T>(string msg, LogLevel logLevel = LogLevel.Information, int status = 200, bool IsAutomatic = false)
        {
            var logger = BC.HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
            LoggerMsg(msg, logLevel, status, IsAutomatic, logger);
        }


        protected virtual bool FilterLog(SysLog log)
        {
            if ((int)log.LogLevel < 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
