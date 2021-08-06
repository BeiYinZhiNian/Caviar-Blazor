using Caviar.Core.Services;
using Caviar.Core.Services.BaseSdk;
using Caviar.Infrastructure.API;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.FunctionalTests
{
    [TestClass]
    public class DataCheckSdkTests:BaseApiController<BaseSdk<SysMenu>, SysMenu>
    {
        DataCheckSdk DataCheckSdk = new DataCheckSdk();
        public DataCheckSdkTests()
        {
        }
        public IActionResult OkResult()
        {
            return Ok();
        }

        public IActionResult OkObjectResult()
        {
            return Ok("test");
        }

        public IActionResult OkObjectResult(PageData<SysMenu> pageData)
        {
            return Ok(pageData);
        }

        public IActionResult NotFoundResult()
        {
            return NotFound();
        }

        [TestMethod]
        public void OkResultTest()
        {
            var result = OkResult();
            var resultMsg = DataCheckSdk.ResultHandle(result);
            Assert.AreEqual(HttpState.OK, resultMsg.Status);
        }

        [TestMethod]
        public void OkObjectResultTest()
        {
            var result = OkObjectResult();
            var resultMsg = DataCheckSdk.ResultHandle(result);
            Assert.AreEqual("test", resultMsg.Title);
        }

        [TestMethod]
        public void OkObjectResultTest1()
        {
            var result = OkObjectResult(new PageData<SysMenu>());
            var resultMsg = DataCheckSdk.ResultHandle(result);
            Assert.IsTrue(resultMsg is ResultMsg<object>);
        }

        [TestMethod]
        public void NotFoundResultTest()
        {
            var result = NotFoundResult();
            var resultMsg = DataCheckSdk.ResultHandle(result);
            Assert.AreEqual(HttpState.NotFound, resultMsg.Status);
        }
    }
}
