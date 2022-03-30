using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Caviar.Infrastructure.API.BaseApi;
using Caviar.SharedKernel.Entities.View;
using Caviar.SharedKernel.Entities;
using Caviar.Core.Services;
using Caviar.Core.Interface;

namespace Caviar.Infrastructure.API.CodeGeneration
{
    /// <summary>
    /// 代码生成控制器
    /// </summary>
    public class CodeGenerationController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> CodeFileGenerate(CodeGenerateOptions codeGenerateOptions,bool isPerview)
        {
            CodeGenerationServices CodeService = CreateService<CodeGenerationServices>();
            //获取该类的所有字段
            var fieldsData = FieldScannerServices.GetClassFields(codeGenerateOptions.EntitieName, codeGenerateOptions.FullName, LanguageService);//类信息
            var entityData = FieldScannerServices.GetEntity(codeGenerateOptions.EntitieName, codeGenerateOptions.FullName,LanguageService);//类下字段信息
            var result = CodeService.CodePreview(entityData,fieldsData, codeGenerateOptions,""); //生成预览代码
            if (!isPerview)
            {
                var apiCount = 0;
                //生成api
                if (codeGenerateOptions.IsGenerateController)
                {
                    var dbContext = CreateService<IAppDbContext>();
                    apiCount = await ApiScannerServices.CreateInitApi(dbContext.DbContext, codeGenerateOptions);
                }
                //将生成的代码输出
                var msg = CodeService.WriteCodeFile(result, codeGenerateOptions);
                msg = $"生成api {apiCount}个，" + msg;
                return Ok(msg);
            }
            return Ok(result);
        }

        /// <summary>
        /// 只能获取自身字段
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> GetFields()
        {
            var fields = await GetFields<SysFields>();
            return Ok(fields);
        }
    }
}
