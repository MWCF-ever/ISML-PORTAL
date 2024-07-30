using SEWC_NetDevLib.SEWC_NetLibExtend;
using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox.DAL.SecondModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using SEWC_ToolBox.DAL.Extions;
using SEWC_ToolBox.DAL.EFs.EnumCollect;

namespace SEWC_ToolBox.DAL.DBHelpers
{
    public static class DBHelper_Content
    {
        #region Favorite

        // 添加或移除Favorite
        public static bool Update_Favorite(int f_TypeID, int f_ObjectID, bool IsAdd, string User_GID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                // 通过EF调用存储过程，存储过程中结合最高隔离级别与更新锁，来处理并发数据统计
                dbContext.SP_Update_Favorite(f_TypeID, f_ObjectID, IsAdd, User_GID);

                return true;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public static List<ProcessLinkageModel> GetPageFileFavoritesLists(int pageSize, int pageIndex, out int total, bool isAsc, string User_GID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            List<ProcessLinkageModel> rptModel = new List<ProcessLinkageModel>();

            var list = from pl in dbContext.t_ProcessLinkage
                       join f in dbContext.t_Favorite
                         on
                           new { typeID = 2, objectID = pl.pl_ID }
                         equals
                           new { typeID = f.f_TypeID, objectID = f.f_ObjectID }
                       into t
                       from p_f in t
                       where p_f.f_GID == User_GID
                       select new ProcessLinkageModel
                       {
                           pl_ID = pl.pl_ID,
                           pl_Name = pl.pl_Name,
                           pl_Type = pl.pl_Type,
                           pl_pn_ID = pl.pl_pn_ID,
                           pl_Linkage = pl.pl_Linkage,
                           pl_Sort = pl.pl_Sort,
                           Favorite = p_f.f_GID
                       };

            total = list.Count();


            if (isAsc)
            {
                rptModel = list.OrderBy(x => x.pl_Name).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

                //dbContext.Set<v_Report>().Where(x => x.r_Name.Contains(keywords)).OrderBy(x => x.r_ID).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            }
            else
            {
                rptModel = list.OrderByDescending(x => x.pl_Name).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                //dbContext.Set<v_Report>().Where(x => x.r_Name.Contains(keywords)).OrderByDescending(x => x.r_ID).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            }

            return rptModel;
        }


        #endregion

        #region Report 相关

        // 根据MenuID和GID获取满足条件的报告列表
        public static List<ReportModel> Get_ReportList_ByMenuID(int m_ID, string User_GID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from r in dbContext.v_Report
                             join f in dbContext.t_Favorite
                               on
                                 new { typeID = 0, objectID = r.r_ID, GID = User_GID }
                               equals
                                 new { typeID = f.f_TypeID, objectID = f.f_ObjectID, GID = f.f_GID }
                             into t
                             from r_f in t.DefaultIfEmpty()
                             where r.r_m_ID_Category == m_ID || r.r_m_ID_Frequency == m_ID
                             select new ReportModel
                             {
                                 r_ID = r.r_ID,
                                 r_m_DeptID = r.r_m_DeptID,
                                 r_m_ID_Category = r.r_m_ID_Category,
                                 Menu_CategoryName = r.Menu_CategoryName,
                                 Menu_CategoryActionName = r.Menu_CategoryActionName,
                                 r_m_ID_Frequency = r.r_m_ID_Frequency,
                                 Menu_FrequencyName = r.Menu_FrequencyName,
                                 Menu_FrequencyActionName = r.Menu_FrequencyActionName,
                                 r_Name = r.r_Name,
                                 r_Description = r.r_Description,
                                 r_Owner = r.r_Owner,
                                 User_Owner = r.User_Owner,
                                 User_OwnerEmail = r.User_OwnerEmail,
                                 User_CrOwner = r.User_CrOwner,
                                 User_CrEmail = r.User_CrOwnerEmail,
                                 r_AccessOwner = r.r_AccessOwner,
                                 User_AccessOwner = r.User_AccessOwner,
                                 User_AccessOwnerEmail = r.User_AccessOwnerEmail,
                                 r_Admin = r.r_Admin,
                                 r_AdminEmail = r.r_AdminEmail,
                                 r_PicPath = r.r_PicPath,
                                 r_Linkage = r.r_Linkage,
                                 r_Status = r.r_Status,
                                 r_AccessLevel = r.r_AccessLevel,
                                 r_Sort = r.r_Sort,
                                 r_cs_ClickTimes = r.r_cs_ClickTimes,
                                 r_lr_LastReloadTime = r.r_lr_LastReloadTime,
                                 Menu_e_ID = r.Menu_e_ID,
                                 Favorite_IsAdded = r_f.f_GID,
                                 CreateUser = r.CreateUser
                             };

                List<ReportModel> ReportList = result.ToList<ReportModel>();
                return ReportList;
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

        // 根据MenuID和GID获取满足条件的报告列表
        public static List<ReportModel> Get_ReportList_ByDisabled(int status)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from r in dbContext.v_Report
                             join f in dbContext.t_Favorite
                               on
                                 new { typeID = 0, objectID = r.r_ID }
                               equals
                                 new { typeID = f.f_TypeID, objectID = f.f_ObjectID }
                             into t
                             from r_f in t.DefaultIfEmpty()
                             where r.r_Status == status || r.r_Status == status
                             select new ReportModel
                             {
                                 r_ID = r.r_ID,
                                 r_m_DeptID = r.r_m_DeptID,
                                 r_m_ID_Category = r.r_m_ID_Category,
                                 Menu_CategoryName = r.Menu_CategoryName,
                                 Menu_CategoryActionName = r.Menu_CategoryActionName,
                                 r_m_ID_Frequency = r.r_m_ID_Frequency,
                                 Menu_FrequencyName = r.Menu_FrequencyName,
                                 Menu_FrequencyActionName = r.Menu_FrequencyActionName,
                                 r_Name = r.r_Name,
                                 r_Description = r.r_Description,
                                 r_Owner = r.r_Owner,
                                 User_Owner = r.User_Owner,
                                 User_OwnerEmail = r.User_OwnerEmail,
                                 User_CrOwner = r.User_CrOwner,
                                 User_CrEmail = r.User_CrOwnerEmail,
                                 r_AccessOwner = r.r_AccessOwner,
                                 User_AccessOwner = r.User_AccessOwner,
                                 User_AccessOwnerEmail = r.User_AccessOwnerEmail,
                                 r_Admin = r.r_Admin,
                                 r_AdminEmail = r.r_AdminEmail,
                                 r_PicPath = r.r_PicPath,
                                 r_Linkage = r.r_Linkage,
                                 r_Status = r.r_Status,
                                 r_AccessLevel = r.r_AccessLevel,
                                 r_Sort = r.r_Sort,
                                 r_cs_ClickTimes = r.r_cs_ClickTimes,
                                 r_lr_LastReloadTime = r.r_lr_LastReloadTime,
                                 Menu_e_ID = r.Menu_e_ID,
                                 Favorite_IsAdded = r_f.f_GID
                             };
                string a = result.ToString();

                List<ReportModel> ReportList = result.ToList<ReportModel>();
                return ReportList;
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


        // 根据DeptID和GID获取收藏报告列表
        public static List<ReportModel> Get_FavoriteReportList_ByDeptID(int DeptID, string User_GID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from r in dbContext.v_Report
                             join f in dbContext.t_Favorite
                               on
                                 new { typeID = 0, objectID = r.r_ID, GID = User_GID }
                               equals
                                 new { typeID = f.f_TypeID, objectID = f.f_ObjectID, GID = f.f_GID }
                             into t
                             from r_f in t.DefaultIfEmpty()
                             where r.r_m_DeptID == DeptID
                                && r_f.f_GID != null
                             select new ReportModel
                             {
                                 r_ID = r.r_ID,
                                 r_m_DeptID = r.r_m_DeptID,
                                 r_m_ID_Category = r.r_m_ID_Category,
                                 Menu_CategoryName = r.Menu_CategoryName,
                                 Menu_CategoryActionName = r.Menu_CategoryActionName,
                                 r_m_ID_Frequency = r.r_m_ID_Frequency,
                                 Menu_FrequencyName = r.Menu_FrequencyName,
                                 Menu_FrequencyActionName = r.Menu_FrequencyActionName,
                                 r_Name = r.r_Name,
                                 r_Description = r.r_Description,
                                 r_Owner = r.r_Owner,
                                 User_Owner = r.User_Owner,
                                 User_OwnerEmail = r.User_OwnerEmail,
                                 r_AccessOwner = r.r_AccessOwner,
                                 User_AccessOwner = r.User_AccessOwner,
                                 User_AccessOwnerEmail = r.User_AccessOwnerEmail,
                                 r_PicPath = r.r_PicPath,
                                 r_Linkage = r.r_Linkage,
                                 r_Status = r.r_Status,
                                 r_AccessLevel = r.r_AccessLevel,
                                 r_Sort = r.r_Sort,
                                 r_cs_ClickTimes = r.r_cs_ClickTimes,
                                 r_lr_LastReloadTime = r.r_lr_LastReloadTime,
                                 r_Admin = r.r_Admin,
                                 r_AdminEmail = r.r_AdminEmail,
                                 Menu_e_ID = r.Menu_e_ID,
                                 CreateUser = r.CreateUser
                             };

                List<ReportModel> ReportList = result.ToList<ReportModel>();
                return ReportList;
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

        public static List<t_ProcessConnection> Get_process_Connections()
        {
            List<t_ProcessConnection> list = new List<t_ProcessConnection>();
            SEWC_ToolBoxEntities entities = new SEWC_ToolBoxEntities();
            try
            {
                list = entities.t_ProcessConnection.OrderBy(x => x.pc_Node_ID).ToList();
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                list = null;
            }
            finally
            {
                entities.Dispose();
            }
            return list;
        }

        // 根据ReportID来进行报告点击次数统计
        public static int Update_ClickStatistics(int r_ID, string User_GID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            SqlParameter[] sqlparams = new SqlParameter[2];
            sqlparams[0] = new SqlParameter("@r_ID", r_ID);
            sqlparams[1] = new SqlParameter("@User_GID", User_GID);

            try
            {
                // 通过EF调用存储过程，存储过程中结合最高隔离级别与更新锁，来处理并发数据统计
                //return dbContext.SP_Update_ClickStatistics(r_ID, User_GID).Cast<int>().First();

                return dbContext.SP_Update_ClickStatistics(r_ID, User_GID);

            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return -1;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 添加新报表
        public static bool Add_Report(t_Report NewReport)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                dbContext.t_Report.Add(NewReport);
                dbContext.SaveChanges();


                return true;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 编辑报表
        public static bool Update_Report(t_Report TargetReport)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                //将实体附加到对象管理器中
                dbContext.t_Report.Attach(TargetReport);
                //把当前实体的状态改为Modified
                dbContext.Entry(TargetReport).State = EntityState.Modified;
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 根据报表ID获取指定报表
        public static t_Report Get_Report_ByReportID(int r_ID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from r in dbContext.t_Report
                             where r.r_ID == r_ID
                             select r;

                t_Report TargetReport = result.First<t_Report>();

                return TargetReport;
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

        /// <summary>
        /// 根据报表名称和报表链接获取指定报表
        /// </summary>
        /// <param name="r_name">报表名称</param>
        /// <param name="r_link">报表链接</param>
        /// <returns></returns>
        public static t_Report Get_Report_ByReportNameAndLinkage(string r_name, string r_link)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from r in dbContext.t_Report
                             where r.r_Name == r_name && r.r_Linkage == r_link
                             select r;

                t_Report TargetReport = result.First<t_Report>();

                return TargetReport;
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


        // 根据报表ID列表获取报表列表
        public static List<t_Report> Get_ReportList_ByReportIDList(List<int> r_ID_List)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from r in dbContext.t_Report
                             where r_ID_List.Contains(r.r_ID)
                             orderby r.r_AccessOwner
                             select r;

                List<t_Report> ReportList = result.ToList<t_Report>();

                return ReportList;
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

        // 查找报表管理员
        public static string Get_ReportAccessManager()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from r in dbContext.t_ReportRole
                             where r.Role.Contains("AccessManager")
                             select r;

                List<t_ReportRole> ReportList = result.ToList<t_ReportRole>();

                string accessManagerList = "";

                for (int i = 0; i < ReportList.Count; i++)
                {
                    if (i == ReportList.Count - 1)
                        accessManagerList += ReportList[i].Email;
                    else
                        accessManagerList += ReportList[i].Email + ',';
                }

                return accessManagerList;
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

        /// <summary>
        ///  Get It Suporter Email
        /// </summary>
        /// <returns></returns>
        public static string Get_ITSuporter()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from r in dbContext.t_ReportRole
                             where r.Enabled == true
                             select r;

                IList<t_ReportRole> reportRoleModel = result.ToList();
                string suporterEmail = "";
                if (reportRoleModel != null)
                {
                    foreach (var rm in reportRoleModel)
                    {
                        suporterEmail += rm.Email + ",";
                    }
                    return suporterEmail.TrimEnd(',');
                }
                else
                {
                    return suporterEmail;
                }
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return "";
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        // 新增报表问题
        public static OperationMessage Add_ReportIssue(t_ReportIssue reportIssue)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                dbContext.t_ReportIssue.Add(reportIssue);
                dbContext.SaveChanges();
                ReturnMsg.OpResult = true;

                return ReturnMsg;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;

                return ReturnMsg;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public static void RequestSenseAuthor(string gid, string iname, string fname)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            try
            {
                dbContext.SP_RequestSenseAuthor(gid, iname, iname);
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region QuickLink 相关

        // 根据MenuID和GID获取满足条件的快速链接
        public static List<QuickLinkModel> Get_QuickLinkList_ByMenuID(int m_ID, string User_GID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ql in dbContext.t_QuickLink
                             join m in dbContext.t_Menu_New
                               on
                                 ql.ql_m_ID equals m.m_ID
                             join f in dbContext.t_Favorite
                               on
                                 new { typeID = 1, objectID = ql.ql_ID, GID = User_GID }
                               equals
                                 new { typeID = f.f_TypeID, objectID = f.f_ObjectID, GID = f.f_GID }
                             into t
                             from ql_f in t.DefaultIfEmpty()
                             orderby ql.ql_Sort, ql.ql_Name
                             where ql.ql_m_ID == m_ID
                             select new QuickLinkModel
                             {
                                 ql_ID = ql.ql_ID,
                                 ql_GUID = ql.ql_GUID,
                                 ql_m_DeptID = ql.ql_m_DeptID,
                                 ql_m_ID = ql.ql_m_ID,
                                 ql_m_ActionName = m.m_ActionName,
                                 ql_Name = ql.ql_Name,
                                 ql_Description = ql.ql_Description,
                                 ql_PicPath = ql.ql_PicPath,
                                 ql_Linkage = ql.ql_Linkage,
                                 ql_Status = ql.ql_Status,
                                 ql_AccessLevel = ql.ql_AccessLevel,
                                 ql_Sort = ql.ql_Sort,
                                 Favorite_IsAdded = ql_f.f_GID
                             };

                List<QuickLinkModel> QuickLinkList = result.ToList<QuickLinkModel>();
                return QuickLinkList;
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

        // 根据DeptID和GID获取收藏报告列表
        public static List<QuickLinkModel> Get_FavoriteQuickLinkList_ByDeptID(int DeptID, string User_GID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ql in dbContext.t_QuickLink
                             join m in dbContext.t_Menu_New
                               on
                                 ql.ql_m_ID equals m.m_ID
                             join f in dbContext.t_Favorite
                               on
                                 new { typeID = 1, objectID = ql.ql_ID, GID = User_GID }
                               equals
                                 new { typeID = f.f_TypeID, objectID = f.f_ObjectID, GID = f.f_GID }
                             into t
                             from ql_f in t.DefaultIfEmpty()
                             orderby ql.ql_m_ID, ql.ql_Sort, ql.ql_Name
                             where ql.ql_m_DeptID == DeptID
                                && ql_f.f_GID != null
                             select new QuickLinkModel
                             {
                                 ql_ID = ql.ql_ID,
                                 ql_GUID = ql.ql_GUID,
                                 ql_m_DeptID = ql.ql_m_DeptID,
                                 ql_m_ID = ql.ql_m_ID,
                                 ql_m_ActionName = m.m_ActionName,
                                 ql_Name = ql.ql_Name,
                                 ql_Description = ql.ql_Description,
                                 ql_PicPath = ql.ql_PicPath,
                                 ql_Linkage = ql.ql_Linkage,
                                 ql_Status = ql.ql_Status,
                                 ql_AccessLevel = ql.ql_AccessLevel,
                                 ql_Sort = ql.ql_Sort,
                                 Favorite_IsAdded = ql_f.f_GID
                             };

                List<QuickLinkModel> QuickLinkList = result.ToList<QuickLinkModel>();
                return QuickLinkList;
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

        // 添加新快速链接
        public static bool Add_QuickLink(t_QuickLink NewQuickLink)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                dbContext.t_QuickLink.Add(NewQuickLink);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 编辑快速链接
        public static bool Update_QuickLink(t_QuickLink TargetQuickLink)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                //将实体附加到对象管理器中
                dbContext.t_QuickLink.Attach(TargetQuickLink);
                //把当前实体的状态改为Modified
                dbContext.Entry(TargetQuickLink).State = EntityState.Modified;
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 根据快速链接ID获取指定快速链接
        public static t_QuickLink Get_QuickLink_ByQuickLinkID(int ql_ID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ql in dbContext.t_QuickLink
                             where ql.ql_ID == ql_ID
                             select ql;

                t_QuickLink TargetQuickLink = result.First<t_QuickLink>();

                return TargetQuickLink;
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

        #endregion

        #region Customization 相关

        // 根据DeptID和GID获取收藏报告列表
        public static List<t_Customization> Get_CustomizationList_ByDeptID(int DeptID, string User_GID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from c in dbContext.t_Customization
                             where c.c_m_DeptID == DeptID
                                && c.c_Owner == User_GID
                             orderby c.c_Sort, c.c_Category
                             select c;

                List<t_Customization> CustomizationList = result.ToList<t_Customization>();

                return CustomizationList;
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

        // 根据DeptID获取自定义内容分类列表
        public static List<string> Get_CustomizationCategory_ByDeptID(int DeptID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = (from c in dbContext.t_Customization
                              where c.c_m_DeptID == DeptID
                              orderby c.c_Category
                              select c.c_Category).Distinct();

                List<string> CustomizationCategory_List = result.ToList<string>();

                return CustomizationCategory_List;
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

        // 添加自定义内容
        public static bool Add_Customization(t_Customization NewCustomization)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                dbContext.t_Customization.Add(NewCustomization);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 编辑自定义内容
        public static bool Update_Customization(t_Customization TargetCustomization)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                //将实体附加到对象管理器中
                dbContext.t_Customization.Attach(TargetCustomization);
                //把当前实体的状态改为Modified
                dbContext.Entry(TargetCustomization).State = EntityState.Modified;
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 根据快速链接ID获取指定快速链接
        public static t_Customization Get_Customization_ByCustomizationID(int c_ID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from c in dbContext.t_Customization
                             where c.c_ID == c_ID
                             select c;

                t_Customization TargetCustomization = result.First<t_Customization>();

                return TargetCustomization;
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

        #endregion

        #region Item 统一操作

        // 根据Item类别和ItemID删除指定Item
        public static bool Delete_ByItemTypeAndItemID(int ItemType, int ItemID, v_User CurrentUser)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            t_OprationLog log = new t_OprationLog();
            string strName = "";

            try
            {
                if (ItemType == 0)
                {
                    var result = from r in dbContext.t_Report
                                 where r.r_ID.Equals(ItemID)
                                 orderby r.r_AccessOwner
                                 select r;

                    t_Report reportModel = result.FirstOrDefault();
                    if (reportModel != null)
                    {
                        strName = reportModel.r_Name;
                    }

                    log = new t_OprationLog
                    {
                        Type = "删除报表",
                        Text = CurrentUser.User_GID + "(" + CurrentUser.User_Name_CH + ")" + " 删除了报表：" + strName,
                        CreateUser = CurrentUser.User_GID,
                        CreateDate = System.DateTime.Now,
                        Status = 0
                    };

                    dbContext.Delete_Report(ItemID);

                }
                else if (ItemType == 1)
                {
                    t_QuickLink TargetQuickLink = new t_QuickLink() { ql_ID = ItemID };

                    var result = from r in dbContext.t_QuickLink
                                 where r.ql_ID.Equals(ItemID)
                                 select r;

                    t_QuickLink quickLinkModel = result.FirstOrDefault();
                    if (quickLinkModel != null)
                    {
                        strName = quickLinkModel.ql_Name;


                        log = new t_OprationLog
                        {
                            Type = "删除QuickLink",
                            Text = CurrentUser.User_GID + "(" + CurrentUser.User_Name_CH + ")" + " 删除了QuickLink：" + strName,
                            CreateUser = CurrentUser.User_GID,
                            CreateDate = System.DateTime.Now,
                            Status = 0
                        };

                        // 将实体附加到对象管理器中
                        //dbContext.t_QuickLink.Attach(TargetQuickLink);
                        // 进行删除
                        //dbContext.t_QuickLink.Remove(TargetQuickLink);
                        dbContext.t_QuickLink.Remove(quickLinkModel);
                        dbContext.SaveChanges();
                    }

                }
                else if (ItemType == 2)
                {
                    t_Customization TargetCustomization = new t_Customization() { c_ID = ItemID };

                    var result = from r in dbContext.t_Customization
                                 where r.c_ID.Equals(ItemID)
                                 select r;
                    t_Customization customizationModel = result.FirstOrDefault();

                    if (customizationModel != null)
                    {
                        strName = customizationModel.c_Name;


                        log = new t_OprationLog
                        {
                            Type = "删除QuickLink",
                            Text = CurrentUser.User_GID + "(" + CurrentUser.User_Name_CH + ")" + " 删除了QuickLink：" + strName,
                            CreateUser = CurrentUser.User_GID,
                            CreateDate = System.DateTime.Now,
                            Status = 0
                        };

                        // 将实体附加到对象管理器中
                        //dbContext.t_Customization.Attach(TargetCustomization);
                        // 进行删除
                        //dbContext.t_Customization.Remove(TargetCustomization);
                        dbContext.t_Customization.Remove(customizationModel);
                        dbContext.SaveChanges();
                    }
                }



                DBHelper_Content.Add_Log(log);

                return true;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 根据Item类别和ItemID获取指定Item的图片路径
        public static string Get_PicPath_ByItemTypeAndItemID(int ItemType, int ItemID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                string PicPath = string.Empty;

                if (ItemType == 0)
                {
                    var result = from r in dbContext.t_Report
                                 where r.r_ID == ItemID
                                 select r.r_PicPath;

                    PicPath = result.First<string>();
                }
                else if (ItemType == 1)
                {
                    var result = from ql in dbContext.t_QuickLink
                                 where ql.ql_ID == ItemID
                                 select ql.ql_PicPath;

                    PicPath = result.First<string>();
                }
                else if (ItemType == 2)
                {
                    var result = from c in dbContext.t_Customization
                                 where c.c_ID == ItemID
                                 select c.c_PicPath;

                    PicPath = result.First<string>();
                }

                return PicPath;
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

        #endregion

        #region Process 相关

        public static bool Delete_ProcessLinkage(t_ProcessLinkage TargetLinkage)
        {
            bool flag;
            SEWC_ToolBoxEntities entities = new SEWC_ToolBoxEntities();
            try
            {
                entities.t_ProcessLinkage.Attach(TargetLinkage);
                entities.t_ProcessLinkage.Remove(TargetLinkage);
                entities.SaveChanges();
                flag = true;
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                flag = false;
            }
            finally
            {
                entities.Dispose();
            }
            return flag;
        }


        public static bool Add_ProcessLinkage(t_ProcessLinkage NewProcessLinkage)
        {
            bool flag;
            SEWC_ToolBoxEntities entities = new SEWC_ToolBoxEntities();
            try
            {
                entities.t_ProcessLinkage.Add(NewProcessLinkage);
                entities.SaveChanges();
                flag = true;
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                flag = false;
            }
            finally
            {
                entities.Dispose();
            }
            return flag;
        }

        public static bool Update_ProcessLinkage(t_ProcessLinkage TargetProcessLinkage)
        {
            bool flag;
            SEWC_ToolBoxEntities entities = new SEWC_ToolBoxEntities();
            try
            {
                entities.t_ProcessLinkage.Attach(TargetProcessLinkage);
                entities.Entry(TargetProcessLinkage).State = EntityState.Modified;
                entities.SaveChanges();
                flag = true;
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                flag = false;
            }
            finally
            {
                entities.Dispose();
            }
            return flag;
        }


        // 根据MenuID和GID获取满足条件的报告列表
        //public static List<v_ProcessTopology> Get_ProcessTopologyList_ByMenuID(int m_ID)
        //{
        //    SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

        //    try
        //    {
        //        var result = from pt in dbContext.v_ProcessTopology
        //                     where pt.pn_m_ID == m_ID
        //                     orderby pt.pn_ID, pt.pc_NextNode_ID
        //                     select pt;

        //        List<v_ProcessTopology> ProcessTopologyList = result.ToList<v_ProcessTopology>();

        //        return ProcessTopologyList;
        //    }
        //    catch (Exception ex)
        //    {
        //        EventlogHelper.AddLog(ex.Message);
        //        return null;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        // 根据MenuID获取满足条件的流程节点
        public static List<t_ProcessNode> Get_ProcessNodes_ByMenuID(int m_ID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from pn in dbContext.t_ProcessNode
                             where pn.pn_m_ID == m_ID
                             orderby pn.pn_ID
                             select pn;

                List<t_ProcessNode> NodeList = result.ToList<t_ProcessNode>();

                return NodeList;
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

        // 根据MenuID和GID获取满足条件的报告列表

        public static List<t_ProcessNode> Get_ProcessNodes_ByDeptID(int dept_ID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from pn in dbContext.t_ProcessNode
                             where pn.pn_m_DeptID == dept_ID
                             orderby pn.pn_ID
                             select pn;

                List<t_ProcessNode> NodeList = result.ToList<t_ProcessNode>();

                return NodeList;
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

        public static List<t_ProcessLinkage> Get_ProcessLinkage_ByNodeID(int node_ID)
        {
            List<t_ProcessLinkage> list;
            SEWC_ToolBoxEntities entities = new SEWC_ToolBoxEntities();
            try
            {
                list = entities.t_ProcessLinkage.Where(pl => pl.pl_pn_ID == node_ID).OrderBy(pl => pl.pl_Type).ThenBy(pl => pl.pl_Sort).ThenBy(pl => pl.pl_Name).ToList();
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                list = null;
            }
            finally
            {
                entities.Dispose();
            }
            return list;
        }

        public static List<ProcessLinkageModel> Get_ProcessLinkage_ByNodeID(int node_ID, string User_GID)
        {
            List<ProcessLinkageModel> list;
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            var plModel = from pl in dbContext.t_ProcessLinkage
                          join f in dbContext.t_Favorite
                            on
                              new { typeID = 2, objectID = pl.pl_ID, GID = User_GID }
                            equals
                              new { typeID = f.f_TypeID, objectID = f.f_ObjectID, GID = f.f_GID }
                          into t
                          from p_f in t.DefaultIfEmpty()
                          where pl.pl_pn_ID == node_ID
                          select new ProcessLinkageModel
                          {
                              pl_ID = pl.pl_ID,
                              pl_Name = pl.pl_Name,
                              pl_Type = pl.pl_Type,
                              pl_pn_ID = pl.pl_pn_ID,
                              pl_Linkage = pl.pl_Linkage,
                              pl_Sort = pl.pl_Sort,
                              Favorite = p_f.f_GID
                          };

            list = plModel.Distinct().OrderBy(x => x.pl_Sort).ToList();

            return list;
        }

        public static List<ProcessLinkageModel> Get_FavoriteLinkage_ByDeptID(int DeptID, string User_GID)
        {
            List<ProcessLinkageModel> list;
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            var plModel = from pl in dbContext.t_ProcessLinkage
                          join f in dbContext.t_Favorite
                            on
                              new { typeID = 2, objectID = pl.pl_ID }
                            equals
                              new { typeID = f.f_TypeID, objectID = f.f_ObjectID }
                          into t
                          from p_f in t
                          where p_f.f_GID == User_GID
                          select new ProcessLinkageModel
                          {
                              pl_ID = pl.pl_ID,
                              pl_Name = pl.pl_Name,
                              pl_Type = pl.pl_Type,
                              pl_pn_ID = pl.pl_pn_ID,
                              pl_Linkage = pl.pl_Linkage,
                              pl_Sort = pl.pl_Sort,
                              Favorite = p_f.f_GID
                          };

            list = plModel.ToList();

            return list;
        }

        public static t_Attachment Get_Attachment_ByID(int attachment_ID)
        {
            t_Attachment attachment;
            SEWC_ToolBoxEntities entities = new SEWC_ToolBoxEntities();
            try
            {
                attachment = entities.t_Attachment.Where(a => a.a_ID == attachment_ID).OrderBy(a => a.a_FileName).FirstOrDefault();
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                attachment = null;
            }
            finally
            {
                entities.Dispose();
            }
            return attachment;
        }

        public static List<v_ProcessNode> Get_ProcessNodes()
        {
            List<v_ProcessNode> list;
            SEWC_ToolBoxEntities entities = new SEWC_ToolBoxEntities();

            try
            {
                list = entities.v_ProcessNode.ToList();

            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                list = null;
            }
            finally
            {
                entities.Dispose();
            }

            return list;
        }

        // 根据MenuID获取满足条件的流程链接
        public static List<t_ProcessConnection> Get_Process_Connections_ByMenuID(int m_ID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from pc in dbContext.t_ProcessConnection
                             where pc.pc_m_ID == m_ID
                             orderby pc.pc_Node_ID
                             select pc;

                List<t_ProcessConnection> ConnectionList = result.ToList<t_ProcessConnection>();

                return ConnectionList;
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

        // 根据NodeID获取相关的文件
        //public static List<t_ProcessAttachment> Get_Process_Attachments_ByNodeID(int pn_ID)
        //{
        //    SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

        //    try
        //    {
        //        var result = from pa in dbContext.t_ProcessAttachment
        //                     where pa.pa_pn_ID == pn_ID
        //                     orderby pa.pa_FileName
        //                     select pa;

        //        List<t_ProcessAttachment> ConnectionList = result.ToList<t_ProcessAttachment>();

        //        return ConnectionList;
        //    }
        //    catch (Exception ex)
        //    {
        //        EventlogHelper.AddLog(ex.Message);
        //        return null;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        public static t_ProcessLinkage Get_ProcessLinkage_ByID(int pl_ID)
        {
            t_ProcessLinkage linkage;
            SEWC_ToolBoxEntities entities = new SEWC_ToolBoxEntities();
            try
            {
                linkage = entities.t_ProcessLinkage.Where(pl => pl.pl_ID == pl_ID).FirstOrDefault();
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                linkage = null;
            }
            finally
            {
                entities.Dispose();
            }
            return linkage;
        }


        #endregion

        #region Search
        // ajax Search name List 
        public static string[] GetNameList(string searchStr)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            string[] searchModels = null;
            List<string> linkNameList = null;
            List<t_ProcessLinkage> list;
            List<ReportModel> reportList;
            List<string> rportNametList;
            SEWC_ToolBoxEntities entities = new SEWC_ToolBoxEntities();

            try
            {
                list = entities.t_ProcessLinkage.Where(pl => pl.pl_Type == 1 && pl.pl_Name.Contains(searchStr)).OrderBy(pl => pl.pl_Sort).ThenBy(pl => pl.pl_Name).ToList();

                linkNameList = list.Select(x => x.pl_Name).ToList();

                var result = from r in dbContext.v_Report
                             where r.r_Name.Contains(searchStr)
                             select new ReportModel
                             {
                                 r_ID = r.r_ID,
                                 r_Name = r.r_Name,
                             };
                reportList = result.ToList();
                rportNametList = reportList.Select(x => x.r_Name).ToList();
                linkNameList.AddRange(rportNametList);
                searchModels = linkNameList.ToArray();

            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                list = null;
            }
            finally
            {
                entities.Dispose();
                dbContext.Dispose();
            }

            return searchModels;
        }

        public static List<SearchModels> AutoNameList(string keywords)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            List<SearchModels> searchModelList = null;
            List<SearchModels> linkNameList = null;
            List<SearchModels> reportList;
            SEWC_ToolBoxEntities entities = new SEWC_ToolBoxEntities();

            try
            {
                var rt = from t in dbContext.t_ProcessLinkage
                         where t.pl_Type == 1 && t.pl_Name.Contains(keywords)
                         orderby t.pl_Sort
                         orderby t.pl_Name
                         select new SearchModels
                         {
                             Id = t.pl_ID,
                             Name = t.pl_Name
                         };

                var result = from r in dbContext.v_Report
                             where r.r_Name.Contains(keywords)
                             select new SearchModels
                             {
                                 Id = r.r_ID,
                                 Name = r.r_Name,
                             };

                var quickLinkResult = from q in dbContext.t_QuickLink
                                      where q.ql_Name.Contains(keywords)
                                      select new SearchModels
                                      {
                                          Id = q.ql_ID,
                                          Name = q.ql_Name,
                                      };
                reportList = rt.ToList();
                linkNameList = result.ToList();

                searchModelList = reportList.Union(linkNameList).Union(quickLinkResult).ToList<SearchModels>();

            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                searchModelList = null;
            }
            finally
            {
                entities.Dispose();
                dbContext.Dispose();
            }

            return searchModelList;
        }


        public static SearchModel GetSearchList(string searchStr)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            SearchModel searchModels = new SearchModel();

            List<t_ProcessLinkage> list = new List<t_ProcessLinkage>();
            SEWC_ToolBoxEntities entities = new SEWC_ToolBoxEntities();
            try
            {
                list = entities.t_ProcessLinkage.Where(pl => pl.pl_Type == 1 && pl.pl_Name.Contains(searchStr)).OrderBy(pl => pl.pl_Sort).ThenBy(pl => pl.pl_Name).ToList();

                var result = from r in dbContext.v_Report
                             select new ReportModel
                             {
                                 r_ID = r.r_ID,
                                 r_m_DeptID = r.r_m_DeptID,
                                 r_m_ID_Category = r.r_m_ID_Category,
                                 Menu_CategoryName = r.Menu_CategoryName,
                                 Menu_CategoryActionName = r.Menu_CategoryActionName,
                                 r_m_ID_Frequency = r.r_m_ID_Frequency,
                                 Menu_FrequencyName = r.Menu_FrequencyName,
                                 Menu_FrequencyActionName = r.Menu_FrequencyActionName,
                                 r_Name = r.r_Name,
                                 r_Description = r.r_Description,
                                 r_Owner = r.r_Owner,
                                 User_Owner = r.User_Owner,
                                 User_OwnerEmail = r.User_OwnerEmail,
                                 r_AccessOwner = r.r_AccessOwner,
                                 User_AccessOwner = r.User_AccessOwner,
                                 User_AccessOwnerEmail = r.User_AccessOwnerEmail,
                                 r_Admin = r.r_Admin,
                                 r_AdminEmail = r.r_AdminEmail,
                                 r_PicPath = r.r_PicPath,
                                 r_Linkage = r.r_Linkage,
                                 r_Status = r.r_Status,
                                 r_AccessLevel = r.r_AccessLevel,
                                 r_Sort = r.r_Sort,
                                 r_cs_ClickTimes = r.r_cs_ClickTimes,
                                 r_lr_LastReloadTime = r.r_lr_LastReloadTime,
                                 Menu_e_ID = r.Menu_e_ID,
                             };
                List<ReportModel> reportList = result.ToList();
                if (list.Count > 0)
                {
                    searchModels.DocumentList = list;
                }
                if (reportList.Count > 0)
                {
                    searchModels.ReprotList = reportList;
                }
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                list = null;
            }
            finally
            {
                entities.Dispose();
                dbContext.Dispose();
            }

            return searchModels;
        }

        public static List<t_ProcessLinkage> GetSearchDocumentList()
        {
            List<t_ProcessLinkage> list = new List<t_ProcessLinkage>();
            SEWC_ToolBoxEntities entities = new SEWC_ToolBoxEntities();
            try
            {
                list = entities.t_ProcessLinkage.Where(pl => pl.pl_Type == 1).OrderBy(pl => pl.pl_Sort).ThenBy(pl => pl.pl_Name).ToList();
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                list = null;
            }
            finally
            {
                entities.Dispose();
            }

            return list;
        }

        public static List<ReportModel> GetSearchReportList()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            List<t_ProcessLinkage> list = new List<t_ProcessLinkage>();
            SEWC_ToolBoxEntities entities = new SEWC_ToolBoxEntities();
            List<ReportModel> reportList = null;
            try
            {

                var result = from r in dbContext.v_Report
                             select new ReportModel
                             {
                                 r_ID = r.r_ID,
                                 r_m_DeptID = r.r_m_DeptID,
                                 r_m_ID_Category = r.r_m_ID_Category,
                                 Menu_CategoryName = r.Menu_CategoryName,
                                 Menu_CategoryActionName = r.Menu_CategoryActionName,
                                 r_m_ID_Frequency = r.r_m_ID_Frequency,
                                 Menu_FrequencyName = r.Menu_FrequencyName,
                                 Menu_FrequencyActionName = r.Menu_FrequencyActionName,
                                 r_Name = r.r_Name,
                                 r_Description = r.r_Description,
                                 r_Owner = r.r_Owner,
                                 User_Owner = r.User_Owner,
                                 User_OwnerEmail = r.User_OwnerEmail,
                                 r_AccessOwner = r.r_AccessOwner,
                                 User_AccessOwner = r.User_AccessOwner,
                                 User_AccessOwnerEmail = r.User_AccessOwnerEmail,
                                 r_Admin = r.r_Admin,
                                 r_AdminEmail = r.r_AdminEmail,
                                 r_PicPath = r.r_PicPath,
                                 r_Linkage = r.r_Linkage,
                                 r_Status = r.r_Status,
                                 r_AccessLevel = r.r_AccessLevel,
                                 r_Sort = r.r_Sort,
                                 r_cs_ClickTimes = r.r_cs_ClickTimes,
                                 r_lr_LastReloadTime = r.r_lr_LastReloadTime,
                                 Menu_e_ID = r.Menu_e_ID,
                             };
                reportList = result.ToList();

            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                list = null;
            }
            finally
            {
                entities.Dispose();
                dbContext.Dispose();
            }

            return reportList;
        }

        /// <summary>
        /// /Get search ProcessLinkage
        /// </summary>
        /// <param name="keywords">check key words</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="total">page count</param>
        /// <param name="isAsc">bool order by desc or asc</param>
        /// <returns></returns>
        public static List<ProcessLinkageModel> GetPageSearchDocumentList(string keywords, int pageSize, int pageIndex, out int total, bool isAsc, string User_GID)
        {
            //SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            //IQueryable<ProcessLinkageModel> tempdata;
            //total = dbContext.Set<ProcessLinkageModel>().Where(x => x.pl_Name.Contains(keywords)).Count();

            //if (isAsc)
            //{
            //    tempdata = dbContext.Set<ProcessLinkageModel>().Where(x => x.pl_Name.Contains(keywords)).OrderBy(x => x.pl_ID).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            //}
            //else
            //{
            //    tempdata = dbContext.Set<ProcessLinkageModel>().Where(x => x.pl_Name.Contains(keywords)).OrderByDescending(x => x.pl_ID).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            //}

            //return tempdata;


            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            List<ProcessLinkageModel> rptModel = new List<ProcessLinkageModel>();

            var list = from r in dbContext.t_ProcessLinkage
                       join n in dbContext.t_ProcessNode on r.pl_pn_ID equals n.pn_ID
                       join m in dbContext.t_Menu_New on n.pn_m_ID equals m.m_ID
                       join mp in dbContext.t_Menu_New on m.m_ParentID equals mp.m_ID
                       join f in dbContext.t_Favorite
                         on
                           new { typeID = 2, objectID = r.pl_ID, GID = User_GID }
                         equals
                           new { typeID = f.f_TypeID, objectID = f.f_ObjectID, GID = f.f_GID }
                       into t
                       from r_f in t.DefaultIfEmpty()
                       where r.pl_Name.Contains(keywords)
                       select new ProcessLinkageModel
                       {
                           pl_ID = r.pl_ID,
                           pl_Name = r.pl_Name,
                           pl_Linkage = r.pl_Linkage,
                           pl_pn_ID = r.pl_pn_ID,
                           pl_Sort = r.pl_Sort,
                           pl_Type = r.pl_Type,
                           Favorite = r_f.f_GID,
                           Location = mp.m_MenuName + " -> " + m.m_MenuName + " -> " + n.pn_NodeName
                       };

            total = list.Count();


            if (isAsc)
            {
                rptModel = list.OrderBy(x => x.pl_Name).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

                //dbContext.Set<v_Report>().Where(x => x.r_Name.Contains(keywords)).OrderBy(x => x.r_ID).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            }
            else
            {
                rptModel = list.OrderByDescending(x => x.pl_Name).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                //dbContext.Set<v_Report>().Where(x => x.r_Name.Contains(keywords)).OrderByDescending(x => x.r_ID).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            }

            return rptModel;
        }


        /// <summary>
        /// /Get search ProcessLinkage
        /// </summary>
        /// <param name="keywords">check key words</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="total">page count</param>
        /// <param name="isAsc">bool order by desc or asc</param>
        /// <returns></returns>
        public static IQueryable<v_Report> GetPageSearchReportList(string keywords, int pageSize, int pageIndex, out int total, bool isAsc)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            IQueryable<v_Report> tempdata;
            total = dbContext.Set<v_Report>().Where(x => x.r_Name.Contains(keywords)).Count();

            if (isAsc)
            {
                tempdata = dbContext.Set<v_Report>().Where(x => x.r_Name.Contains(keywords)).OrderBy(x => x.r_ID).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            }
            else
            {
                tempdata = dbContext.Set<v_Report>().Where(x => x.r_Name.Contains(keywords)).OrderByDescending(x => x.r_ID).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            }

            return tempdata;
        }


        public static List<ReportModel> GetPageSearchReportLists(string keywords, int pageSize, int pageIndex, out int total, bool isAsc, string User_GID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            List<ReportModel> rptModel = new List<ReportModel>();

            var list = from r in dbContext.v_Report
                       join f in dbContext.t_Favorite
                         on
                           new { typeID = 0, objectID = r.r_ID, GID = User_GID }
                         equals
                           new { typeID = f.f_TypeID, objectID = f.f_ObjectID, GID = f.f_GID }
                       into t
                       from r_f in t.DefaultIfEmpty()
                       where r.r_Name.Contains(keywords)
                       select new ReportModel
                       {
                           r_ID = r.r_ID,
                           r_m_DeptID = r.r_m_DeptID,
                           r_m_ID_Category = r.r_m_ID_Category,
                           Menu_SubCategoryName = r.Menu_SubCategoryName,
                           Menu_CategoryName = r.Menu_CategoryName,
                           Menu_CategoryActionName = r.Menu_CategoryActionName,
                           r_m_ID_Frequency = r.r_m_ID_Frequency,
                           Menu_FrequencyName = r.Menu_FrequencyName,
                           Menu_FrequencyActionName = r.Menu_FrequencyActionName,
                           r_Name = r.r_Name,
                           r_Description = r.r_Description,
                           r_Owner = r.r_Owner,
                           User_Owner = r.User_Owner,
                           User_OwnerEmail = r.User_OwnerEmail,
                           User_CrOwner = r.User_CrOwner,
                           User_CrEmail = r.User_CrOwnerEmail,
                           r_AccessOwner = r.r_AccessOwner,
                           User_AccessOwner = r.User_AccessOwner,
                           User_AccessOwnerEmail = r.User_AccessOwnerEmail,
                           r_Admin = r.r_Admin,
                           r_AdminEmail = r.r_AdminEmail,
                           r_PicPath = r.r_PicPath,
                           r_Linkage = r.r_Linkage,
                           r_Status = r.r_Status,
                           r_AccessLevel = r.r_AccessLevel,
                           r_Sort = r.r_Sort,
                           r_cs_ClickTimes = r.r_cs_ClickTimes,
                           r_lr_LastReloadTime = r.r_lr_LastReloadTime,
                           Menu_e_ID = r.Menu_e_ID,
                           Favorite_IsAdded = r_f.f_GID,
                           Location = "Business Transparency" + " -> " + r.Menu_SubCategoryName + "->" + r.Menu_CategoryName
                       };

            total = list.Count();

            if (isAsc)
            {
                rptModel = list.OrderBy(x => x.r_ID).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            }
            else
            {
                rptModel = list.OrderByDescending(x => x.r_ID).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            }

            return rptModel;
        }

        /// <summary>
        /// 获取quilick分页查询列表
        /// </summary>
        /// <param name="keywords">搜索关键字</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="total">数量</param>
        /// <param name="isAsc">是否降序</param>
        /// <param name="User_GID">用户Gid</param>
        /// <returns></returns>
        public static List<QuickLinkModel> GetPageSearchQuickLinkList(string keywords, int pageSize, int pageIndex, out int total, bool isAsc, string User_GID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            List<QuickLinkModel> qlkModel = new List<QuickLinkModel>();

            var result = from ql in dbContext.t_QuickLink
                         join m in dbContext.t_Menu_New
                         on ql.ql_m_ID equals m.m_ID
                         join pm in dbContext.t_Menu_New
                         on m.m_ParentID equals pm.m_ID
                         join f in dbContext.t_Favorite
                           on
                             new { typeID = 1, objectID = ql.ql_ID, GID = User_GID }
                           equals
                             new { typeID = f.f_TypeID, objectID = f.f_ObjectID, GID = f.f_GID }
                         into t
                         from ql_f in t.DefaultIfEmpty()
                         where ql.ql_Name.Contains(keywords)
                         orderby ql.ql_m_ID, ql.ql_Sort, ql.ql_Name
                         select new QuickLinkModel
                         {
                             ql_ID = ql.ql_ID,
                             ql_GUID = ql.ql_GUID,
                             ql_m_DeptID = ql.ql_m_DeptID,
                             ql_m_ID = ql.ql_m_ID,
                             ql_Name = ql.ql_Name,
                             ql_Description = ql.ql_Description,
                             ql_PicPath = ql.ql_PicPath,
                             ql_Linkage = ql.ql_Linkage,
                             ql_Status = ql.ql_Status,
                             ql_AccessLevel = ql.ql_AccessLevel,
                             ql_Sort = ql.ql_Sort,
                             Favorite_IsAdded = ql_f.f_GID,
                             Location = pm.m_MenuName + " -> " + m.m_MenuName
                         };

            List<QuickLinkModel> QuickLinkList = result.ToList<QuickLinkModel>();

            total = result.Count();

            if (isAsc)
            {
                qlkModel = result.OrderBy(x => x.ql_ID).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            }
            else
            {
                qlkModel = result.OrderByDescending(x => x.ql_ID).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            }

            return qlkModel;


        }

        public static string ConvertToJson<T>(T data)
        {
            try
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(data.GetType());
                using (MemoryStream ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, data);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch
            {
                return null;
            }
        }

        public static Object ConvertoToObject(string json, Type t)
        {
            try
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(t);
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    return serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region OprationLog
        // 添加操作日志
        public static bool Add_Log(t_OprationLog newLog)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                dbContext.t_OprationLog.Add(newLog);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public static List<t_OprationLog> Get_LogList()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from o in dbContext.t_OprationLog
                             orderby o.CreateDate descending
                             select o;


                return result.Take(10).ToList();
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

        #endregion

        #region 更新报表记录 remove list
        public static bool UpdateRemove(string oldAppId, string newReportLink)
        {
            string reportId = string.Empty;
            int start = newReportLink.IndexOf("app", StringComparison.OrdinalIgnoreCase);
            if (start > 0)
            {
                reportId = newReportLink.Substring(start + 4, 36);
            }
            else
            {
                return false;
            }
            try
            {
                using (SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities())
                {
                    var list = dbContext.BI_LU_SCM_REPORT_ACCESS_REMOVED.Where(v => v.REPORT_LINK.Contains(oldAppId) || v.APPIDS.Contains(oldAppId));
                    foreach (var item in list)
                    {
                        if (!string.IsNullOrWhiteSpace(item.APPIDS))
                        {
                            if (!item.APPIDS.Contains(reportId))
                            {
                                item.APPIDS = item.APPIDS + "," + reportId;
                            }
                        }
                        else
                        {
                            item.APPIDS = reportId;
                        }
                    }
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }
        }

        #endregion

        #region 申请报表前去removelist中查询
        public static bool CheckRemoveList(string G_ID, string reportLink)
        {
            try
            {
                using (SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities())
                {
                    return dbContext.BI_LU_SCM_REPORT_ACCESS_REMOVED.Any(v => v.USER_GID == G_ID && v.REPORT_LINK == reportLink);
                }

            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }

        }

        #endregion

        #region Issue处理

        public static t_ReportIssue IssueFirst(int id)
        {
            t_ReportIssue model = null;
            using (SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities())
            {
                try
                {
                    model = dbContext.t_ReportIssue.FirstOrDefault(a => a.id == id);
                }
                catch (Exception ex)
                {

                    EventlogHelper.AddLog(ex.ToString());
                }
            }

            return model;
        }

        /// <summary>
        /// 更新Issue内容
        /// </summary>
        /// <param name="inEnt">要更新的对象，内部ID必填</param>
        /// <returns></returns>
        public static bool IssueUpdate(t_ReportIssue inEnt)
        {
            var flag = false;
            using (SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities())
            {
                try
                {
                    var model = dbContext.t_ReportIssue.FirstOrDefault(a => a.id == inEnt.id);
                    if (model != null)
                    {
                        //回答内容
                        model.HandleTime = inEnt.HandleTime;
                        model.HandleUser = inEnt.HandleUser;
                        model.HandleUserName = inEnt.HandleUserName;
                        model.State = inEnt.State;
                        model.AnswerReason = inEnt.AnswerReason;
                        model.AnswerDetail = inEnt.AnswerDetail;

                        //基本不变信息
                        model.AdminOwner = inEnt.AdminOwner;
                        model.AdminOwnerName = inEnt.AdminOwnerName;
                        model.CreateUser = inEnt.CreateUser;
                        model.CreateUserName = inEnt.CreateUserName;
                        model.Issues = inEnt.Issues;
                        model.Issue_image = inEnt.Issue_image;
                        model.Issusue_title = inEnt.Issusue_title;
                        model.ReportTitle = inEnt.ReportTitle;
                        model.r_Id = inEnt.r_Id;

                        flag = dbContext.SaveChanges() > 0;
                    }
                    else
                    {
                        throw new Exception($"无效Issue的ID号码{inEnt.id}");
                    }
                }
                catch (Exception ex)
                {

                    EventlogHelper.AddLog(ex.ToString());
                }
            }

            return flag;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="id">问题ID</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public static bool IssueSetState(int id, short state)
        {
            var flag = false;
            using (SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities())
            {
                try
                {
                    var model = dbContext.t_ReportIssue.FirstOrDefault(a => a.id == id);
                    if (model != null)
                    {
                        model.State = state;

                        flag = dbContext.SaveChanges() > 0;
                    }
                    else
                    {
                        throw new Exception($"无效Issue的ID号码{id}");
                    }
                }
                catch (Exception ex)
                {

                    EventlogHelper.AddLog(ex.ToString());
                }
            }

            return flag;
        }

        /// <summary>
        /// 获取所有ISSUE数据
        /// </summary>
        /// <param name="reportTitle">报表名字</param>
        /// <param name="createUser">创建人</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public static List<t_ReportIssue> GetIssueList(string reportTitle = "", string createUser = "", int state = 0)
        {
            return GetIssueList(DateTime.MinValue, DateTime.MinValue, reportTitle, createUser, state);

        }

        /// <summary>
        /// 获取所有ISSUE数据
        /// </summary>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="reportTitle">报表名字</param>
        /// <param name="createUser">创建人</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public static List<t_ReportIssue> GetIssueList(DateTime start, DateTime end, string reportTitle = "", string createUser = "", int state = 0)
        {
            List<t_ReportIssue> result = null;
            using (SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities())
            {
                try
                {
                    var query = dbContext.t_ReportIssue.AsNoTracking().AsEnumerable();
                    if (!string.IsNullOrEmpty(reportTitle))
                    {
                        query = query.Where(a => a.ReportTitle != null && a.ReportTitle.ToLower().IndexOf(reportTitle.ToLower()) != -1);
                    }

                    if (!string.IsNullOrEmpty(createUser))
                    {
                        query = query.Where(a => createUser.Equals(a.CreateUser, StringComparison.OrdinalIgnoreCase));
                    }
                    if (state > 0)
                    {
                        query = query.Where(a => a.State == state);
                    }
                    else
                    {
                        var s = (int)IssueState.Revoke;
                        query = query.Where(a => a.State != s);
                    }

                    if (start < end)
                    {
                        query = query.Where(a => a.Createtime >= start && a.Createtime <= start);
                    }

                    result = query.OrderByDescending(a => a.id).ToList();
                }
                catch (Exception ex)
                {
                    EventlogHelper.AddLog(ex.ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// 分页列取所有报表提问的问题
        /// </summary>
        /// <param name="page">当前页码</param>
        /// <param name="size">每页数量</param>
        /// <param name="reportTitle">报表名字</param>
        /// <param name="questionUserName">提问人名称</param>
        /// <param name="state">状态 枚举类型 IssueState</param>
        /// <returns></returns>
        public static IPagerList<t_ReportIssue> GetIssuePageList(int page, int size, string reportTitle = "", string createUser = "", int state = 0)
        {
            IPagerList<t_ReportIssue> result = null;
            using (SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities())
            {
                try
                {
                    var query = dbContext.t_ReportIssue.AsNoTracking().AsEnumerable();
                    if (!string.IsNullOrEmpty(reportTitle))
                    {
                        //query = query.Where(a => a.ReportTitle == reportTitle);
                        //query = query.Where(a => a.ReportTitle != null && a.ReportTitle.Contains(reportTitle));
                        //query = query.Where(a => a.ReportTitle != null && reportTitle.ToLower().IndexOf(a.ReportTitle.ToLower()) != -1);
                        query = query.Where(a => a.ReportTitle != null && a.ReportTitle.ToLower().IndexOf(reportTitle.ToLower()) != -1);
                    }

                    if (!string.IsNullOrEmpty(createUser))
                    {
                        //query = query.Where(a => a.CreateUser == createUser);
                        query = query.Where(a => createUser.Equals(a.CreateUser, StringComparison.OrdinalIgnoreCase));
                    }
                    if (state > 0)
                    {
                        query = query.Where(a => a.State == state);
                    }
                    else
                    {
                        var s = (int)IssueState.Revoke;
                        query = query.Where(a => a.State != s);
                    }

                    result = query.OrderByDescending(a => a.id).ToPagedList(page, size);
                }
                catch (Exception ex)
                {
                    EventlogHelper.AddLog(ex.ToString());

                    result = new PagerList<t_ReportIssue>(null, 0, page, size);
                }
            }
            return result;
        }

        #endregion

        #region 用户

        /// <summary>
        /// 获取用户集合
        /// </summary>
        /// <param name="uName">用户的英文名或者中文或者GID</param>
        /// <returns></returns>
        public static List<v_User> ContainsUsers(string uName)
        {
            using (SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities())
            {
                var res = dbContext.v_User.Where(a => a.User_GID.Contains(uName) || a.User_Name_CH.Contains(uName) || a.User_Name_EN.Contains(uName));

                return res.ToList();
            }
        }

        #endregion
    }

    public enum LogType
    {
        AddLog,
        UpdateLog,
        DelLog,
    }
}
