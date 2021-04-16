using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    /// <summary>
    /// 返回给客户端使用的数据，禁止填写敏感信息
    /// </summary>
    public class UserToken
    {
        public string UserName { get; set; } = "未登录用户";

        public int Id { get; set; } = 0;

        public string Token { get; set; } = "";

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public Guid Uid { get; set; } = Guid.NewGuid();

    }
}
