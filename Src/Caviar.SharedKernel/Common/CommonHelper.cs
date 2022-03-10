using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using System.Web;

namespace Caviar.SharedKernel.Entities
{
    public static class CommonHelper
    {
        public static TimeSlot GetTimeSlot()
        {
            DateTime time1 = Convert.ToDateTime("0:00:00");
            DateTime time2 = Convert.ToDateTime(DateTime.Now.ToString());
            TimeSpan TS = new TimeSpan(time2.Ticks - time1.Ticks);
            int Time = (int)TS.TotalHours;
            if (Time < 5) // 0 - 5
            {
                return TimeSlot.Midnight;
            }
            else if (Time >= 5 && Time < 11)// 5 - 11
            {
                return TimeSlot.Morning;
            }
            else if (Time >= 11 && Time < 14)// 11 - 14
            {
                return TimeSlot.Noon;
            }
            else if (Time >= 14 && Time < 18)// 14 - 18
            {
                return TimeSlot.Afternoon;
            }
            else// 18 - 24
            {
                return TimeSlot.Night;
            }
        }
        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SHA256EncryptString(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 是否为手机号
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber(string number)
        {
            return Regex.IsMatch(number, @"^[1][3-9]\\d{9}");
        }
        /// <summary>
        /// 获取字符串右边的字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="contrastText"></param>
        /// <param name="index"></param>
        /// <param name="IsLastIndex"></param>
        /// <returns></returns>
        public static string GetRightText(this string text, string contrastText, int index = 0,bool IsLastIndex = false)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(contrastText)) return "";
            if (IsLastIndex)
            {
                index = text.LastIndexOf(contrastText, index);
            }
            else
            {
                index = text.IndexOf(contrastText, index);
            }
            if (index == -1) return "";
            return text.Substring(index + contrastText.Length, text.Length - index - contrastText.Length);
        }
        /// <summary>
        /// 获取字符串左边的字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="contrastText"></param>
        /// <param name="index"></param>
        /// <param name="IsLastIndex"></param>
        /// <returns></returns>
        public static string GetLeftText(this string text, string contrastText, int index = 0, bool IsLastIndex = false)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(contrastText)) return "";
            if (IsLastIndex)
            {
                index = text.LastIndexOf(contrastText, index);
            }
            else
            {
                index = text.IndexOf(contrastText, index);
            }
            if (index == -1) return "";
            return text.Substring(0, index);
        }


        /// <summary>
        /// 拷贝目标属性
        /// 将目标属性进行拷贝，不进行深度拷贝
        /// </summary>
        /// <param name="target">拷贝目标</param>
        /// <returns></returns>
        public static T AutoAssign<T, K>(this T example, K target)
        {
            if (target == null) throw new Exception("赋值类型不可为null");
            var targetType = target.GetType();//获得类型
            var exampleType = typeof(T);
            foreach (PropertyInfo sp in targetType.GetProperties())//获得类型的属性字段
            {
                foreach (PropertyInfo dp in exampleType.GetProperties())
                {
                    if (dp.Name.ToLower() == sp.Name.ToLower())//判断属性名是否相同
                    {
                        try
                        {
                            dp.SetValue(example, sp.GetValue(target, null), null);//获得s对象属性的值复制给d对象的属性
                        }
                        catch
                        {
                            //属性不一致或空属性不需要复制，所以直接忽略即可
                        }
                        break;
                    }
                }
            }
            return example;
        }

        /// <summary>
        /// 利用json将两个类型进行转换，短小精悍
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="A"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void AToB<B, A>(this A value,out B obj)
        {
            var json = JsonSerializer.Serialize(value);
            obj = JsonSerializer.Deserialize<B>(json);
        }

        /// <summary>
        /// 获取泛型某一属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="example"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetObjValue<T>(this T example,string name)
        {
            if (example == null) return null;
            var exampleType = example.GetType();//获得类型
            foreach (PropertyInfo sp in exampleType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))//获得类型的属性
            {
                if (sp.Name.ToLower() == name.ToLower())
                {
                    return sp.GetValue(example, null);
                }
            }
            foreach (FieldInfo sp in exampleType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))//获得类型的字段
            {
                if (sp.Name.ToLower() == name.ToLower())
                {
                    return sp.GetValue(example);
                }
            }
            return null;
        }

        private static List<Assembly> _assemblies;

        /// <summary>
        /// 使用加载器技术
        /// </summary>
        /// <returns></returns>
        public static List<Assembly> GetAssembly()
        {
            if (_assemblies == null)
            {
                _assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(u => !u.FullName.Contains("Microsoft"))//排除微软类库
                .Where(u => !u.FullName.Contains("System"))//排除系统类库
                .Where(u => !u.FullName.Contains("Newtonsoft"))//排除Newtonsoft.json
                .Where(u => !u.FullName.Contains("Swagger"))//排除Swagger
                .Where(u => !u.FullName.Contains("EntityFrameworkCore"))//排除EntityFrameworkCore
                .ToList();
            }
            return _assemblies;
        }
        /// <summary>
        /// 反射获取所有继承IBaseEntity的类
        /// 排除BaseEntity基类
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetEntityList()
        {
            List<Type> types = new List<Type>();
            GetAssembly()
                    //遍历查找
                    .ForEach((t =>
                    {
                        //获取所有对象
                        var assemblyTypes = t.GetTypes()
                            //查找是否包含IBaseModel接口的类
                            .Where(u => u.GetInterfaces().Contains(typeof(IBaseEntity)))
                            //判断是否是类
                            .Where(u => u.IsClass);
                        //转换成list
                        assemblyTypes.ToList()
                            //循环,并添注入
                            .ForEach(t =>
                            {
                                if(t != typeof(SysBaseEntity) && t != typeof(SysUseEntity))
                                {
                                    types.Add(t);
                                }
                            });
                    }));
            return types;
        }

        /// <summary>
        /// 列表转树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<T> ListToTree<T>(this IList<T> data) where T: class,ITree<T>
        {
            List<T> Tree = new List<T>();
            if (data == null) return Tree;
            foreach (var item in data)
            {
                if (item.ParentId == 0)
                {
                    Tree.Add(item);
                }
                else
                {
                    var ParentNode = data.SingleOrDefault(u => u.Id == item.ParentId);
                    if(ParentNode == null)
                    {
                        Tree.Add(item);//没有找到父节点，所以直接加入最上层节点
                    }
                    else
                    {
                        ParentNode.Children.Add(item);//加入父节点
                    }
                }
            }
            return Tree;
        }

        /// <summary>
        /// 多个树转列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="list"></param>
        public static void TreeToList<T>(this IList<T> data,IList<T> list) where T : class, ITree<T>
        {
            foreach (var item in data)
            {
                list.Add(item);
                if (item.Children!=null && item.Children.Count > 0)
                {
                    TreeToList(item.Children,list);
                }
            }
        }
        /// <summary>
        /// 单个树转为列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="list"></param>
        /// <param name="needParent">是否需要加入根节点，默认将根节点加入列表</param>
        public static void TreeToList<T>(this T data, IList<T> list,bool needParent = true) where T : class, ITree<T>
        {
            if (list == null)
            {
                list = new List<T>();
            }
            if (list.Count == 0 && needParent)
            {
                list.Add(data);
            }
            foreach (var item in data.Children)
            {
                list.Add(item);
                if (item.Children != null && item.Children.Count > 0)
                {
                    TreeToList(item, list, needParent);
                }
            }
        }
        /// <summary>
        /// 获取枚举的名称和值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<T, string> GetEnumModelHeader<T>(Type type,ILanguageService languageService)
        {
            if(!type.IsEnum) return null;
            var enumFields = type.GetFields();
            Dictionary<T, string> dic = null;
            if (enumFields != null && enumFields.Length >= 2)//枚举有一个隐藏的int所以要从下一位置开始
            {
                dic = new Dictionary<T, string>();
                for (int i = 0; i < enumFields.Length; i++)
                {
                    if (enumFields[i].Name == "value__") continue;
                    var value = (T)enumFields[i].GetValue(null);
                    var enumName = languageService[$"{CurrencyConstant.Enum}.{enumFields[i].Name}"];
                    if (!dic.ContainsKey(value))
                    {
                        dic.Add(value, enumName);
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// 用于查找该类是否继承了指定父类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type ContainBaseClass(this Type type,Type contrast)
        {
            var baseType = type.BaseType;
            if (baseType == null)
            {
                return null;
            }
            else if (baseType == contrast)
            {
                return type;
            }
            else
            {
                return baseType.ContainBaseClass(contrast);
            }
        }

        public static string UrlBase64Handle(string base64)
        {
            var base64url = HttpUtility.UrlDecode(base64);
            base64 = Encoding.UTF8.GetString(Convert.FromBase64String(base64url));
            return base64;
        }


        /// <summary>
        /// 利用反射来判断对象是否包含某个属性
        /// </summary>
        /// <param name="instance">object</param>
        /// <param name="propertyName">需要判断的属性</param>
        /// <returns>是否包含</returns>
        public static PropertyInfo ContainProperty(this object instance, string propertyName,Type returnType)
        {
            if (instance != null && !string.IsNullOrEmpty(propertyName))
            {
                PropertyInfo _findedPropertyInfo = instance.GetType().GetProperty(propertyName,returnType);
                return _findedPropertyInfo;
            }
            return null;
        }


        public static string GetClaimValue(SysFields fields)
        {
            return $"{fields.FullName}-{fields.FieldName}";
        }
    }
}
