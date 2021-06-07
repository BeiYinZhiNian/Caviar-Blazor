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
            if (url == null)
            {
                return ResultError("��������ȷ��ַ");
            }
            var menus = BC.Menus.Where(u => u.Url?.ToLower() == url.ToLower()).FirstOrDefault();
            if (menus == null)
            {
                ResultMsg.Data = new List<SysMenu>();
                return ResultOK();
            }
            var buttons = BC.Menus.Where(u => u.MenuType == MenuType.Button && u.ParentId == menus.Id).OrderBy(u => u.Number);
            ResultMsg.Data = buttons;
            return ResultOK();
        }

        public override async Task<IActionResult> Delete(ViewMenu view)
        {
            if (view.IsDeleteAll)
            {
                return await DeleteAllEntity(view);
            }
            _Action.AutoAssign(view);
            List<ViewMenu> viewMenuList = new List<ViewMenu>();
            view.TreeToList(viewMenuList);
            var count = 0;
            if (viewMenuList != null && viewMenuList.Count != 0)
            {
                List<SysMenu> menus = new List<SysMenu>();
                menus.ListAutoAssign(viewMenuList);
                foreach (var item in menus)
                {
                    if (item.ParentId == view.Id)
                    {
                        item.ParentId = view.ParentId;
                    }
                }
                count = await _Action.UpdateEntity(menus);
                if (count != viewMenuList.Count)
                {
                    return ResultError("ɾ���˵�ʧ��,�Ӳ˵��ƶ�ʧ��");
                }
            }
            count = await _Action.DeleteEntity();
            if (count > 0)
            {
                return ResultOK();
            }
            return ResultError("ɾ���˵�ʧ��");
        }

        /// <summary>
        /// ����ɾ��
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        private async Task<IActionResult> DeleteAllEntity(ViewMenu viewMen)
        {
            List<ViewMenu> viewMenuList = new List<ViewMenu>();
            viewMen.TreeToList(viewMenuList);
            viewMenuList.Add(viewMen);//���Լ������ɾ������
            List<SysMenu> menus = new List<SysMenu>();
            menus.ListAutoAssign(viewMenuList);//��viewתΪsys
            var count = await _Action.DeleteEntity(menus);
            if(count == menus.Count)
            {
                return ResultOK();
            }
            return ResultError("����ɾ������ʧ��");
        }

    }
}