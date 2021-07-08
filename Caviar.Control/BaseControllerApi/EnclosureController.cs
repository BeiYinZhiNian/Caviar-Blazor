using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.Enclosure
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
            if(files.Length==0) return ResultError("未找到实体文件");
            double length = (double)files.Length / 1024 / 1024;
            if (length > CaviarConfig.EnclosureConfig.Size) return ResultError("上传文件大小超过限制");
            ResultMsg.Data = await _Action.Upload(files);
            if(ResultMsg.Data == null)
            {
                return ResultError("文件保存失败");
            }
            return ResultOK();
        }
    }
}
