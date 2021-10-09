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

        public CodePreviewTab ReadCodePreviewTab(string entityName,string fileName,string extendName)
        {
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}{CurrencyConstant.CodeGenerateFilePath}{fileName}.txt";
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("代码生成文件未找到，请确认路径是否正确：" + path);
            }
            string txt = File.ReadAllText(path);
            string name = entityName + fileName + extendName;
            CodePreviewTab codePreviewTab = new CodePreviewTab()
            {
                TabName = name,
                KeyName = name,
                Content = txt
            };
            return codePreviewTab;
        }

    }
}
