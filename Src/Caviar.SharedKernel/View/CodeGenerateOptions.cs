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
        [DisplayName("IsGenerateIndex")]
        public bool IsGenerateIndex { get; set; } = true;
        [DisplayName("IsGenerateController")]
        public bool IsGenerateController { get; set; } = true;
        [DisplayName("IsGenerateDataTemplate")]
        public bool IsGenerateDataTemplate { get; set; } = true;
        [DisplayName("IsGenerateViewModel")]
        public bool IsGenerateViewModel { get; set; } = true;
        [DisplayName("实体")]
        public string EntitieName { get; set; }
        [DisplayName("命名空间")]
        public string FullName { get; set; }
        [DisplayName("标签名称")]
        [Required(ErrorMessage = "请输入要生成的标签名称")]
        public string LabelName { get; set; }
        /// <summary>
        /// 是否覆盖
        /// </summary>
        [DisplayName("是否覆盖")]
        public bool IsCover { get; set; }
    }
}
