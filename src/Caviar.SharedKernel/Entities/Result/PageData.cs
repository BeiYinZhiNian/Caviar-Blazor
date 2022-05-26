// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;

namespace Caviar.SharedKernel.Entities
{

    public class PageData<T>
    {
        public PageData()
        {

        }

        public PageData(List<T> rows)
        {
            Rows = rows;
        }

        /// <summary>
        /// 数据
        /// </summary>
        public List<T> Rows { get; set; }
        /// <summary>
        /// 总计
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
