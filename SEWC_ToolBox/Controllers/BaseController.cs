using SEWC_NetDevLib.SEWC_NetLibExtend;
using SEWC_ToolBox.DAL.DBHelpers;
using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox.Languages;
using SEWC_ToolBox.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;

namespace SEWC_ToolBox_Project.Controllers
{
    /// <summary>
    /// 基Controller，用于重写Controller的各种方法，实现自定义操作
    /// </summary>
    [Authorize]
    public class BaseController : Controller
    {
        //从写默认Controller
        protected virtual string OverrideContollerName { get; }

        // Action执行之前检查是否登录及权限
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 获取请求参数的值
            base.OnActionExecuting(filterContext);

            ViewBag.PreUrl = AppConfig.PreURL;


            // 非CustomError、NotFound、ToEnglish和ToChinese的Action均进行登录和权限验证
            if (!filterContext.ActionDescriptor.ActionName.Equals("CustomError") && !filterContext.ActionDescriptor.ActionName.Equals("NotFound") && !filterContext.ActionDescriptor.ActionName.Equals("ToEnglish") && !filterContext.ActionDescriptor.ActionName.Equals("ToChinese"))
            {
                try
                {
                    ViewBag.ControllerName = OverrideContollerName ?? filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

                    #region 从 Session 判断当前用户是否已登录

                    v_User CurrentUser = new v_User();

                    if (Session["CurrentUser"] == null)
                    {
                        var gid = HttpContext.User.Identity.Name;
                        var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;
                        gid = userClaims?.FindFirst("gid")?.Value;
                        CurrentUser = DBHelper_User.Get_User_ByGID(gid);

                        if (CurrentUser != null)
                            Session["CurrentUser"] = CurrentUser;
                        else
                        {
                            CurrentUser = new v_User { User_GID = gid, User_Name_EN = "Guest", User_Name_CH = "Guest" };
                            Session["CurrentUser"] = CurrentUser;
                        }

                    }
                    // 如果已登录，从Session中获取人员实体
                    else
                        CurrentUser = Session["CurrentUser"] as v_User;

                    ViewBag.CurrentUserGID = CurrentUser.User_GID;
                    var auth = FormsAuthentication.GetAuthCookie(CurrentUser.User_GID, false).Value;


                    #endregion

                    #region 加载当前用户语言偏好信息

                    if (Session["CurrentLanguage"] == null)
                    {
                        List<t_LanguageProfile> LanguageProfileList = CacheConfig.Get<List<t_LanguageProfile>>(CacheConfig.CacheType.LanguageProfileList);

                        t_LanguageProfile CurUserLanguage = (LanguageProfileList ?? new List<t_LanguageProfile>()).Where(o => o.lp_u_GID == CurrentUser.User_GID).FirstOrDefault();

                        if (CurUserLanguage == null)
                        {
                            CurUserLanguage = new t_LanguageProfile()
                            {
                                lp_LanguageType = 1,
                                lp_u_GID = CurrentUser.User_GID
                            };

                            DBHelper_User.Add_LanguageProfile(CurUserLanguage);
                            CacheConfig.Set(CacheConfig.CacheType.LanguageProfileList);
                        }

                        Session["CurrentLanguage"] = CurUserLanguage.lp_LanguageType.Value;
                    }

                    #endregion

                    #region 根据 ControllerName 获取部门信息

                    List<t_Entry> EntryList = CacheConfig.Get<List<t_Entry>>(CacheConfig.CacheType.EntryList);

                    var controllerName = "";
                    if (!string.IsNullOrEmpty(ViewBag.ControllerName))
                    {
                        controllerName = ViewBag.ControllerName.ToString();
                    }

                    t_Entry CurEntry = (EntryList ?? new List<t_Entry>()).Where(o => controllerName.Equals(o.e_ControllerName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                    if (CurEntry != null)
                    {
                        ViewBag.DeptID = CurEntry.e_Dept_ID;
                        ViewBag.DeptShortName = CurEntry.e_Dept_AbbreviatedName;
                    }
                    else
                        ViewBag.DeptID = 0;

                    #endregion

                    // 解析当前用户角色权限列表
                    Get_CurrentUser_RoleList();
                }
                // 若发生错误，则
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        // 下载文件
        public FileStreamResult Download(string FileName, string DownloadPath)
        {
            EventlogHelper.AddLog(FileName);
            EventlogHelper.AddLog(DownloadPath);

            // 获取物理路径
            string FilePath = Server.MapPath(DownloadPath);

            EventlogHelper.AddLog(FilePath);

            // 返回文件
            return File(new FileStream(FilePath, FileMode.Open), "text/plain", FileName);
        }

        // 解析当前用户角色权限列表
        private void Get_CurrentUser_RoleList()
        {
            List<v_UserRole> User_RoleList = CacheConfig.Get<List<v_UserRole>>(CacheConfig.CacheType.UserRoleList);
            List<v_UserRole> CurrentUser_RoleList = User_RoleList.Where(o => o.User_GID == ViewBag.CurrentUserGID).ToList();
            t_Access DefaultAccess = DBHelper_User.Get_DefaultAccess();

            // 若无配置的任何角色，默认为公共元素只读
            if (CurrentUser_RoleList.Count == 0)
            {
                CurrentUser_RoleList.Add(new v_UserRole()
                {
                    RowNo = 0,
                    ur_e_Dept_ID = 0,
                    e_Dept_AbbreviatedName = string.Empty,
                    User_GID = ViewBag.CurrentUserGID,
                    User_DisplayText = string.Empty,
                    a_Name = DefaultAccess.a_Name,
                    a_Type_R = DefaultAccess.a_Type_R,
                    a_Type_C = DefaultAccess.a_Type_C,
                    a_Type_U = DefaultAccess.a_Type_U,
                    a_Type_D = DefaultAccess.a_Type_D
                });
            }
            // 若有配置权限，但没有超级管理员权限，则也添加只读权限
            else if (!CurrentUser_RoleList.Exists(o => o.a_Name == "SuperAdmin"))
            {
                CurrentUser_RoleList.Add(new v_UserRole()
                {
                    RowNo = 0,
                    ur_e_Dept_ID = 0,
                    e_Dept_AbbreviatedName = string.Empty,
                    User_GID = ViewBag.CurrentUserGID,
                    User_DisplayText = string.Empty,
                    a_Name = DefaultAccess.a_Name,
                    a_Type_R = DefaultAccess.a_Type_R,
                    a_Type_C = DefaultAccess.a_Type_C,
                    a_Type_U = DefaultAccess.a_Type_U,
                    a_Type_D = DefaultAccess.a_Type_D
                });
            }

            // 保存当前人员的权限列表
            ViewBag.CurrentUser_RoleList = CurrentUser_RoleList;

            // 判断是否具有私有元素、内容阅读权限
            if (CurrentUser_RoleList.Where(o => o.a_Type_R >= 2).Count() > 0)
                ViewBag.EntrancePrivateAccess = true;
            else
                ViewBag.EntrancePrivateAccess = false;
        }
    }
}