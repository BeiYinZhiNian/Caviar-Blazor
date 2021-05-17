using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBTypeEnum { SqlServer, MySql, PgSql, Memory, SQLite, Oracle }
    /// <summary>
    /// 按钮类型
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// 目录
        /// </summary>
        [Display(Name = "目录")]
        Catalog,
        /// <summary>
        /// 菜单
        /// </summary>
        [Display(Name = "菜单")]
        Menu,
        /// <summary>
        /// 按钮
        /// </summary>
        [Display(Name = "按钮")]
        Button,
    }
    /// <summary>
    /// 打开类型
    /// </summary>
    public enum TargetType
    {
        /// <summary>
        /// 当前页面
        /// </summary>
        [Display(Name = "当前页面")]
        CurrentPage,
        /// <summary>
        /// 弹出窗口
        /// </summary>
        [Display(Name = "弹出窗口")]
        EjectPage,
        /// <summary>
        /// 新建页面
        /// </summary>
        [Display(Name = "新建页面")]
        NewLabel,
        /// <summary>
        /// 回调
        /// </summary>
        [Display(Name = "回调")]
        Callback,
    }

    /// <summary>
    /// 按钮位置
    /// </summary>
    public enum ButtonPosition
    {
        [Display(Name = "表头")]
        Header,
        [Display(Name = "行")]
        Row,
    }

}
