using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services.ScannerServices
{
    public static class ApiScannerServices
    {
        /// <summary>
        /// 获取所有API集合
        /// </summary>
        /// <returns></returns>
        public static List<SysMenu> GetAllApi(Type baseController)
        {
            List<Type> controllerList = new List<Type>();
            CommonHelper.GetAssembly().ForEach(u =>
            {
                var type = u.GetTypes().Where(u => u.ContainBaseClass(baseController)!=null).ToList();
                controllerList.AddRange(type);
            });
            return null;
        }
    }
}
