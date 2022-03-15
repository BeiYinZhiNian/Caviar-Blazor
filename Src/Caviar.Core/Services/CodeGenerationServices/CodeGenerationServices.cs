using Caviar.SharedKernel.Entities.View;
using Caviar.SharedKernel.Entities.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caviar.SharedKernel.Entities;
using Microsoft.EntityFrameworkCore;
using Caviar.Core.Interface;

namespace Caviar.Core.Services
{
    /// <summary>
    /// 代码生成服务
    /// </summary>
    public class CodeGenerationServices : BaseServices
    {
        private ILanguageService _languageService;
        public CodeGenerationServices( ILanguageService languageService) : base()
        {
            _languageService = languageService;
        }

        public string WriteCodeFile(List<PreviewCode> previewCodes,CodeGenerateOptions codeGenerateOptions)
        {
            int count = 0;
            int writeCount = 0;
            int coverCount = 0;
            int skipCount = 0;
            foreach (var item in previewCodes)
            {
                count++;
                var storePath = item.Options.StorePath + $"{codeGenerateOptions.EntitieName}s/";
                if (!Directory.Exists(storePath))
                {
                    Directory.CreateDirectory(storePath);
                }
                var outName = storePath + item.KeyName;
                if (!File.Exists(outName) || codeGenerateOptions.IsCover)
                {
                    if (File.Exists(outName)) coverCount++;
                    string fileData = _languageService[$"{ CurrencyConstant.Page }.{ CurrencyConstant.FolderErrorMsg}"];
                    if (File.Exists(outName)) fileData = File.ReadAllText(outName);
                    if (!string.IsNullOrEmpty(fileData))
                    {
                        writeCount++;
                        File.WriteAllText(outName, item.Content);
                    }
                }
                else
                {
                    skipCount++;
                }
            }
            return _languageService[$"{ CurrencyConstant.Page }.{ CurrencyConstant.ResultMsg}"].Replace("{count}", count.ToString()).Replace("{writeCount}", writeCount.ToString()).Replace("{skipCount}", skipCount.ToString()).Replace("{coverCount}", coverCount.ToString());
        }

        /// <summary>
        /// 代码预览
        /// </summary>
        /// <param name="entityData">实体信息</param>
        /// <param name="fieldsData">实体字段信息</param>
        /// <param name="codeGenerateOptions">代码生成配置信息</param>
        /// <returns></returns>
        public List<PreviewCode> CodePreview(FieldsView entityData, List<FieldsView> fieldsData, CodeGenerateOptions codeGenerateOptions,CaviarConfig config,string producer)
        {
            List<PreviewCode> list = new List<PreviewCode>();
            var entitieName = codeGenerateOptions.EntitieName;
            if (codeGenerateOptions.IsGenerateController)
            {
                var suffixName = "Controller";
                var extendName = ".cs";
                var codePreview = GetPreviewCode(entitieName, suffixName, extendName, config.ControllerOptions);
                codePreview = PreviewCodeReplace(entityData, fieldsData, codePreview, codeGenerateOptions, producer);
                list.Add(codePreview);
            }
            if (codeGenerateOptions.IsGenerateDataTemplate)
            {
                var suffixName = "DataTemplate";
                var extendName = ".razor";
                var codePreview = GetPreviewCode(entitieName, suffixName, extendName, config.DataTemplateOptions);
                codePreview = PreviewCodeReplace(entityData, fieldsData, codePreview, codeGenerateOptions, producer);
                list.Add(codePreview);
            }
            if (codeGenerateOptions.IsGenerateIndex)
            {
                var suffixName = "Index";
                var extendName = ".razor";
                var codePreview = GetPreviewCode(entitieName, suffixName, extendName, config.IndexOptions);
                codePreview = PreviewCodeReplace(entityData, fieldsData, codePreview, codeGenerateOptions, producer);
                list.Add(codePreview);
            }
            if (codeGenerateOptions.IsGenerateViewModel)
            {
                var suffixName = "View";
                var extendName = ".cs";
                var codePreview = GetPreviewCode(entitieName, suffixName, extendName, config.ViewModelOptions);
                codePreview = PreviewCodeReplace(entityData, fieldsData, codePreview, codeGenerateOptions, producer);
                list.Add(codePreview);
            }
            return list;
        }
        /// <summary>
        /// 获取未修改预览生成的代码
        /// </summary>
        /// <param name="entityName">实体名称</param>
        /// <param name="suffixName">后缀名</param>
        /// <param name="extendName">扩展名</param>
        /// <returns></returns>
        protected PreviewCode GetPreviewCode(string entityName,string suffixName,string extendName, CodeGeneration options)
        {
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}{UrlConfig.CodeGenerateFilePath}/{suffixName}.txt";
            if (!File.Exists(path))
            { 
                throw new FileNotFoundException(_languageService[$"{ CurrencyConstant.Page }.{ CurrencyConstant.RouteErrorMsg}"] + path);
            }
            string txt = File.ReadAllText(path);
            string name = entityName + suffixName + extendName;
            PreviewCode codePreviewTab = new PreviewCode()
            {
                TabName = name,
                KeyName = name,
                Content = txt,
                Options = options
            };
            return codePreviewTab;
        }

