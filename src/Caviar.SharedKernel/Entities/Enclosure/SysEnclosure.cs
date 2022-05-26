// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.ComponentModel.DataAnnotations;

namespace Caviar.SharedKernel.Entities
{
    public partial class SysEnclosure : SysUseEntity
    {
        [Required]
        [StringLength(50)]
        public string FileName { get; set; }
        [StringLength(50)]
        public string FileExtend { get; set; }
        /// <summary>
        /// M为单位
        /// </summary>
        public double FileSize { get; set; }
        [StringLength(1024)]
        public string FilePath { get; set; }
    }
}
