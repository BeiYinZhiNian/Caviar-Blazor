using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public static class UrlConfig
    {
        /// <summary>
        /// 分配用户角色
        /// </summary>
        public const string PermissionUserRoles = "Permission/UserRoles";
        /// <summary>
        /// 获取菜单栏
        /// </summary>
        public const string GetMenuBar = "SysMenu/GetMenuBar";
        /// <summary>
        /// 字段权限url
        /// </summary>
        public const string FieldPermissionsUrl = "Permission/RoleFields";
        /// <summary>
        /// 菜单权限url
        /// </summary>
        public const string MenuPermissionsUrl = "Permission/RoleMenus";
        /// <summary>
        /// 视图类代码文件路径
        /// </summary>
        public const string CodeGenerateFilePath = "Template/File";
        /// <summary>
        /// 登录地址
        /// </summary>
        public const string Login = "ApplicationUser/Login";
        /// <summary>
        /// 附件映射路径
        /// </summary>
        public const string Enclosure = "Enclosure";
        /// <summary>
        /// 首页
        /// </summary>
        public const string Home = "/";
        /// <summary>
        /// 设置语言
        /// </summary>
        public const string SetCookieLanguage = "Permission/SetCookieLanguage";
        /// <summary>
        /// 退出登录
        /// </summary>
        public const string Logout = "ApplicationUser/Logout";
        /// <summary>
        /// server模式退出登录
        /// </summary>
        public const string LogoutServer = "ApplicationUser/LogoutServer";
        /// <summary>
        /// 用户基础信息
        /// </summary>
        public const string MyDetails = "ApplicationUser/MyUserDetails";
        /// <summary>
        /// 修改用户基础信息
        /// </summary>
        public const string UpdateDetails = "ApplicationUser/UpdateDetails";
        /// <summary>
        /// 修改密码
        /// </summary>
        public const string UpdatePwd = "ApplicationUser/UpdatePwd";

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public const string CurrentUserInfo  = "ApplicationUser/CurrentUserInfo";
        /// <summary>
        /// 登录
        /// </summary>
        public const string SignInActual = "ApplicationUser/SignInActual";
        /// <summary>
        /// 获取Api列表
        /// </summary>
        public const string GetApis = "SysMenu/GetApis";
        /// <summary>
        /// 数据范围
        /// </summary>
        public const string DataRange = "SysUserGroup/DataRange";
        /// <summary>
        /// 角色列表首页
        /// </summary>
        public const string RoleIndex = "ApplicationRole/Index";
        /// <summary>
        /// 上传文件
        /// </summary>
        public const string Upload = "SysEnclosure/Upload";
        /// <summary>
        /// 头像上传
        /// </summary>
        public const string UploadHeadPortrait = "SysEnclosure/UploadHeadPortrait";
        /// <summary>
        /// 网站设置
        /// </summary>
        public const string WebConfig = "Setting/WebConfig";
    }
}
