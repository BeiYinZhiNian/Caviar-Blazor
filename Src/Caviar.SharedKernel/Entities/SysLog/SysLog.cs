using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public partial class SysLog: SysUseEntity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [StringLength(256, ErrorMessage = "LengthErrorMsg")]
        public string UserName { get; set; }
        /// <summary>
        /// 请求的控制器
        /// </summary>
        [StringLength(256, ErrorMessage = "LengthErrorMsg")]
        public string ControllerName { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// 请求的完整地址
        /// </summary>
        [StringLength(1024, ErrorMessage = "LengthErrorMsg")]
        public string AbsoluteUri { get; set; }
        /// <summary>
        /// 请求ip
        /// </summary>
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string Ipaddress { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public double Elapsed { get; set; }
        public HttpStatusCode Status { get; set; }
        /// <summary>
        /// 日志消息
        /// </summary>
        [StringLength(2048, ErrorMessage = "LengthErrorMsg")]
        public string Msg { get; set; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        [StringLength(1024, ErrorMessage = "LengthErrorMsg")]
        public string Browser { get; set; }
        /// <summary>
        /// 日志等级
        /// </summary>
        public CavLogLevel LogLevel { get; set; }
        /// <summary>
        /// 是否为自动记录
        /// </summary>
        public bool IsAutomatic { get; set; }
        /// <summary>
        /// 请求方法
        /// </summary>
        [StringLength(10, ErrorMessage = "LengthErrorMsg")]
        public string Method { get; set; }
        /// <summary>
        /// post所提交的数据
        /// </summary>
        public string PostData { get; set; }
    }

    public enum CavLogLevel
    {
        //
        // 摘要:
        //     Logs that contain the most detailed messages. These messages may contain sensitive
        //     application data. These messages are disabled by default and should never be
        //     enabled in a production environment.
        Trace = 0,
        //
        // 摘要:
        //     Logs that are used for interactive investigation during development. These logs
        //     should primarily contain information useful for debugging and have no long-term
        //     value.
        Debug = 1,
        //
        // 摘要:
        //     Logs that track the general flow of the application. These logs should have long-term
        //     value.
        Information = 2,
        //
        // 摘要:
        //     Logs that highlight an abnormal or unexpected event in the application flow,
        //     but do not otherwise cause the application execution to stop.
        Warning = 3,
        //
        // 摘要:
        //     Logs that highlight when the current flow of execution is stopped due to a failure.
        //     These should indicate a failure in the current activity, not an application-wide
        //     failure.
        Error = 4,
        //
        // 摘要:
        //     Logs that describe an unrecoverable application or system crash, or a catastrophic
        //     failure that requires immediate attention.
        Critical = 5,
        //
        // 摘要:
        //     Not used for writing log messages. Specifies that a logging category should not
        //     write any messages.
        None = 6
    }
}
