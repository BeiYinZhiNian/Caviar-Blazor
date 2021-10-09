﻿using Caviar.SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel
{
    public static class CurrencyConstant
    {
        /// <summary>
        /// 附件映射路径
        /// </summary>
        public const string Enclosure = "/Enclosure";
        /// <summary>
        /// 头像文件夹名称
        /// </summary>
        public const string HeadPortrait = "headPortrait";
        /// <summary>
        /// url名称
        /// </summary>
        public const string CavModelUrl = "Url";
        /// <summary>
        /// jwt验证中token名称
        /// </summary>
        public const string TokenPayLoadName = "Data";
        /// <summary>
        /// 验证方式
        /// </summary>
        public const string Authorization = "Authorization";
        /// <summary>
        /// JWT验证
        /// </summary>
        public const string JWT = "JWT ";
        /// <summary>
        /// 基础实体名称
        /// </summary>
        public static string BaseEntityName = typeof(SysBaseEntity).Name;
    }
}