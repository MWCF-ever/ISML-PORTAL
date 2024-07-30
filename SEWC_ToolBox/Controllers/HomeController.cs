using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox.DAL.DBHelpers;
using System.Collections.Generic;
using System.Web.Mvc;
using SEWC_ToolBox.Utilities.Helpers;
using System.Linq;
using SEWC_ToolBox.Languages;

namespace SEWC_ToolBox_Project.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult HomePage()
        {
            #region 从缓存加载入口列表
            
            List<t_Entry> EntryList = CacheConfig.Get<List<t_Entry>>(CacheConfig.CacheType.EntryList);

            #endregion

            #region 去除无法访问的入口

            // 若当前用户只有公共入口访问权限，则去掉其中的私有入口
            if (!ViewBag.EntrancePrivateAccess)
                EntryList = EntryList.Where(o => o.e_AccessLevel == 0).ToList();

            #endregion

            return View(EntryList);
        }

        public ActionResult PersonalInformation()
        {
            v_User CurrentUser = Session["CurrentUser"] as v_User;

            #region 解析当前用户角色权限

            List<v_UserRole> CurrentUser_RoleList = ViewBag.CurrentUser_RoleList as List<v_UserRole>;

            List<v_UserRole> CurrentUserRole_Display = new List<v_UserRole>();

            foreach(v_UserRole Cur in CurrentUser_RoleList)
            {
                CurrentUserRole_Display.Add(Cur);
            }

            // 未配置或是超管权限
            if (CurrentUserRole_Display.Count == 1 && CurrentUserRole_Display[0].ur_e_Dept_ID == 0)
                CurrentUserRole_Display[0].e_Dept_AbbreviatedName = LanguageHelper.InnerLang(this.HttpContext, "txt_AllDepts");
            // 若已配置过权限，则额外设置一个对其他部门的只读权限
            else
                CurrentUserRole_Display.Where(o => o.ur_e_Dept_ID == 0).FirstOrDefault().e_Dept_AbbreviatedName = LanguageHelper.InnerLang(this.HttpContext, "txt_RestOfDepts");
                
            ViewBag.UserRole_List = CurrentUserRole_Display;

            #endregion

            return View(CurrentUser);
        }
    }
}