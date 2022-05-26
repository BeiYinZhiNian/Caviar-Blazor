// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Caviar.SharedKernel.Entities
{
    public class ApplicationRole : IdentityRole<int>, IUseEntity
    {
        [Required]
        [StringLength(256)]
        public override string Name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatTime { get; set; } = CommonHelper.GetSysDateTimeNow();
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = CommonHelper.GetSysDateTimeNow();
        /// <summary>
        /// 根据配置确定删除后是否保留条目
        /// </summary>
        public bool IsDelete { get; set; } = false;
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [StringLength(256)]
        public string OperatorCare { get; set; }
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [StringLength(256)]
        public string OperatorUp { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(300)]
        public string Remark { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisable { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        [StringLength(50)]
        public string Number { get; set; } = "999";
        /// <summary>
        /// 数据权限
        /// </summary>
        public int DataId { get; set; }
        /// <summary>
        /// 数据范围
        /// </summary>
        public DataRange DataRange { get; set; }
        /// <summary>
        /// 范围集合"1;2;3;5"储存权限id
        /// </summary>
        public string DataList { get; set; }
    }
}
