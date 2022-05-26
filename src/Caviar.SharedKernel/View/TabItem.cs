// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

namespace Caviar.SharedKernel.Entities.View
{
    /// <summary>
    /// 代码预览tab
    /// </summary>
    public class PreviewCode
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string KeyName { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string TabName { get; set; }
        /// <summary>
        /// 代码内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        public CodeGeneration Options { get; set; }
    }
}
