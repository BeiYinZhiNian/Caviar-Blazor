using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Caviar.Control.Permission;

namespace Caviar.Control
{
    public partial class CaviarBaseController
    {
        protected IAssemblyDynamicCreation CavAssembly => BC.HttpContext.RequestServices.GetService<IAssemblyDynamicCreation>();

        CaviarBaseAction BaseAction => new CaviarBaseAction() { BC = BC };

        #region API
        /// <summary>
        /// 代码生成
        /// </summary>
        /// <param name="generate"></param>
        /// <param name="isPerview">是否预览</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CodeFileGenerate(CodeGenerateData generate,bool isPerview = true)
        {
            var result = await BaseAction.CodeFileGenerate(generate, isPerview);
            return Ok(result);
        }
        #endregion
    }
}
