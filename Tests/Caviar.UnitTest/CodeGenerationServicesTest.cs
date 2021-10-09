using Caviar.Core.Services.CodeGenerationServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit.Sdk;

namespace Caviar.UnitTest
{
    [TestClass]
    public class CodeGenerationServicesTest
    {
        [TestMethod]
        public void TestGenerateViewClassServices()
        {
            CodeGenerationServices codeGenerationServices = new CodeGenerationServices();
            var entitieName = "TestEntitie";
            var tab = codeGenerationServices.ReadCodePreviewTab(entitieName,"View",".cs");
            Assert.IsNotNull(tab);
            Assert.AreNotEqual(tab.Content, "");
        }
    }
}
