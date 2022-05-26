// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;

namespace Caviar.SharedKernel.Entities
{
    public class ResultException : SystemException
    {
        public ResultMsg ResultMsg { get; set; }
        public ResultException(ResultMsg resultMsg)
        {
            ResultMsg = resultMsg;
        }
    }

    public class ResultException<T> : ResultException
    {
        public new ResultMsg<T> ResultMsg { get; set; }
        public ResultException(ResultMsg<T> resultMsg) : base(resultMsg)
        {
            ResultMsg = resultMsg;
        }
    }
}
