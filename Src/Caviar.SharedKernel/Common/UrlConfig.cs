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

        public const string Logout = "ApplicationUser/Logout";

        public const string MyDetails = "ApplicationUser/MyDetails";

        public const string UpdatePwd = "ApplicationUserUpdatePwd";
        public const string CurrentUserInfo  = "ApplicationUser/CurrentUserInfo";
        public const string GetApiList = "SysMenu/GetApiList";
        public const string SetCookieLanguage = "SysMenu/SetCookieLanguage";
    }
}
