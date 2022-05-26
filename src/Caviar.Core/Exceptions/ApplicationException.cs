// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;

namespace Caviar.Core
{
    public class ApplicationException : Exception
    {
        public ApplicationException(string errorMsg)
            : base(errorMsg)
        {
        }
    }
}
