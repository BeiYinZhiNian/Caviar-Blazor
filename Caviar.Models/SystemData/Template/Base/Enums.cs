using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData.Template
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBTypeEnum { SqlServer, MySql, PgSql, Memory, SQLite, Oracle }
    public enum MenuType
    {
        [Display(Name = "目录")]
        Catalog,
        [Display(Name = "菜单")]
        Menu,
        [Display(Name = "按钮")]
        Button
    }

    public enum TargetType
    {
        [Display(Name = "当前页面")]
        CurrentPage,
        [Display(Name = "弹出窗口")]
        EjectPage,
        [Display(Name = "新建页面")]
        NewLabel
    }

}
