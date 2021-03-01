using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData.Template
{
    public partial class Sys_Power_Menu:Sys_BaseModel
    {
        [DisplayName("类型")]
        public MenuType MenuType { get; set; }

        [DisplayName("打开方式")]
        public TargetType TargetType { get; set; }

        [DisplayName("菜单名称")]
        [StringLength(50, ErrorMessage = "菜单名称请不要超过{1}个字符")]
        public string MenuName { get; set; }

        [DisplayName("请求地址")]
        public string Url { get; set; }

        [DisplayName("图标")]
        public string Icon { get; set; }

        [DisplayName("顺序")]
        public int Order { get; set; }

        [DisplayName("上层id")]
        public int UpLayerId { get; set; }
    }


}