        /// <summary>
        /// 替换文件生成内容
        /// </summary>
        /// <param name="entityData">类的信息</param>
        /// <param name="fieldsData">类的字段信息</param>
        /// <param name="codePreview">预览的代码</param>
        /// <param name="producer">生成者</param>
        /// <returns></returns>
        protected PreviewCode PreviewCodeReplace(FieldsView entityData,List<FieldsView> fieldsData, PreviewCode codePreview,CodeGenerateOptions codeGenerateOptions , string producer)
        {
            StringBuilder txt = new StringBuilder(codePreview.Content);
            var baseEntityNaemspace = entityData.Entity.FullName.Replace($".{entityData.Entity.FieldName}", "");
            txt = txt.Replace("{GenerationTime}", CommonHelper.GetSysDateTimeNow().ToString("yyyy-MM-dd HH:mm:ss"));
            txt = txt.Replace("{Producer}", producer);
            txt = txt.Replace("{EntityNamespace}", codePreview.Options.NameSpace);
            txt = txt.Replace("{BaseEntityNamespace}", baseEntityNaemspace);
            txt = txt.Replace("{EntityName}", entityData.Entity.FieldName);
            txt = txt.Replace("{FormItem}", CreateFormItem(fieldsData));
            txt = txt.Replace("{Lable}", codeGenerateOptions.EntitieName);
            codePreview.Content = txt.ToString();
            return codePreview;
        }

        /// <summary>
        /// 创建formItem
        /// </summary>
        /// <param name="generate"></param>
        /// <returns></returns>
        protected virtual string CreateFormItem(List<FieldsView> headers)
        {
            headers = CreateOrUpFilterField(headers);
            var html = "";
            foreach (var item in headers)
            {
                var txt = "";
                txt += $"<CavFormItem  FieldName='{item.Entity.FieldName}' FieldRules='@context.Entity'>";
                var IsWrite = CreateCurrencyAssembly(item, ref txt);
                txt += "</CavFormItem>";
                if (IsWrite) html += txt;
            }
            html = FormatHtml(html);
            return html;
        }

        /// <summary>
        /// 根据类型创建通用组件
        /// 先检查特殊组件是否创建，如果未创建则创建通用组件
        /// 如果通用组件未创建，则该字段丢弃
        /// </summary>
        /// <param name="item"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        protected virtual bool CreateCurrencyAssembly(FieldsView item, ref string txt)
        {
            var IsWrite = true;
            IsWrite = CreateSpecialAssembly(item, ref txt);
            if (IsWrite) return IsWrite;
            var modelType = item.EntityType.ToLower();
            switch (modelType)
            {
                case "string":
                    txt += $"<Input @bind-Value='@context.Entity.{item.Entity.FieldName}' />";
                    break;
                case "int32":
                    txt += $"<AntDesign.InputNumber @bind-Value='@context.Entity.{item.Entity.FieldName}' Style='width:50%'/>";
                    break;
                case "boolean":
                    txt += $"<Switch @bind-Value='@context.Entity.{item.Entity.FieldName}'/>";
                    break;
                case "datetime":
                    txt += $"<DatePicker @bind-Value='@context.Entity.{item.Entity.FieldName}'/>";
                    break;
                default:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 创建特殊组件
        /// </summary>
        /// <returns></returns>
        protected virtual bool CreateSpecialAssembly(FieldsView item, ref string txt)
        {
            if (item.IsEnum)
            {
                return CreateEnumAssembly(item, ref txt);
            }
            var modelType = item.Entity.FullName.ToLower();
            switch (modelType)
            {
                case "dataid"://数据权限
                    txt += @"<CavUserGroup DataSource='ViewUserGroups'
                                    UserGroupName='@UserGroupName'
                                    OnSelect='OnUserGroupSelect'
                                    OnCancel='OnUserGroupCancel'>
                                 </CavUserGroup>";
                    break;
                default:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 创建枚举组件
        /// </summary>
        /// <param name="item"></param>
        /// <param name="txt"></param>
        protected virtual bool CreateEnumAssembly(FieldsView item, ref string txt)
        {
            if (!item.IsEnum) return false;
            txt += $"<RadioGroup @bind-Value='@context.Entity.{item.Entity.FieldName}'>";
            foreach (var keyValue in item.EnumValueName)
            {
                txt += $"<Radio RadioButton Value='({item.Entity.FieldName}){keyValue.Key}'>{keyValue.Value}</Radio>";
            }

            txt += $"</RadioGroup>";
            return true;
        }

        /// <summary>
        /// 创建或修改时过滤字段
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        protected virtual List<FieldsView> CreateOrUpFilterField(List<FieldsView> headers)
        {
            if (headers == null) return null;
            string[] violation = new string[] { "id", "Uid", "CreatTime", "UpdateTime", "IsDelete", "OperatorCare", "OperatorUp", "ParentId","DataId" };
            var result = new List<FieldsView>();
            foreach (var item in headers)
            {
                if (violation.SingleOrDefault(u => u.ToLower() == item.Entity.FieldName.ToLower()) == null)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 将html代码格式化
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected virtual string FormatHtml(string html)
        {
            string[] arr = html.Split('>');
            string formatHtml = "";
            int count = 0;
            bool isSame = false;
            bool isFirst = true;
            foreach (var item in arr)
            {
                if (string.IsNullOrEmpty(item)) continue;
                var lene = item + ">";
                if (lene[0] != '<')
                {
                    formatHtml += lene;
                    isSame = true;
                }
                else
                {
                    if (isSame && lene.Contains("</"))
                    {
                        isSame = false;
                        count -= 1;
                    }
                    if (isFirst)
                    {
                        formatHtml = GetSpace(count) + lene;
                    }
                    else
                    {
                        formatHtml += "\r\n" + GetSpace(count) + lene;
                    }
                    isFirst = false;

                }
                if (lene.Contains("</"))
                {
                    count -= 1;
                    if (count < 0) count = 0;
                }
                else if (lene.Contains("/>"))
                {
                    isSame = true;
                }
                else
                {
                    count += 1;
                }

            }
            return formatHtml;
        }

        protected string GetSpace(int count)
        {
            string space = "    ";
            for (int i = 0; i < count; i++)
            {
                space += space;
            }
            return space;
        }

    }
}
