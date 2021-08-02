using Caviar.SharedKernel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Enclosure
{
    public partial class EnclosureAction
    {
        public async Task<ResultMsg<ViewEnclosure>> Upload(IFormFile formFile)
        {
            if (formFile.Length == 0) return Error<ViewEnclosure>("未找到实体文件");
            double length = (double)formFile.Length / 1024 / 1024;
            if (length > CaviarConfig.EnclosureConfig.Size) return Error<ViewEnclosure>("上传文件大小超过限制");
            var extend = Path.GetExtension(formFile.FileName);
            ViewEnclosure enclosure = new ViewEnclosure
            {
                Extend = extend,//拓展名
                Name = Guid.NewGuid().ToString() + extend,//文件名
                Path = formFile.Name,
                Size = Math.Round(length, 3),
                Use = formFile.Name
            };
            var filePath = CaviarConfig.EnclosureConfig.Path + "/" + enclosure.Path;//储存路径
            var path = CaviarConfig.EnclosureConfig.CurrentDirectory + "/" + filePath + "/";//物理路径
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (var stream = File.Create(path + enclosure.Name))
            {
                await formFile.CopyToAsync(stream);
            }
            await AddEntity(enclosure);
            return Ok(ToViewModel(enclosure));
        }
    }
}
