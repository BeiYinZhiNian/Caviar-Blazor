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
        Menu,
        Field,
    }
    /// <summary>
    /// 权限身份
    /// </summary>
    public enum PermissionIdentity
    {
        User,
        Role,
        UserGroup
    }

    public enum HybridType
    {
        /// <summary>
        /// Application only works as Server
        /// </summary>
        ServerSide,
        /// <summary>
        /// Application only works as WebAssembly
        /// </summary>
        WebAssembly,
        /// <summary>
        /// Switch to WebAssembly manually by calling switchToWasm
        /// </summary>
        HybridManual,
        /// <summary>
        /// Switch to WebAssembly when navigating
        /// </summary>
        HybridOnNavigation,
        /// <summary>
        /// Switch to WebAssembly as soon as it loads
        /// </summary>
        HybridOnReady
    }

    public class HybridOptions
    {
        public HybridType HybridType { get; set; }
    }
}
