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

namespace Caviar.Control
{
    public partial class CaviarBaseController
    {
        #region API
        [HttpGet]
        public IActionResult GetModelHeader(string name)
        {
            if (string.IsNullOrEmpty(name)) return ResultError(400, "请输入需要获取的数据名称");
            ResultMsg.Data = GetViewModelHeaders(name);
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
            bool isCover = generate?.Config?.SingleOrDefault(u => u == "覆盖") == null ? false : true;
            foreach (var item in data)
            {
                string outName = "";
                string path = "";
                if (item.KeyName.IndexOf(".razor") != -1)
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.WebUIPath + "/Pages/";
                }
                else if (item.KeyName.IndexOf(".viewModel") != -1)
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.ModelsPath + "/ViewModels/";
                }
                else if(item.KeyName.IndexOf(".action") != -1)
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.WebApiPath + "/ModelAction/";
                }
                else if (item.KeyName.IndexOf(".controller") != -1)
                {
                    path = Directory.GetCurrentDirectory() + "/" + CaviarConfig.WebApiPath + "/Controllers/";
                }
                if (path == "") continue;
                path += generate.OutName + "/";
                outName = path + item.TabName;
                WriteCodeFile(path, outName, item.Content, isCover);
            }
            return ResultOK();
        }

        #endregion


        #region 私有方法

        private void WriteCodeFile(string path, string outName, string content, bool isCover)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!System.IO.File.Exists(outName) || isCover)
            {
                System.IO.File.WriteAllText(outName, content);
            }
        }

        private List<TabItem> CodeGenerate(CodeGenerateData generate)
        {
            List<TabItem> lstTabs = new List<TabItem>();
            foreach (var item in generate?.Page)
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
                CreateViewFile(generate, ref lstTabs, name, ".razor.cs");
                CreateViewFile(generate, ref lstTabs, name, ".razor");
            }
            foreach (var item in generate?.WebApi)
            {
                switch (item)
                {
                    case "控制器":
                        CreateControllerFile(generate, ref lstTabs, "Controller", ".cs");
                        break;
                    case "模型":
                        CreateModelFile(generate, ref lstTabs, "ViewModel", ".cs");
                        break;
                    case "模型操作器":
                        CreateActionFile(generate, ref lstTabs, "Action", ".cs");
                        break;
                    default:
                        break;
                }
            }
            return lstTabs;
        }

        private void CreateViewFile(CodeGenerateData generate, ref List<TabItem> lstTabs, string name, string extend)
        {
            var txt = CreateFile(generate, name, extend);
            lstTabs.Add(new TabItem() { KeyName = name + extend, TabName = name + extend, Content = txt });
        }

        private void CreateModelFile(CodeGenerateData generate, ref List<TabItem> lstTabs, string name, string extend)
        {
            var txt = CreateFile(generate, name, extend);
            lstTabs.Add(new TabItem() { KeyName = name + extend + ".viewModel", TabName = "View" + generate.OutName + extend, Content = txt });
        }

        private void CreateControllerFile(CodeGenerateData generate, ref List<TabItem> lstTabs, string name, string extend)
        {
            var txt = CreateFile(generate, name, extend);
            lstTabs.Add(new TabItem() { KeyName = name + extend + ".controller", TabName = generate.OutName + "Controller" + extend, Content = txt });
        }

        private void CreateActionFile(CodeGenerateData generate, ref List<TabItem> lstTabs, string name, string extend)
        {
            var txt = CreateFile(generate, name, extend);
            lstTabs.Add(new TabItem() { KeyName = name + extend + ".action", TabName = generate.OutName + "Action" + extend, Content = txt });
        }

        /// <summary>
        /// 关键词替换
        /// </summary>
        /// <param name="generate"></param>
        /// <param name="name"></param>
        /// <param name="extend"></param>
        /// <returns></returns>
        protected virtual string CreateFile(CodeGenerateData generate, string name, string extend)
        {
            string txt = System.IO.File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/Template/File/{name}{extend}.temp");
            txt = txt.Replace("{Producer}", BC.UserToken.UserName);
            txt = txt.Replace("{GenerationTime}", DateTime.Now.ToString());
            txt = txt.Replace("{ViewOutName}", $"View{generate.OutName}");
            txt = txt.Replace("{OutName}", $"{generate.OutName}");
            txt = txt.Replace("{EntityName}", generate.EntityName);
            txt = txt.Replace("{EntityNamespace}", generate.EntityNamespace);
            txt = txt.Replace("{WebUINamespace}", CaviarConfig.WebUINamespace);
            txt = txt.Replace("{ModelsNamespace}", CaviarConfig.ModelsNamespace);
            txt = txt.Replace("{WebApiNamespace}", CaviarConfig.WebApiNamespace);
            txt = txt.Replace("{BaseController}", CaviarConfig.BaseController);
            txt = txt.Replace("{page}", "/" + generate.OutName + "/" + name);
            txt = txt.Replace("{DataSourceWebApi}", $"{generate.OutName}/GetPages");
            txt = txt.Replace("{EntityDisplayName}", generate.EntityDisplayName);
            txt = txt.Replace("{FormItem}", CreateFormItem(generate));
            return txt;
        }
        private List<ViewModelHeader> GetViewModelHeaders(string name)
        {
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
                    var valueLen = item.GetCustomAttributes<StringLengthAttribute>()?.Cast<StringLengthAttribute>().SingleOrDefault()?.MaximumLength;
                    var filter = new ViewModelHeader()
                    {
                        TypeName = item.Name,
                        ModelType = typeName,
                        DispLayName = dispLayName,
                        ValueLen = valueLen,
                        IsEnum = item.PropertyType.IsEnum
                    };
                    if (filter.IsEnum)
                    {
                        var enumFields = item.PropertyType.GetFields();
                        if(enumFields!=null && enumFields.Length >= 2)//枚举有一个隐藏的int所以要从下一位置开始
                        {
                            filter.EnumValueName = new Dictionary<int, string>();
                            for (int i = 1; i < enumFields.Length; i++)
                            {
                                var enumName = enumFields[i].GetCustomAttribute<DisplayAttribute>()?.Name;
                                var value = (int)enumFields[i].GetValue(null);
                                filter.EnumValueName.Add(value, enumName);
                            }
                        }
                    }
                    filter = TurnMeaning(filter);
                    viewModelNames.Add(filter);
                }
            }
            return viewModelNames;
        }
        /// <summary>
        /// 创建formItem
        /// </summary>
        /// <param name="generate"></param>
        /// <returns></returns>
        protected virtual string CreateFormItem(CodeGenerateData generate)
        {
            var headers = GetViewModelHeaders(generate.EntityName);
            headers = CreateOrUpFilterField(headers);
            var html = "";
            foreach (var item in headers)
            {
                var txt = "";
                txt += $"    <FormItem Label='{item.DispLayName}'>\r\n        ";
                var IsWrite = CreateCurrencyAssembly(item,ref txt);
                txt += "\r\n    </FormItem>\r\n";
                if (IsWrite) html += txt;
            }
            return html;
        }
        /// <summary>
        /// 根据类型创建通用组件
        /// </summary>
        /// <param name="item"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        protected virtual bool CreateCurrencyAssembly(ViewModelHeader item,ref string txt)
        {
            var IsWrite = true;
            if (item.IsEnum)
            {
                GetEnumAssembly(item, ref txt);
            }
            else
            {
                var modelType = item.ModelType.ToLower();
                switch (modelType)
                {
                    case "string":
                        if (item.ValueLen != null && item.ValueLen >= 200)
                        {
                            txt += $"<TextArea @bind-Value='@context.{item.TypeName}' Style='width:50%'/>";
                        }
                        else
                        {
                            txt += $"<Input @bind-Value='@context.{item.TypeName}' Style='width:50%' />";
                        }
                        break;
                    case "int32":
                        txt += $"<AntDesign.InputNumber @bind-Value='@context.{item.TypeName}' />";
                        break;
                    case "boolean":
                        txt += $"<Switch @bind-Value='@context.{item.TypeName}'/>";
                        break;
                    case "datetime":
                        txt += $"<DatePicker @bind-Value='@context.{item.TypeName}'/>";
                        break;
                    default:
                        IsWrite = false;
                        break;
                }
            }
            return IsWrite;
        }
        protected virtual void GetEnumAssembly(ViewModelHeader item, ref string txt)
        {
            if (!item.IsEnum) return;
            txt += $"<RadioGroup @bind-Value='@context.{item.TypeName}'>";
            foreach (var keyValue in item.EnumValueName)
            {
                txt += $"\r\n            <Radio RadioButton Value='({item.ModelType}){keyValue.Key}'>{keyValue.Value}</Radio>";
            }

            txt += $"\r\n        </RadioGroup>";
        }
        /// <summary>
        /// 创建或修改时过滤字段
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        protected virtual List<ViewModelHeader> CreateOrUpFilterField(List<ViewModelHeader> headers)
        {
            if (headers == null) return null;
            string[] violation = new string[] { "id", "Uid", "CreatTime", "UpdateTime", "IsDelete", "OperatorCare", "OperatorUp" };
            var result = new List<ViewModelHeader>();
            foreach (var item in headers)
            {
                if (violation.SingleOrDefault(u => u.ToLower() == item.TypeName.ToLower()) == null)
                {
                    result.Add(item);
                }
            }
            return result;
        }
        /// <summary>
        /// 字段类型转义
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        protected virtual ViewModelHeader TurnMeaning(ViewModelHeader headers)
        {
            string[] violation = new string[] { "icon","image" };
            var typeName = violation.SingleOrDefault(u => u.ToLower() == headers.TypeName.ToLower());
            if (typeName != null)
            {
                headers.ModelType = typeName;
                return headers;
            }
            return headers;
        }
        #endregion
    }
}
