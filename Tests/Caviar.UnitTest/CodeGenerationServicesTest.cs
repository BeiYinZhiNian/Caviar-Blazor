using Caviar.Core.Services.CodeGenerationServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit.Sdk;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.View;
using System.Collections.Generic;

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
/// 生成者：{Producer}
/// 代码由代码生成器自动生成，更改的代码可能被进行替换
/// 可在上层目录使用partial关键字进行扩展
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
/// 生成者：test
/// 代码由代码生成器自动生成，更改的代码可能被进行替换
/// 可在上层目录使用partial关键字进行扩展
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
            List<ViewFields> list = new List<ViewFields>()
            {
                new ViewFields()
                {
                    Entity = new SysFields()
                    {
                        FieldName = "test",
                        FullName = "string",
                    },
                    EntityNamespace = "caviar.test"
                },
            };
            var result = codeGenerationServices.PreviewCodeReplace(fields, list, previewCode, "test");
            Assert.AreEqual(generateTxt, result.Content);

        }

    }
}
