using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities.Base
{
    public class CaviarConfig
    {
        public CodeGeneration IndexOptions { get; set; } = new CodeGeneration()
        {
            StorePath = "../Caviar.Demo.AntDesignUI/Template/",
            NameSpace = "Caviar.AntDesignUI.Pages"
        };
        public CodeGeneration DataTemplateOptions { get; set; } = new CodeGeneration()
        {
            StorePath = "../Caviar.Demo.AntDesignUI/Template/",
            NameSpace = "Caviar.AntDesignUI.Pages"
        };
        public CodeGeneration ControllerOptions { get; set; } = new CodeGeneration()
        {
            StorePath = "../Caviar.Demo.WebApi/Template/API/",
            NameSpace = "Caviar.Infrastructure.API"
        };

        public CodeGeneration ViewModelOptions { get; set; } = new CodeGeneration()
        {
            StorePath = "../Caviar.Demo.Model/Template/View/",
            NameSpace = "Caviar.SharedKernel.View"
        };
    }

    public class CodeGeneration
    {
        /// <summary>
        /// 代码生成的储存路径
        /// </summary>
        public string StorePath { get; set; }
        /// <summary>
        /// 类命名空间
        /// </summary>
        public string NameSpace { get; set; }
    }
}
