using AntDesign;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Caviar.SharedKernel.Entities.View;
using Caviar.SharedKernel.Entities;

namespace Caviar.AntDesignUI.Core
{
    public partial class DataComponentBase<ViewT,T> : CavComponentBase,ITableTemplate where ViewT:IView<T>,new() where T: IUseEntity,new()
    {
        #region 参数
        [Parameter]
        public ViewT DataSource { get; set; } = new ViewT()
        {
            Entity = new T()
        };


        [Parameter]
        public string SuccMsg { get; set; } = "操作成功";

        /// <summary>
        /// 模型字段
        /// </summary>
        protected List<FieldsView> ViewFields { get; set; } = new List<FieldsView>();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected virtual FormValidationRule[] GetFormValidationRules(object model, string fieldName)
        {
            var type = model.GetType();
            var property = type.GetProperty(fieldName);
            if (property == null) return null;
            List<FormValidationRule> formValidationRules = new List<FormValidationRule>();
            var lenAttribute = property.GetCustomAttributes<StringLengthAttribute>()?.SingleOrDefault();
            if(lenAttribute != null)
            {
                formValidationRules.Add(new FormValidationRule()
                {
                    Len = lenAttribute.MaximumLength,
                    Message = LanguageService[$"{CurrencyConstant.ErrorMessage}.LengthErrorMsg"].Replace("{fieldName}", LanguageService[$"{CurrencyConstant.EntitysName}.{fieldName}"]).Replace("{length}", lenAttribute.MaximumLength.ToString()),
                });
            }
            var maxAttribute = property.GetCustomAttributes<MaxLengthAttribute>()?.SingleOrDefault();
            if(maxAttribute != null)
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

        #region 回调
        public Form<ViewT> _meunForm;
        /// <summary>
        /// 提交前本地校验
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> Validate()
        {
            if (_meunForm == null)//当_meunForm为null时，不进行表单验证
            {
                return await FormSubmit();
            }
            //数据效验
            if (_meunForm.Validate())
            {
                return await FormSubmit();
            }
            return false;
        }
        /// <summary>
        /// 开始表单提交
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> FormSubmit()
        {
            var result = await HttpService.PostJson(SubmitUrl, DataSource);
            if (result.Status == HttpStatusCode.OK)
            {
                _ = MessageService.Success(SuccMsg);
                return true;
            }
            return false;
        }

        #endregion
    }
}