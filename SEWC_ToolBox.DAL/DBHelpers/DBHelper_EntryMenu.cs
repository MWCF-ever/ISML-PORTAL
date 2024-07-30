using SEWC_NetDevLib.SEWC_NetLibExtend;
using SEWC_ToolBox.DAL.EFs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SEWC_ToolBox.DAL.DBHelpers
{
    public static class DBHelper_EntryMenu
    {
        // 通过部门ID获取入口
        public static t_Entry Get_Entry_ByDeptID(int e_Dept_ID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from e in dbContext.t_Entry
                             where e.e_Dept_ID == e_Dept_ID
                             select e;

                t_Entry Entry = result.First<t_Entry>();

                return Entry;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 通过ControllerName获取入口
        public static t_Entry Get_Entry_ByControllerName(string e_ControllerName)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from e in dbContext.t_Entry
                             where e.e_ControllerName == e_ControllerName
                             select e;

                t_Entry Entry = result.First<t_Entry>();

                return Entry;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获取所有入口
        public static List<t_Entry> Get_EntryList()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from e in dbContext.t_Entry
                             where e.e_Enabled == true
                             orderby e.e_ControllerName
                             select e;

                List<t_Entry> EntryList = result.ToList<t_Entry>();

                return EntryList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 通过ID获取菜单
        public static t_Menu_New Get_Menu_ByID(int m_ID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from m in dbContext.t_Menu_New
                             where m.m_ID == m_ID
                             select m;

                t_Menu_New Menu = result.First<t_Menu_New>();

                return Menu;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 通过Action名获取菜单
        public static t_Menu_New Get_Menu_ByActionName(string ActionName)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from m in dbContext.t_Menu_New
                             where m.m_ActionName == ActionName
                             select m;

                t_Menu_New Menu = result.First<t_Menu_New>();

                return Menu;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 通过父ID获取菜单集合
        public static List<t_Menu_New> Get_MenuList_ByParentID(int m_ParentID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from m in dbContext.t_Menu_New
                             where m.m_ParentID == m_ParentID
                             select m;

                List<t_Menu_New> MenuList = result.ToList<t_Menu_New>();

                return MenuList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 通过部门ID获取菜单集合
        public static List<t_Menu_New> Get_MenuList_ByDeptID(int m_DeptID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from m in dbContext.t_Menu_New
                             where m.m_DeptID == m_DeptID
                             select m;

                List<t_Menu_New> MenuList = result.ToList<t_Menu_New>();

                return MenuList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获取所有菜单
        public static List<v_MenuList> Get_MenuList()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ml in dbContext.v_MenuList
                             select ml;
                // select * from v_MenuList

                List<v_MenuList> MenuList = result.ToList<v_MenuList>();

                return MenuList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获取MenuCategory菜单
        public static List<v_SideMenuList> MenuCategory()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ml in dbContext.v_SideMenuList
                             select ml;
                // select * from v_MenuList

                List<v_SideMenuList> MenuList = result.ToList<v_SideMenuList>();

                return MenuList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        // 通过ControllerName获取菜单
        public static List<v_MenuList> Get_MenuList_ByControllerName(string ml_e_ControllerName)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ml in dbContext.v_MenuList
                             where ml.ml_e_ControllerName == ml_e_ControllerName
                             select ml;

                List<v_MenuList> MenuList = result.ToList<v_MenuList>();

                return MenuList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 通过ControllerName和ActionName获取菜单
        public static List<v_MenuList> Get_MenuList_ByControllerName_ActionName(string ml_e_ControllerName, string ml_Main_ActionName)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ml in dbContext.v_MenuList
                             where ml.ml_e_ControllerName == ml_e_ControllerName
                                && ml.ml_Main_ActionName == ml_Main_ActionName
                             select ml;

                List<v_MenuList> MenuList = result.ToList<v_MenuList>();

                return MenuList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获取默认选中的二级菜单
        public static v_MenuList Get_DefaultSubMenu(string ml_e_ControllerName, string ml_Main_ActionName)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ml in dbContext.v_MenuList
                             where ml.ml_e_ControllerName == ml_e_ControllerName
                                && ml.ml_Main_ActionName == ml_Main_ActionName
                             orderby ml.ml_RowNum
                             select ml;
                //string sql = "select * from v_MenuList ml where ml.ml_e_ControllerName == ml_e_ControllerName and ml.ml_Main_ActionName == ml_Main_ActionName order by ml.ml_Sub_ID";

                v_MenuList SubMenu = result.First<v_MenuList>();

                if (string.IsNullOrWhiteSpace(SubMenu.ml_Sub_ActionName))
                    SubMenu.ml_Sub_ActionName = string.Empty;

                return SubMenu;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获取默认选中的三级菜单
        public static v_MenuList Get_DefaultThirdMenu(string ml_e_ControllerName, string ml_Main_ActionName, string ml_Sub_ActionName)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ml in dbContext.v_MenuList
                             where ml.ml_e_ControllerName == ml_e_ControllerName
                                && ml.ml_Main_ActionName == ml_Main_ActionName
                                && ml.ml_Sub_ActionName == ml_Sub_ActionName
                             select ml;

                // select * from [SEWC_ToolBox].[dbo].[v_MenuList] where ml_e_ControllerName = ml_e_ControllerName and ml_Main_ActionName = ml_Main_ActionName and ml_Sub_ActionName = ml_Sub_ActionName

                v_MenuList SelectedMenu = result.First<v_MenuList>();

                if (string.IsNullOrWhiteSpace(SelectedMenu.ml_Third_ActionName))
                    SelectedMenu.ml_Third_ActionName = string.Empty;

                return SelectedMenu;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        // 获取默认选中的si级菜单
        public static v_MenuList Get_DefaultFourthMenu(string ml_e_ControllerName, string ml_Main_ActionName, string ml_Sub_ActionName, string ml_Third_ActionName)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ml in dbContext.v_MenuList
                             where ml.ml_e_ControllerName == ml_e_ControllerName
                                && ml.ml_Main_ActionName == ml_Main_ActionName
                                && ml.ml_Sub_ActionName == ml_Sub_ActionName
                                && ml.ml_Third_ActionName == ml_Third_ActionName
                             select ml;

                // select * from [SEWC_ToolBox].[dbo].[v_MenuList] where ml_e_ControllerName = ml_e_ControllerName and ml_Main_ActionName = ml_Main_ActionName and ml_Sub_ActionName = ml_Sub_ActionName

                v_MenuList SelectedMenu = result.First<v_MenuList>();

                if (string.IsNullOrWhiteSpace(SelectedMenu.ml_Third_ActionName))
                    SelectedMenu.ml_Third_ActionName = string.Empty;

                return SelectedMenu;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获取指定的二级菜单实体
        public static v_MenuList Get_SubMenu_ByActionName(string ml_e_ControllerName, string ml_Main_ActionName, string ml_Sub_ActionName)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ml in dbContext.v_MenuList
                             where ml.ml_e_ControllerName == ml_e_ControllerName
                                && ml.ml_Main_ActionName == ml_Main_ActionName
                                && ml.ml_Sub_ActionName == ml_Sub_ActionName
                             select ml;

                v_MenuList SubMenu = result.First<v_MenuList>();

                return SubMenu;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获取指定的三级菜单实体
        public static v_MenuList Get_ThirdMenu_ByActionName(string ml_e_ControllerName, string ml_Main_ActionName, string ml_Sub_ActionName, string ml_Third_ActionName)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ml in dbContext.v_MenuList
                             where ml.ml_e_ControllerName == ml_e_ControllerName
                                && ml.ml_Main_ActionName == ml_Main_ActionName
                                && ml.ml_Sub_ActionName == ml_Sub_ActionName
                                && ml.ml_Third_ActionName == ml_Third_ActionName
                             select ml;

                v_MenuList SubMenu = result.First<v_MenuList>();

                return SubMenu;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获取指定的三级菜单实体
        public static v_MenuList Get_ThirdMenu_ByActionName(string ml_e_ControllerName, string ml_Main_ActionName, string ml_Sub_ActionName, string ml_Third_ActionName, string ml_Four_ActionName)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ml in dbContext.v_MenuList
                             where ml.ml_e_ControllerName == ml_e_ControllerName
                                && ml.ml_Main_ActionName == ml_Main_ActionName
                                && ml.ml_Sub_ActionName == ml_Sub_ActionName
                                && ml.ml_Third_ActionName == ml_Third_ActionName
                                && ml.ml_Fourth_ActionName == ml_Four_ActionName
                             select ml;

                v_MenuList SubMenu = result.First<v_MenuList>();

                return SubMenu;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        // 获取所有报表类别
        public static List<v_MenuList> Get_Report_SubCategoryList()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            try
            {
                var result = from ml in dbContext.v_MenuList
                             where ml.ml_Main_ActionName == "Reporting" && ml.ml_e_ControllerName.Equals("SSML")
                             orderby ml.ml_Main_DeptID
                             select ml;

                List<v_MenuList> MenuList = result.ToList<v_MenuList>();

                return MenuList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获取所有报表类别
        public static List<t_Menu_New> Get_Report_Categorys()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            try
            {
                var result = from ml in dbContext.t_Menu_New
                             where ml.m_ParentID == 2
                             orderby ml.m_MenuName
                             select ml;

                List<t_Menu_New> MenuList = result.ToList<t_Menu_New>();

                return MenuList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        // 获取所有报表类别
        public static List<v_MenuList> Get_Report_CategoryList()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            try
            {
                var result = from ml in dbContext.v_MenuList
                             where ml.ml_Main_ActionName == "Reporting" && ml.ml_e_ControllerName.Equals("SSML")
                             //&& ml.ml_Third_ActionName != "UpdateFrequency"
                             orderby ml.ml_Main_DeptID
                             select ml;

                List<v_MenuList> MenuList = result.ToList<v_MenuList>();

                return MenuList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获取所有报表更新频率
        public static List<v_MenuList> Get_Report_FrequencyList()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ml in dbContext.v_MenuList
                             where ml.ml_Main_ActionName == "Reporting" && ml.ml_e_ControllerName.Equals("SSML")
                                && ml.ml_Third_ActionName == "UpdateFrequency"
                             orderby ml.ml_Main_DeptID
                             select ml;

                List<v_MenuList> MenuList = result.ToList<v_MenuList>();

                return MenuList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获取所有快速链接类型
        public static List<v_MenuList> Get_QuickLink_TypeList()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ml in dbContext.v_MenuList
                             where ml.ml_Main_ActionName == "QuickLink"
                             select ml;

                List<v_MenuList> MenuList = result.ToList<v_MenuList>();

                return MenuList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
