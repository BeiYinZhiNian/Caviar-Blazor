using Caviar.Core.Services.CodeGenerationServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit.Sdk;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.View;

namespace Caviar.UnitTest
{
    [TestClass]
    public class CodeGenerationServicesTest
    {
        [TestMethod]
        public void TestPreviewCodeView()
        {
            CodeGenerationServices codeGenerationServices = new CodeGenerationServices();
            var entitieName = "TestEntitie";
            var suffixName = "View";
            var extendName = ".cs";
            var fileName = entitieName + suffixName + extendName;
            var tab = codeGenerationServices.PreviewCode(entitieName, suffixName, extendName);
            Assert.IsNotNull(tab);
            Assert.AreNotEqual(tab.Content, "");
            Assert.AreEqual(fileName, tab.KeyName);
        }

        [TestMethod]
        public void TestPreviewCodeController()
        {
            CodeGenerationServices codeGenerationServices = new CodeGenerationServices();
            var entitieName = "TestEntitie";
            var suffixName = "Controller";
            var extendName = ".cs";
            var fileName = entitieName + suffixName + extendName;
            var tab = codeGenerationServices.PreviewCode(entitieName, suffixName, extendName);
            Assert.IsNotNull(tab);
            Assert.AreNotEqual(tab.Content, "");
            Assert.AreEqual(fileName, tab.KeyName);
        }

        [TestMethod]
        public void TestPreviewCodeReplace()
        {
            CodeGenerationServices codeGenerationServices = new CodeGenerationServices();
            var txt = @"using Caviar.SharedKernel.Entities;
using System.ComponentModel;
using {EntityNamespace};
/// <summary>
/// �����ߣ�{Producer}
/// �����ɴ����������Զ����ɣ����ĵĴ�����ܱ������滻
/// �����ϲ�Ŀ¼ʹ��partial�ؼ��ֽ�����չ
/// </summary>
namespace {EntityNamespace}.View
{
    [DisplayName({EntityName})]
    public partial class View{EntityName} : BaseView<{EntityName}>
    {
    }
}
";
            var generateTxt = @"using Caviar.SharedKernel.Entities;
using System.ComponentModel;
using caviar.test;
/// <summary>
/// �����ߣ�test
/// �����ɴ����������Զ����ɣ����ĵĴ�����ܱ������滻
/// �����ϲ�Ŀ¼ʹ��partial�ؼ��ֽ�����չ
/// </summary>
namespace caviar.test.View
{
    [DisplayName(test)]
    public partial class Viewtest : BaseView<test>
    {
    }
}
";
            PreviewCode previewCode = new PreviewCode()
            {
                Content = txt
            };
            ViewFields fields = new ViewFields()
            {
                Entity = new SysFields()
                {
                    FieldName = "test"
                },
                EntityNamespace = "caviar.test"
            };
            var result = codeGenerationServices.PreviewCodeReplace(fields, previewCode, "test");
            Assert.AreEqual(generateTxt, result.Content);

        }
    }
}
