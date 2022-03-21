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
        public string wwwroot = "wwwroot/";
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
            if(formFile == null) throw new FileNotFoundException("未找到实体文件");
            if (formFile.Length == 0) throw new FileNotFoundException("未找到实体文件");
            double length = (double)formFile.Length / 1024 / 1024;
            length = Math.Round(length, 2);
            if (length > enclosureConfig.LimitSize) throw new FileLoadException($"上传文件{length}M,超过{enclosureConfig.LimitSize}M限制");
            var extend = Path.GetExtension(formFile.FileName);
            SysEnclosure enclosure = new SysEnclosure
            {
                FileExtend = extend,//拓展名
                FileName = formFile.FileName,//文件名
                FileSize = Math.Round(length, 3),
            };
            var filePath = enclosureConfig.Path + enclosure.FilePath;//储存路径
            var dir = enclosureConfig.CurrentDirectory + filePath;//物理路径
            var guidName = Guid.NewGuid().ToString() + extend;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var path = dir + guidName;
            enclosure.FilePath = enclosureConfig.Path + guidName;
            using (var stream = File.Create(path))
            {
                await formFile.CopyToAsync(stream);
            }
            await AppDbContext.AddEntityAsync(enclosure);
            return enclosure;
        }

        public async Task<bool> Delete(SysEnclosureView enclosure, EnclosureConfig enclosureConfig)
        {
            if (enclosure == null) throw new ArgumentNullException($"{nameof(enclosure)}参数为空");
            if(enclosure.Entity==null) throw new ArgumentNullException($"{nameof(enclosure.Entity)}参数为空");
            var entity = await AppDbContext.SingleOrDefaultAsync<SysEnclosure>(u=>u.Id == enclosure.Entity.Id);
            if(entity == null) throw new ArgumentNullException($"文件未找到");
            var filePath = $"{enclosureConfig.CurrentDirectory}{entity.FilePath}";
            if (!File.Exists(filePath)) throw new FileNotFoundException($"{enclosure.Entity.FileName}文件不存在");
            File.Delete(filePath);
            return await AppDbContext.DeleteEntityAsync(entity);
        }

        public async Task<string> Download(SysEnclosureView enclosure)
        {
            if (enclosure == null) throw new ArgumentNullException($"{nameof(enclosure)}参数为空");
            if (enclosure.Entity == null) throw new ArgumentNullException($"{nameof(enclosure.Entity)}参数为空");
            var entity = await AppDbContext.SingleOrDefaultAsync<SysEnclosure>(u => u.Id == enclosure.Entity.Id);
            if (entity == null) throw new ArgumentNullException($"文件未找到");
            return enclosure.Entity.FilePath;
        }
    }
}
