using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    /// <summary>
    /// 按钮类型
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// 目录
        /// </summary>
        Catalog,
        /// <summary>
        /// 菜单
        /// </summary>
        Menu,
        /// <summary>
        /// 按钮
        /// </summary>
        Button,
        /// <summary>
        /// API
        /// </summary>
        API,
    }
    /// <summary>
    /// 打开类型
    /// </summary>
    public enum TargetType
    {
        /// <summary>
        /// 当前页面
        /// </summary>
        CurrentPage,
        /// <summary>
        /// 弹出窗口
        /// </summary>
        EjectPage,
        /// <summary>
        /// 新建页面
        /// </summary>
        NewLabel,
        /// <summary>
        /// 回调
        /// </summary>
        Callback,
    }

    /// <summary>
    /// 按钮位置
    /// </summary>
    public enum ButtonPosition
    {
        Default,
        Row,
    }

    /// <summary>
    /// 权限类型
    /// </summary>
    public enum PermissionType
    {
        /// <summary>
        /// 角色字段
        /// </summary>
        RoleFields,
        /// <summary>
        /// 角色菜单
        /// </summary>
        RoleMenus
    }
    /// <summary>
    /// 数据范围
    /// </summary>
    public enum DataRange
    {
        /// <summary>
        /// 本级别
        /// </summary>
        Level,
        /// <summary>
        /// 本级和下级
        /// </summary>
        Subordinate,
        /// <summary>
        /// 自定义
        /// </summary>
        Custom,
        /// <summary>
        /// 所有数据权限
        /// </summary>
        All,
        
    }


}
