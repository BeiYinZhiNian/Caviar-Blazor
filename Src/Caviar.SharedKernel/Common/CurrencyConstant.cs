using Caviar.SharedKernel.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public static class CurrencyConstant
    {
        /// <summary>
        /// 附件映射路径
        /// </summary>
        public const string Enclosure = "/Enclosure";
        /// <summary>
        /// 头像文件夹名称
        /// </summary>
        public const string HeadPortrait = "headPortrait";
        /// <summary>
        /// url名称
        /// </summary>
        public const string CavModelUrl = "CurrentUrl";
        /// <summary>
        /// jwt验证中token名称
        /// </summary>
        public const string TokenPayLoadName = "Data";
        /// <summary>
        /// 验证方式
        /// </summary>
        public const string Authorization = "Authorization";
        /// <summary>
        /// 系统管理key
        /// </summary>
        public const string SysManagementKey = "SysManagement";
        /// <summary>
        /// 代码生成key
        /// </summary>
        public const string CodeGenerationKey = "CodeGeneration";
        /// <summary>
        /// 创建按钮key
        /// </summary>
        public const string CreateEntityKey = "CreateEntity";
        /// <summary>
        /// 修改按钮key
        /// </summary>
        public const string UpdateEntityKey = "UpdateEntity";
        /// <summary>
        /// 删除按钮key
        /// </summary>
        public const string DeleteEntityKey = "DeleteEntity";
        /// <summary>
        /// 获取实体
        /// </summary>
        public const string GetEntityKey = "GetEntity";
        /// <summary>
        /// 获取所有字段
        /// </summary>
        public const string GetFieldsKey = "GetFields";
        /// <summary>
        /// 首页
        /// </summary>
        public const string HomeIndex = "Index";
        /// <summary>
        /// JWT验证
        /// </summary>
        public const string JWT = "JWT ";
        /// <summary>
        /// 基础实体名称
        /// </summary>
        public static string BaseEntityName = typeof(SysBaseEntity).Name;
        /// <summary>
        /// 视图类代码文件路径
        /// </summary>
        public const string CodeGenerateFilePath = "/Template/File/";

        public const string ServerName = "server";

        public static Dictionary<string, string> LanguageDic = new Dictionary<string, string>() 
        {
            {"zh-CN","中文" },
            {"en-US","English" },
        };

        public static string JsIframeMessage = "iframeMessage";

        public static string LanguageHeader = "Current-Language";

        public static string EntitysName = "SharedKernel.EntitysName";
        public static string MenuBar = "SharedKernel.MenuBar";
        public static string ResuleMsg = "SharedKernel.ResuleMsg";
        public static string Enum = "SharedKernel.Enum";
        public static string ErrorMessage = "SharedKernel.ErrorMessage";
        public static string AppNull = "app.null";
        /// <summary>
        /// 登录地址
        /// </summary>
        public static string Login { get; set; } = "ApplicationUser/Login";
    }
}
