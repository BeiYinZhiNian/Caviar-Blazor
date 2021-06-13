﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class ViewModelFields : SysModelFields
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        [DisplayName("显示名称")]
        public string DisplayName { get; set; }
        /// <summary>
        /// 类型名称如string,int等
        /// </summary>
        public string ModelType { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 是否为枚举
        /// </summary>
        public bool IsEnum { get; set; }
        /// <summary>
        /// 枚举 值-名称
        /// </summary>
        public Dictionary<int,string> EnumValueName { get; set; }

    }
}