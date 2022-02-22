using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    public static class FieldScannerServices
    {
        /// <summary>
        /// 获取继承了IBaseEntity类的所有字段字段信息
        /// </summary>
        /// <returns></returns>
        public static List<FieldsView> GetApplicationFields(ILanguageService languageService)
        {
            var entityList = CommonHelper.GetEntityList();
            var fields = new List<FieldsView>();
            foreach (var item in entityList)
            {
                var _field = GetClassFields(item.Name,item.FullName, languageService);
                fields.AddRange(_field);
            }
            return fields;
        }

        /// <summary>
        /// 获取继承了IBaseEntity实体信息
        /// </summary>
        /// <returns></returns>
        public static List<FieldsView> GetEntitys(ILanguageService languageService)
        {
            var entityList = CommonHelper.GetEntityList();
            var fields = new List<FieldsView>();
            foreach (var item in entityList)
            {
                
                fields.Add(new FieldsView() { 
                    DisplayName = languageService[$"{CurrencyConstant.EntitysName}.{item.Name}"],
                    Entity = new SysFields()
                    {
                        FieldName = item.Name,
                        FullName = item.FullName
                    }
                });
            }
            return fields;
        }
        /// <summary>
        /// 获取指定实体类信息
        /// </summary>
        /// <returns></returns>
        public static FieldsView GetEntity(string name, string fullName, ILanguageService languageService)
        {
            var listFields = GetEntitys(languageService);
            foreach (var item in listFields)
            {
                if(item.Entity.FieldName == name && item.Entity.FullName == fullName)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取类下所有字段
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<FieldsView> GetClassFields(string name,string fullName, ILanguageService languageService)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (string.IsNullOrEmpty(fullName)) return null;
            var assemblyList = CommonHelper.GetAssembly();
            Type type = null;
            foreach (var item in assemblyList)
            {
                type = item.GetTypes().SingleOrDefault(u => 
                u.Name.ToLower() == name.ToLower() &&
                u.FullName.ToLower() == fullName.ToLower());
                if (type != null) break;
            }
            if (type == null) throw new Exception("未找到该类：" + name);
            return GetClassFields(type, languageService);
        }
        /// <summary>
        /// 获取一个类的字段信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isFieldTurnMeaning">是否开启转义，默认开启</param>
        /// <returns></returns>
        public static List<FieldsView> GetClassFields(Type type,ILanguageService languageService,bool isFieldTurnMeaning = true)
        {
            List<FieldsView> fields = new List<FieldsView>();
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
                    var baseType = typeof(SysUseEntity);
                    var dispLayName = languageService[$"{CurrencyConstant.EntitysName}.{item.Name}"];
                    var fieldLen = item.GetCustomAttributes<StringLengthAttribute>()?.Cast<StringLengthAttribute>().SingleOrDefault()?.MaximumLength;
                    var field = new FieldsView()
                    {
                        Entity = new SysFields()
                        {
                            FieldName = item.Name,
                            FieldLen = fieldLen,
                            FullName = type.FullName,
                            BaseFullName = baseType?.Name,
                            IsDisable = true,
                        },
                        EntityType = typeName,
                        DisplayName = dispLayName,
                        IsEnum = item.PropertyType.IsEnum
                    };

                    if (field.IsEnum)
                    {
                        field.EnumValueName = CommonHelper.GetEnumModelHeader<int>(item.PropertyType, languageService);
                    }
                    if(isFieldTurnMeaning) field = FieldTurnMeaning(field);
                    fields.Add(field);
                }
            }
            return fields;
        }

        public static Dictionary<string, string> TurnMeaningDic { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 字段类型转义
        /// 找到指定字段，将该字段改为指定字段
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static FieldsView FieldTurnMeaning(FieldsView headers)
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
