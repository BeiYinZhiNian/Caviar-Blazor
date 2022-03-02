using Caviar.Infrastructure.API.BaseApi;
using System.ComponentModel;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Caviar.Core.Services;

namespace Caviar.Infrastructure.API
{
    /// <summary>
    /// SysEnclosureController
    /// </summary>
    public partial class SysEnclosureController
    {
        SysEnclosureServices _sysEnclosureServices;
        public SysEnclosureController(SysEnclosureServices enclosureServices)
        {
            _sysEnclosureServices = enclosureServices;
        }

        public override Task<IActionResult> UpdateEntity(SysEnclosureView vm)
        {
            throw new NotificationException("接口暂未开放");
        }

        public Task<IActionResult> Download(SysEnclosureView vm)
        {
            return Task.FromResult(Ok());
        }

        public override Task<IActionResult> CreateEntity(SysEnclosureView vm)
        {
            throw new NotificationException("接口暂未开放");
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile files)
        {
            var result = await _sysEnclosureServices.Upload(files,Configure.CaviarConfig.EnclosureConfig);
            return Ok(result);
        }
    }
}