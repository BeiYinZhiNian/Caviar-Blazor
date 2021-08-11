using AntDesign;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Shared
{
    public partial class CavDataTemplate
    {
        public RenderFragment text;


        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        public RenderFragment Render() => builder =>
        {
            var ComponentType = typeof(Form<SysMenu>);
            var index = 0;
            builder.OpenComponent(index++, ComponentType);

            builder.AddContent(index++, u =>
            {
                u.OpenComponent(0, typeof(FormItem));
                u.AddMultipleAttributes(1, new List<KeyValuePair<string, object?>>()
                { 
                    new KeyValuePair<string, object?>("Label","菜单名称")
                });
                u.AddContent(2, t =>
                {
                    t.OpenComponent(0, typeof(Input<string>));
                    t.CloseComponent();
                });
                u.CloseComponent();
            });
            builder.CloseComponent();

        };

        void SetModelRef(object obj)
        {

        }
    }
}
