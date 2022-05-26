// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using Caviar.SharedKernel.Entities;

namespace Caviar.Core.Exceptions
{
    public class ResultException : Exception
    {
        public int StatusCode { get; set; }
        public ResultMsg ResultMsg { get; set; }
        public ResultException(int statusCode, string errorMsg, ResultMsg resultMsg = null) : base(errorMsg)
        {
            StatusCode = statusCode;
            ResultMsg = resultMsg;
        }
    }
}
