using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public static class CommonHelper
    {

        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SHA256EncryptString(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static bool IsPhoneNumber(string number)
        {
            return Regex.IsMatch(number, @"^[1][3-9]\\d{9}");
        }


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
        /// 自动分配当前属性值
        /// 不进行深度分配
        /// </summary>
        /// <param name="target">拷贝目标</param>
        /// <returns></returns>
        public static T AutoAssign<T, K>(this T example, K target)
        {
            var targetType = target.GetType();//获得类型
            var exampleType = typeof(T);
            foreach (PropertyInfo sp in targetType.GetProperties())//获得类型的属性字段
            {
                foreach (PropertyInfo dp in exampleType.GetProperties())
                {
                    if (dp.Name.ToLower() == "BaseControllerModel".ToLower()) continue;
                    if (dp.Name == sp.Name)//判断属性名是否相同
                    {
                        dp.SetValue(example, sp.GetValue(target, null), null);//获得s对象属性的值复制给d对象的属性
                    }
                }
            }
            return example;
        }
        /// <summary>
        /// 自动将target添加到example并进行类型转换
        /// 不进行深度转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="example"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IList<T> ListAutoAssign<T, K>(this IList<T> example, IList<K> target) where T:class, new()
        {
            foreach (var item in target)
            {
                example.Add(new T().AutoAssign(item));
            }
            return example;
        }


        /// <summary>
        /// 获取泛型某一属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="example"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object? GetObjValue<T>(this T example,string name)
        {
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
    }
}
