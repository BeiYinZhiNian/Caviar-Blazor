using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Caviar.SharedKernel.Entities
{
    public class ApplicationUser: IdentityUser<int>, IUseEntity
    {
        
        [Required(ErrorMessage = "RequiredErrorMsg")]
        public override string UserName { get => base.UserName; set => base.UserName = value; }

        [Required(ErrorMessage = "RequiredErrorMsg")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "RequiredErrorMsg")]
        public override string Email { get => base.Email; set => base.Email = value; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 根据配置确定删除后是否保留条目
        /// </summary>
        public bool IsDelete { get; set; } = false;
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [StringLength(256, ErrorMessage = "LengthErrorMsg")]
        public string OperatorCare { get; set; }
        /// <summary>
        /// 创建操作员的名称
        /// </summary>
        [StringLength(256, ErrorMessage = "LengthErrorMsg")]
        public string OperatorUp { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(300, ErrorMessage = "LengthErrorMsg")]
        public string Remark { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisable 
        { 
            get => LockoutEnabled;
            set 
            {
                LockoutEnabled = value;
                if (value)
                {
                    LockoutEnd = DateTime.Now.AddYears(99);
                }
                else
                {
                    LockoutEnd = null;
                }
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string Number { get; set; } = "999";
        /// <summary>
        /// 数据权限
        /// </summary>
        public int DataId { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(300, ErrorMessage = "LengthErrorMsg")]
        public string HeadPortrait { get; set; }
        /// <summary>
        /// 所在用户组
        /// </summary>
        [Required(ErrorMessage = "RequiredErrorMsg")]
        public int UserGroupId { get; set; }
    }
}
