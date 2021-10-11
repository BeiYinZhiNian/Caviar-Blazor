using Caviar.SharedKernel;
using Caviar.SharedKernel.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services.CodeGenerationServices
{
    /// <summary>
    /// 代码生成服务
    /// </summary>
    public class CodeGenerationServices : BaseServices
    {
        ///// <summary>
        ///// 代码预览
        ///// </summary>
        ///// <param name="codeGenerateOptions">代码生成配置类</param>
        ///// <returns>代码结果</returns>
        //public List<CodePreviewTab> CodePreview(CodeGenerateOptions codeGenerateOptions)
        //{

        //}
        /// <summary>
        /// 预览生成的代码
        /// </summary>
        /// <param name="entityName">实体名称</param>
        /// <param name="suffixName">后缀名</param>
        /// <param name="extendName">扩展名</param>
        /// <returns></returns>
        public PreviewCode PreviewCode(string entityName,string suffixName,string extendName)
        {
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}{CurrencyConstant.CodeGenerateFilePath}{suffixName}.txt";
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("代码生成文件未找到，请确认路径是否正确：" + path);
            }
            string txt = File.ReadAllText(path);
            string name = entityName + suffixName + extendName;
            PreviewCode codePreviewTab = new PreviewCode()
            {
                TabName = name,
                KeyName = name,
                Content = txt
            };
            return codePreviewTab;
        }
        /// <summary>
        /// 替换代码文件中内容
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="codePreview"></param>
        /// <param name="producer"></param>
        /// <returns></returns>
        public PreviewCode PreviewCodeReplace(ViewFields fields, PreviewCode codePreview,string producer)
        {
            StringBuilder txt = new StringBuilder(codePreview.Content);
            txt = txt.Replace("{GenerationTime}", DateTime.Now.ToString());
            txt = txt.Replace("{Producer}", producer);
            txt = txt.Replace("{EntityNamespace}", fields.EntityNamespace);
            txt = txt.Replace("{EntityName}", fields.Entity.FieldName);
            codePreview.Content = txt.ToString();
            return codePreview;
        }

    }
}
