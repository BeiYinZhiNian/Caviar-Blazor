using Caviar.Core.Services.CodeGenerationServices;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.API.CodeGeneration
{
    /// <summary>
    /// 代码生成控制器
    /// </summary>
    public class CodeGenerationController : BaseApiController
    {
        CodeGenerationServices CodeService;
        public CodeGenerationController()
        {
            //创建service
            CodeService = CreateService<CodeGenerationServices>();
        }
        public IActionResult CodeFileGenerate(CodeGenerateOptions codeGenerateOptions,bool isPerview)
        {
            var result = CodeService.CodePreview(codeGenerateOptions);
            if (isPerview)
            {
                //将生成的代码输出
            }
            return Ok(result);
        }
    }
}
