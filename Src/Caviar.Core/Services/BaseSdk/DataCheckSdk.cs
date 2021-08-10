using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services.BaseSdk
{
    public class DataCheckSdk
    {
        /// <summary>
        /// 用户拥有的字段权限
        /// </summary>
        public List<SysFields> Fields { get; set; }

        public ResultMsg ResultHandle(IActionResult result)
        {
            var StatusCode = result.GetObjValue("StatusCode");
            var value = result.GetObjValue("Value");
            if(value==null && StatusCode==null)
            {
                return null;
            }
            if (value != null)
            {
                ArgumentsModel(value.GetType(), value);
            }
            var code = StatusCode == null ? 200 : (int)StatusCode;
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
        public void ArgumentsModel(Type type, object data)
        {
            if (data == null) return;
            if (!type.IsClass)//排除非类
            {
                return;
            }
            else if (type == typeof(string))//排除字符串（特殊类）
            {
                return;
            }
            bool isBaseModel;
            isBaseModel = type.GetInterfaces().Contains(typeof(IBaseEntity));
            if (isBaseModel)
            {
                if (data == null) return;
                //进行非法字段检测
                ArgumentsFields(type, data);
            }
            else if (type.GetInterfaces().Contains(typeof(System.Collections.ICollection)))
            {
                var list = (System.Collections.IEnumerable)data;
                foreach (var dataItem in list)
                {
                    ArgumentsModel(dataItem.GetType(), dataItem);
                }
            }
            else
            {
                foreach (PropertyInfo sp in type.GetProperties())//获得类型的属性字段
                {
                    var properType = sp.PropertyType;
                    var value = sp.GetValue(data, null);
                    ArgumentsModel(properType, value);
                }
            }

        }

        /// <summary>
        /// 过滤返回的参数，检查是否含有未授权的字段
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void ArgumentsFields(Type type, object data)
        {
            var baseType = CommonHelper.GetBaseClass(type);
            if (baseType == null) return;
            foreach (PropertyInfo sp in baseType.GetProperties())//获得类型的属性字段
            {
                if (sp.Name.ToLower() == "id") continue;//忽略id字段
                if (sp.Name.ToLower() == "uid") continue;//忽略uid字段
                var field = Fields?.FirstOrDefault(u => u.BaseFullName == baseType.Name && sp.Name == u.FieldName);
                if (field == null)//如果为null则标名没有字段权限
                {
                    try
                    {
                        sp.SetValue(data, default, null);//设置为默认字段
                    }
                    catch
                    {
                        //忽略该错误,并记录到错误日志
                    }
                }
            }

        }

        public ResultMsg ResultCheck(int statusCode,object value)
        {
            ResultMsg resultMsg = new ResultMsg();
            if (value != null)
            {
                if(value is ResultMsg)
                {
                    return (ResultMsg)value;
                }
                else if(value is string)
                {
                    resultMsg = new ResultMsg();
                    resultMsg.Title = (string)value;
                }
                else
                {
                    resultMsg = new ResultMsg<object>(value);
                }
            }
            resultMsg.Status = statusCode;
            return resultMsg;
        }
    }
}
