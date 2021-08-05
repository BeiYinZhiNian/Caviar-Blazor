using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    [DisplayName("SysEnclosure")]
    public partial class SysEnclosure: SysBaseEntity, IBaseEntity
    {
        [Required(ErrorMessage = "RequiredErrorMsg")]
        [DisplayName("FileName")]
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string FileName { get; set; }
        [DisplayName("FileExtend")]
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string FileExtend { get; set; }
        /// <summary>
        /// M为点位
        /// </summary>
        [DisplayName("FileSize")]
        public double FileSize { get; set; }
        [DisplayName("FilePath")]
        [StringLength(1024, ErrorMessage = "LengthErrorMsg")]
        public string FilePath { get; set; }
        [DisplayName("FileUse")]
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string FileUse { get; set; }
    }
}
