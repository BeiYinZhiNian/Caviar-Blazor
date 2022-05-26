// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.IO;

namespace Caviar.SharedKernel.Entities
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
            NameSpace = "Caviar.SharedKernel.Entities.View"
        };

        public JWTOptions JWTOptions { get; set; } = new JWTOptions();

        public EnclosureConfig EnclosureConfig { get; set; } = new EnclosureConfig()
        {
            LimitSize = 3, //限制3M大小文件
            Path = "caviar-data/", //文件储存路径
            CurrentDirectory = Directory.GetCurrentDirectory() + "/wwwroot/",
        };
        /// <summary>
        /// 是否允许游客浏览
        /// 游客自动继承游客角色权限
        /// </summary>
        public bool TouristVisit { get; set; }
        /// <summary>
        /// api是否为调试状态
        /// </summary>
        public bool IsDebug { get; set; }
        /// <summary>
        /// 是否处于演示模式
        /// </summary>
        public bool DemonstrationMode { get; set; }
        /// <summary>
        /// 用户配置的启动地址
        /// </summary>
        public string Urls { get; set; }
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

    public class EnclosureConfig
    {
        public double LimitSize { get; set; }

        public string Path { get; set; }

        public string CurrentDirectory { get; set; }
    }
}
