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
            throw new System.DllNotFoundException("接口暂未开放");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public override async Task<IActionResult> DeleteEntity(SysEnclosureView vm)
        {
            var result = await _sysEnclosureServices.Delete(vm);
            return Ok(result);
        }

        public override Task<IActionResult> CreateEntity(SysEnclosureView vm)
        {
            throw new System.DllNotFoundException("接口暂未开放");
        }

        /// <summary>
        /// 头像上传
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadHeadPortrait([FromForm(Name = "HeadPortrait")] IFormFile files)
        {
            var result = await _sysEnclosureServices.Upload(files);
            return Ok(result);
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile files)
        {
            var result = await _sysEnclosureServices.Upload(files);
            return Ok(result);
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Download(SysEnclosureView vm)
        {
            var result = await _sysEnclosureServices.Download(vm);
            return Ok(result);
        }
    }
}