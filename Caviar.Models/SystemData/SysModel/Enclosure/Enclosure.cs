using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    [DisplayName("附件")]
    public partial class Enclosure:SysBaseModel
    {
        [Required(ErrorMessage = "请输入附件名称")]
        [DisplayName("名称")]
        [StringLength(50, ErrorMessage = "附件名称请不要超过{1}个字符")]
        public string Name { get; set; }
        [DisplayName("文件类型")]
        [StringLength(50, ErrorMessage = "文件类型名请不要超过{1}个字符")]
        public string Extend { get; set; }
        /// <summary>
        /// M为点位
        /// </summary>
        [DisplayName("文件大小")]
        public int Size { get; set; }
        [DisplayName("文件位置")]
        [StringLength(1024, ErrorMessage = "文件路径请不要超过{1}个字符")]
        public string Path { get; set; }

    }
}
