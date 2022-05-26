// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.ComponentModel.DataAnnotations;

namespace Caviar.SharedKernel.Entities
{
    public partial interface IBaseEntity : IDIinjectAtteribute
    {

    }

    public partial interface IUseEntity : IBaseEntity
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 第一次并不会引发删除，只是隐藏，第二次会真正的删除数据
        /// 当IsDelete为false时，删除会将IsDelete值改为true，当IsDelete值为true时将删除数据
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        public string OperatorCare { get; set; }
        /// <summary>
        /// 更新操作员的名称
        /// </summary>
        public string OperatorUp { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisable { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 数据权限
        /// </summary>
        public int DataId { get; set; }
    }
}
