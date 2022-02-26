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
        public const string DefaultUserAgent = "Caviar-Server";
        public const string UserAgent = "User-Agent";
        /// <summary>
        /// 默认密码
        /// </summary>
        public const string DefaultPassword = "123456";
        /// <summary>
        /// 头像文件夹名称
        /// </summary>
        public const string HeadPortrait = "headPortrait";
        /// <summary>
        /// 提交地址
        /// </summary>
        public const string CavModelSubmitUrl = "SubmitUrl";
        /// <summary>
        /// controllerName名称
        /// </summary>
        public const string ControllerName = "ControllerName";
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
        /// 首页key
        /// </summary>
        public const string HomeKey = "Home";
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
        public const string GetPermissionMenus = "GetPermissionMenus";
        /// <summary>
        /// 保存菜单权限
        /// </summary>
        public const string SavePermissionMenu = "SavePermissionMenus";
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
        public const string Index = "Index";
        /// <summary>
        /// 菜单key
        /// </summary>
        public const string SysMenuKey = "SysMenu";
        /// <summary>
        /// 角色key
        /// </summary>
        public const string ApplicationRoleKey = "ApplicationRole";
        /// <summary>
        /// 用户key
        /// </summary>
        public const string ApplicationUserKey = "ApplicationUser";
        /// <summary>
        /// 用户组key
        /// </summary>
        public const string SysUserGroupKey = "SysUserGroup";
        /// <summary>
        /// 系统日志key
        /// </summary>
        public const string SysLogKey = "SysLog";
        /// <summary>
        /// 字段权限key
        /// </summary>
        public const string FieldPermissionsKey = "FieldPermissions";
        /// <summary>
        /// 用户角色key
        /// </summary>
        public const string PermissionUserRolesKey = "PermissionUserRoles";
        /// <summary>
        /// 菜单权限
        /// </summary>
        public const string MenuPermissionsKey = "MenuPermissions";
        /// <summary>
        /// 分配角色
        /// </summary>
        public const string AssignRolesKey = "AssignRoles";
        /// <summary>
        /// 获取指定用户角色
        /// </summary>
        public const string GetUserRoles = "GetUserRoles";
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
        /// <summary>
        /// 默认语言
        /// </summary>
        public const string DefaultLanguage = "zh-CN";

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
        public const string ExceptionMessage = "SharedKernel.ExceptionMessage";
        public const string Page = "SharedKernel.Page";
        public const string AppNull = "app.null";
        public const string Admin = "admin";
        /// <summary>
        /// 未授权
        /// </summary>
        public const string Unauthorized = "Unauthorized";
        /// <summary>
        /// 服务器发生错误
        /// </summary>
        public const string InternalServerError = "InternalServerError";
        /// <summary>
        /// 自定义处理发生错误
        /// </summary>
        public const string NotificationException = "NotificationException";
        /// <summary>
        /// 登录过期
        /// </summary>
        public const string LoginExpiration = "Login expiration";
        /// <summary>
        /// 数据为空
        /// </summary>
        public const string Null = "null";
        /// <summary>
        /// 为找到资源
        /// </summary>
        public const string NotFound = "Not Found";

        /// <summary>
        /// 请选择图标
        /// </summary>
        public const string SelectIcon = "SelectIcon";
        /// <summary>
        /// 个人中心
        /// </summary>
        public const string PersonalCenter = "PersonalCenter";
        /// <summary>
        /// 修改密码
        /// </summary>
        public const string ChangePassword = "ChangePassword";
        /// <summary>
        /// 退出登录
        /// </summary>
        public const string Logout = "Logout";
        /// <summary>
        /// 选择查询的字段
        /// </summary>
        public const string SelectQueryFields = "SelectQueryFields";
        /// <summary>
        /// 查询
        /// </summary>
        public const string Query = "Query";
        /// <summary>
        /// 是
        /// </summary>
        public const string Yes = "Yes";
        /// <summary>
        /// 否
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 操作
        /// </summary>
        public const string Operation = "Operation";
        /// <summary>
        /// 更多
        /// </summary>
        public const string More = "More";
        /// <summary>
        /// 确认
        /// </summary>
        public const string Confirm = "Confirm";
        /// <summary>
        /// 取消
        /// </summary>
        public const string Cancel = "Cancel";
        /// <summary>
        /// 确认{menu.DisplayName}该数据吗
        /// </summary>
        public const string DeleteTitle = "DeleteTitle";
        /// <summary>
        /// {field.DisplayName}搜索
        /// </summary>
        public const string FieldSearch = "FieldSearch";
        /// <summary>
        /// 未找到{title}组件，请检查url地址：{url}
        /// </summary>
        public const string ComponentErrorMsg = "ComponentErrorMsg";
        /// <summary>
        /// 请选择{DataSource.Entity.Name}可访问的菜单
        /// </summary>
        public const string SelectAccessibleMenu = "SelectAccessibleMenu";
        /// <summary>
        /// 用户登录
        /// </summary>
        public const string UserLogin = "UserLogin";
        /// <summary>
        /// 请输入用户名/手机号/邮箱
        /// </summary>
        public const string UserName = "UserName";
        /// <summary>
        /// 请输入密码
        /// </summary>
        public const string Password = "Password";
        /// <summary>
        /// 注册账户
        /// </summary>
        public const string RegisteredAccount = "RegisteredAccount";
        /// <summary>
        /// 忘记密码
        /// </summary>
        public const string ForgetPassword = "ForgetPassword";
        /// <summary>
        /// 请为用户分配部门
        /// </summary>
        public const string UserGroupRuleErrorMsg = "UserGroupRuleErrorMsg";
        /// <summary>
        /// 请输入正确的手机号
        /// </summary>
        public const string PhoneNumberRuleErrorMsg = "PhoneNumberRuleErrorMsg";
        /// <summary>
        /// 请输入正确的邮箱
        /// </summary>
        public const string EmailRuleErrorMsg = "EmailRuleErrorMsg";
        /// <summary>
        /// 请分配部门
        /// </summary>
        public const string ParentMenuName = "ParentMenuName";
        /// <summary>
        /// 上一步
        /// </summary>
        public const string Back = "Back";
        /// <summary>
        /// 下一步
        /// </summary>
        public const string Next = "Next";
        /// <summary>
        /// 生成
        /// </summary>
        public const string Generate = "Generate";
        /// <summary>
        /// 生成内容
        /// </summary>
        public const string GenerateContent = "GenerateContent";
        /// <summary>
        /// 控制器
        /// </summary>
        public const string Controller = "Controller";
        /// <summary>
        /// 列表页面
        /// </summary>
        public const string ListPage = "ListPage";
        /// <summary>
        /// 数据模板页面
        /// </summary>
        public const string DataTemplatePage = "DataTemplatePage";
        /// <summary>
        /// 页面模型
        /// </summary>
        public const string PageModel = "PageModel";
        /// <summary>
        /// 返回首页
        /// </summary>
        public const string ReturnHome = "ReturnHome";
        /// <summary>
        /// 继续生成
        /// </summary>
        public const string ContinueGeneration = "ContinueGeneration";
        /// <summary>
        /// 选择生成模块
        /// </summary>
        public const string SelectGenerationModule = "SelectGenerationModule";
        /// <summary>
        /// 功能配置
        /// </summary>
        public const string FunctionConfiguration = "FunctionConfiguration";
        /// <summary>
        /// 查看代码
        /// </summary>
        public const string ViewCode = "ViewCode";
        /// <summary>
        /// 生成结果
        /// </summary>
        public const string GenerateResults = "GenerateResults";
        /// <summary>
        /// 代码生成完毕,代码生效需要关闭程序重新编译运行
        /// </summary>
        public const string ResultSubTitle = "ResultSubTitle";
        /// <summary>
        /// 共
        /// </summary>
        public const string Total = "Total";
        /// <summary>
        /// 条
        /// </summary>
        public const string Record = "Record";
        /// <summary>
        /// 无上层目录
        /// </summary>
        public const string NoUpperLevel = "NoUpperLevel";
        /// <summary>
        /// 代码生成只能在debug模式下进行！
        /// </summary>
        public const string DebugErrorMsg = "DebugErrorMsg";

    }
}
