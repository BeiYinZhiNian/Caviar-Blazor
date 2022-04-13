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
        [Required]
        [StringLength(50)]
        public string FileName { get; set; }
        [StringLength(50)]
        public string FileExtend { get; set; }
        /// <summary>
        /// M为单位
        /// </summary>
        public double FileSize { get; set; }
        [StringLength(1024)]
        public string FilePath { get; set; }
    }
}
