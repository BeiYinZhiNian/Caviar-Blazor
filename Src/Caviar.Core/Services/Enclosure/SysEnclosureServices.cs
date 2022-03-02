using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.Base;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    public class SysEnclosureServices : DbServices
    {
        public SysEnclosureServices(IAppDbContext dbContext) : base(dbContext)
        {
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="formFile">文件</param>
        /// <param name="enclosureConfig">配置</param>
        /// <returns></returns>
        /// <exception cref="NotificationException"></exception>
        public async Task<SysEnclosure> Upload(IFormFile formFile,EnclosureConfig enclosureConfig)
        {
            if (formFile.Length == 0) throw new NotificationException("未找到实体文件");
            double length = (double)formFile.Length / 1024 / 1024;
            if (length > enclosureConfig.LimitSize) throw new NotificationException("上传文件大小超过限制");
            var extend = Path.GetExtension(formFile.FileName);
            SysEnclosure enclosure = new SysEnclosure
            {
                FileExtend = extend,//拓展名
                FileName = Guid.NewGuid().ToString() + extend,//文件名
                FilePath = formFile.Name,
                FileSize = Math.Round(length, 3),
            };
            var filePath = enclosureConfig.Path + "/" + enclosure.FilePath;//储存路径
            var path = enclosureConfig.CurrentDirectory + "/" + filePath + "/";//物理路径
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (var stream = File.Create(path + enclosure.FileName))
            {
                await formFile.CopyToAsync(stream);
            }
            await AppDbContext.AddEntityAsync(enclosure);
            return enclosure;
        }
    }
}
