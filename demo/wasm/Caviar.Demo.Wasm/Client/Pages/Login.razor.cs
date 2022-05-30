// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using Caviar.SharedKernel.Entities.User;

namespace Caviar.Demo.Wasm.Client.Pages
{
    public partial class Login
    {
        public UserLogin ApplicationUser { get; set; } = new UserLogin()
        {
            UserName = "admin",
            Password = "123456",
            RememberMe = true,
        };

        string? _style;

        protected override void OnInitialized()
        {
            string backgroundImage = "_content/Caviar.AntDesignUI/images/grov.jpg";
            _style = $"min-height:100vh;background-image: url({backgroundImage});";
            base.OnInitialized();
        }
    }
}
