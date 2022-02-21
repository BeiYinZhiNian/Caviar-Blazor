using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
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

        private ILanguageService LanguageService => UserConfig.LanguageService;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (string.IsNullOrEmpty(Label) && !string.IsNullOrEmpty(FieldName))
            {
                Label = LanguageService[$"{CurrencyConstant.EntitysName}.{FieldName}"];
            }
            if (FieldRules != null)
            {
                Rules = GetFormValidationRules(FieldRules, FieldName);
            }

        }

        protected virtual FormValidationRule[] GetFormValidationRules(object model, string fieldName)
        {
            var type = model.GetType();
            var property = type.GetProperty(fieldName);
            if (property == null) return null;
            List<FormValidationRule> formValidationRules = new List<FormValidationRule>();
            var lenAttribute = property.GetCustomAttributes<StringLengthAttribute>()?.SingleOrDefault();
            if (lenAttribute != null)
            {
                formValidationRules.Add(new FormValidationRule()
                {
                    Len = lenAttribute.MaximumLength,
                    Message = LanguageService[$"{CurrencyConstant.ErrorMessage}.LengthErrorMsg"].Replace("{fieldName}", LanguageService[$"{CurrencyConstant.EntitysName}.{fieldName}"]).Replace("{length}", lenAttribute.MaximumLength.ToString()),
                });
            }
            var maxAttribute = property.GetCustomAttributes<MaxLengthAttribute>()?.SingleOrDefault();
            if (maxAttribute != null)
            {
                var message = LanguageService[$"{CurrencyConstant.ErrorMessage}.MaxErrorMsg"].Replace("{fieldName}", LanguageService[$"{CurrencyConstant.EntitysName}.{fieldName}"]).Replace("{length}", maxAttribute.Length.ToString());
                formValidationRules.Add(new FormValidationRule()
                {
                    Max = maxAttribute.Length,
                    Message = message,
                });
            }
            var minAttribute = property.GetCustomAttributes<MinLengthAttribute>()?.SingleOrDefault();
            if (minAttribute != null)
            {
                formValidationRules.Add(new FormValidationRule()
                {
                    Min = minAttribute.Length,
                    Message = LanguageService[$"{CurrencyConstant.ErrorMessage}.MinErrorMsg"].Replace("{fieldName}", LanguageService[$"{CurrencyConstant.EntitysName}.{fieldName}"]).Replace("{length}", minAttribute.Length.ToString()),
                });
            }
            var requiredAttribute = property.GetCustomAttributes<RequiredAttribute>()?.SingleOrDefault();
            if (requiredAttribute != null)
            {
                formValidationRules.Add(new FormValidationRule()
                {
                    Required = true,
                    Message = LanguageService[$"{CurrencyConstant.ErrorMessage}.RequiredErrorMsg"].Replace("{fieldName}", LanguageService[$"{CurrencyConstant.EntitysName}.{fieldName}"]),
                });
            }
            return formValidationRules.ToArray();
        }
    }
}
