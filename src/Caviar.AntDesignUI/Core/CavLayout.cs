// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using AntDesign;

namespace Caviar.AntDesignUI.Core
{
    /// <summary>
    /// 布局类
    /// </summary>
    public class CavLayout
    {
        public CavLayout(MessageService messageService)
        {
            _messageService = messageService;
        }
        MessageService _messageService { get; set; }
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
        public string Theme
        {
            get { return _theme; }
            set
            {
                ThemeSwitch(value);
                ThemeChanged?.Invoke(_theme, value);
                _theme = value;
            }
        }

        private void ThemeSwitch(string theme)
        {
            switch (theme)
            {
                case "ant-design-blazor.dark.css":
                    if (_leftMenuTheme != LeftMenuThemeEnum.Dark)
                    {
                        _messageService.Warning("黑暗主题不能使用明亮菜单，已为您自动切换");
                    }
                    _leftMenuTheme = LeftMenuThemeEnum.Dark;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 是否table页
        /// </summary>
        public bool IsTable { get; set; } = true;

        private LeftMenuThemeEnum _leftMenuTheme = LeftMenuThemeEnum.Dark;
        public LeftMenuThemeEnum LeftMenuTheme
        {
            get { return _leftMenuTheme; }
            set
            {
                _leftMenuTheme = value; // 先赋值，如果黑暗主题选择了明亮菜单，在回调里自动改回来
                ThemeSwitch(_theme);
                ThemeChanged?.Invoke(_theme, _theme);
            }
        }
        public SiderTheme SiderTheme
        {
            get
            {
                switch (LeftMenuTheme)
                {
                    case LeftMenuThemeEnum.Light:
                        SiderTheme.Light.Value = 1;
                        SiderTheme.Light.Name = "Light".ToLowerInvariant();
                        return SiderTheme.Light;
                    case LeftMenuThemeEnum.Dark:
                        SiderTheme.Dark.Value = 2;
                        SiderTheme.Dark.Name = "Dark".ToLowerInvariant();
                        return SiderTheme.Dark;
                    default:
                        return SiderTheme.Dark;

                }
            }
        }
        public MenuTheme MenuTheme
        {
            get
            {
                switch (LeftMenuTheme)
                {
                    case LeftMenuThemeEnum.Light:
                        MenuTheme.Light.Value = 1;
                        MenuTheme.Light.Name = "Light".ToLowerInvariant();
                        return MenuTheme.Light;
                    case LeftMenuThemeEnum.Dark:
                        MenuTheme.Dark.Value = 2;
                        MenuTheme.Dark.Name = "Dark".ToLowerInvariant();
                        return MenuTheme.Dark;
                    default:
                        return MenuTheme.Dark;

                }
            }
        }

        public enum LeftMenuThemeEnum
        {
            Light,
            Dark
        }
    }
}
