// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;
using System.Security.Claims;

namespace Caviar.SharedKernel.Entities.User
{
    public class CurrentUser
    {
        public string UserName { get; set; }
        public bool IsAuthenticated { get; set; }
        public IEnumerable<CaviarClaim> Claims { get; set; }
    }

    /// <summary>
    /// 防止递归循环
    /// </summary>
    public class CaviarClaim
    {
        public CaviarClaim(Claim claim)
        {
            Type = claim.Type;
            Value = claim.Value;
        }

        public CaviarClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public CaviarClaim()
        {

        }

        public string Type { get; set; }

        public string Value { get; set; }
    }
}
