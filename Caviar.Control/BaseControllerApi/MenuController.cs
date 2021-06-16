using Caviar.Control;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.Control.Menu
{
    public partial class MenuController
    {

        /// <summary>
        /// ��ȡ��ҳ��İ�ť
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetButtons(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return ResultError("��������ȷ��ַ");
            }
            var buttons = _Action.GetButton(url);
            ResultMsg.Data = buttons;
            return ResultOK();
        }

        public override async Task<IActionResult> Delete(ViewMenu view)
        {
            int count;
            if (view.IsDeleteAll)
            {
                count = await _Action.DeleteEntityAll(view);
            }
            else
            {
                count = await _Action.DeleteEntity(view);
            }
            if (count > 0)
            {
                return ResultOK();
            }
            return ResultError("ɾ���˵�ʧ��");
        }

    }
}