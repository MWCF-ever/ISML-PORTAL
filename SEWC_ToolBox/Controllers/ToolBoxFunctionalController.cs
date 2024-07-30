using SEWC_ToolBox.DAL.DBHelpers;
using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox.DAL.SecondModels;
using SEWC_ToolBox.Utilities.Helpers;
using SEWC_ToolBox_Project;
using SEWC_ToolBox_Project.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEWC_ToolBox.Controllers
{
    public class ToolBoxFunctionalController : BaseController
    {
        public ActionResult Reporting_Add(int DeptID, int SubID, int ThirdID, int fourthID)
        {
            ViewBag.DeptID = DeptID;
            ViewBag.ReportGUID = Guid.NewGuid().ToString();
            ViewBag.SubID = SubID;
            ViewBag.ThirdID = ThirdID;
            ViewBag.FourthID = fourthID;

            v_User CurrentUser = Session["CurrentUser"] as v_User;

            #region 解析当前用户角色权限

            List<v_UserRole> CurrentUser_RoleList = ViewBag.CurrentUser_RoleList as List<v_UserRole>;
            v_UserRole UserRole_CurrentDept = CurrentUser_RoleList.Where(o => o.ur_e_Dept_ID == DeptID).FirstOrDefault();

            // 若用户无当前部门的权限，则说明具有超级管理员或默认权限，将其设置为对当前部门的权限
            if (UserRole_CurrentDept == null)
            {
                UserRole_CurrentDept = CurrentUser_RoleList.Where(o => o.ur_e_Dept_ID == 0).FirstOrDefault();
                UserRole_CurrentDept.ur_e_Dept_ID = DeptID;
            }

            ViewBag.UserRole_CurrentDept = UserRole_CurrentDept;

            #endregion

            #region User List

            List<v_User> OwnerList = CacheConfig.Get<List<v_User>>(CacheConfig.CacheType.UserList);

            ViewBag.OwnerList = OwnerList;

            #endregion

            #region Report Category List

            List<v_MenuList> Report_CategoryList = CacheConfig.Get<List<v_MenuList>>(CacheConfig.CacheType.Report_CategoryList);
            List<v_MenuList> Rreport_SubCategoryList = CacheConfig.Get<List<v_MenuList>>(CacheConfig.CacheType.Report_SubCategoryList);

            // 若当前用户只有该部门的公共元素访问权限，则去掉其中的私有元素
            if (UserRole_CurrentDept.a_Type_R < 2)
            {
                Report_CategoryList = Report_CategoryList.Where(o => (o.ml_Main_DeptID == DeptID && o.ml_Main_AccessLevel == 0 && o.ml_Sub_AccessLevel == 0 && o.ml_Third_AccessLevel == 0)).ToList();
                Rreport_SubCategoryList = Report_CategoryList.Where(o => (o.ml_Main_DeptID == DeptID && o.ml_Main_AccessLevel == 0 && o.ml_Sub_AccessLevel == 0 && o.ml_Third_AccessLevel == 0 && o.ml_Sub_ID == SubID)).Distinct().ToList();
            }
            //若有访问私有元素权限，显示对应部门中的所有菜单
            else
            {
                Report_CategoryList = Report_CategoryList.Where(o => o.ml_Main_DeptID == DeptID && o.ml_Sub_ID == SubID && o.ml_Third_ActionName != "UpdateFrequency").ToList();
                Rreport_SubCategoryList = Rreport_SubCategoryList.Where(o => o.ml_Main_DeptID == DeptID).Distinct().ToList();
            }

            ViewBag.CategoryList = Report_CategoryList;
            ViewBag.SubCategoryList = Rreport_SubCategoryList;


            #endregion

            #region Update Frequency List

            List<v_MenuList> Report_FrequencyList = CacheConfig.Get<List<v_MenuList>>(CacheConfig.CacheType.Report_FrequencyList);

            // 若当前用户只有该部门的公共元素访问权限，则去掉其中的私有元素
            if (UserRole_CurrentDept.a_Type_R < 2)
                Report_FrequencyList = Report_FrequencyList.Where(o => (o.ml_Main_DeptID == DeptID && o.ml_Main_AccessLevel == 0 && o.ml_Sub_AccessLevel == 0 && o.ml_Third_AccessLevel == 0 && o.ml_Sub_ID == SubID)).ToList();
            // 若有访问私有元素权限，显示对应部门中的所有菜单
            else
                Report_FrequencyList = Report_FrequencyList.Where(o => o.ml_Main_DeptID == DeptID && o.ml_Sub_ID == SubID).ToList();

            ViewBag.UpdateFrequencyList = Report_FrequencyList;

            #endregion

            return View();
        }

        public ActionResult Reporting_Edit(int ReportID)
        {
            t_Report TargetReport = DBHelper_Content.Get_Report_ByReportID(ReportID);

            if (TargetReport != null)
            {
                ViewBag.TargetReport = TargetReport;
                v_User CurrentUser = Session["CurrentUser"] as v_User;

                #region 解析当前用户角色权限

                List<v_UserRole> CurrentUser_RoleList = ViewBag.CurrentUser_RoleList as List<v_UserRole>;
                v_UserRole UserRole_CurrentDept = CurrentUser_RoleList.Where(o => o.ur_e_Dept_ID == TargetReport.r_m_DeptID.Value).FirstOrDefault();

                // 若用户无当前部门的权限，则说明具有超级管理员或默认权限，将其设置为对当前部门的权限
                if (UserRole_CurrentDept == null)
                {
                    UserRole_CurrentDept = CurrentUser_RoleList.Where(o => o.ur_e_Dept_ID == 0).FirstOrDefault();
                    UserRole_CurrentDept.ur_e_Dept_ID = TargetReport.r_m_DeptID.Value;
                }

                ViewBag.UserRole_CurrentDept = UserRole_CurrentDept;

                #endregion

                #region User List

                List<v_User> OwnerList = CacheConfig.Get<List<v_User>>(CacheConfig.CacheType.UserList);

                ViewBag.OwnerList = OwnerList;

                List<v_UserRole> AdminList = CacheConfig.Get<List<v_UserRole>>(CacheConfig.CacheType.UserRoleList);

                ViewBag.AdminList = AdminList;

                #endregion

                #region Report Category List

                List<v_MenuList> Report_CategoryList = CacheConfig.Get<List<v_MenuList>>(CacheConfig.CacheType.Report_CategoryList);
                List<v_MenuList> Rreport_SubCategoryList = CacheConfig.Get<List<v_MenuList>>(CacheConfig.CacheType.Report_SubCategoryList);

                // 若当前用户只有该部门的公共元素访问权限，则去掉其中的私有元素
                if (UserRole_CurrentDept.a_Type_R < 2)
                {
                    Report_CategoryList = Report_CategoryList.Where(o => (o.ml_Main_DeptID == TargetReport.r_m_DeptID.Value && o.ml_Main_AccessLevel == 0 && o.ml_Sub_AccessLevel == 0 && o.ml_Third_AccessLevel == 0 && o.ml_Sub_ID == TargetReport.r_m_ID_SubCategory)).ToList();
                    Rreport_SubCategoryList = Rreport_SubCategoryList.Where(o => (o.ml_Main_DeptID == TargetReport.r_m_DeptID.Value && o.ml_Main_AccessLevel == 0 && o.ml_Sub_AccessLevel == 0 && o.ml_Third_AccessLevel == 0)).ToList();

                }
                // 若有访问私有元素权限，显示对应部门中的所有菜单
                else
                {
                    Report_CategoryList = Report_CategoryList.Where(o => o.ml_Main_DeptID == TargetReport.r_m_DeptID.Value && o.ml_Sub_ID == TargetReport.r_m_ID_SubCategory).ToList();
                    Rreport_SubCategoryList = Rreport_SubCategoryList.Where(o => o.ml_Main_DeptID == TargetReport.r_m_DeptID.Value).Distinct().ToList();

                }


                ViewBag.CategoryList = Report_CategoryList;
                ViewBag.SubCategoryList = Rreport_SubCategoryList;

                #endregion

                #region Report Frequency List

                List<v_MenuList> Report_FrequencyList = CacheConfig.Get<List<v_MenuList>>(CacheConfig.CacheType.Report_FrequencyList);

                // 若当前用户只有该部门的公共元素访问权限，则去掉其中的私有元素
                if (UserRole_CurrentDept.a_Type_R < 2)
                    Report_FrequencyList = Report_FrequencyList.Where(o => (o.ml_Main_DeptID == TargetReport.r_m_DeptID.Value && o.ml_Main_AccessLevel == 0 && o.ml_Sub_AccessLevel == 0 && o.ml_Third_AccessLevel == 0 && o.ml_Sub_ID == TargetReport.r_m_ID_SubCategory)).ToList();
                // 若有访问私有元素权限，显示对应部门中的所有菜单
                else
                    Report_FrequencyList = Report_FrequencyList.Where(o => o.ml_Main_DeptID == TargetReport.r_m_DeptID.Value && o.ml_Sub_ID == TargetReport.r_m_ID_SubCategory).ToList();

                ViewBag.UpdateFrequencyList = Report_FrequencyList;

                #endregion
            }

            return View();
        }

        public ActionResult Report_GetAccess_Multiple(int DeptID, string ReportID_List = "", string Sub_Action = "", string Third_Action = "", string Fourth_Action = "")
        {
            TempData["ReportID_List"] = ReportID_List;

            #region 若未选择子菜单，则初始化为默认选择项

            t_Entry CurEntry = DBHelper_EntryMenu.Get_Entry_ByDeptID(DeptID);

            if (CurEntry != null)
                ViewBag.DeptID = CurEntry.e_Dept_ID;

            string ControllerName = CurEntry.e_ControllerName;

            ViewBag.ControllerName = ControllerName;
            ViewBag.Main_ActionName = "Reporting";

            if (string.IsNullOrWhiteSpace(Sub_Action))
            {
                v_MenuList SubMenu = DBHelper_EntryMenu.Get_DefaultSubMenu(ControllerName, ViewBag.Main_ActionName);
                ViewBag.Sub_ActionName = SubMenu.ml_Sub_ActionName;
            }
            else
            {
                v_MenuList SubMenu = DBHelper_EntryMenu.Get_SubMenu_ByActionName(ControllerName, ViewBag.Main_ActionName, Sub_Action);
                ViewBag.Sub_ActionName = Sub_Action;
            }

            if (string.IsNullOrWhiteSpace(Third_Action))
            {
                v_MenuList ThirdMenu = PublicHelper.Get_ThirdMenu(TempData["Third_ActionName"], ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName);
                ViewBag.Third_ActionName = ThirdMenu.ml_Third_ActionName;
            }
            else
            {
                v_MenuList ThirdMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Third_Action);
                ViewBag.Third_ActionName = Third_Action;
            }

            if (string.IsNullOrWhiteSpace(Fourth_Action))
            {
                v_MenuList FourthMenu = PublicHelper.Get_FourthMenu(TempData["Fourth_ActionName"], ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, ViewBag.Third_ActionName);
                ViewBag.Fourth_ActionName = FourthMenu.ml_Fourth_ActionName;
            }
            else
            {
                v_MenuList FourthMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Third_Action, Fourth_Action);
                ViewBag.Fourth_ActionName = Fourth_Action;
            }


            #endregion

            #region 解析当前用户角色权限

            v_User CurrentUser = Session["CurrentUser"] as v_User;

            List<v_UserRole> CurrentUser_RoleList = ViewBag.CurrentUser_RoleList as List<v_UserRole>;
            v_UserRole UserRole_CurrentDept = CurrentUser_RoleList.Where(o => o.ur_e_Dept_ID == DeptID).FirstOrDefault();

            // 若用户无当前部门的权限，则说明具有超级管理员或默认权限，将其设置为对当前部门的权限
            if (UserRole_CurrentDept == null)
            {
                UserRole_CurrentDept = CurrentUser_RoleList.Where(o => o.ur_e_Dept_ID == 0).FirstOrDefault();
                UserRole_CurrentDept.ur_e_Dept_ID = DeptID;
            }

            ViewBag.UserRole_CurrentDept = UserRole_CurrentDept;

            #endregion

            #region 根据 ControllerName 从缓存或数据库加载菜单

            List<v_MenuList> MenuList = CacheConfig.Get<List<v_MenuList>>(CacheConfig.CacheType.MenuList);

            // 若当前用户只有该部门的公共元素访问权限，则去掉其中的私有元素
            if (UserRole_CurrentDept.a_Type_R < 2)
                MenuList = MenuList.Where(o => (o.ml_Main_DeptID == DeptID && o.ml_Main_AccessLevel == 0 && o.ml_Sub_AccessLevel == 0 && o.ml_Third_AccessLevel == 0)).ToList();
            // 若有访问私有元素权限，显示对应部门中的所有菜单
            else
                MenuList = MenuList.Where(o => o.ml_Main_DeptID == DeptID).ToList();

            // 生成侧边菜单
            List<v_MenuList> MenuList_Side = MenuList.Where(o => o.ml_Main_ActionName.Equals(ViewBag.Main_ActionName)).ToList();

            ViewBag.MenuList_Side = MenuList_Side;

            #endregion

            #region 加载选中的分类下的所有报告列表

            int SelectedMenuID = 0;

            if (ViewBag.MenuList_Side != null)
            {
                for (int i = 0; i < ViewBag.MenuList_Side.Count; i++)
                {
                    if (Sub_Action == "Disabled")
                    {
                        if (ViewBag.MenuList_Side[i].ml_Third_ActionName.Equals(ViewBag.Third_ActionName))
                        {
                            SelectedMenuID = ViewBag.MenuList_Side[i].ml_Third_ID;
                            break;
                        }
                    }
                    else
                    {
                        if (ViewBag.MenuList_Side[i].ml_Fourth_ActionName.Equals(ViewBag.Fourth_ActionName))
                        {
                            SelectedMenuID = ViewBag.MenuList_Side[i].ml_Fourth_ID;
                            break;
                        }
                    }
                }
            }

            if (SelectedMenuID != 0)
            {
                List<ReportModel> ReportList = DBHelper_Content.Get_ReportList_ByMenuID(SelectedMenuID, (Session["CurrentUser"] as v_User).User_GID);
                ViewBag.ReportList = ReportList;
            }

            #endregion

            return View();
        }

        public ActionResult QuickLink_Add(int DeptID, int SubID)
        {
            ViewBag.DeptID = DeptID;
            ViewBag.QuickLinkGUID = Guid.NewGuid().ToString();
            ViewBag.SubID = SubID;

            #region 解析当前用户角色权限

            v_User CurrentUser = Session["CurrentUser"] as v_User;

            List<v_UserRole> CurrentUser_RoleList = ViewBag.CurrentUser_RoleList as List<v_UserRole>;
            v_UserRole UserRole_CurrentDept = CurrentUser_RoleList.Where(o => o.ur_e_Dept_ID == DeptID).FirstOrDefault();

            // 若用户无当前部门的权限，则说明具有超级管理员或默认权限，将其设置为对当前部门的权限
            if (UserRole_CurrentDept == null)
            {
                UserRole_CurrentDept = CurrentUser_RoleList.Where(o => o.ur_e_Dept_ID == 0).FirstOrDefault();
                UserRole_CurrentDept.ur_e_Dept_ID = DeptID;
            }

            ViewBag.UserRole_CurrentDept = UserRole_CurrentDept;

            #endregion

            #region Type List

            List<v_MenuList> QuickLink_TypeList = CacheConfig.Get<List<v_MenuList>>(CacheConfig.CacheType.QuickLink_TypeList).Where(x => x.ml_Sub_ActionName != "Organization").ToList();

            // 若当前用户只有该部门的公共元素访问权限，则去掉其中的私有元素
            if (UserRole_CurrentDept.a_Type_R < 2)
                QuickLink_TypeList = QuickLink_TypeList.Where(o => (o.ml_Main_DeptID == DeptID && o.ml_Main_AccessLevel == 0 && o.ml_Sub_AccessLevel == 0 && o.ml_Third_AccessLevel == 0)).ToList();
            // 若有访问私有元素权限，显示对应部门中的所有菜单
            else
                QuickLink_TypeList = QuickLink_TypeList.Where(o => o.ml_Main_DeptID == DeptID).ToList();

            ViewBag.QuickLink_TypeList = QuickLink_TypeList;

            #endregion

            return View();
        }

        public ActionResult QuickLink_Edit(int QuickLinkID)
        {
            t_QuickLink TargetQuickLink = DBHelper_Content.Get_QuickLink_ByQuickLinkID(QuickLinkID);

            if (TargetQuickLink != null)
            {
                ViewBag.TargetQuickLink = TargetQuickLink;

                #region 解析当前用户角色权限

                v_User CurrentUser = Session["CurrentUser"] as v_User;

                List<v_UserRole> CurrentUser_RoleList = ViewBag.CurrentUser_RoleList as List<v_UserRole>;
                v_UserRole UserRole_CurrentDept = CurrentUser_RoleList.Where(o => o.ur_e_Dept_ID == TargetQuickLink.ql_m_DeptID.Value).FirstOrDefault();

                // 若用户无当前部门的权限，则说明具有超级管理员或默认权限，将其设置为对当前部门的权限
                if (UserRole_CurrentDept == null)
                {
                    UserRole_CurrentDept = CurrentUser_RoleList.Where(o => o.ur_e_Dept_ID == 0).FirstOrDefault();
                    UserRole_CurrentDept.ur_e_Dept_ID = TargetQuickLink.ql_m_DeptID.Value;
                }

                ViewBag.UserRole_CurrentDept = UserRole_CurrentDept;

                #endregion

                #region Type List

                List<v_MenuList> QuickLink_TypeList = CacheConfig.Get<List<v_MenuList>>(CacheConfig.CacheType.QuickLink_TypeList);

                // 若当前用户只有该部门的公共元素访问权限，则去掉其中的私有元素
                if (UserRole_CurrentDept.a_Type_R < 2)
                    QuickLink_TypeList = QuickLink_TypeList.Where(o => (o.ml_Main_DeptID == TargetQuickLink.ql_m_DeptID.Value && o.ml_Main_AccessLevel == 0 && o.ml_Sub_AccessLevel == 0 && o.ml_Third_AccessLevel == 0)).ToList();
                // 若有访问私有元素权限，显示对应部门中的所有菜单
                else
                    QuickLink_TypeList = QuickLink_TypeList.Where(o => o.ml_Main_DeptID == TargetQuickLink.ql_m_DeptID.Value).ToList();

                ViewBag.QuickLink_TypeList = QuickLink_TypeList;

                #endregion
            }

            return View();
        }

        public ActionResult Customization_Add(int DeptID)
        {
            ViewBag.DeptID = DeptID;
            ViewBag.QuickLinkGUID = Guid.NewGuid().ToString();

            #region Customization Category List

            List<string> CustomizationCategory_List = DBHelper_Content.Get_CustomizationCategory_ByDeptID(DeptID);
            ViewBag.CustomizationCategory_List = CustomizationCategory_List;

            #endregion

            return View();
        }

        public ActionResult Customization_Edit(int CustomizationID)
        {
            t_Customization TargetCustomization = DBHelper_Content.Get_Customization_ByCustomizationID(CustomizationID);

            if (TargetCustomization != null)
            {
                ViewBag.TargetCustomization = TargetCustomization;

                #region Customization Category List

                List<string> CustomizationCategory_List = DBHelper_Content.Get_CustomizationCategory_ByDeptID(TargetCustomization.c_m_DeptID.Value);
                ViewBag.CustomizationCategory_List = CustomizationCategory_List;

                #endregion

            }

            return View();
        }

        public ActionResult ProcessLinkage_Add(int node_ID, int pl_Type)
        {

            if (ViewBag.NodeID == null)
            {
                ViewBag.NodeID = node_ID;
            }
            if (ViewBag.LinkageType == null)
            {
                ViewBag.LinkageType = pl_Type;
            }

            return View();
        }

        public ActionResult ProcessLinkage_Edit(int node_ID, int pl_ID, int pl_Type)
        {
            t_ProcessLinkage linkage = DBHelper_Content.Get_ProcessLinkage_ByID(pl_ID);
            if (linkage == null)
            {
                ViewBag.TargetProcessLinkage = null;
            }
            ViewBag.TargetProcessLinkage = linkage;
            return View();
        }


        public ActionResult UserRole_View(int UserRoleID)
        {
            #region User Role

            t_UserRole TargetUserRole = DBHelper_User.Get_UserRole_ByID(UserRoleID);

            ViewBag.TargetUserRole = TargetUserRole;

            #endregion

            #region Entry

            List<t_Entry> EntryList = CacheConfig.Get<List<t_Entry>>(CacheConfig.CacheType.EntryList);
            t_Entry Entry = EntryList.Where(o => o.e_Dept_ID == TargetUserRole.ur_e_Dept_ID).FirstOrDefault();

            ViewBag.Entry = Entry;

            #endregion

            #region User

            List<v_User> UserList = CacheConfig.Get<List<v_User>>(CacheConfig.CacheType.UserList);
            v_User User = UserList.Where(o => o.User_GID == TargetUserRole.ur_User_GID).FirstOrDefault();

            ViewBag.User = User;

            #endregion

            #region Access

            List<t_Access> AccessList = CacheConfig.Get<List<t_Access>>(CacheConfig.CacheType.AccessList);
            t_Access Access = AccessList.Where(o => o.a_ID == TargetUserRole.ur_a_ID).FirstOrDefault();

            ViewBag.Access = Access;

            #endregion

            return View();
        }

        public ActionResult UserRole_Add(int DeptID)
        {
            #region Entry

            List<t_Entry> EntryList = CacheConfig.Get<List<t_Entry>>(CacheConfig.CacheType.EntryList);
            EntryList = EntryList.Where(o => o.e_Dept_ID == DeptID).ToList();

            ViewBag.EntryList = EntryList;

            #endregion

            #region User List

            List<v_User> UserList = CacheConfig.Get<List<v_User>>(CacheConfig.CacheType.UserList);

            ViewBag.UserList = UserList;

            #endregion

            #region Access List

            List<t_Access> AccessList_Temp = CacheConfig.Get<List<t_Access>>(CacheConfig.CacheType.AccessList);
            List<v_UserRole> CurrentUser_RoleList = ViewBag.CurrentUser_RoleList as List<v_UserRole>;

            List<t_Access> AccessList = new List<t_Access>();
            bool IsSuperAdmin = false;

            // 只有超级管理员才可以添加超级管理员权限
            if (CurrentUser_RoleList.Exists(o => o.a_Name == "SuperAdmin"))
                IsSuperAdmin = true;

            foreach (t_Access Cur in AccessList_Temp)
            {
                if (IsSuperAdmin || Cur.a_Name != "SuperAdmin")
                    AccessList.Add(Cur);
            }

            ViewBag.AccessList = AccessList;

            #endregion

            return View();
        }

        public ActionResult UserRole_Update(int UserRoleID)
        {
            #region User Role

            t_UserRole TargetUserRole = DBHelper_User.Get_UserRole_ByID(UserRoleID);

            ViewBag.TargetUserRole = TargetUserRole;

            #endregion

            #region Entry List

            List<t_Entry> EntryList = CacheConfig.Get<List<t_Entry>>(CacheConfig.CacheType.EntryList);
            t_Entry Entry = EntryList.Where(o => o.e_Dept_ID == TargetUserRole.ur_e_Dept_ID).FirstOrDefault();

            ViewBag.Entry = Entry;

            #endregion

            #region User List

            List<v_User> UserList = CacheConfig.Get<List<v_User>>(CacheConfig.CacheType.UserList);
            v_User User = UserList.Where(o => o.User_GID == TargetUserRole.ur_User_GID).FirstOrDefault();

            ViewBag.User = User;

            #endregion

            #region Access List

            List<t_Access> AccessList_Temp = CacheConfig.Get<List<t_Access>>(CacheConfig.CacheType.AccessList);
            List<v_UserRole> CurrentUser_RoleList = ViewBag.CurrentUser_RoleList as List<v_UserRole>;

            List<t_Access> AccessList = new List<t_Access>();
            bool IsSuperAdmin = false;

            // 只有超级管理员才可以添加超级管理员权限
            if (CurrentUser_RoleList.Exists(o => o.a_Name == "SuperAdmin"))
                IsSuperAdmin = true;

            foreach (t_Access Cur in AccessList_Temp)
            {
                if (IsSuperAdmin || Cur.a_Name != "SuperAdmin")
                    AccessList.Add(Cur);
            }

            ViewBag.AccessList = AccessList;

            #endregion

            return View();
        }
    }
}