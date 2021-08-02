using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel
{
    /// <summary>
    /// 返回给客户端使用的数据，禁止填写敏感信息
    /// </summary>
    public partial class UserToken
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = "未登录用户";
        /// <summary>
        /// 用户id
        /// </summary>
        public int Id { get; set; } = 0;
        /// <summary>
        /// 用户token
        /// </summary>
        public string Token { get; set; } = "";
        /// <summary>
        /// 用户头像路径
        /// </summary>
        public string HeadPortrait { get; set; }
        /// <summary>
        /// 所在用户组
        /// </summary>
        public int UserGroupId { get; set; }
        /// <summary>
        /// token使用时长
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// uid
        /// </summary>
        public Guid Uid { get; set; } = Guid.NewGuid();
        /// <summary>
        /// 是否登录
        /// </summary>
        /// <returns></returns>
        public bool IsLogin()
        {
            return Id > 0 && Token != "";
        }

    }
}
