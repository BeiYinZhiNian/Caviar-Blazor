using Caviar.Control;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.Control.Menu
{
    public partial class MenuController : CaviarBaseController
    {
        /// <summary>
        /// ��ȡ��ҳ��İ�ť
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetButtons(string url)
        {
            if(url == null)
            {
                return ResultErrorMsg("��������ȷ��ַ");
            }
            var menus = Action.GetEntitys(u => u.Url.ToLower() == url.ToLower()).FirstOrDefault();
            if (menus == null)
            {
                ResultMsg.Data = new List<SysMenu>();
                return ResultOK();
            }
            var buttons = Action.GetEntitys(u => u.MenuType == MenuType.Button && u.ParentId == menus.Id).OrderBy(u=>u.Number);
            ResultMsg.Data = buttons;
            return ResultOK();
        }

        /// <summary>
        /// �Ƴ��˵�,�����Ӳ˵���Ĭ���ƶ�����һ��
        /// </summary>
        /// <param name="viewMen"></param>
        /// <param name="IsDeleteAll">ΪTrueʱɾ��ȫ���Ӳ˵�</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Move(ViewMenu viewMen,bool IsDeleteAll)
        {
            if (IsDeleteAll)
            {
                return await DeleteAllEntity(viewMen);
            }
            Action.AutoAssign(viewMen);
            List<ViewMenu> viewMenuList = new List<ViewMenu>();
            viewMen.TreeToList(viewMenuList);
            var count = 0;
            if(viewMenuList!=null && viewMenuList.Count != 0)
            {
                List<SysMenu> menus = new List<SysMenu>();
                menus.ListAutoAssign(viewMenuList);
                foreach (var item in menus)
                {
                    if (item.ParentId == viewMen.Id)
                    {
                        item.ParentId = viewMen.ParentId;
                    }
                }
                count = await Action.UpdateEntity(menus);
                if(count != viewMenuList.Count)
                {
                    return ResultErrorMsg("ɾ���˵�ʧ��,�Ӳ˵��ƶ�ʧ��");
                }
            }
            count = await Action.DeleteEntity();
            if (count > 0)
            {
                return ResultOK();
            }
            return ResultErrorMsg("ɾ���˵�ʧ��");
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
            var count = await Action.DeleteEntity(menus);
            if(count == menus.Count)
            {
                return ResultOK();
            }
            return ResultErrorMsg("����ɾ������ʧ��");
        }



        
    }
}