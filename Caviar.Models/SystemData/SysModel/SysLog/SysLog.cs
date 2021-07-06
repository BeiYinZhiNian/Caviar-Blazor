using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    [DisplayName("日志")]
    public partial class SysLog:SysBaseModel
    {
        [DisplayName("用户名")]
        [StringLength(50, ErrorMessage = "用户名请不要超过{1}个字符")]
        public string UserName { get; set; }
        [DisplayName("用户id")]
        public int? UserId { get; set; }
        [DisplayName("访问地址")]
        [StringLength(1024, ErrorMessage = "访问地址请不要超过{1}个字符")]
        public string AbsoluteUri { get; set; }
        [DisplayName("IP")]
        [StringLength(50, ErrorMessage = "IP请不要超过{1}个字符")]
        public string Ipaddress { get; set; }
        [DisplayName("执行时间")]
        public double Elapsed { get; set; }
        [DisplayName("状态码")]
        public int Status { get; set; }
        [DisplayName("执行消息")]
        [StringLength(50, ErrorMessage = "执行消息请不要超过{1}个字符")]
        public string Msg { get; set; }
        [DisplayName("浏览器")]
        [StringLength(1024, ErrorMessage = "浏览器请不要超过{1}个字符")]
        public string Browser { get; set; }
        [DisplayName("日志等级")]
        public CavLogLevel LogLevel { get; set; }
        [DisplayName("自动记录")]
        public bool IsAutomatic { get; set; }
        [DisplayName("请求方法")]
        [StringLength(10, ErrorMessage = "请求方法请不要超过{1}个字符")]
        public string Method { get; set; }
    }

    public enum CavLogLevel
    {
        //
        // 摘要:
        //     Logs that contain the most detailed messages. These messages may contain sensitive
        //     application data. These messages are disabled by default and should never be
        //     enabled in a production environment.
        [Display(Name = "输出")]
        Trace = 0,
        //
        // 摘要:
        //     Logs that are used for interactive investigation during development. These logs
        //     should primarily contain information useful for debugging and have no long-term
        //     value.
        [Display(Name = "调试")]
        Debug = 1,
        //
        // 摘要:
        //     Logs that track the general flow of the application. These logs should have long-term
        //     value.
        [Display(Name = "信息")]
        Information = 2,
        //
        // 摘要:
        //     Logs that highlight an abnormal or unexpected event in the application flow,
        //     but do not otherwise cause the application execution to stop.
        [Display(Name = "警告")]
        Warning = 3,
        //
        // 摘要:
        //     Logs that highlight when the current flow of execution is stopped due to a failure.
        //     These should indicate a failure in the current activity, not an application-wide
        //     failure.
        [Display(Name = "错误")]
        Error = 4,
        //
        // 摘要:
        //     Logs that describe an unrecoverable application or system crash, or a catastrophic
        //     failure that requires immediate attention.
        [Display(Name = "关键")]
        Critical = 5,
        //
        // 摘要:
        //     Not used for writing log messages. Specifies that a logging category should not
        //     write any messages.
        [Display(Name = "无")]
        None = 6
    }
}
