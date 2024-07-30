using SEWC_ToolBox.DAL.DBHelpers;
using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox_Project;
using SEWC_ToolBox_Project.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SEWC_ToolBox.Controllers
{
    public class ToolBoxBaseController : BaseController
    {
        // ToolBox中Action执行之前加载菜单
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // 非CustomError、NotFound、PersonalInformation的Action均进行菜单加载
            if (!filterContext.ActionDescriptor.ActionName.Equals("CustomError") && !filterContext.ActionDescriptor.ActionName.Equals("NotFound") && !filterContext.ActionDescriptor.ActionName.Equals("PersonalInformation"))
            {
                try
                {
                    ViewBag.Main_ActionName = filterContext.ActionDescriptor.ActionName;

                    #region 解析当前用户在当前部门的人员权限

                    List<v_UserRole> CurrentUser_RoleList = ViewBag.CurrentUser_RoleList as List<v_UserRole>;
                    v_UserRole UserRole_CurrentDept = CurrentUser_RoleList.Where(o => o.ur_e_Dept_ID == ViewBag.DeptID).FirstOrDefault();

                    // 若用户无当前部门的权限，将当前部门权限设置为超默认权限或超级管理员
                    if (UserRole_CurrentDept == null)
                    {
                        v_UserRole temp = CurrentUser_RoleList.Where(o => o.ur_e_Dept_ID == 0).FirstOrDefault();

                        UserRole_CurrentDept = new v_UserRole()
                        {
                            a_ID = temp.a_ID,
                            a_Name = temp.a_Name,
                            a_Type_R = temp.a_Type_R,
                            a_Type_C = temp.a_Type_C,
                            a_Type_U = temp.a_Type_U,
                            a_Type_D = temp.a_Type_D,
                            ur_e_Dept_ID = ViewBag.DeptID,
                            User_GID = temp.User_GID,
                            User_DisplayText = temp.User_DisplayText
                        };
                    }

                    ViewBag.UserRole_CurrentDept = UserRole_CurrentDept;

                    #endregion

                    #region 访问具体菜单前，检查访问权限



                    #endregion

                    #region 根据 ControllerName 从缓存或数据库加载菜单

                    List<v_MenuList> MenuList = CacheConfig.Get<List<v_MenuList>>(CacheConfig.CacheType.MenuList);

                    List<v_SideMenuList> categoryMenuList = CacheConfig.Get<List<v_SideMenuList>>(CacheConfig.CacheType.MenuCategoryList);

                    // 若当前用户只有该部门的公共元素访问权限，则去掉其中的私有元素
                    if (UserRole_CurrentDept.a_Type_R < 2)
                        MenuList = MenuList.Where(o => (o.ml_Main_DeptID == ViewBag.DeptID && o.ml_Main_AccessLevel == 0 && o.ml_Sub_AccessLevel == 0 && o.ml_Third_AccessLevel == 0)).ToList();
                    // 若有访问私有元素权限，显示对应部门中的所有菜单
                    else
                        MenuList = MenuList.Where(o => o.ml_Main_DeptID == ViewBag.DeptID).ToList();



                    // 生成侧边顶部
                    List<v_MenuList> MenuList_Top = new List<v_MenuList>();

                    // 生成SubTop
                    List<v_MenuList> MenuList_SubTop = new List<v_MenuList>();



                    int last_MainMenuID = 0;
                    Nullable<int> last_SubMenuID = 0;

                    for (int i = 0; i < MenuList.Count; i++)
                    {
                        if (last_MainMenuID != MenuList[i].ml_Main_ID || (last_MainMenuID == MenuList[i].ml_Main_ID && last_SubMenuID != MenuList[i].ml_Sub_ID))
                            MenuList_Top.Add(MenuList[i]);

                        last_MainMenuID = MenuList[i].ml_Main_ID;
                        last_SubMenuID = MenuList[i].ml_Sub_ID;
                    }

                    // 生成侧边菜单(至少有一个子菜单)


                    List<v_MenuList> MenuList_Side = null;

                    if (ViewBag.Main_ActionName == "Reporting")
                    {
                        MenuList_Side = MenuList.Where(o => (o.ml_Main_ActionName.Equals(ViewBag.Main_ActionName) && o.ml_Sub_ID.HasValue && o.ml_Third_ID.HasValue)).ToList();
                    }
                    else if (ViewBag.Main_ActionName == "Home")
                    {
                        MenuList_Side = MenuList.Where(o => (o.ml_Main_ActionName.Equals(ViewBag.Main_ActionName) && o.ml_Sub_ID.HasValue && o.ml_Third_ID.HasValue)).ToList();
                    }
                    else
                    {
                        MenuList_Side = MenuList.Where(o => (o.ml_Main_ActionName.Equals(ViewBag.Main_ActionName) && o.ml_Sub_ID.HasValue)).ToList();
                    }
                    ViewBag.MenuList = MenuList_Top;
                    ViewBag.MenuList_Side = MenuList_Side.Count == 0 ? null : MenuList_Side;
                    ViewBag.CategoryMenuList = categoryMenuList;

                    #endregion
                }
                // 若发生错误，则
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #region Organization菜单

        public ActionResult Organization()
        {
            try
            {
                return View("~/Views/ToolBoxBase/Organization.cshtml");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Management菜单

        public ActionResult Management(string Sub_Action)
        {
            #region 若未选择子菜单，则初始化为默认选择项

            if (string.IsNullOrWhiteSpace(Sub_Action))
            {
                v_MenuList SubMenu = DBHelper_EntryMenu.Get_DefaultSubMenu(ViewBag.ControllerName, ViewBag.Main_ActionName);
                ViewBag.SubID = SubMenu.ml_Sub_ID;
                ViewBag.Sub_ActionName = SubMenu.ml_Sub_ActionName;
            }
            else
            {
                v_MenuList SubMenu = DBHelper_EntryMenu.Get_SubMenu_ByActionName(ViewBag.ControllerName, ViewBag.Main_ActionName, Sub_Action);
                ViewBag.SubID = SubMenu.ml_Sub_ID;
                ViewBag.Sub_ActionName = Sub_Action;
            }

            #endregion

            #region 根据不同子菜单加载不同内容

            // 如果是User Role
            if (ViewBag.Sub_ActionName.Equals("UserRole"))
            {
                List<v_UserRole> UserRole_List = DBHelper_User.Get_UserRole_ByDeptID(ViewBag.DeptID);

                ViewBag.UserRole_List = UserRole_List;
            }
            else if (ViewBag.Sub_ActionName.Equals("IssueFeedback"))
            {
                return RedirectToAction("Index", "Issue");
            }
            // 如果是Menu Management
            else if (ViewBag.Sub_ActionName.Equals("MenuManagement"))
            {
            }

            #endregion

            return View("~/Views/ToolBoxBase/Management.cshtml");
        }

        #endregion

        #region BusinessProcessManagement

        public ActionResult BusinessProcessManagement()
        {

            return Redirect("https://bpm-sewc.siemens.com.cn/ssologin.aspx");

            //ServiceReference1.SSOServiceSoapClient s = new ServiceReference1.SSOServiceSoapClient();
            //string usergid = "SEWC\\" + ViewBag.CurrentUserGID;
            ////string usergid = "SEWC\\z003y0ks";
            //string userToken = s.GetUserToken(usergid);
            //ViewBag.HasSession = false;
            //var sessionstr = System.Web.HttpContext.Current.Session["AUTH_SESSION_ID"];
            //var sessionstr2 = System.Web.HttpContext.Current.Session[".UltimusFormAuth"];
            //var sessionstr3 = System.Web.HttpContext.Current.Session["ASP.NET_SessionId"];

            //if (sessionstr != null && sessionstr2 != null)
            //{
            //    ViewBag.HasSession = true;
            //}

            //ViewBag.UserToken = userToken;
            //try
            //{
            //    return View("/" + ViewBag.PreUrl + "/Views/ToolBoxBase/BusinessProcessManagement.cshtml");
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        #endregion



    }
}