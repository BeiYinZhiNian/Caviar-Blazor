﻿// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Threading.Tasks;
using Caviar.Core.Interface;
using Caviar.Core.Services;
using Caviar.Infrastructure.API.BaseApi;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Mvc;

namespace Caviar.Infrastructure.API.CodeGeneration
{
    /// <summary>
    /// 代码生成控制器
    /// </summary>
    public class CodeGenerationController : BaseApiController
    {
        private readonly ILanguageService _languageService;
        public CodeGenerationController(ILanguageService languageService)
        {
            _languageService = languageService;
        }
        [HttpPost]
        public async Task<IActionResult> CodeFileGenerate(CodeGenerateOptions codeGenerateOptions, bool isPerview)
        {
            CodeGenerationServices codeService = CreateService<CodeGenerationServices>();
            //获取该类的所有字段
            var fieldsData = FieldScannerServices.GetClassFields(codeGenerateOptions.EntitieName, codeGenerateOptions.FullName, _languageService);//类信息
            var entityData = FieldScannerServices.GetEntity(codeGenerateOptions.EntitieName, codeGenerateOptions.FullName, _languageService);//类下字段信息
            var result = codeService.CodePreview(entityData, fieldsData, codeGenerateOptions, ""); //生成预览代码
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
                var msg = codeService.WriteCodeFile(result, codeGenerateOptions);
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
