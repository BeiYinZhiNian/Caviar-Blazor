using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.Enclosure
{
    public partial class EnclosureAction
    {
        public async Task<List<ViewEnclosure>> Upload(List<IFormFile> files)
        {
            List<SysEnclosure> list = new List<SysEnclosure>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    double length = (double)formFile.Length / 1024 / 1024;
                    var extend = Path.GetExtension(formFile.FileName);
                    SysEnclosure enclosure = new SysEnclosure
                    {
                        Extend = extend,//拓展名
                        Name = Guid.NewGuid().ToString() + extend,//文件名
                        Path = formFile.Name,
                        Size = Math.Round(length, 3),
                        Use = formFile.Name
                    };
                    var filePath = CaviarConfig.EnclosureConfig.Path + "/" + enclosure.Path;//储存路径
                    var path = Directory.GetCurrentDirectory() + "/" + filePath + "/";//物理路径
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (var stream = File.Create(path + enclosure.Name))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    await AddEntity(enclosure);
                    list.Add(enclosure);
                }
            }
            return ModelToViewModel(list);
        }
    }
}
