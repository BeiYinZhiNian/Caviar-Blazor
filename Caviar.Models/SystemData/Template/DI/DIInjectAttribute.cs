using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    /// <summary>
    /// 自动注入
    /// 允许被继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class DIInjectAttribute : Attribute
    {
        public DIInjectAttribute(InjectType injectType)
        {
            this.InjectType = injectType;
        }
        public DIInjectAttribute()
        {
            InjectType = InjectType.SCOPED;
        }
        public InjectType InjectType { get; }

    }
    /// <summary>
    /// 注入类型选择
    /// </summary>
    public enum InjectType
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        SINGLETON,
        /// <summary>
        /// 瞬时
        /// </summary>
        TRANSIENT,
        /// <summary>
        /// 单次
        /// </summary>
        SCOPED
    }
}
