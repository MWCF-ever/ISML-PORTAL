using SEWC_ToolBox.DAL.DBHelpers;
using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox.DAL.SecondModels;
using SEWC_ToolBox.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SEWC_ToolBox.Controllers
{
    public class SEWCController : ToolBoxBaseController
    {
        #region ctor

        private List<t_ProcessLinkage> plg = new List<t_ProcessLinkage>();
        private List<ReportModel> rm = new List<ReportModel>();

        #endregion

        #region Method
        #region Home菜单

        public ActionResult Home(string Sub_Action, string Third_Action, string Fourth_Action)
        {
            #region 若未选择子菜单，则初始化为默认选择项
            //ViewBag.ControllerName = Entrance;
            if (string.IsNullOrWhiteSpace(Sub_Action))
            {
                v_MenuList SubMenu = DBHelper_EntryMenu.Get_DefaultSubMenu(ViewBag.ControllerName, ViewBag.Main_ActionName);
                //v_MenuList SubMenu = DBHelper_EntryMenu.Get_DefaultSubMenu(Entrance, ViewBag.Main_ActionName);
                if (SubMenu != null)
                    ViewBag.Sub_ActionName = SubMenu.ml_Sub_ActionName;
            }
            else
                ViewBag.Sub_ActionName = Sub_Action;

            if (string.IsNullOrWhiteSpace(Third_Action))
            {
                v_MenuList ThirdMenu = PublicHelper.Get_ThirdMenu(TempData["Third_ActionName"], ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName);
                //v_MenuList ThirdMenu = PublicHelper.Get_ThirdMenu(TempData["Third_ActionName"], Entrance, ViewBag.Main_ActionName, ViewBag.Sub_ActionName);
                ViewBag.Third_ActionName = ThirdMenu.ml_Third_ActionName;
            }
            else
            {
                v_MenuList ThirdMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Third_Action);
                //v_MenuList ThirdMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(Entrance, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Third_Action);
                ViewBag.Third_ActionName = Third_Action;
            }

            if (string.IsNullOrWhiteSpace(Fourth_Action))
            {
                v_MenuList FourthdMenu = PublicHelper.Get_FourthMenu(TempData["Fourth_ActionName"], ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, ViewBag.Fourth_ActionName);
                //v_MenuList FourthdMenu = PublicHelper.Get_FourthMenu(TempData["Fourth_ActionName"], Entrance, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, ViewBag.Fourth_ActionName);
                ViewBag.Fourth_ActionName = FourthdMenu.ml_Fourth_ActionName;
            }
            else
            {
                v_MenuList ThirdMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Fourth_Action);
                //v_MenuList ThirdMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(Entrance, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Fourth_Action);
                ViewBag.Fourth_ActionName = Fourth_Action;
            }



            #endregion

            #region 根据不同子菜单加载不同内容

            // 如果是SCM ToolBox
            //if (ViewBag.Sub_ActionName.Equals("SCMToolBox"))
            //{

            //}
            var s = ViewBag.Sub_ActionName;
            if (ViewBag.Sub_ActionName.Equals("ToolBoxNews"))
            {
                List<t_OprationLog> oprationLogs = DBHelper_Content.Get_LogList();
                ViewBag.OprationLogs = oprationLogs;
            }
            // 如果是My ToolBox
            else if (ViewBag.Sub_ActionName.Equals("MyToolBox"))
            {
                if (ViewBag.Third_ActionName.Equals("Reporting"))
                {
                    List<ReportModel> ReportList = DBHelper_Content.Get_FavoriteReportList_ByDeptID(ViewBag.DeptID, ViewBag.CurrentUserGID);
                    ReportList = ReportList.OrderBy(v => v.Menu_CategoryActionName).ToList();
                    ViewBag.ReportList = ReportList;

                    // 默认以报表类型作为分类维度
                    //if (string.IsNullOrEmpty(Third_Action))
                    //    ViewBag.DisplayMode = "PerformanceMeasurement";
                    //else
                    ViewBag.DisplayMode = Third_Action;
                }
                else if (ViewBag.Third_ActionName.Equals("QuickLink"))
                {
                    List<QuickLinkModel> QuickLinkList = DBHelper_Content.Get_FavoriteQuickLinkList_ByDeptID(ViewBag.DeptID, ViewBag.CurrentUserGID);
                    ViewBag.QuickLinkList = QuickLinkList;
                }
                else if (ViewBag.Third_ActionName.Equals("Files"))
                {
                    List<ProcessLinkageModel> processLinkList = DBHelper_Content.Get_FavoriteLinkage_ByDeptID(ViewBag.DeptID, ViewBag.CurrentUserGID);
                    ViewBag.ProcessLinkList = processLinkList;
                }
                else if (ViewBag.Third_ActionName.Equals("Customization"))
                {
                    List<t_Customization> CustomizationList = DBHelper_Content.Get_CustomizationList_ByDeptID(ViewBag.DeptID, ViewBag.CurrentUserGID);
                    ViewBag.CustomizationList = CustomizationList;
                }
            }
            else if (ViewBag.Sub_ActionName.Equals("IssueFeedback"))
            {
                return RedirectToAction("Index", "Issue");
            }
            #endregion

            return View();
        }

        public ActionResult FavorateFiles()
        {
            int pageSize = 10;
            int pageIndex = 1;
            int total = 0;

            if (!string.IsNullOrEmpty(Request.QueryString["page"]))
            {
                pageIndex = Convert.ToInt32(Request.QueryString["page"]);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["limit"]))
            {
                pageSize = Convert.ToInt32(Request.QueryString["limit"]);
            }

            List<ProcessLinkageModel> rmList = DBHelper_Content.GetPageFileFavoritesLists(pageSize, pageIndex, out total, true, ViewBag.CurrentUserGID);

            var s = Json(new
            {
                code = 0,
                msg = "",
                count = total,
                data = rmList.Select(x => new
                {
                    pl_ID = x.pl_ID,
                    pl_Name = x.pl_Name,
                    pl_Type = x.pl_Type,
                    pl_pn_ID = x.pl_pn_ID,
                    pl_Linkage = x.pl_Linkage,
                    pl_Sort = x.pl_Sort,
                    x.Favorite
                })
            }, JsonRequestBehavior.AllowGet); ;
            return s;
        }


        #endregion

        public ActionResult MyToolBox(string Sub_Action, string Third_Action, string Fourth_Action)
        {
            #region 若未选择子菜单，则初始化为默认选择项
            //ViewBag.ControllerName = Entrance;
            if (string.IsNullOrWhiteSpace(Sub_Action))
            {
                v_MenuList SubMenu = DBHelper_EntryMenu.Get_DefaultSubMenu(ViewBag.ControllerName, ViewBag.Main_ActionName);
                //v_MenuList SubMenu = DBHelper_EntryMenu.Get_DefaultSubMenu(Entrance, ViewBag.Main_ActionName);
                if (SubMenu != null)
                    ViewBag.Sub_ActionName = SubMenu.ml_Sub_ActionName;
            }
            else
                ViewBag.Sub_ActionName = Sub_Action;

            if (string.IsNullOrWhiteSpace(Third_Action))
            {
                v_MenuList ThirdMenu = PublicHelper.Get_ThirdMenu(TempData["Third_ActionName"], ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName);
                //v_MenuList ThirdMenu = PublicHelper.Get_ThirdMenu(TempData["Third_ActionName"], Entrance, ViewBag.Main_ActionName, ViewBag.Sub_ActionName);
                ViewBag.Third_ActionName = ThirdMenu.ml_Third_ActionName;
            }
            else
            {
                v_MenuList ThirdMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Third_Action);
                //v_MenuList ThirdMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(Entrance, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Third_Action);
                ViewBag.Third_ActionName = Third_Action;
            }

            if (string.IsNullOrWhiteSpace(Fourth_Action))
            {
                v_MenuList FourthdMenu = PublicHelper.Get_FourthMenu(TempData["Fourth_ActionName"], ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, ViewBag.Fourth_ActionName);
                //v_MenuList FourthdMenu = PublicHelper.Get_FourthMenu(TempData["Fourth_ActionName"], Entrance, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, ViewBag.Fourth_ActionName);
                ViewBag.Fourth_ActionName = FourthdMenu.ml_Fourth_ActionName;
            }
            else
            {
                v_MenuList ThirdMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Fourth_Action);
                //v_MenuList ThirdMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(Entrance, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Fourth_Action);
                ViewBag.Fourth_ActionName = Fourth_Action;
            }



            #endregion

            #region 根据不同子菜单加载不同内容

            // 如果是SCM ToolBox
            //if (ViewBag.Sub_ActionName.Equals("SCMToolBox"))
            //{

            //}
            var s = ViewBag.Sub_ActionName;
            if (ViewBag.Sub_ActionName.Equals("SEWCToolBox"))
            {

            }
            // 如果是My ToolBox
            else if (ViewBag.Main_ActionName.Equals("MyToolBox"))
            {
                if (ViewBag.Sub_ActionName.Equals("Reporting"))
                {
                    List<ReportModel> ReportList = DBHelper_Content.Get_FavoriteReportList_ByDeptID(ViewBag.DeptID, ViewBag.CurrentUserGID);
                    ViewBag.ReportList = ReportList;

                    // 默认以报表类型作为分类维度
                    //if (string.IsNullOrEmpty(Third_Action))
                    //    ViewBag.DisplayMode = "PerformanceMeasurement";
                    //else
                    ViewBag.DisplayMode = Third_Action;
                }
                else if (ViewBag.Sub_ActionName.Equals("QuickLink"))
                {
                    List<QuickLinkModel> QuickLinkList = DBHelper_Content.Get_FavoriteQuickLinkList_ByDeptID(ViewBag.DeptID, ViewBag.CurrentUserGID);
                    ViewBag.QuickLinkList = QuickLinkList;
                }
                else if (ViewBag.Sub_ActionName.Equals("Files"))
                {
                    List<ProcessLinkageModel> processLinkList = DBHelper_Content.Get_FavoriteLinkage_ByDeptID(ViewBag.DeptID, ViewBag.CurrentUserGID);
                    ViewBag.ProcessLinkList = processLinkList;
                }
                else if (ViewBag.Sub_ActionName.Equals("Customization"))
                {
                    List<t_Customization> CustomizationList = DBHelper_Content.Get_CustomizationList_ByDeptID(ViewBag.DeptID, ViewBag.CurrentUserGID);
                    ViewBag.CustomizationList = CustomizationList;
                }
            }

            #endregion

            return View();
        }


        #region Reporting菜单

        //public ActionResult Reporting(string Sub_Action, string Third_Action)
        //{
        //    #region 若未选择子菜单，则初始化为默认选择项

        //    if (string.IsNullOrWhiteSpace(Sub_Action))
        //    {
        //        v_MenuList SubMenu = DBHelper_EntryMenu.Get_DefaultSubMenu(ViewBag.ControllerName, ViewBag.Main_ActionName);
        //        ViewBag.SubID = SubMenu.ml_Sub_ID;
        //        ViewBag.Sub_ActionName = SubMenu.ml_Sub_ActionName;
        //    }
        //    else
        //    {
        //        v_MenuList SubMenu = DBHelper_EntryMenu.Get_SubMenu_ByActionName(ViewBag.ControllerName, ViewBag.Main_ActionName, Sub_Action);
        //        ViewBag.SubID = SubMenu.ml_Sub_ID;
        //        ViewBag.Sub_ActionName = Sub_Action;
        //    }

        //    if (string.IsNullOrWhiteSpace(Third_Action))
        //    {
        //        v_MenuList ThirdMenu = PublicHelper.Get_ThirdMenu(TempData["Third_ActionName"], ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName);
        //        ViewBag.ThirdID = ThirdMenu.ml_Third_ID;
        //        ViewBag.Third_ActionName = ThirdMenu.ml_Third_ActionName;
        //    }
        //    else
        //    {
        //        v_MenuList ThirdMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Third_Action);
        //        ViewBag.ThirdID = ThirdMenu.ml_Third_ID;
        //        ViewBag.Third_ActionName = Third_Action;
        //    }

        //    #endregion

        //    #region 加载选中的分类下的所有报告列表

        //    int SelectedMenuID = 0;

        //    if (ViewBag.MenuList_Side != null)
        //    {
        //        for (int i = 0; i < ViewBag.MenuList_Side.Count; i++)
        //        {
        //            if (ViewBag.MenuList_Side[i].ml_Third_ActionName.Equals(ViewBag.Third_ActionName))
        //            {
        //                SelectedMenuID = ViewBag.MenuList_Side[i].ml_Third_ID;
        //                break;
        //            }
        //        }
        //    }

        //    if (SelectedMenuID != 0)
        //    {
        //        List<ReportModel> ReportList = DBHelper_Content.Get_ReportList_ByMenuID(SelectedMenuID, ViewBag.CurrentUserGID);
        //        ViewBag.ReportList = ReportList;
        //    }

        //    #endregion

        //    return View();
        //}

        public ActionResult Reporting(string Sub_Action, string Third_Action, string Fourth_Action)
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

            if (string.IsNullOrWhiteSpace(Third_Action))
            {
                v_MenuList ThirdMenu = PublicHelper.Get_ThirdMenu(TempData["Third_ActionName"], ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName);
                ViewBag.ThirdID = ThirdMenu.ml_Third_ID;
                ViewBag.Third_ActionName = ThirdMenu.ml_Third_ActionName;
            }
            else
            {
                v_MenuList ThirdMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Third_Action);
                ViewBag.ThirdID = ThirdMenu.ml_Third_ID;
                ViewBag.Third_ActionName = Third_Action;
            }


            if (string.IsNullOrWhiteSpace(Fourth_Action))
            {
                v_MenuList FourthMenu = PublicHelper.Get_ThirdMenu(TempData["Fourth_ActionName"], ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName);
                ViewBag.FourthID = FourthMenu.ml_Fourth_ID;
                ViewBag.Fourth_ActionName = FourthMenu.ml_Fourth_ActionName;
            }
            else
            {
                v_MenuList fourthMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Third_Action, Fourth_Action);
                ViewBag.FourthID = fourthMenu.ml_Fourth_ID;
                ViewBag.Fourth_ActionName = Fourth_Action;
            }

            List<v_MenuList> sideMenulist = ViewBag.MenuList_Side;

            ViewBag.MenuList_Side = sideMenulist.Where(sm => sm.ml_Sub_ActionName.Equals(ViewBag.Sub_ActionName)).ToList();

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
                            if (ViewBag.MenuList_Side[i].ml_Third_ActionName.Equals(ViewBag.Third_ActionName))
                            {
                                SelectedMenuID = ViewBag.MenuList_Side[i].ml_Third_ID;
                                break;
                            }
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
                List<ReportModel> ReportList = DBHelper_Content.Get_ReportList_ByMenuID(SelectedMenuID, ViewBag.CurrentUserGID);
                ViewBag.ReportList = ReportList;
            }

            if (Sub_Action == "Disabled")
            {
                List<ReportModel> ReportList = DBHelper_Content.Get_ReportList_ByDisabled(0);
                ViewBag.ReportList = ReportList;
            }
            #endregion

            return View();
        }

        #endregion

        #region Process菜单

        public ActionResult BusinessOverView(string Sub_Action, string Third_Action)
        {
            #region 若未选择子菜单，则初始化为默认选择项

            if (string.IsNullOrWhiteSpace(Sub_Action))
            {
                v_MenuList SubMenu = DBHelper_EntryMenu.Get_DefaultSubMenu(ViewBag.ControllerName, ViewBag.Main_ActionName);
                ViewBag.SubID = SubMenu.ml_Sub_ID;
                ViewBag.Sub_ActionName = SubMenu.ml_Sub_ActionName;
                ViewBag.OverView = ViewBag.Sub_ActionName;
            }
            else
            {
                if (Sub_Action == "E2E")
                {
                    ViewBag.OverView = Sub_Action;
                }
                else
                {
                    v_MenuList SubMenu = DBHelper_EntryMenu.Get_SubMenu_ByActionName(ViewBag.ControllerName, ViewBag.Main_ActionName, Sub_Action);
                    ViewBag.SubID = SubMenu.ml_Sub_ID;
                    ViewBag.Sub_ActionName = Sub_Action;
                }
            }

            if (string.IsNullOrWhiteSpace(Third_Action))
            {
                v_MenuList ThirdMenu = PublicHelper.Get_ThirdMenu(TempData["Third_ActionName"], ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName);
                ViewBag.ThirdID = ThirdMenu.ml_Third_ID;
                ViewBag.Third_ActionName = ThirdMenu.ml_Third_ActionName;
            }
            else
            {
                v_MenuList ThirdMenu = DBHelper_EntryMenu.Get_ThirdMenu_ByActionName(ViewBag.ControllerName, ViewBag.Main_ActionName, ViewBag.Sub_ActionName, Third_Action);
                ViewBag.ThirdID = ThirdMenu.ml_Third_ID;
                ViewBag.Third_ActionName = Third_Action;
            }

            #endregion

            return View();
        }

        #endregion

        #region Quick Link菜单

        public ActionResult QuickLink(string Sub_Action)
        {
            if (Sub_Action == "Organization")
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
            else
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

                #region 加载选中的分类下的快速链接

                int SelectedMenuID = 0;

                if (ViewBag.MenuList_Side != null)
                {
                    for (int i = 0; i < ViewBag.MenuList_Side.Count; i++)
                    {
                        if (ViewBag.MenuList_Side[i].ml_Sub_ActionName.Equals(ViewBag.Sub_ActionName))
                        {
                            SelectedMenuID = ViewBag.MenuList_Side[i].ml_Sub_ID;
                            break;
                        }
                    }
                }

                if (SelectedMenuID != 0)
                {
                    List<QuickLinkModel> QuickLinkList = DBHelper_Content.Get_QuickLinkList_ByMenuID(SelectedMenuID, ViewBag.CurrentUserGID);
                    ViewBag.QuickLinkList = QuickLinkList;
                }

                #endregion

                return View();
            }
        }

        #endregion

        #region 编辑器页面
        public ActionResult layeditor()
        {
            return View();
        }
        #endregion

        #region Search菜单

        public ActionResult Search(string searchtxt)
        {
            SearchModel sm = DBHelper_Content.GetSearchList(searchtxt);
            ViewBag.SearchReportList = sm.ReprotList;
            ViewBag.SearchDocumentList = sm.DocumentList;
            return View();
        }

        public ActionResult Editor()
        {
            return View();
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSearchDocumentList()
        {
            string keywords = " ";
            int pageSize = 10;
            int pageIndex = 1;
            int total = 0;

            if (!string.IsNullOrEmpty(Request.QueryString["page"]))
            {
                pageIndex = Convert.ToInt32(Request.QueryString["page"]);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["limit"]))
            {
                pageSize = Convert.ToInt32(Request.QueryString["limit"]);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["keywords"]))
            {
                keywords = Request.QueryString["keywords"];
            }

            List<ProcessLinkageModel> pl = DBHelper_Content.GetPageSearchDocumentList(keywords, pageSize, pageIndex, out total, true, ViewBag.CurrentUserGID);

            var s = Json(new
            {
                code = 0,
                msg = "",
                count = total,
                data = pl.Select(x => new
                {
                    x.pl_ID,
                    x.pl_Name,
                    x.pl_Linkage,
                    x.Location,
                    x.Favorite
                }).ToList()
            }, JsonRequestBehavior.AllowGet);

            return s;
        }

        /// <summary>
        /// 获取报表列表
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchReportList()
        {
            string keywords = " ";
            int pageSize = 10;
            int pageIndex = 1;
            int total = 0;

            if (!string.IsNullOrEmpty(Request.QueryString["page"]))
            {
                pageIndex = Convert.ToInt32(Request.QueryString["page"]);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["limit"]))
            {
                pageSize = Convert.ToInt32(Request.QueryString["limit"]);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["keywords"]))
            {
                keywords = Request.QueryString["keywords"];
            }

            List<ReportModel> rmList = DBHelper_Content.GetPageSearchReportLists(keywords, pageSize, pageIndex, out total, true, ViewBag.CurrentUserGID);


            var s = Json(new
            {
                code = 0,
                msg = "",
                count = total,
                data = rmList.Select(x => new
                {
                    x.r_ID,
                    x.r_Name,
                    x.r_Admin,
                    x.Location,
                    x.r_Linkage,
                    x.Favorite_IsAdded
                })
            }, JsonRequestBehavior.AllowGet); ;
            return s;
        }


        /// <summary>
        /// 获取QuickLink列表
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchQuickLinkList()
        {
            string keywords = " ";
            int pageSize = 10;
            int pageIndex = 1;
            int total = 0;

            if (!string.IsNullOrEmpty(Request.QueryString["page"]))
            {
                pageIndex = Convert.ToInt32(Request.QueryString["page"]);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["limit"]))
            {
                pageSize = Convert.ToInt32(Request.QueryString["limit"]);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["keywords"]))
            {
                keywords = Request.QueryString["keywords"];
            }

            List<QuickLinkModel> qlkList = DBHelper_Content.GetPageSearchQuickLinkList(keywords, pageSize, pageIndex, out total, true, ViewBag.CurrentUserGID);


            var s = Json(new
            {
                code = 0,
                msg = "",
                count = total,
                data = qlkList.Select(x => new
                {
                    x.ql_ID,
                    x.ql_Name,
                    x.ql_Description,
                    x.Location,
                    x.ql_Linkage,
                    x.Favorite_IsAdded
                })
            }, JsonRequestBehavior.AllowGet);
            return s;
        }
        #endregion

        #region Help

        public ActionResult Help()
        {
            return View("~/Views/ToolBoxBase/Help.cshtml");
        }

        #endregion
        #endregion
    }
}