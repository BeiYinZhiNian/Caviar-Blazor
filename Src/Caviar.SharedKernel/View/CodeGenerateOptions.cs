using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.View
{
    public class CodeGenerateOptions
    {
        [DisplayName("视图名称")]
        [Required(ErrorMessage = "视图名称名称必填")]
        public string ViewName { get; set; }
        [DisplayName("服务名称")]
        [Required(ErrorMessage = "服务名称必填")]
        public string ServiceName { get; set; }
        [DisplayName("控制器名称")]
        [Required(ErrorMessage = "控制器名称必填")]
        public string ControllerName { get; set; }
        [DisplayName("实体名称")]
        [Required(ErrorMessage = "实体名称必填")]
        public string EntityName { get; set; }
        [Required(ErrorMessage = "目录名称必填")]
        public string OutName { get; set; }
        [Required(ErrorMessage = "模块名称必填")]
        public string ModelName { get; set; }

        public string EntityNamespace { get; set; }

        public string EntityDisplayName { get; set; }

        public string[] Page { get; set; }

        public string[] Button { get; set; }

        public string[] Config { get; set; }

        public string[] WebApi { get; set; }
    }
}
