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
        public async Task<IActionResult> Upload([FromForm(Name = "headPortrait")] List<IFormFile> files)
        {
            if (files.Count != 1) return ResultError("上传数量错误");
            double length = (double)files.Sum(u => u.Length) / 1024 / 1024;
            if (length > CaviarConfig.EnclosureConfig.Size) return ResultError("上传总文件大小超过限制");
            ResultMsg.Data = (await _Action.Upload(files))[0];
            return ResultOK();
        }
    }
}
