using Caviar.Core;
using Caviar.Core.Services.CodeGenerationServices;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caviar.Infrastructure.API.BaseApi;

namespace Caviar.Infrastructure.API.CodeGeneration
{
    /// <summary>
    /// 代码生成控制器
    /// </summary>
    public class CodeGenerationController : BaseApiController
    {
        [HttpPost]
        public IActionResult CodeFileGenerate(CodeGenerateOptions codeGenerateOptions,bool isPerview)
        {
            CodeGenerationServices CodeService = CreateService<CodeGenerationServices>();
            //获取该类的所有字段
            var fieldsData = FieldScannerServices.GetClassFields(codeGenerateOptions.EntitieName, codeGenerateOptions.FullName);//类信息
            var entityData = FieldScannerServices.GetEntity(codeGenerateOptions.EntitieName, codeGenerateOptions.FullName);//类下字段信息
            var result = CodeService.CodePreview(entityData,fieldsData, codeGenerateOptions,Configure.CaviarConfig,""); //生成预览代码
            if (!isPerview)
            {
                //将生成的代码输出
                var msg = CodeService.WriteCodeFile(result, codeGenerateOptions);
                return Ok(msg);
            }
            return Ok(result);
        }
    }
}
