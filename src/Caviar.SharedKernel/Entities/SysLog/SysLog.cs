using Microsoft.Extensions.Logging;
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
        /// 日志跟踪id
        /// </summary>
        [StringLength(256)]
        public string TraceId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [StringLength(256)]
        public string UserName { get; set; }
        /// <summary>
        /// 请求的控制器
        /// </summary>
        [StringLength(256)]
        public string ControllerName { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// 请求的完整地址
        /// </summary>
        [StringLength(1024)]
        public string AbsoluteUri { get; set; }
        /// <summary>
        /// 请求ip
        /// </summary>
        [StringLength(50)]
        public string Ipaddress { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public double Elapsed { get; set; }
        public HttpStatusCode Status { get; set; }
        /// <summary>
        /// 日志消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        [StringLength(1024)]
        public string Browser { get; set; }
        /// <summary>
        /// 日志等级
        /// </summary>
        public LogLevel LogLevel { get; set; }
        /// <summary>
        /// 请求方法
        /// </summary>
        [StringLength(10)]
        public string Method { get; set; }
        /// <summary>
        /// post所提交的数据
        /// </summary>
        public string PostData { get; set; }
    }
}
