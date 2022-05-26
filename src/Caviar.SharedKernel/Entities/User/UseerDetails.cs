// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Caviar.SharedKernel.Entities
{
    public class UserDetails
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage = CurrencyConstant.EmailRuleErrorMsg)]
        public string Email { get; set; }
        [Required]
        public string AccountName { get; set; }
        [RegularExpression(@"^1[3456789]\d{9}$", ErrorMessage = CurrencyConstant.PhoneNumberRuleErrorMsg)]
        public string PhoneNumber { get; set; }

        public string UserGroupName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(300)]
        public string HeadPortrait { get; set; }

        public IList<string> Roles { get; set; }
        /// <summary>
        /// 个性签名
        /// </summary>
        [StringLength(300)]
        public string Remark { get; set; }
    }
}
