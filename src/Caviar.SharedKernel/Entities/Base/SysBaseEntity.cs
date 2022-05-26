// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.ComponentModel.DataAnnotations;

namespace Caviar.SharedKernel.Entities
{

    public partial class SysBaseEntity : IBaseEntity
    {

    }
    /// <summary>
    /// 数据基础类
    /// </summary>
    public partial class SysUseEntity : IUseEntity
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public virtual int Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreatTime { get; set; } = CommonHelper.GetSysDateTimeNow();
        /// <summary>
        /// 修改时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; } = CommonHelper.GetSysDateTimeNow();
        /// <summary>
        /// 根据配置确定删除后是否保留条目
        /// </summary>
        public virtual bool IsDelete { get; set; } = false;
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [StringLength(256)]
        public virtual string OperatorCare { get; set; }
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [StringLength(256)]
        public virtual string OperatorUp { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public virtual bool IsDisable { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        [StringLength(50)]
        public virtual string Number { get; set; } = "999";
        /// <summary>
        /// 数据权限
        /// </summary>
        public virtual int DataId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(300)]
        public virtual string Remark { get; set; }
    }
}
