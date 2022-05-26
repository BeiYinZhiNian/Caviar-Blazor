// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using AntDesign;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Shared
{
    public class CavFormItem : FormItem
    {
        [Parameter]
        public string FieldName { get; set; }

        [Parameter]
        public object FieldRules { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }
        private string _displayName;
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
                if (Rules != null)
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
                    Type = FormFieldType.String,
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
                    Type = FormFieldType.String,
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
                    Type = FormFieldType.String,
                    Min = minAttribute.Length,
                    Message = LanguageService[$"{CurrencyConstant.ErrorMessage}.{errorMeg}"].Replace("{0}", _displayName).Replace("{1}", minAttribute.Length.ToString()),
                });
            }
            var rangeAttribute = property.GetCustomAttributes<RangeAttribute>()?.SingleOrDefault();
            if (rangeAttribute != null)
            {
                var errorMeg = rangeAttribute.ErrorMessage ?? "RangeErrorMsg";
                formValidationRules.Add(new FormValidationRule()
                {
                    Type = FormFieldType.Number,
                    Min = decimal.Parse(rangeAttribute?.Minimum.ToString()),
                    Max = decimal.Parse(rangeAttribute?.Maximum.ToString()),
                    Message = LanguageService[$"{CurrencyConstant.ErrorMessage}.{errorMeg}"]
                    .Replace("{0}", _displayName)
                    .Replace("{1}", rangeAttribute?.Minimum.ToString())
                    .Replace("{2}", rangeAttribute?.Maximum.ToString()),
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
                else if (property.PropertyType.IsEnum)
                {
                    formField = FormFieldType.Enum;
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    formField = FormFieldType.Date;
                }
                else if (property.PropertyType.IsArray)
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
            if (regularExpression != null)
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
