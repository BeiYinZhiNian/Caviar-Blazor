using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.Extensions.DependencyInjection;
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
    public class LogServices<T>: BaseServices
    {
        public ILogger<T> Logger { get; set; }
        private readonly Interactor _interactor;
        private IAppDbContext _appDbContext;
        public LogServices(ILogger<T> logger,Interactor interactor, IServiceProvider serviceProvider)
        {
            Logger = logger;
            _interactor = interactor;
            _appDbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IAppDbContext>();
        }
        /// <summary>
        /// 最总保存日志处理，可以写到数据库也可以写到文件
        /// 当日志量过大，建议采用消息队列的方式控制
        /// 采用单独的dbContext不会使其不影响其他数据的处理
        /// </summary>
        /// <param name="log"></param>
        public SysLog LogSave(SysLog log)
        {
            //日志不执行权限操作，所以使用set
            var set = _appDbContext.DbContext.Set<SysLog>();
            set.Add(log);
            _appDbContext.DbContext.SaveChanges();
            return log;
        }

        public SysLog Log(SysLog log)
        {
            Logger.Log(log.LogLevel, log.Msg);
            return LogSave(log);
        }

        public SysLog CreateLog(string message, LogLevel logLevel,string postData = null, double elapsed = 0, HttpStatusCode status = HttpStatusCode.OK)
        {
            return new SysLog() 
            {
                TraceId = _interactor.TraceId.ToString(),
                UserName = _interactor.UserName,
                ControllerName = typeof(T).Name,
                UserId = _interactor.UserInfo?.Id,
                AbsoluteUri = _interactor.Current_AbsoluteUri,
                Ipaddress = _interactor.Current_Ipaddress,
                Elapsed = elapsed,
                Status = status,
                Msg = message,
                Browser = _interactor.Browser,
                LogLevel = logLevel,
                Method = _interactor.Method,
                PostData = postData,
                DataId = _interactor.UserInfo?.UserGroupId ?? -1,
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

        public SysLog Infro(string message)
        {
            Logger.LogInformation(message);
            var log = CreateLog(message,LogLevel.Information);
            return LogSave(log);
        }

        public SysLog Warning(string message)
        {
            Logger.LogWarning(message);
            var log = CreateLog(message, LogLevel.Warning);
            return LogSave(log);
        }

        public SysLog Error(string message)
        {
            Logger.LogError(message);
            var log = CreateLog(message, LogLevel.Error);
            return LogSave(log);
        }

        public SysLog Critical(string message)
        {
            Logger.LogCritical(message);
            var log = CreateLog(message, LogLevel.Critical);
            return LogSave(log);
        }
    }
}
