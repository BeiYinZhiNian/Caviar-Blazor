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
        /// 字段权限url
        /// </summary>
        public const string FieldPermissionsUrl = "Permission/RoleFields";
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
        /// 用户详细信息
        /// </summary>
        public const string MyDetails = "ApplicationUser/MyDetails";
        /// <summary>
        /// 修改密码
        /// </summary>
        public const string UpdatePwd = "ApplicationUser/UpdatePwd";

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public const string CurrentUserInfo  = "ApplicationUser/CurrentUserInfo";
        /// <summary>
        /// 获取菜单列表
        /// </summary>
        public const string GetApiList = "SysMenu/GetMenuList";
        
    }
}
