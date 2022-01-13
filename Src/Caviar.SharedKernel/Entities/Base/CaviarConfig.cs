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
            StorePath = "../Caviar.Demo.Wasm/Template/",
            NameSpace = "Caviar.Demo.Wasm.Pages"
        };
        public CodeGeneration DataTemplateOptions { get; set; } = new CodeGeneration()
        {
            StorePath = "../Caviar.Demo.Wasm/Template/",
            NameSpace = "Caviar.Demo.Wasm.Pages"
        };
        public CodeGeneration ControllerOptions { get; set; } = new CodeGeneration()
        {
            StorePath = "../Caviar.Demo.Hybrid/Template/API/",
            NameSpace = "Caviar.Infrastructure.API"
        };

        public CodeGeneration ViewModelOptions { get; set; } = new CodeGeneration()
        {
            StorePath = "../Caviar.Demo.Hybrid/Template/View/",
            NameSpace = "Caviar.SharedKernel.View"
        };

        public JWTOptions JWTOptions { get; set; } = new JWTOptions();
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

    public class JWTOptions
    {
        public string JwtSecurityKey { get; set; } = "RANDOM_KEY_MUST_NOT_BE_SHARED";

        public int JwtExpiryInDays { get; set; } = 1;

        public string JwtIssuer { get; set; } = "https://localhost";

        public string JwtAudience { get; set; } = "https://localhost";
    }
}
