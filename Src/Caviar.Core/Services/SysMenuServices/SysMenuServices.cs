using Caviar.SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services.SysMenuServices
{
    public partial class SysMenuServices: EasyBaseServices<SysMenu>
    {
        /// <summary>
        /// 获取当前菜单
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysMenu>> GetMenuBar()
        {
            var menus = await GetAllAsync();
            return menus;
        }
    }
}
