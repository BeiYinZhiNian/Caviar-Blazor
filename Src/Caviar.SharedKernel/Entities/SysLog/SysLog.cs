using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public partial class SysLog: SysUseEntity
    {
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string UserName { get; set; }
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string ControllerName { get; set; }
        public int? UserId { get; set; }
        [StringLength(1024, ErrorMessage = "LengthErrorMsg")]
        public string AbsoluteUri { get; set; }
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string Ipaddress { get; set; }
        public double Elapsed { get; set; }
        public int Status { get; set; }
        [StringLength(2048, ErrorMessage = "LengthErrorMsg")]
        public string Msg { get; set; }
        [StringLength(1024, ErrorMessage = "LengthErrorMsg")]
        public string Browser { get; set; }
        public CavLogLevel LogLevel { get; set; }
        public bool IsAutomatic { get; set; }
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
        [Display(Name = "Trace")]
        Trace = 0,
        //
        // 摘要:
        //     Logs that are used for interactive investigation during development. These logs
        //     should primarily contain information useful for debugging and have no long-term
        //     value.
        [Display(Name = "Debug")]
        Debug = 1,
        //
        // 摘要:
        //     Logs that track the general flow of the application. These logs should have long-term
        //     value.
        [Display(Name = "Information")]
        Information = 2,
        //
        // 摘要:
        //     Logs that highlight an abnormal or unexpected event in the application flow,
        //     but do not otherwise cause the application execution to stop.
        [Display(Name = "Warning")]
        Warning = 3,
        //
        // 摘要:
        //     Logs that highlight when the current flow of execution is stopped due to a failure.
        //     These should indicate a failure in the current activity, not an application-wide
        //     failure.
        [Display(Name = "Error")]
        Error = 4,
        //
        // 摘要:
        //     Logs that describe an unrecoverable application or system crash, or a catastrophic
        //     failure that requires immediate attention.
        [Display(Name = "Critical")]
        Critical = 5,
        //
        // 摘要:
        //     Not used for writing log messages. Specifies that a logging category should not
        //     write any messages.
        [Display(Name = "None")]
        None = 6
    }
}
