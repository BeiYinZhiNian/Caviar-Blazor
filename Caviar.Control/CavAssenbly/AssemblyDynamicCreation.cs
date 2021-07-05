using Caviar.Models.SystemData;
using System;
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
    public class AssemblyDynamicCreation:IAssemblyDynamicCreation
    {
        public virtual void WriteCodeFile(string path, string outName, string content, bool isCover)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(outName) || isCover)
            {
                string fileData = "存在文件";
                if (File.Exists(outName)) fileData = File.ReadAllText(outName);
                if (!string.IsNullOrEmpty(fileData))
                {
                    File.WriteAllText(outName, content);
                }
            }
        }
        public List<TabItem> CodeGenerate(CodeGenerateData generate,string userName)
        {
            List<TabItem> lstTabs = new List<TabItem>();
            List<string> list = new List<string>();
            if (generate.WebApi != null)
            {
                list.AddRange(generate.WebApi);
            }
            if (generate.Page != null)
            {
                list.AddRange(generate.Page);
            }

            foreach (var key in list)
            {
                var content = "";
                var task = TaskIdentification(key);
                content = CreateFile(generate, task.Item1, task.Item2, userName);
                lstTabs.Add(CreatTabItem(generate.OutName,task.Item1,task.Item2,content));
            }
            return lstTabs;
        }

        private TabItem CreatTabItem(string outName,string fileName,string extend,string content)
        {
            var item = new TabItem() { KeyName = fileName + extend, TabName =  fileName.Replace("(name)", outName) + extend, Content = content };
            if (extend.IndexOf(".razor") != -1)
            {
                item.TabName = fileName + extend;
            }
            return item;
        }

        /// <summary>
        /// item1:fileName
        /// item2:extend
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        (string,string) TaskIdentification(string key)
        {
            string fileName = "";
            var extend = ".cs";
            switch (key)
            {
                case "数据模板":
                    fileName = "DataTemplate";
                    extend = ".razor";
                    break;
                case "列表":
                    fileName = "Index";
                    extend = ".razor";
                    break;
                case "控制器":
                    fileName = "(name)Controller";
                    break;
                case "模型":
                    fileName = "View(name)";
                    break;
                case "模型操作器":
                    fileName = "(name)Action";
                    break;
                default:
                    break;
            }
            return (fileName,extend);
        }

        /// <summary>
        /// 创建任务文件
        /// </summary>
        /// <param name="generate"></param>
        /// <param name="fileName"></param>
        /// <param name="extend"></param>
        /// <returns></returns>
        protected virtual string CreateFile(CodeGenerateData generate, string fileName, string extend,string producer)
        {
            string txt = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/Template/File/{fileName}{extend}.temp");
            txt = txt.Replace("{Producer}", producer);
            txt = txt.Replace("{GenerationTime}", DateTime.Now.ToString());
            txt = txt.Replace("{EntityNamespace}", generate.EntityNamespace);
            txt = txt.Replace("{WebUINamespace}", CaviarConfig.WebUI.Namespace);
            txt = txt.Replace("{ModelsNamespace}", CaviarConfig.Models.Namespace);
            txt = txt.Replace("{WebApiNamespace}", CaviarConfig.WebAPI.Namespace);
            txt = txt.Replace("{BaseController}", CaviarConfig.WebAPI.Base);
            txt = txt.Replace("{page}", "/" + generate.OutName + "/" + fileName);
            txt = txt.Replace("{DataSourceWebApi}", $"{generate.OutName}/GetPages");
            txt = txt.Replace("{EntityDisplayName}", generate.ModelName);
            txt = txt.Replace("{FormItem}", CreateFormItem(generate));
            //以下为最小单元，必须为最后替换
            txt = txt.Replace("{ViewOutName}", $"View{generate.OutName}");
            txt = txt.Replace("{OutName}", $"{generate.OutName}");
            txt = txt.Replace("{EntityName}", generate.EntityName);
            return txt;
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
                txt += $"<FormItem Label='{item.DisplayName}'>";
                var IsWrite = CreateCurrencyAssembly(item, ref txt);
                txt += "</FormItem>";
                if (IsWrite) html += txt;
            }
            html = FormatHtml(html);
            return html;
        }
        /// <summary>
        /// 根据类型创建通用组件
        /// </summary>
        /// <param name="item"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        protected virtual bool CreateCurrencyAssembly(ViewModelFields item, ref string txt)
        {
            var IsWrite = true;
            if (item.IsEnum)
            {
                CreateEnumAssembly(item, ref txt);
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
        /// <summary>
        /// 创建枚举组件
        /// </summary>
        /// <param name="item"></param>
        /// <param name="txt"></param>
        protected virtual void CreateEnumAssembly(ViewModelFields item, ref string txt)
        {
            if (!item.IsEnum) return;
            txt += $"<RadioGroup @bind-Value='@context.{item.TypeName}'>";
            foreach (var keyValue in item.EnumValueName)
            {
                txt += $"<Radio RadioButton Value='({item.ModelType}){keyValue.Key}'>{keyValue.Value}</Radio>";
            }

            txt += $"</RadioGroup>";
        }
        /// <summary>
        /// 创建icon组件
        /// </summary>
        /// <param name="item"></param>
        /// <param name="txt"></param>
        protected virtual void CreateIconAssmbly(ViewModelFields item, ref string txt)
        {


        }

        /// <summary>
        /// 将html代码格式化
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected virtual string FormatHtml(string html)
        {
            string[] arr = html.Split('>');
            string formatHtml = "";
            int count = 0;
            bool isSame = false;
            bool isFirst = true;
            foreach (var item in arr)
            {
                if (string.IsNullOrEmpty(item)) continue;
                var lene = item + ">";
                if (lene[0] != '<')
                {
                    formatHtml += lene;
                    isSame = true;
                }
                else
                {
                    if (isSame && lene.Contains("</"))
                    {
                        isSame = false;
                        count -= 1;
                    }
                    if (isFirst)
                    {
                        formatHtml = GetSpace(count) + lene;
                    }
                    else
                    {
                        formatHtml += "\r\n" + GetSpace(count) + lene;
                    }
                    isFirst = false;
                    
                }
                if (lene.Contains("</"))
                {
                    count -= 1;
                    if (count < 0) count = 0;
                }
                else if (lene.Contains("/>"))
                {
                    isSame = true;
                }
                else
                {
                    count += 1;
                }

            }
            return formatHtml;
        }

        public string GetSpace(int count)
        {
            string space = "    ";
            for (int i = 0; i < count; i++)
            {
                space += space;
            }
            return space;
        }


        /// <summary>
        /// 创建或修改时过滤字段
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        protected virtual List<ViewModelFields> CreateOrUpFilterField(List<ViewModelFields> headers)
        {
            if (headers == null) return null;
            string[] violation = new string[] { "id", "Uid", "CreatTime", "UpdateTime", "IsDelete", "OperatorCare", "OperatorUp", "ParentId" };
            var result = new List<ViewModelFields>();
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
        public virtual ViewModelFields TurnMeaning(ViewModelFields headers)
        {
            string[] violation = new string[] { "icon", "image" , "headportrait" };
            var typeName = violation.SingleOrDefault(u => u.ToLower() == headers.TypeName.ToLower());
            if (typeName != null)
            {
                headers.ModelType = typeName;
                return headers;
            }
            return headers;
        }

        public List<ViewModelFields> GetViewModelHeaders(string name)
        {
            var assemblyList = CommonHelper.GetAssembly();
            Type type = null;
            foreach (var item in assemblyList)
            {
                type = item.GetTypes().SingleOrDefault(u => u.Name.ToLower() == name.ToLower());
                if (type != null) break;
            }
            List<ViewModelFields> viewModelNames = new List<ViewModelFields>();
            if (type != null)
            {
                foreach (var item in type.GetRuntimeProperties())
                {
                    var typeName = item.PropertyType.Name;
                    if(typeName== "Nullable`1")//可为null的字段
                    {
                        var Arguments = item.PropertyType.GenericTypeArguments;
                        if (Arguments.Length > 0)
                        {
                            typeName = Arguments[0].Name;
                        }
                    }
                    var baseType = CommonHelper.GetCavBaseType(type);
                    var dispLayName = item.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                    var valueLen = item.GetCustomAttributes<StringLengthAttribute>()?.Cast<StringLengthAttribute>().SingleOrDefault()?.MaximumLength;
                    var filter = new ViewModelFields()
                    {
                        TypeName = item.Name,
                        ModelType = typeName,
                        DisplayName = dispLayName,
                        ValueLen = valueLen,
                        IsEnum = item.PropertyType.IsEnum,
                        FullName = name,
                        BaseTypeName = baseType.Name,
                        IsDisable = true,
                    };
                    if (filter.IsEnum)
                    {
                        filter.EnumValueName = CommonHelper.GetEnenuModelHeader(item.PropertyType);
                    }
                    filter = TurnMeaning(filter);
                    viewModelNames.Add(filter);
                }
            }
            return viewModelNames;
        }
    }
}
