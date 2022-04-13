using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Caviar.AntDesignUI.Shared
{
    public class CavFormItem: FormItem
    {
        [Inject]
        UserConfig UserConfig { get; set; }

        [Parameter]
        public string FieldName { get; set; }

        [Parameter]
        public object FieldRules { get; set; }

        private string _displayName;

        private ILanguageService LanguageService => UserConfig.LanguageService;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (Label == null && !string.IsNullOrEmpty(FieldName))
            {
                Label = LanguageService[$"{CurrencyConstant.EntitysName}.{FieldName}"];
            }
            if (string.IsNullOrEmpty(Label))
            {
                _displayName = LanguageService[$"{CurrencyConstant.EntitysName}.{FieldName}"];
            }
            else
            {
                _displayName = Label;
            }
            if (FieldRules != null)
            {
                var customRules = GetFormValidationRules(FieldRules, FieldName);
                if(Rules != null)
                {
                    customRules.AddRange(Rules);
                }
                Rules = customRules?.ToArray();
            }

        }

        protected virtual List<FormValidationRule> GetFormValidationRules(object model, string fieldName)
        {
            var type = model.GetType();
            var property = type.GetProperty(fieldName);
            if (property == null) return null;
            List<FormValidationRule> formValidationRules = new List<FormValidationRule>();
            var lenAttribute = property.GetCustomAttributes<StringLengthAttribute>()?.SingleOrDefault();
            if (lenAttribute != null)
            {
                var errorMeg = lenAttribute.ErrorMessage ?? "LengthErrorMsg";
                formValidationRules.Add(new FormValidationRule()
                {
                    Len = lenAttribute.MaximumLength,
                    Message = LanguageService[$"{CurrencyConstant.ErrorMessage}.{errorMeg}"].Replace("{0}", _displayName).Replace("{1}", lenAttribute.MaximumLength.ToString()),
                });
            }
            var maxAttribute = property.GetCustomAttributes<MaxLengthAttribute>()?.SingleOrDefault();
            if (maxAttribute != null)
            {
                var errorMeg = maxAttribute.ErrorMessage ?? "MaxErrorMsg";
                var message = LanguageService[$"{CurrencyConstant.ErrorMessage}.{errorMeg}"].Replace("{0}", _displayName).Replace("{1}", maxAttribute.Length.ToString());
                formValidationRules.Add(new FormValidationRule()
                {
                    Type = FormFieldType.Number,
                    Max = maxAttribute.Length,
                    Message = message,
                });
            }
            var minAttribute = property.GetCustomAttributes<MinLengthAttribute>()?.SingleOrDefault();
            if (minAttribute != null)
            {
                var errorMeg = minAttribute.ErrorMessage ?? "MinErrorMsg";
                formValidationRules.Add(new FormValidationRule()
                {
                    Type = FormFieldType.Number,
                    Min = minAttribute.Length,
                    Message = LanguageService[$"{CurrencyConstant.ErrorMessage}.{errorMeg}"].Replace("{0}", _displayName).Replace("{1}", minAttribute.Length.ToString()),
                });
            }
            var requiredAttribute = property.GetCustomAttributes<RequiredAttribute>()?.SingleOrDefault();
            if (requiredAttribute != null)
            {
                var formField = FormFieldType.String;
                if (property.PropertyType == typeof(bool))
                {
                    formField = FormFieldType.Boolean;
                }
                else if (property.PropertyType == typeof(Enum))
                {
                    formField = FormFieldType.Enum;
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    formField = FormFieldType.Date;
                }
                else if (property.PropertyType == typeof(IEnumerable<>))
                {
                    formField = FormFieldType.Array;
                }
                else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(uint))
                {
                    formField = FormFieldType.Number;
                }
                else if (property.PropertyType == typeof(float) || property.PropertyType == typeof(double))
                {
                    formField = FormFieldType.Float;
                }
                Required = true;
                var errorMeg = requiredAttribute.ErrorMessage ?? "RequiredErrorMsg";
                formValidationRules.Add(new FormValidationRule()
                {
                    Type = formField,
                    Required = true,
                    Message = LanguageService[$"{CurrencyConstant.ErrorMessage}.{errorMeg}"].Replace("{0}", _displayName),
                });
            }
            var regularExpression = property.GetCustomAttributes<RegularExpressionAttribute>()?.SingleOrDefault();
            if (regularExpression!= null)
            {
                var errorMeg = regularExpression.ErrorMessage ?? "RegularErrorMsg";
                formValidationRules.Add(new FormValidationRule()
                {
                    Type = FormFieldType.Regexp,
                    Pattern = regularExpression.Pattern,
                    Message = LanguageService[$"{CurrencyConstant.ErrorMessage}.{errorMeg}"].Replace("{0}", _displayName),
                });
            }
            return formValidationRules;
        }
    }
}
