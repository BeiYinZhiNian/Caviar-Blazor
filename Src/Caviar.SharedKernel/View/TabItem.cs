using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.View
{
    /// <summary>
    /// 代码预览tab
    /// </summary>
    public class CodePreviewTab
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string KeyName { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string TabName { get; set; }
        /// <summary>
        /// 代码内容
        /// </summary>
        public string Content { get; set; }
    }
}
