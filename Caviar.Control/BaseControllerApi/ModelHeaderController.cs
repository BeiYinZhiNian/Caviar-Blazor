using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.ModelHeader
{
    public partial class ModelHeaderController
    {
        /// <summary>
        /// 获取所有model
        /// </summary>
        [HttpGet]
        public IActionResult GetModels()
        {
            List<ViewModelHeader> viewModels = new List<ViewModelHeader>();
            var types = CommonHelper.GetModelList();
            foreach (var item in types)
            {
                var displayName = item.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                viewModels.Add(new ViewModelHeader() { TypeName = item.Name,DisplayName = displayName ,FullName = item.FullName.Replace("." + item.Name, "") });
            }
            ResultMsg.Data = viewModels;
            return ResultOK();
        }

        /// <summary>
        /// 获取model里面的字段
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetModelHeader(string name)
        {
            if (string.IsNullOrEmpty(name)) return ResultError(400, "请输入需要获取的数据名称");
            ResultMsg.Data = CavAssembly.GetViewModelHeaders(name);
            return ResultOK();
        }
    }
}
