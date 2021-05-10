using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control
{
    public partial class BaseController
    {
        #region API
        [HttpGet]
        public IActionResult GetModelHeader(string name)
        {
            if (string.IsNullOrEmpty(name)) return ResultError(400, "请输入需要获取的数据名称");
            var assemblyList = CommonHelper.GetAssembly();
            Type type = null;
            foreach (var item in assemblyList)
            {
                type = item.GetTypes().SingleOrDefault(u => u.Name.ToLower() == name.ToLower());
                if (type != null) break;
            }
            List<ViewModelHeader> viewModelNames = new List<ViewModelHeader>();
            if (type != null)
            {
                foreach (var item in type.GetRuntimeProperties())
                {
                    var typeName = item.PropertyType.Name;
                    var dispLayName = item.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                    viewModelNames.Add(new ViewModelHeader() { TypeName = item.Name, ModelType = typeName, DispLayName = dispLayName });
                }
            }
            ResultMsg.Data = viewModelNames;
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
            ResultMsg.Data = CodeGenerate(generate);
            return ResultOK();
        }
        [HttpPost]
        public IActionResult CodeFileGenerate(CodeGenerateData generate)
        {
            var data = CodeGenerate(generate);
            

            foreach (var item in data)
            {
                if (item.KeyName.IndexOf(".razor") != -1)
                {
                    var path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.WebUIPath;

                    path += generate.OutName + "/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    if (System.IO.File.Exists(path + item.TabName))
                    {

                    }
                    else
                    {
                        System.IO.File.WriteAllText(path + item.TabName, item.Content);
                    }
                }
                else if (item.KeyName.IndexOf(".viewModel") != -1)
                {
                    var path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.ModelsPath;

                    path += generate.OutName + "/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var outName = path + generate.OutName + ".cs";
                    if (System.IO.File.Exists(outName))
                    {

                    }
                    else
                    {
                        System.IO.File.WriteAllText(outName, item.Content);
                    }
                }
            }
            return ResultOK();
        }


        private List<TabItem> CodeGenerate(CodeGenerateData generate)
        {
            List<TabItem> lstTabs = new List<TabItem>();
            CreateModelFile(generate, ref lstTabs, "ViewModel", ".cs");
            foreach (var item in generate.Page)
            {
                string name = "";
                switch (item)
                {
                    case "新增":
                        name = "Add";
                        break;
                    case "列表":
                        name = "Index";
                        break;
                    default:
                        continue;
                }
                CreateViewFile(generate,ref lstTabs, name , ".razor.cs");
                CreateViewFile(generate,ref lstTabs, name , ".razor");
            }
            return lstTabs;
        }

        private void CreateViewFile(CodeGenerateData generate,ref List<TabItem> lstTabs, string name,string extend)
        {
            string txt = System.IO.File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/Template/{name}{extend}.temp");
            txt = CreateFile(txt, generate);
            txt = txt.Replace("{namespace}", CaviarConfig.WebUINamespace);
            txt = txt.Replace("{page}", "/" +generate.OutName + "/" + name);
            lstTabs.Add(new TabItem() { KeyName = name + extend, TabName = name + extend, Content = txt });
        }

        private void CreateModelFile(CodeGenerateData generate, ref List<TabItem> lstTabs, string name, string extend)
        {
            string txt = System.IO.File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/Template/{name}{extend}.temp");
            txt = CreateFile(txt, generate);
            txt = txt.Replace("{namespace}", CaviarConfig.ModelsNamespace);
            lstTabs.Add(new TabItem() { KeyName = name + extend + ".viewModel", TabName = "View" + generate.OutName + extend, Content = txt });
        }


        private string CreateFile(string txt,CodeGenerateData generate)
        {
            txt = txt.Replace("{Producer}", BC.UserToken.UserName);
            txt = txt.Replace("{GenerationTime}", DateTime.Now.ToString());
            txt = txt.Replace("{OutName}", $"View{generate.OutName}");
            txt = txt.Replace("{EntityName}",  generate.EntityName);
            txt = txt.Replace("{EntityNamespace}", generate.EntityNamespace);
            txt = txt.Replace("{WebUINamespace}", CaviarConfig.WebUINamespace);
            txt = txt.Replace("{ModelsNamespace}", CaviarConfig.ModelsNamespace);
            txt = txt.Replace("{DataSourceWebApi}", $"{generate.OutName}/Get{generate.OutName}s");
            return txt;
        }
        #endregion
    }
}
