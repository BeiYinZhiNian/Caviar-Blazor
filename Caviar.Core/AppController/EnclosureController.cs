using Caviar.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Enclosure
{
    public partial class EnclosureController
    {
        /// <summary>
        /// 用于头像上传
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm(Name = CurrencyConstant.HeadPortrait)] IFormFile files)
        {
            
            var result = await _Action.Upload(files);
            return Ok(result);
        }
    }
}
