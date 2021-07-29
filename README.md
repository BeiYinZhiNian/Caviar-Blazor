<p align="center">
  <a href="#">
    <img src="https://images.gitee.com/uploads/images/2021/0722/235619_37006555_1456276.png">
  </a>
</p>
<h1 align="center">Caviar Blazor</h1>
Caviar(鱼子酱)框架采用了：Blazor + Ant Design + .NET 5 本框架使用简洁架构，包含了后台管理基础功能，在开发上为了方便，封装了很多Ant Design组件，包含了自动列表、 高级搜索、自动化菜单、布局等。在代码生成上做到了100%的代码隔离，有修改，重新代码生成，不会对开发者有任何影响，并且不会降低开发的灵活性。 在权限设计上，可以控制任意一个字段、菜单、按钮、api、甚至是元素。 在外观上，界面风格统一，做了很多兼容，包括手机端，可以在手机端也有较好的体验。 但是框架还是在一个比较初级的阶段，很多功能不完善，下一阶段主要会致力于代码整理和文档的编写,希望大家可以多多参与和使用，感谢。

## ✨ 特性  

- 🌈 代码生成器一键自动生成前后端，生成代码做到100%隔离，无需担心代码混乱。  
- 🏁 拥有字段权限、数据权限、菜单权限、按钮权限，甚至可以细化到元素权限。  
- 📦 开箱即用的高质量框架，封装了多个Ant Design组件简化使用过程
- 📱 兼容PC、手机、Ipad，一处运行，到处使用
- 💕 支持 WebAssembly、Server(开发中) 模式
- ⚙️ 支持多种数据库：SqlServer、MySql等
- 🎁 内置日志管理、菜单管理、附件管理、部门管理、用户管理、角色管理、代码生成等
- 💿 多种主题任意切换

## 项目架构

<p align="center">
    <img src="https://images.gitee.com/uploads/images/2021/0723/154334_7f00d449_1456276.png">
</p>



## 🔗 链接

- <a target='_blank' href='http://121.40.180.228/'>演示地址</a>  
  用户名：admin  
  密码：123456  
  由于刚搭建好，很多功能没有限制，大家想测试可以自行创建数据进行测试，就不要删除原先的数据了，感谢！  
- 项目文档正在编写
- [Ant Design](https://ant-design-blazor.gitee.io/zh-CN/)

## ☀️ 第一次点亮
- 1、首先下载源代码解压后，打开Caviar.sln。  
  2、如果以前有运行过代码，请先打开SqlServer资源管理器，连接到本地数据库【(localdb)\MSSQLLocalDB】，找到Caviar_db数据库后删除就可以了。  
  3、右键解决方案，选择属性，选择多个项目启动，把Caviar.Demo.AntDesignUI和Caviar.Demo.AntWebApi设定为启动，然后点击确定。  
  4、F5启动后会自动打开，第一次启动loading会比较长，请耐性等待。  
  5、如果无法启动可以加入QQ群进行联系。  

## 🍡 使用环境

- .NET 5
- Ant Design:0.9.0
- Visual Studio 2019
- SqlServer

| [<img src="https://cdn.jsdelivr.net/gh/alrra/browser-logos/src/edge/edge_48x48.png" alt="IE / Edge" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br> Edge / IE | [<img src="https://cdn.jsdelivr.net/gh/alrra/browser-logos/src/firefox/firefox_48x48.png" alt="Firefox" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>Firefox | [<img src="https://cdn.jsdelivr.net/gh/alrra/browser-logos/src/chrome/chrome_48x48.png" alt="Chrome" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>Chrome | [<img src="https://cdn.jsdelivr.net/gh/alrra/browser-logos/src/safari/safari_48x48.png" alt="Safari" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>Safari | [<img src="https://cdn.jsdelivr.net/gh/alrra/browser-logos/src/opera/opera_48x48.png" alt="Opera" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>Opera | [<img src="https://cdn.jsdelivr.net/gh/alrra/browser-logos/src/electron/electron_48x48.png" alt="Electron" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>Electron |
| :-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | :----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | :----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: |
|                                                                                          Edge 16 / IE 11†                                                                                           |                                                                                                 522                                                                                                  |                                                                                                57                                                                                                |                                                                                                11                                                                                                |                                                                                              44                                                                                              |                                                                                               Chromium 57                                                                                                |

> 由于 [WebAssembly](https://webassembly.org) 的限制，Blazor WebAssembly 不支持 IE 浏览器，但 Blazor Server 支持 IE 11†。 详见[官网说明](https://docs.microsoft.com/en-us/aspnet/core/blazor/supported-platforms?view=aspnetcore-3.1&WT.mc_id=DT-MVP-5003987)。

## 🏁 项目截图

![load](https://images.gitee.com/uploads/images/2021/0723/112613_679d6eb8_1456276.png "屏幕截图.png")
![登录](https://images.gitee.com/uploads/images/2021/0723/112634_5f188e73_1456276.png "屏幕截图.png")
![首页](https://images.gitee.com/uploads/images/2021/0723/112647_21601c71_1456276.png "屏幕截图.png")
![菜单](https://images.gitee.com/uploads/images/2021/0723/112716_2a38aec6_1456276.png "屏幕截图.png")
![暗黑](https://images.gitee.com/uploads/images/2021/0723/112733_aac71b79_1456276.png "屏幕截图.png")
![手机端](https://images.gitee.com/uploads/images/2021/0723/140728_26e85f8c_1456276.png "屏幕截图.png")

## 🍻 社区互助

- 如果在使用过程中遇到任何问题，可以通过以下途径解决，我定当竭尽所能

![qq群](https://images.gitee.com/uploads/images/2021/0723/143814_11a0a270_1456276.png "屏幕截图.png")

## 🌠 授权协议

![授权协议](https://images.gitee.com/uploads/images/2021/0723/144214_9f81ab38_1456276.png "屏幕截图.png")

框架正在全力开发中，争取10月前完成，敬请期待…… :wink: 
