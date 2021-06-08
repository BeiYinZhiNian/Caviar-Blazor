using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData.SysModel.Permission
{
    public class SysTableHeader:SysBaseModel
    {
        /// <summary>
        /// 所属实体名称
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// 表头名称
        /// </summary>
        public string HeaderName { get; set; }
    }
}
