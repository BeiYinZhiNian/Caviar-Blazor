using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    public class LogServices<T>: DbServices
    {
        public ILogger<T> Logger { get; set; }
        Interactor Interactor { get; set; }
        public LogServices(IAppDbContext dbContext,ILogger<T> logger,Interactor interactor) : base(dbContext)
        {
            Logger = logger;
            Interactor = interactor;
        }
        /// <summary>
        /// 最总保存日志处理，可以写道数据库也可以写道文件
        /// 当日志量过大，建议采用消息队列的方式控制
        /// </summary>
        /// <param name="log"></param>
        public void LogSave(SysLog log)
        {
            //日志不执行权限操作，所以使用set
            var set = AppDbContext.DbContext.Set<SysLog>();
            set.Add(log);
            AppDbContext.DbContext.SaveChanges();
        }

        public void Log(SysLog log)
        {
            Logger.Log(log.LogLevel, log.Msg);
            LogSave(log);
        }

        public SysLog CreateLog(string message, LogLevel logLevel,string postData = null, double elapsed = 0, HttpStatusCode status = HttpStatusCode.OK)
        {
            return new SysLog() 
            {
                TraceId = Interactor.TraceId.ToString(),
                UserName = Interactor.UserName,
                ControllerName = typeof(T).Name,
                UserId = Interactor.UserInfo?.Id,
                AbsoluteUri = Interactor.Current_AbsoluteUri,
                Ipaddress = Interactor.Current_Ipaddress,
                Elapsed = elapsed,
                Status = status,
                Msg = message,
                Browser = Interactor.Browser,
                LogLevel = logLevel,
                Method = Interactor.Method,
                PostData = postData
            };
        }

        public void Debug(string message)
        {
            Logger.LogDebug(message);
        }

        public void Trace(string message)
        {
            Logger.LogTrace(message);
        }

        public void Infro(string message)
        {
            Logger.LogInformation(message);
            var log = CreateLog(message,LogLevel.Information);
            LogSave(log);
        }

        public void Warning(string message)
        {
            Logger.LogWarning(message);
            var log = CreateLog(message, LogLevel.Warning);
            LogSave(log);
        }

        public void Error(string message)
        {
            Logger.LogError(message);
            var log = CreateLog(message, LogLevel.Error);
            LogSave(log);
        }

        public void Critical(string message)
        {
            Logger.LogCritical(message);
            var log = CreateLog(message, LogLevel.Critical);
            LogSave(log);
        }
    }
}
