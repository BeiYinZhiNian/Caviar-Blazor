// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;

namespace Caviar.SharedKernel.Entities
{
    public static partial class CurrencyConstant
    {
        /// <summary>
        /// 不存在的指令
        /// </summary>
        public const string NonexistentInstruction = "Nonexistent instruction";
        /// <summary>
        /// 成功
        /// </summary>
        public const string Success = "success";
        /// <summary>
        /// 未就绪
        /// </summary>
        public const string NotReady = "not ready";
        /// <summary>
        /// 切换为wasm名称
        /// </summary>
        public const string SwitchWasm = "switch_wasm";
        /// <summary>
        /// 切换为server模式js名称
        /// </summary>
        public const string SwitchServer = "switch_server";
        /// <summary>
        /// 公共数据id
        /// </summary>
        public const int PublicData = -1;
        /// <summary>
        /// 数据源
        /// </summary>
        public const string DataSource = "DataSource";
        /// <summary>
        /// 时区
        /// </summary>
        public const string TimeZone = "Asia/Shanghai";
        /// <summary>
        /// 缓存代理Ip
        /// </summary>
        public const string XForwardedFor = "X-Forwarded-For";
        /// <summary>
        /// 搜索
        /// </summary>
        public const string Search = "Search";
        /// <summary>
        /// 高级搜索
        /// </summary>
        public const string AdvancedSearch = "AdvancedSearch";
        /// <summary>
        /// 关闭搜索
        /// </summary>
        public const string CancelSearch = "CancelSearch";
        /// <summary>
        /// 路由前缀
        /// </summary>
        public const string Api = "api/";
        /// <summary>
        /// 角色自定义数据分隔符
        /// </summary>
        public const string CustomDataSeparator = ";";
        /// <summary>
        /// 默认server模式的userAgent
        /// </summary>
        public const string DefaultUserAgent = "Caviar-Server";
        /// <summary>
        /// userAgent协议头
        /// </summary>
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
        /// 账号名称
        /// </summary>
        public const string AccountName = "AccountName";
        /// <summary>
        /// 游客登录
        /// </summary>
        public const string TouristVisit = "TouristVisit";
        /// <summary>
        /// 当前页面url
        /// </summary>
        public const string CurrentUrl = "CurrentUrl";
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
        /// 上传key
        /// </summary>
        public const string UploadKey = "Upload";
        /// <summary>
        /// 下载key
        /// </summary>
        public const string DownloadKey = "Download";
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
        /// 移除
        /// </summary>
        public const string Remove = "Remove";
        /// <summary>
        /// 系统日志key
        /// </summary>
        public const string SysLogKey = "SysLog";
        /// <summary>
        /// 附件key
        /// </summary>
        public const string SysEnclosureKey = "SysEnclosure";
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
        /// 网站配置
        /// </summary>
        public const string WebConfig = "WebConfig";
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
        /// <summary>
        /// 设置cookie
        /// </summary>
        public const string SetCookie = "setCookie";
        /// <summary>
        /// 获取cookies
        /// </summary>
        public const string GetCookie = "getCookie";
        /// <summary>
        /// 删除cookie
        /// </summary>
        public const string DelCookie = "delCookie";
        /// <summary>
        /// 布局主题
        /// </summary>
        public const string LayoutTheme = "layoutTheme";
        public const string EntitysName = "SharedKernel.EntitysName";
        public const string Menu = "SharedKernel.Menu";
        public const string ResuleMsg = "SharedKernel.ResuleMsg";
        public const string Enum = "SharedKernel.Enum";
        public const string ErrorMessage = "SharedKernel.ErrorMessage";
        public const string ExceptionMessage = "SharedKernel.ExceptionMessage";
        public const string Page = "SharedKernel.Page";
        public const string AppNull = "app.null";
        public const string Admin = "admin";
        public const string InconsistentPasswords = "Inconsistent passwords";
        /// <summary>
        /// 保存
        /// </summary>
        public const string Preservation = "Preservation";
        /// <summary>
        /// 模板角色
        /// </summary>
        public const string TemplateRole = "TemplateRole";
        /// <summary>
        /// 游客用户
        /// </summary>
        public const string TouristUser = "tourist";
        /// <summary>
        /// 游客角色
        /// </summary>
        public const string TouristRole = "Tourist";
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
        /// 选择类型
        /// </summary>
        public const string SelectType = "SelectType";
        /// <summary>
        /// 且
        /// </summary>
        public const string And = "And";
        /// <summary>
        /// 或
        /// </summary>
        public const string Or = "Or";
        /// <summary>
        /// 输入搜索内容
        /// </summary>
        public const string SearchData = "SearchData";
        /// <summary>
        /// 搜索条数
        /// </summary>
        public const string SearchNumber = "SearchNumber";
        /// <summary>
        /// 添加条件
        /// </summary>
        public const string AddCondition = "AddCondition";
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
        /// 请输入用户名
        /// </summary>
        public const string InputUserName = "InputUserName";
        /// <summary>
        /// 请输入密码
        /// </summary>
        public const string InputPassword = "InputPassword";
        /// <summary>
        /// 账号
        /// </summary>
        public const string UserName = "UserName";
        /// <summary>
        /// 密码
        /// </summary>
        public const string Password = "Password";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string UserGroupName = "UserGroupName";
        /// <summary>
        /// 手机号
        /// </summary>
        public const string PhoneNumber = "PhoneNumber";
        /// <summary>
        /// 注册账户
        /// </summary>
        public const string RegisteredAccount = "RegisteredAccount";
        /// <summary>
        /// 个性签名
        /// </summary>
        public const string PersonalSignature = "PersonalSignature";
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
        public const string AllocationDepartment = "AllocationDepartment";
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
        /// <summary>
        /// 是否覆盖
        /// </summary>
        public const string IsCover = "IsCover";
        /// <summary>
        /// 代码生成文件未找到，请确认路径是否正确：
        /// </summary>
        public const string RouteErrorMsg = "RouteErrorMsg";
        /// <summary>
        /// 文件夹为空则不进行任何替换
        /// </summary>
        public const string FolderErrorMsg = "FolderErrorMsg";
        /// <summary>
        /// 共{count}个文件，写出文件{writeCount}个，跳过文件{skipCount}个，覆盖文件{coverCount}个
        /// </summary>
        public const string GenerateMsg = "GenerateMsg";
        /// <summary>
        /// 当前密码
        /// </summary>
        public const string CurrentPassword = "CurrentPassword";
        /// <summary>
        /// 新密码
        /// </summary>
        public const string NewPassword = "NewPassword";
        /// <summary>
        /// 确认密码
        /// </summary>
        public const string ConfirmPassword = "ConfirmPassword";
        /// <summary>
        /// 请输入备注
        /// </summary>
        public const string InputRemark = "InputRemark";
        /// <summary>
        /// 个人中心
        /// </summary>
        public const string MyUserDetails = "MyUserDetails";
        /// <summary>
        /// 布局设置
        /// </summary>
        public const string LayoutSettings = "LayoutSettings";
        /// <summary>
        /// 运行模式
        /// </summary>
        public const string OperationMode = "OperationMode";
    }
}
