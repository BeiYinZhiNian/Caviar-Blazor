using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace Caviar.Control
{
    public partial class CaviarBaseController
    {
        protected IAssemblyDynamicCreation CavAssembly => BC.HttpContext.RequestServices.GetService<IAssemblyDynamicCreation>();


        #region API
        [HttpGet]
        public IActionResult GetModelHeader(string name)
        {
            if (string.IsNullOrEmpty(name)) return ResultError(400, "请输入需要获取的数据名称");
            ResultMsg.Data = CavAssembly.GetViewModelHeaders(name);
            return ResultOK();
        }
        
        /// <summary>
        /// 模糊查询，暂未使用权限,需要使用权限验证sql的正确性
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult FuzzyQuery(ViewQuery query)
        {
            if (string.IsNullOrEmpty(query.QueryObj)) return ResultErrorMsg("查询对象不可为空");
            var assemblyList = CommonHelper.GetAssembly();
            Type type = null;
            foreach (var item in assemblyList)
            {
                type = item.GetTypes().SingleOrDefault(u => u.Name.ToLower() == query.QueryObj.ToLower());
                if (type != null) break;
            }
            if (type == null) return ResultErrorMsg("没有对该对象的查询权限");


            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("@queryStr", "%" + query.QueryStr + "%"),
            };
            var queryField = "";
            if (query.QueryField != null && query.QueryField.Count > 0)
            {
                queryField = "and (";
                for (int i = 0; i < query.QueryField.Count; i++)
                {
                    queryField += $" {query.QueryField[i]} LIKE @queryStr ";
                    var index = i + 1;
                    if (index < query.QueryField.Count)
                    {
                        queryField += " or ";
                    }
                }
                queryField += ")";
            }

            string sql = $"select top(20)* from {query.QueryObj} where IsDelete=0 " + queryField;
            if (query.StartTime != null)
            {
                sql += $" and CreatTime>=@StartTime ";
                parameters.Add(new SqlParameter("@StartTime", query.StartTime));
            }
            if (query.EndTime != null)
            {
                sql += $" and CreatTime<=@EndTime ";
                parameters.Add(new SqlParameter("@EndTime", query.EndTime));
            }
            var data = BC.DC.SqlQuery(sql, parameters.ToArray());
            ResultMsg.Data = data.ToList(type);
            return ResultOK();
        }


        [HttpPost]
        public IActionResult CodePreview(CodeGenerateData generate)
        {
            ResultMsg.Data = CavAssembly.CodeGenerate(generate,BC.UserName);
            return ResultOK();
        }
        [HttpPost]
        public IActionResult CodeFileGenerate(CodeGenerateData generate)
        {
            var data = CavAssembly.CodeGenerate(generate, BC.UserName);
            bool isCover = generate?.Config?.SingleOrDefault(u => u == "覆盖") == null ? false : true;
            foreach (var item in data)
            {
                string outName = "";
                string path = "";
                if (item.KeyName.IndexOf(".razor") != -1)
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.WebUIPath + "/Pages/";
                }
                else if (item.KeyName.IndexOf("ViewModel.cs") != -1)
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.ModelsPath + "/ViewModels/";
                }
                else if(item.KeyName.IndexOf("Action.cs") != -1)
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.WebApiPath + "/ModelAction/";
                }
                else if (item.KeyName.IndexOf("Controller.cs") != -1)
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.WebApiPath + "/Controllers/";
                }
                if (path == "") continue;
                path += generate.OutName + "/";
                outName = path + item.TabName;
                CavAssembly.WriteCodeFile(path, outName, item.Content, isCover);
            }
            return ResultOK();
        }

        #endregion


        #region 私有方法

       
        #endregion
    }
}
