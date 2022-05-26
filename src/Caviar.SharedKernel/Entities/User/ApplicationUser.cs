// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Caviar.SharedKernel.Entities
{
    public class ApplicationUser : IdentityUser<int>, IUseEntity
    {

        [Required]
        public override string UserName { get => base.UserName; set => base.UserName = value; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage = CurrencyConstant.EmailRuleErrorMsg)]
        public override string Email { get => base.Email; set => base.Email = value; }
        [RegularExpression(@"^1[3456789]\d{9}$", ErrorMessage = CurrencyConstant.PhoneNumberRuleErrorMsg)]
        public override string PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }
        /// <summary>
        /// 所在用户组
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = CurrencyConstant.UserGroupRuleErrorMsg)]
        public int UserGroupId { get; set; }
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
        /// 是否禁用
        /// </summary>
        public bool IsDisable
        {
            get => LockoutEnabled;
            set
            {
                LockoutEnabled = value;
                if (value)
                {
                    LockoutEnd = CommonHelper.GetSysDateTimeNow().AddYears(99);
                }
                else
                {
                    LockoutEnd = null;
                }
            }
        }
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
        /// 头像
        /// </summary>
        [StringLength(300)]
        public string HeadPortrait { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(300)]
        public string Remark { get; set; }
    }
}
