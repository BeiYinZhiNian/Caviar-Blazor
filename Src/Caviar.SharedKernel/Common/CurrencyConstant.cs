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
        /// 获取多个实体
        /// </summary>
        public const string GetEntitysKey = "GetEntitys";
        /// <summary>
        /// 获取授权菜单列表
        /// </summary>
        public const string GetMenusKey = "GetMenus";
        /// <summary>
        /// 获取所有字段
        /// </summary>
        public const string GetFieldsKey = "GetFields";
        /// <summary>
        /// 保存角色字段权限
        /// </summary>
        public const string SaveRoleFields = "SaveRoleFields";
        /// <summary>
        /// 首页
        /// </summary>
        public const string HomeIndex = "Index";
        /// <summary>
        /// 角色key
        /// </summary>
        public const string ApplicationRoleKey = "ApplicationRole";
        /// <summary>
        /// 用户key
        /// </summary>
        public const string ApplicationUserKey = "ApplicationUser";
        /// <summary>
        /// 字段权限key
        /// </summary>
        public const string FieldPermissionsKey = "FieldPermissions";
        /// <summary>
        /// 菜单权限
        /// </summary>
        public const string MenuPermissionsKey = "MenuPermissions";
        
        /// <summary>
        /// 权限key
        /// </summary>
        public const string PermissionKey = "Permission";
        /// <summary>
        /// JWT验证
        /// </summary>
        public const string JWT = "JWT ";
        /// <summary>
        /// 基础实体名称
        /// </summary>
        public static string BaseEntityName = typeof(SysUseEntity).Name;
       

        public const string ServerName = "server";

        public static Dictionary<string, string> LanguageDic = new Dictionary<string, string>() 
        {
            {"zh-CN","中文" },
            {"en-US","English" },
        };

        public const string JsIframeMessage = "iframeMessage";

        public const string LanguageHeader = "Current-Language";

        public const string EntitysName = "SharedKernel.EntitysName";
        public const string Menu = "SharedKernel.Menu";
        public const string ResuleMsg = "SharedKernel.ResuleMsg";
        public const string Enum = "SharedKernel.Enum";
        public const string ErrorMessage = "SharedKernel.ErrorMessage";
        public const string AppNull = "app.null";
        public const string Admin = "admin";
       
    }
}
