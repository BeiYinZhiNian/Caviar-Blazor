// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.ComponentModel.DataAnnotations;

namespace Caviar.SharedKernel.Entities
{
    public class SysPermission : SysBaseEntity
    {
        [StringLength(200)]
        public string Permission { get; set; }
        /// <summary>
        /// 实体id
        /// </summary>
        [StringLength(200)]
        public int Entity { get; set; }
        /// <summary>
        /// 权限类型
        /// </summary>
        public int PermissionType { get; set; }
    }
}
