using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public partial class SysEnclosure: SysUseEntity
    {
        [Required(ErrorMessage = "RequiredErrorMsg")]
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string FileName { get; set; }
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string FileExtend { get; set; }
        /// <summary>
        /// M为单位
        /// </summary>
        public double FileSize { get; set; }
        [StringLength(1024, ErrorMessage = "LengthErrorMsg")]
        public string FilePath { get; set; }
    }
}
