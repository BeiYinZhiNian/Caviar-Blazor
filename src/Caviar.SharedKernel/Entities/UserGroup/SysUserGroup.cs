// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.ComponentModel.DataAnnotations;

namespace Caviar.SharedKernel.Entities
{
    /// <summary>
    /// 部门/用户组
    /// </summary>
    public class SysUserGroup : SysUseEntity
    {
        /// <summary>
        /// 用户组名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 父id
        /// </summary>
        public int ParentId { get; set; }
    }
}
