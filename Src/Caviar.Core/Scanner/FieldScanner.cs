using Caviar.SharedKernel.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Scanner
{
    public class FieldScanner
    {
        /// <summary>
        /// 获取继承了IBaseEntity类的所有字段字段信息
        /// </summary>
        /// <returns></returns>
        public List<ViewFields> GetApplicationFields()
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
        public List<ViewFields> GetClassFields(string name)
        {
            var assemblyList = CommonHelper.GetAssembly();
            Type type = null;
            foreach (var item in assemblyList)
            {
                type = item.GetTypes().SingleOrDefault(u => u.Name.ToLower() == name.ToLower());
                if (type != null) break;
            }
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
                    var baseType = CommonHelper.GetCavBaseType(type);
                    var dispLayName = item.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                    var valueLen = item.GetCustomAttributes<StringLengthAttribute>()?.Cast<StringLengthAttribute>().SingleOrDefault()?.MaximumLength;
                    var filter = new ViewFields()
                    {
                        FildName = item.Name,
                        EntityType = typeName,
                        DisplayName = dispLayName,
                        ValueLen = valueLen,
                        IsEnum = item.PropertyType.IsEnum,
                        FullName = name,
                        BaseFullName = baseType.Name,
                        IsDisable = true,
                    };
                    if (filter.IsEnum)
                    {
                        filter.EnumValueName = CommonHelper.GetEnenuModelHeader(item.PropertyType);
                    }
                    filter = FieldTurnMeaning(filter);
                    fields.Add(filter);
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
        public virtual ViewFields FieldTurnMeaning(ViewFields headers)
        {
            if (TurnMeaningDic == null) return headers;
            if(!TurnMeaningDic.ContainsKey(headers.FildName)) return headers;
            var EntityType = TurnMeaningDic[headers.FildName];
            if (EntityType != null)
            {
                headers.EntityType = EntityType;
                return headers;
            }
            return headers;
        }
    }
}
