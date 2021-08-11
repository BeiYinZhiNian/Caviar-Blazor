using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.View
{
    public class CodeGenerateData
    {
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
