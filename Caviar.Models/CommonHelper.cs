using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
