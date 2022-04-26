
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Caviar.SharedKernel.Entities;
using System.Security.Claims;
using Caviar.Core.Services;
using Caviar.Core.Interface;

namespace Caviar.Infrastructure
{
    public class ResultScannerServices:BaseServices
    {
        /// <summary>
        /// 是否开启数据过滤
        /// </summary>
        public static bool IsDataFilter { get; set; } = true;

        public static List<string> IgnoreField { get; set; } = new List<string>() { "id","uid","key"  };

        public ResultMsg ResultHandle(object result)
        {
            var StatusCode = result.GetObjValue("StatusCode");
            var value = result.GetObjValue("Value");
            if(value==null && StatusCode==null)
            {
                return null;
            }
            if (value != null && IsDataFilter)
            {
                //value = ArgumentsModel(value.GetType(), value);
            }
            var code = StatusCode == null ? HttpStatusCode.OK : (HttpStatusCode)StatusCode;
            if (value is ResultMsg)
            {
                var msg = (ResultMsg)value;
                if (code != HttpStatusCode.OK)
                {
                    msg.Status = code;
                }
                return msg;
            }
            var resultMsg = ResultCheck(code, value);
            return resultMsg;
        }


        /// <summary>
        /// 过滤对象中所有字段，检测包含IbaseEntity接口类
        /// 将包含IbaseEntity接口的类进行字段检测
        /// </summary>
        /// <param name="type">检测字段的type</param>
        /// <param name="data">需要检测的字段,过滤后将未授权字段的值设置为default</param>
        /// <param name="fields">用户拥有的字段权限</param>
        /// <
        public object ArgumentsModel(Type type,object data)
        {
            if (data == null) return data;
            if (!type.IsClass)//排除非类
            {
                return data;
            }
            else if (type == typeof(string))//排除字符串（特殊类）
            {
                return data;
            }
            var copyObj = Activator.CreateInstance(type);
            bool isBaseModel;
            isBaseModel = type.GetInterfaces().Contains(typeof(IUseEntity));
            if (isBaseModel)
            {
                if (data == null) return data;
                //进行非法字段检测
                return ArgumentsFields(type,data);
            }
            else if (type.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)))
            {
                var list = (System.Collections.IEnumerable)data;
                var resultList = new List<object>();
                foreach (var item in list)
                {
                    var result = ArgumentsModel(item.GetType(), item);
                    resultList.Add(result);
                }
                return resultList;
            }
            else
            {
                foreach (PropertyInfo sp in type.GetProperties())//获得类型的属性字段
                {
                    var properType = sp.PropertyType;
                    var value = sp.GetValue(data, null);
                    value = ArgumentsModel(properType, value);
                    try
                    {
                        sp.SetValue(copyObj, value, null);
                    }
                    catch
                    {
                        // 没有set方法的忽略
                    }
                }
                return copyObj;
            }

        }

        public List<SysPermission> PermissionFieldss { get; set; }


        /// <summary>
        /// 过滤返回的参数，检查是否含有未授权的字段
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public object ArgumentsFields(Type type,object data)
        {
            var baseType = type.GetInterfaces().FirstOrDefault(u=>u == typeof(IUseEntity));
            if (baseType == null) return null;
            var copyObj = Activator.CreateInstance(type);
            foreach (PropertyInfo sp in type.GetProperties())//获得类型的属性字段
            {
                bool assignment = true;
                if (!(sp.PropertyType.IsEnum || IgnoreField.FirstOrDefault(u => u.ToLower() == sp.Name.ToLower()) != null))//忽略枚举字段,和指定字段
                {
                    var permissions = PermissionFieldss?.FirstOrDefault(u => u.Permission == (type.FullName + sp.Name));
                    assignment = permissions != null;
                }
                if (assignment)//如果为null则标名没有字段权限
                {
                    try
                    {
                        sp.SetValue(copyObj, sp.GetValue(data), null);//设置为默认字段
                    }
                    catch
                    {
                        //忽略该错误
                    }
                }

            }
            return copyObj;
        }

        public ResultMsg<object> ResultCheck(HttpStatusCode statusCode,object value)
        {
            ResultMsg<object> resultMsg = new ResultMsg<object>();
            if (value != null)
            {
                if(value is ResultMsg)
                {
                    return (ResultMsg<object>)value;
                }
                else
                {
                    resultMsg.Data = value;
                }
            }
            if (statusCode != HttpStatusCode.OK)
            {
                resultMsg.Title = statusCode.ToString();
            }
            resultMsg.Status = statusCode;
            return resultMsg;
        }
    }
}
