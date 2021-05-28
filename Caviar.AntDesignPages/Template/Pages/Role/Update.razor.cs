using Caviar.AntDesignPages.Helper;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
/// <summary>
/// 生成者：未登录用户
/// 生成时间：2021/5/28 12:37:58
/// 代码由代码生成器自动生成，更改的代码可能被进行替换
/// 可在上层目录使用partial关键字进行扩展
/// </summary>
namespace Caviar.AntDesignPages.Pages.Role
{
    public partial class Update : ITableTemplate
    {
        [Inject]
        HttpHelper Http { get; set; }
        [Parameter]
        public int Id { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        [Parameter]
        public ViewRole DataSource { get; set; }
        /// <summary>
        /// 模板
        /// </summary>
        DataTemplate TemplateRef { get; set; }
        /// <summary>
        /// 数据提交
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Submit()
        {
            return await TemplateRef.Submit();
        }
    }
}
