﻿using System;
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
        /// </summary>
        /// <param name="target">拷贝目标</param>
        /// <returns></returns>
        public static void AutoAssign<T, K>(this T example, K target)
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
        }
    }
}
