using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Core
{
    /// <summary>
    /// 布局类
    /// </summary>
    public class CavLayout
    {
        /// <summary>
        /// 背景颜色
        /// </summary>
        public string White { get; set; } = "#F8F8FF";
        /// <summary>
        /// 背景图片
        /// </summary>
        public string Background { get; set; } = "background:#F8F8FF;";
        /// <summary>
        /// 内容样式
        /// </summary>
        public string ContentStyle { get; set; } = "margin: 6px 16px;padding: 24px;min-height: 280px;background:#F8F8FF;";
        /// <summary>
        /// 头样式
        /// </summary>
        public string HeaderStyle { get; set; } = "padding:0;background:#F8F8FF;";
        /// <summary>
        /// 手风琴模式
        /// </summary>
        public bool Accordion { get; set; }
        private string _theme = "ant-design-blazor.css";
        public delegate void ThemeDelegate(string oldThemeName, string newThemeName);
        /// <summary>
        /// 更新主题事件
        /// </summary>
        public event ThemeDelegate ThemeChanged;
        /// <summary>
        /// 主题
        /// </summary>
        public string Theme { 
            get { return _theme; } 
            set {
                ThemeChanged?.Invoke(_theme, value);
                _theme = value;
            } 
        }
        /// <summary>
        /// 是否table页
        /// </summary>
        public bool IsTable { get; set; } = true;

    }
}
