using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure
{
    public static class FieldScanner
    {
        /// <summary>
        /// 获取继承了IBaseEntity类的所有字段字段信息
        /// </summary>
        /// <returns></returns>
        public static List<ViewFields> GetApplicationFields()
        {
            var entityList = CommonHelper.GetEntityList();
            var fields = new List<ViewFields>();
            foreach (var item in entityList)
            {
                var _field = GetClassFields(item.Name);
                fields.AddRange(_field);
            }
            return fields;
        }

        /// <summary>
        /// 获取类下所有字段
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<ViewFields> GetClassFields(string name)
        {
            var assemblyList = CommonHelper.GetAssembly();
            Type type = null;
            foreach (var item in assemblyList)
            {
                type = item.GetTypes().SingleOrDefault(u => u.Name.ToLower() == name.ToLower());
                if (type != null) break;
            }
            return GetClassFields(type);
        }

        public static List<ViewFields> GetClassFields(Type type,bool isFieldTurnMeaning = true)
        {
            List<ViewFields> fields = new List<ViewFields>();
            if (type != null)
            {
                foreach (var item in type.GetRuntimeProperties())
                {
                    var typeName = item.PropertyType.Name;
                    if (typeName == "Nullable`1")//可为null的字段
                    {
                        var Arguments = item.PropertyType.GenericTypeArguments;
                        if (Arguments.Length > 0)
                        {
                            typeName = Arguments[0].Name;
                        }
                    }
                    var baseType = CommonHelper.GetBaseClass(type);
                    var dispLayName = item.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                    var fieldLen = item.GetCustomAttributes<StringLengthAttribute>()?.Cast<StringLengthAttribute>().SingleOrDefault()?.MaximumLength;
                    var field = new ViewFields()
                    {
                        Entity = new SysFields()
                        {
                            FieldName = item.Name,
                            DisplayName = dispLayName,
                            FieldLen = fieldLen,
                            FullName = type.Name,
                            BaseFullName = baseType?.Name,
                            IsDisable = true,
                        },
                        EntityType = typeName,
                        IsEnum = item.PropertyType.IsEnum
                    };

                    if (field.IsEnum)
                    {
                        field.EnumValueName = CommonHelper.GetEnenuModelHeader(item.PropertyType);
                    }
                    if(isFieldTurnMeaning) field = FieldTurnMeaning(field);
                    fields.Add(field);
                }
            }
            return fields;
        }

        public static Dictionary<string,string> TurnMeaningDic { get; set; }

        /// <summary>
        /// 字段类型转义
        /// 找到指定字段，将该字段改为指定字段
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static ViewFields FieldTurnMeaning(ViewFields headers)
        {
            if (TurnMeaningDic == null) return headers;
            if(!TurnMeaningDic.ContainsKey(headers.Entity.FieldName)) return headers;
            var EntityType = TurnMeaningDic[headers.Entity.FieldName];
            if (EntityType != null)
            {
                headers.EntityType = EntityType;
                return headers;
            }
            return headers;
        }
    }
}
