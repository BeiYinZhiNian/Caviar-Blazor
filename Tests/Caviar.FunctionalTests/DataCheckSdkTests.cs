using Caviar.Core.Services;
using Caviar.Infrastructure.API;
using Caviar.SharedKernel.View;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Caviar.SharedKernel;
using Caviar.Infrastructure;

namespace Caviar.FunctionalTests
{
    [TestClass]
    public class DataCheckSdkTests:BaseApiController<ViewMenu, SysMenu>
    {
        ResultDataFilter DataCheckSdk = new ResultDataFilter();
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

        public IActionResult LocalRedirectResult()
        {
            return LocalRedirect("http://www.baidu.com");
        }

        [TestMethod]
        public void OkResultTest()
        {
            var result = OkResult();
            var resultMsg = DataCheckSdk.ResultHandle(result);
            Assert.AreEqual(StatusCodes.Status200OK, resultMsg.Status);
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
            Assert.AreEqual(StatusCodes.Status404NotFound, resultMsg.Status);
        }

        [TestMethod]
        public void LocalRedirectResultTest()
        {
            var result = LocalRedirectResult();
            var resultMsg = DataCheckSdk.ResultHandle(result);
            Assert.AreEqual(null, resultMsg);
        }
    }
}
