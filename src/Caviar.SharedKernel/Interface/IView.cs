// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

namespace Caviar.SharedKernel.Entities
{
    public interface IView<T> where T : IUseEntity
    {
        public T Entity { get; set; }
    }

    public class BaseView<T> : IView<T> where T : IUseEntity
    {
        public virtual T Entity { get; set; }
    }
}
