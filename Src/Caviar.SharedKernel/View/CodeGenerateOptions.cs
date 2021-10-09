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
        [DisplayName("IsGeneratePage")]
        public bool IsGeneratePage { get; set; }
        [DisplayName("IsGenerateController")]
        public bool IsGenerateController { get; set; }
        [DisplayName("IsGenerateApi")]
        public bool IsGenerateApi { get; set; }
        [DisplayName("EntitieName")]
        public string EntitieName { get; set; }
        [DisplayName("EntitieName")]
        public string GenerateNamespace { get; set; } = "Caviar.CodeGeneration";
        /// <summary>
        /// 需要生成的实体对象
        /// </summary>
        public ViewFields ViewFields { get; set; }
        /// <summary>
        /// 是否覆盖
        /// </summary>
        public bool IsCover { get; set; }
    }
}
