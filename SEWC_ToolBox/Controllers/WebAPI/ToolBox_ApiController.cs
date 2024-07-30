using SEWC_NetDevLib.SEWC_NetLibExtend;
using SEWC_ToolBox.ApiParameterModels;
using SEWC_ToolBox.DAL.DBHelpers;
using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox.DAL.EFs.EnumCollect;
using SEWC_ToolBox.DAL.SecondModels;
using SEWC_ToolBox.Languages;
using SEWC_ToolBox.Utilities.Helpers;
using SEWC_ToolBox.Utilities.Mail;
using SEWC_ToolBox.Utilities.Upload;
using SEWC_ToolBox.Utilities.Upload.Entity;
using SEWC_ToolBox_Project;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace SEWC_ToolBox.Controllers.WebAPI
{
    public class ToolBox_ApiController : ApiController
    {
        #region Favorite 相关

        /// <summary>
        /// 呈现对象的添加和移除收藏
        /// </summary>
        /// <param name="f_TypeID">对象类型</param>
        /// <param name="f_ObjectID">对象ID</param>
        /// <param name="IsAdd">是添加、移除</param>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Update_Favorite
        public JsonResult<OperationMessage> Update_Favorite([FromBody] Update_Favorite_Model Parameters)
        {
            OperationMessage ReturnMsg = new OperationMessage();
            v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;

            bool result = DBHelper_Content.Update_Favorite(Parameters.f_TypeID, Parameters.f_ObjectID, Parameters.isAdd, CurrentUser.User_GID);

            ReturnMsg.OpResult = result;

            if (result)
            {
                if (Parameters.isAdd)
                    ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_AddFavorite_Successed");
                else
                    ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_RemoveFavorite_Successed");
            }
            else
            {
                if (Parameters.isAdd)
                    ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_AddToFavorites_Failed");
                else
                    ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_RemoveFavorites_Failed");
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        #endregion

        #region Item 统一操作

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Upload_ItemImage
        public JsonResult<OperationMessage> Upload_ItemImage()
        {
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                if (HttpContext.Current.Request.Files.Count == 1)
                {
                    string PicType = HttpContext.Current.Request["picType"].ToString();
                    var file = HttpContext.Current.Request.Files[0];
                    var stream = file.InputStream;
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);

                    var store = new FileStore(file.FileName, buffer);
                    store.Bucket = "Img/UploadTemp";
                    store.Pre = AppConfig.PreURL;

                    IOSS upload = new OSSImage();
                    var res = upload.Upload(store);

                    if (res.Success)
                    {
                        ReturnMsg.OpResult = true;
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Uploaded");
                        ReturnMsg.EXT01 = res.getFileName();
                        ReturnMsg.EXT02 = res.VirtualPath;
                        ReturnMsg.EXT03 = $"//{HttpContext.Current.Request.Url.Authority}{res.SavePath}";
                    }
                    else
                    {
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Uploaded_Fail");
                    }
                }
                else
                {
                    ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Uploaded_Fail");

                }

            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        /// <summary>
        /// 删除指定Item
        /// </summary>
        /// <param name="itemType">item的类型</param>
        /// <param name="itemID">item的唯一数字ID</param>
        /// <returns></returns>
        [HttpPost]
        // DELETE: api/ToolBox_Api/Delete_Item
        public JsonResult<OperationMessage> Delete_Item([FromBody] ItemDelete_Model Parameters)
        {
            OperationMessage ReturnMsg = new OperationMessage();
            v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;
            try
            {
                IOSS oss = new OSSImage();
                string PicPath = DBHelper_Content.Get_PicPath_ByItemTypeAndItemID(Parameters.itemType, Parameters.itemID);

                using (var scope = new TransactionScope())
                {
                    ReturnMsg.OpResult = DBHelper_Content.Delete_ByItemTypeAndItemID(Parameters.itemType, Parameters.itemID, CurrentUser);

                    if (ReturnMsg.OpResult)
                    {
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Deletion_Successed");

                        oss.Delete(PicPath);

                        scope.Complete();

                    }
                    else
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Deletion_Failed");
                }
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        #endregion

        #region Report 相关

        /// <summary>
        /// 添加新报表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Add_Report
        public JsonResult<OperationMessage> Add_Report()
        {
            OperationMessage ReturnMsg = new OperationMessage();
            v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;

            try
            {
                #region 获取表单数据

                t_Report NewReport = new t_Report();
                NewReport.r_GUID = HttpContext.Current.Request["input_reportGUID"].ToString();
                NewReport.r_m_DeptID = int.Parse(HttpContext.Current.Request["input_deptID"].ToString());
                NewReport.r_m_ID_Category = int.Parse(HttpContext.Current.Request["select_Category"].ToString());
                NewReport.r_m_ID_Frequency = int.Parse(HttpContext.Current.Request["select_UpdateFrequency"].ToString());
                NewReport.r_Name = HttpContext.Current.Request["input_reportName"].ToString();
                NewReport.r_Description = string.IsNullOrWhiteSpace(HttpContext.Current.Request["input_description"].ToString()) ? "N/A" : HttpContext.Current.Request["input_description"].ToString();
                NewReport.r_Owner = HttpContext.Current.Request["select_Owner"].ToString();
                NewReport.r_CROwner = HttpContext.Current.Request["select_CrOwner"].ToString();
                NewReport.r_AccessOwner = HttpContext.Current.Request["select_AccessOwner"].ToString();
                NewReport.r_Admin = HttpContext.Current.Request["select_Admin"].ToString();
                NewReport.r_m_ID_SubCategory = int.Parse(HttpContext.Current.Request["select_SubCategory"].ToString());
                NewReport.r_Linkage = HttpContext.Current.Request["input_linkage"].ToString();
                NewReport.r_Status = int.Parse(HttpContext.Current.Request["input_enabled"].ToString());
                NewReport.r_AccessLevel = 0;
                NewReport.r_Sort = int.Parse(HttpContext.Current.Request["input_sort"].ToString());
                NewReport.r_APPIDS = HttpContext.Current.Request["input_AppIds"].ToString();
                NewReport.CreateDate = DateTime.Now;
                NewReport.CreateUser = CurrentUser.User_GID;
                string FilePath = HttpContext.Current.Request["input_picPath"].ToString();
                MoveStore store = new MoveStore(FilePath);
                store.Pre = AppConfig.PreURL;
                store.Bucket = "Img/ReportImg";
                #endregion

                #region 提交数据

                var oss = new OSSImage();
                var canMove = oss.CanMove(store);
                using (var scope = new TransactionScope())
                {
                    NewReport.r_PicPath = canMove.Success ? canMove.VirtualPath : "N/A";

                    ReturnMsg.OpResult = DBHelper_Content.Add_Report(NewReport);
                    if (!string.IsNullOrWhiteSpace(NewReport.r_APPIDS))
                    {
                        DBHelper_Content.UpdateRemove(NewReport.r_APPIDS, NewReport.r_Linkage);
                    }

                    var flag = false;
                    if (ReturnMsg.OpResult)
                    {
                        if (canMove.Success)
                        {
                            var ossRes = oss.Move(store);
                            if (ossRes.Success)
                            {
                                scope.Complete();
                                flag = true;
                            }
                        }
                        else
                        {
                            scope.Complete();
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Creation_Successed");
                    }
                    else
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Creation_Failed");

                    t_OprationLog log = new t_OprationLog
                    {
                        Type = "新增报表",
                        Text = CurrentUser.User_GID + "(" + CurrentUser.User_Name_CH + ")" + " 新增了报表：" + NewReport.r_Name,
                        CreateUser = CurrentUser.User_GID,
                        CreateDate = System.DateTime.Now,
                        Status = 0
                    };
                    DBHelper_Content.Add_Log(log);
                }
                #endregion
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        /// <summary>
        /// 更新报表信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Update_Report
        public JsonResult<OperationMessage> Update_Report()
        {
            OperationMessage ReturnMsg = new OperationMessage();
            v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;

            try
            {
                #region 获取表单数据

                t_Report TargetReport = new t_Report();
                TargetReport.r_ID = int.Parse(HttpContext.Current.Request["input_reportID"].ToString());
                TargetReport.CreateUser = HttpContext.Current.Request["input_CreatUser"].ToString();
                TargetReport.r_GUID = HttpContext.Current.Request["input_reportGUID"].ToString();
                TargetReport.r_m_DeptID = int.Parse(HttpContext.Current.Request["input_deptID"].ToString());
                TargetReport.r_m_ID_Category = int.Parse(HttpContext.Current.Request["select_Category"].ToString());
                TargetReport.r_m_ID_SubCategory = int.Parse(HttpContext.Current.Request["select_SubCategory"].ToString());
                TargetReport.r_m_ID_Frequency = int.Parse(HttpContext.Current.Request["select_UpdateFrequency"].ToString());
                TargetReport.r_Name = HttpContext.Current.Request["input_reportName"].ToString();
                TargetReport.r_Description = string.IsNullOrWhiteSpace(HttpContext.Current.Request["input_description"].ToString()) ? "N/A" : HttpContext.Current.Request["input_description"].ToString();
                TargetReport.r_Owner = HttpContext.Current.Request["select_Owner"].ToString();
                TargetReport.r_CROwner = HttpContext.Current.Request["select_CROwner"].ToString();
                TargetReport.r_AccessOwner = HttpContext.Current.Request["select_AccessOwner"].ToString();
                TargetReport.r_Admin = HttpContext.Current.Request["select_Admin"].ToString();
                TargetReport.r_Linkage = HttpContext.Current.Request["input_linkage"].ToString();
                TargetReport.r_Status = int.Parse(HttpContext.Current.Request["input_enabled"].ToString());
                TargetReport.r_AccessLevel = 0;
                TargetReport.r_Sort = int.Parse(HttpContext.Current.Request["input_sort"].ToString());
                TargetReport.r_APPIDS = HttpContext.Current.Request["input_AppIds"].ToString();
                TargetReport.UpdateDate = DateTime.Now;
                TargetReport.UpdateUser = CurrentUser.User_GID;
                string FilePath = HttpContext.Current.Request["input_picPath"].ToString();

                if (string.IsNullOrEmpty(FilePath)) FilePath = "N/A";

                MoveStore store = new MoveStore(FilePath);
                store.Pre = AppConfig.PreURL;
                store.Bucket = "Img/ReportImg";
                TargetReport.r_PicPath = FilePath;
                #endregion

                #region 提交数据
                var oss = new OSSImage();
                var oldReport = DBHelper_Content.Get_Report_ByReportID(TargetReport.r_ID);
                var oldReportPic = oldReport.r_PicPath;
                if (string.IsNullOrEmpty(oldReportPic)) oldReportPic = "N/A";

                var isSamePic = FilePath.Equals(oldReportPic, StringComparison.OrdinalIgnoreCase);
                //图片有变化
                if (!isSamePic)
                {
                    var canMove = oss.CanMove(store);
                    TargetReport.r_PicPath = canMove.Success ? canMove.VirtualPath : "N/A";
                }

                using (var scope = new TransactionScope())
                {
                    ReturnMsg.OpResult = DBHelper_Content.Update_Report(TargetReport);
                    if (!string.IsNullOrWhiteSpace(TargetReport.r_APPIDS))
                    {
                        DBHelper_Content.UpdateRemove(TargetReport.r_APPIDS, TargetReport.r_Linkage);
                    }
                    if (ReturnMsg.OpResult)
                    {
                        if (!isSamePic)
                        {
                            var ossRes = oss.Move(store);
                            if (ossRes.Success) scope.Complete();
                        }
                        else scope.Complete();

                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Modification_Successed");
                    }
                    else
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Modification_Failed");

                    t_OprationLog log = new t_OprationLog
                    {
                        Type = "更新报表",
                        Text = CurrentUser.User_GID + "(" + CurrentUser.User_Name_CH + ")" + " 更新了报表：" + TargetReport.r_Name,
                        CreateUser = CurrentUser.User_GID,
                        CreateDate = System.DateTime.Now,
                        Status = 0
                    };
                    DBHelper_Content.Add_Log(log);

                }
                #endregion
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        /// <summary>
        /// 点击访问时更新点击次数
        /// </summary>
        /// <param name="r_ID">报表ID</param>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Update_ClickStatistics
        public JsonResult<OperationMessage> Update_ClickStatistics([FromBody] int r_ID)
        {
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;

                int result = DBHelper_Content.Update_ClickStatistics(r_ID, CurrentUser.User_GID);
                ReturnMsg.OpResult = true;
                ReturnMsg.OpMsg = result.ToString();

                t_Report rmodel = DBHelper_Content.Get_Report_ByReportID(r_ID);

                //t_OprationLog log = new t_OprationLog
                //{
                //    Type = "查看报表",
                //    Text = CurrentUser.User_GID + "(" + CurrentUser.User_Name_CH + ")" + " 查看了报表：" + rmodel.r_Name,
                //    CreateUser = CurrentUser.User_GID,
                //    CreateDate = System.DateTime.Now,
                //    Status = 0
                //};
                //DBHelper_Content.Add_Log(log);
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        /// <summary>
        /// 报告报表问题
        /// </summary>
        /// <param name="r_ID">报表ID</param>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Report_Issues
        public JsonResult<OperationMessage> Report_Issues([FromBody] string pageInfo)
        {
            // [FromBody]int r_ID
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                #region 获取报表相关信息
                string[] infolist = pageInfo.Split('*');
                int r_ID = int.Parse(infolist[0]);
                string reason = infolist[1];
                v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;

                t_Report TargetReport = DBHelper_Content.Get_Report_ByReportID(r_ID);
                v_User ReportOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_Owner);
                v_User ReportAccessOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_AccessOwner);
                v_User reportAdmin = DBHelper_User.Get_User_ByGID(TargetReport.r_Admin);
                v_User reportCrOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_CROwner);
                //v_User AccessApprover = DBHelper_User.Get_User_ByGID(ReportAccessOwner.LineManager_GID);
                t_ReportIssue issue = new t_ReportIssue();

                issue.Issues = RegexHelper.RemoveXSS(reason);
                issue.r_Id = r_ID;
                issue.ReportTitle = TargetReport.r_Name;
                issue.AdminOwnerName = reportAdmin.User_Name_EN;
                issue.AdminOwner = reportAdmin.User_GID;
                issue.Createtime = DateTime.Now;
                issue.CreateUser = CurrentUser.User_GID;
                issue.CreateUserName = CurrentUser.User_Name_EN;
                issue.Groups = (reportAdmin.Dept_FullName ?? string.Empty).IndexOf(" SCM 1") >= 0 ? (short)IssueGroup.SCM : (short)IssueGroup.OTHER;
                //1=待处理，2=已处理
                issue.State = (short)IssueState.Panding;


                var res = DBHelper_Content.Add_ReportIssue(issue);
                if (res.OpResult == false)
                {
                    ReturnMsg.OpResult = false;
                    ReturnMsg.OpMsg = "提交失败,字数超过限制";
                }
                else
                {

                    MailAccessContent MailAccess = new MailAccessContent();

                    MailAccess.Template_Name = "Report_Errors";

                    MailAccess.Report_Name = TargetReport.r_Name;
                    MailAccess.Report_Linkage = TargetReport.r_Linkage;

                    MailAccess.Applicant_Name = CurrentUser.User_Name_EN;
                    MailAccess.Applicant_GID = CurrentUser.User_GID;
                    MailAccess.Applicant_Email = CurrentUser.User_Email;
                    MailAccess.Applicant_JobTitle = CurrentUser.Job_Function;

                    MailAccess.ReportOwner_Name = ReportOwner.User_Name_EN;
                    MailAccess.ReportOwner_Name_CH = ReportOwner.User_Name_CH;
                    MailAccess.ReportOwner_Email = ReportOwner.User_Email;
                    // Admin
                    MailAccess.ReportAdmin_Email = reportAdmin.User_Email;
                    MailAccess.ReportAdmin_Name_CH = reportAdmin.User_Name_CH;
                    MailAccess.ReportAdmin_Name_EN = reportAdmin.User_Name_EN;
                    // CrOwner
                    MailAccess.ReportCrOwner_Email = reportCrOwner.User_Email;
                    MailAccess.ReportCrOwner_Name_CH = reportCrOwner.User_Name_CH;
                    MailAccess.ReportCrOwner_Name_EN = reportCrOwner.User_Name_EN;

                    MailAccess.AccessOwner_GID = ReportAccessOwner.User_GID;
                    MailAccess.AccessOwner_Name = ReportAccessOwner.User_Name_EN;
                    MailAccess.AccessOwner_Email = ReportAccessOwner.User_Email;

                    //if (AccessApprover != null)
                    //{
                    //    MailAccess.AccessApprover_GID = AccessApprover.User_GID;
                    //    MailAccess.AccessApprover_Name_CH = AccessApprover.User_Name_CH;
                    //    MailAccess.AccessApprover_Name_EN = AccessApprover.User_Name_EN;
                    //    MailAccess.AccessApprover_Email = AccessApprover.User_Email;
                    //}
                    MailAccess.AccessApprover_GID = "Z00421WY";
                    MailAccess.AccessApprover_Name_CH = "孙朝明";
                    MailAccess.AccessApprover_Name_EN = "Sun, Chao Ming";
                    MailAccess.AccessApprover_Email = "chaoming.sun@siemens.com";

                    if (!string.IsNullOrWhiteSpace(reason))
                        MailAccess.AccessReason = reason;
                    else
                        MailAccess.AccessReason = "User don't input Errors";

                    #endregion

                    #region 邮件图片

                    // 添加邮件头图片
                    //List<string> EmailPicList = new List<string>();
                    //string preURL = AppConfig.PreURL;
                    //EmailPicList.Add(HttpContext.Current.Server.MapPath(preURL + "/Img/Email/EmailLogo.gif"));
                    //EmailPicList.Add(HttpContext.Current.Server.MapPath(preURL + "/Img/Email/EmailHead.jpg"));

                    #endregion

                    // 发送邮件
                    MailHelper.SendMail_GetReportAccess_SingleNew(MailAccess);

                    t_OprationLog log = new t_OprationLog
                    {
                        Type = "报表报错",
                        Text = CurrentUser.User_GID + "(" + CurrentUser.User_Name_CH + ")" + " 向报表：" + TargetReport.r_Name + "报了一个错误",
                        CreateUser = CurrentUser.User_GID,
                        CreateDate = System.DateTime.Now,
                        Status = 0
                    };
                    DBHelper_Content.Add_Log(log);

                    ReturnMsg.OpResult = true;
                    ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_EmailSent");
                }
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }


        /// <summary>
        /// 申请单个报表权限
        /// </summary>
        /// <param name="r_ID">报表ID</param>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Get_ReportAcess_Single
        public JsonResult<OperationMessage> Get_ReportAcess_Single([FromBody] string pageInfo)
        {
            // [FromBody]int r_ID
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                #region 获取报表相关信息
                string[] infolist = pageInfo.Split('*');
                int r_ID = int.Parse(infolist[0]);
                string reason = infolist[1];
                string appcontent = reason;
                string uemail = "";
                if (infolist.Count() > 2)
                {
                    string ugid = infolist[2];
                    uemail = infolist[3];
                    appcontent = "<br/><br/> UserGid :" + ugid + "<br/><br/> UserEmail :" + uemail + "<br/><br/> Application Content :" + reason;
                }

                t_Report TargetReport = DBHelper_Content.Get_Report_ByReportID(r_ID);
                v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;
                v_User ReportOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_Owner);
                v_User ReportAccessOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_AccessOwner);
                //v_User AccessApprover = DBHelper_User.Get_User_ByGID(ReportAccessOwner.LineManager_GID);
                MailAccessContent MailAccess = new MailAccessContent();
                DBHelper_Content.RequestSenseAuthor(CurrentUser.User_GID, CurrentUser.User_GID, CurrentUser.User_GID);

                MailAccess.Template_Name = "Report_GetAccess_Single";
                MailAccess.Report_Name = TargetReport.r_Name;
                MailAccess.Report_Linkage = TargetReport.r_Linkage;
                MailAccess.Applicant_Name = CurrentUser.User_Name_EN;
                MailAccess.Applicant_GID = CurrentUser.User_GID;
                if (!string.IsNullOrEmpty(uemail))
                {
                    MailAccess.Applicant_Email = uemail;
                }
                else
                {
                    MailAccess.Applicant_Email = CurrentUser.User_Email;
                }
                MailAccess.Applicant_JobTitle = CurrentUser.Job_Function;
                MailAccess.ReportOwner_Name = ReportOwner.User_Name_EN;
                MailAccess.AccessOwner_GID = ReportAccessOwner.User_GID;
                MailAccess.AccessOwner_Name = ReportAccessOwner.User_Name_EN;
                MailAccess.AccessOwner_Email = ReportAccessOwner.User_Email;

                CompareInfo Compare = CultureInfo.InvariantCulture.CompareInfo;
                // TODO temp update accessApprover

                MailAccess.AccessApprover_GID = "Z00421WY";
                MailAccess.AccessApprover_Name_CH = "孙朝明";
                MailAccess.AccessApprover_Name_EN = "Sun, Chao Ming";
                MailAccess.AccessApprover_Email = "chaoming.sun@siemens.com";

                if (!string.IsNullOrWhiteSpace(appcontent))
                    MailAccess.AccessReason = appcontent;
                else
                    MailAccess.AccessReason = "User don't input rason";

                #endregion

                #region 邮件图片
                //string preURL = AppConfig.PreURL;
                //// 添加邮件头图片
                //List<string> EmailPicList = new List<string>();
                //EmailPicList.Add(HttpContext.Current.Server.MapPath(preURL + "/Img/Email/EmailLogo.gif"));
                //EmailPicList.Add(HttpContext.Current.Server.MapPath(preURL + "/Img/Email/EmailHead.jpg"));

                #endregion
                MailHelper.SendMail_GetReportAccess_SingleNew(MailAccess);

                t_OprationLog log = new t_OprationLog
                {
                    Type = "报表权限申请",
                    Text = CurrentUser.User_GID + "(" + CurrentUser.User_Name_CH + ")" + "申请了报表“" + TargetReport.r_Name + "”的访问权限,审批人:" + MailAccess.AccessApprover_Name_CH,
                    CreateUser = CurrentUser.User_GID,
                    CreateDate = System.DateTime.Now,
                    Status = 0
                };
                DBHelper_Content.Add_Log(log);

                ReturnMsg.OpResult = true;
                ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_EmailSent");
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }


        /// <summary>
        ///  BusinessOverview报表申请权限
        /// </summary>
        /// <param name="pageInfo">页面参数对象</param>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Get_ReportAcess_Single
        public JsonResult<OperationMessage> Get_Single_Access([FromBody] string pageInfo)
        {
            // [FromBody]int r_ID
            OperationMessage ReturnMsg = new OperationMessage();

            #region 获取报表相关信息
            string[] infolist = pageInfo.Split('*');
            string r_name = infolist[0];
            string reason = infolist[1];
            string r_link = infolist[2];
            string appcontent = reason;
            if (infolist.Count() > 3)
            {
                string ugid = infolist[3];
                string uemail = infolist[4];
                appcontent = "<br/><br/> UserGid :" + ugid + "<br/><br/> UserEmail :" + uemail + "<br/><br/> Application Content :" + reason;
            }
            t_Report TargetReport = DBHelper_Content.Get_Report_ByReportNameAndLinkage(r_name, r_link);

            if (TargetReport != null)
            {
                try
                {
                    v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;
                    v_User ReportOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_Owner);
                    v_User ReportAccessOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_AccessOwner);
                    //v_User AccessApprover = DBHelper_User.Get_User_ByGID(ReportAccessOwner.LineManager_GID);
                    MailAccessContent MailAccess = new MailAccessContent();
                    DBHelper_Content.RequestSenseAuthor(CurrentUser.User_GID, CurrentUser.User_GID, CurrentUser.User_GID);

                    MailAccess.Template_Name = "Report_GetAccess_Single";
                    MailAccess.Report_Name = TargetReport.r_Name;
                    MailAccess.Report_Linkage = TargetReport.r_Linkage;
                    MailAccess.Applicant_Name = CurrentUser.User_Name_EN;
                    MailAccess.Applicant_GID = CurrentUser.User_GID;
                    MailAccess.Applicant_Email = CurrentUser.User_Email;
                    MailAccess.Applicant_JobTitle = CurrentUser.Job_Function;
                    MailAccess.ReportOwner_Name = ReportOwner.User_Name_EN;
                    MailAccess.AccessOwner_GID = ReportAccessOwner.User_GID;
                    MailAccess.AccessOwner_Name = ReportAccessOwner.User_Name_EN;
                    MailAccess.AccessOwner_Email = ReportAccessOwner.User_Email;
                    CompareInfo Compare = CultureInfo.InvariantCulture.CompareInfo;
                    // TODO temp update accessApprover
                    //if (TargetReport.r_m_ID_Category == 18 || TargetReport.r_m_ID_Category == 19)
                    //{
                    //    //MailAccess.AccessApprover_GID = "Z003WB3T";
                    //    //MailAccess.AccessApprover_Name_CH = "陈强";
                    //    //MailAccess.AccessApprover_Name_EN = "Chen, Qiang";
                    //    //MailAccess.AccessApprover_Email = "chen.qiang@siemens.com";

                    //    MailAccess.AccessApprover_GID = "Z003S2XD";
                    //    MailAccess.AccessApprover_Name_CH = "峗沁怡";
                    //    MailAccess.AccessApprover_Name_EN = "Wei, Qin Yi";
                    //    MailAccess.AccessApprover_Email = "qinyi.wei@siemens.com";
                    //}
                    ////else if (Compare.IndexOf(ReportAccessOwner.Dept_FullName, " SCM 1", CompareOptions.IgnoreCase) == -1)
                    ////{
                    ////    MailAccess.AccessApprover_GID = AccessApprover.User_GID;
                    ////    MailAccess.AccessApprover_Name_CH = AccessApprover.User_Name_CH;
                    ////    MailAccess.AccessApprover_Name_EN = AccessApprover.User_Name_EN;
                    ////    MailAccess.AccessApprover_Email = AccessApprover.User_Email;
                    ////}
                    //else
                    //{
                    //    //MailAccess.AccessApprover_GID = "Z0037NMB";
                    //    //MailAccess.AccessApprover_Name_CH = "王胜兰";
                    //    //MailAccess.AccessApprover_Name_EN = "Wang, Sheng Lan";
                    //    //MailAccess.AccessApprover_Email = "SHENGLAN.WANG@SIEMENS.COM";
                    //    MailAccess.AccessApprover_GID = AccessApprover.User_GID;
                    //    MailAccess.AccessApprover_Name_CH = AccessApprover.User_Name_CH;
                    //    MailAccess.AccessApprover_Name_EN = AccessApprover.User_Name_EN;
                    //    MailAccess.AccessApprover_Email = AccessApprover.User_Email;
                    //}
                    MailAccess.AccessApprover_GID = "Z00421WY";
                    MailAccess.AccessApprover_Name_CH = "孙朝明";
                    MailAccess.AccessApprover_Name_EN = "Sun, Chao Ming";
                    MailAccess.AccessApprover_Email = "chaoming.sun@siemens.com";

                    if (!string.IsNullOrWhiteSpace(appcontent))
                        MailAccess.AccessReason = appcontent;
                    else
                        MailAccess.AccessReason = "User don't input rason";

                    #endregion

                    #region 邮件图片
                    //string preURL = AppConfig.PreURL;
                    //// 添加邮件头图片
                    //List<string> EmailPicList = new List<string>();
                    //EmailPicList.Add(HttpContext.Current.Server.MapPath(preURL + "/Img/Email/EmailLogo.gif"));
                    //EmailPicList.Add(HttpContext.Current.Server.MapPath(preURL + "/Img/Email/EmailHead.jpg"));

                    #endregion
                    // 发送邮件
                    MailHelper.SendMail_GetReportAccess_SingleNew(MailAccess);

                    t_OprationLog log = new t_OprationLog
                    {
                        Type = "报表权限申请",
                        Text = CurrentUser.User_GID + "(" + CurrentUser.User_Name_CH + ")" + "申请了报表“" + TargetReport.r_Name + "”的访问权限,审批人:" + MailAccess.AccessApprover_Name_CH,
                        CreateUser = CurrentUser.User_GID,
                        CreateDate = System.DateTime.Now,
                        Status = 0
                    };
                    DBHelper_Content.Add_Log(log);

                    ReturnMsg.OpResult = true;
                    ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_EmailSent");
                }
                catch (Exception ex)
                {
                    EventlogHelper.AddLog(ex.Message);
                    ReturnMsg.OpResult = false;
                    ReturnMsg.OpMsg = ex.Message;
                }
            }
            else
            {
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = "该报表未在系统中添加，请联系该报表Owner在系统中添加！";
            }
            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }



        /// <summary>
        /// 申请多个报表权限
        /// </summary>
        /// <param name="idList">报表ID列表</param>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Get_ReportAccess_Multiple
        public JsonResult<OperationMessage> Get_ReportAccess_Multiple([FromBody] string pageInfo)
        {
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                #region 获取报表相关信息

                string[] infolist = pageInfo.Split('*');
                string reason = infolist[1];

                //string[] temp_reportIDList = idList.Split(',');
                string[] temp_reportIDList = infolist[0].Split(',');
                List<int> r_ID_List = new List<int>();

                foreach (string Cur in temp_reportIDList)
                {
                    r_ID_List.Add(int.Parse(Cur));
                }

                List<t_Report> TempList = DBHelper_Content.Get_ReportList_ByReportIDList(r_ID_List);
                v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;
                List<MailAccessContent> MailList = new List<MailAccessContent>();

                for (int i = 0; i < TempList.Count; i++)
                {
                    t_Report TargetReport = TempList[i];
                    v_User ReportAccessOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_AccessOwner);
                    v_User ReportOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_Owner);

                    MailAccessContent MailAccess = new MailAccessContent();

                    MailAccess.Template_Name = "Report_GetAccess_Multiple";

                    MailAccess.Report_Name = TargetReport.r_Name;
                    MailAccess.Report_Linkage = TargetReport.r_Linkage;

                    MailAccess.Applicant_Name = CurrentUser.User_Name_EN;
                    MailAccess.Applicant_GID = CurrentUser.User_GID;
                    MailAccess.Applicant_Email = CurrentUser.User_Email;
                    MailAccess.Applicant_JobTitle = CurrentUser.Job_Function;

                    MailAccess.ReportOwner_Name = ReportOwner.User_Name_EN;
                    MailAccess.AccessOwner_GID = ReportAccessOwner.User_GID;
                    MailAccess.AccessOwner_Name = ReportAccessOwner.User_Name_EN;
                    MailAccess.AccessOwner_Email = ReportAccessOwner.User_Email;

                    MailAccess.AccessApprover_GID = "Z00421WY";
                    MailAccess.AccessApprover_Name_CH = "孙朝明";
                    MailAccess.AccessApprover_Name_EN = "Sun, Chao Ming";
                    MailAccess.AccessApprover_Email = "chaoming.sun@siemens.com";

                    if (!string.IsNullOrWhiteSpace(reason))
                        MailAccess.AccessReason = reason;
                    else
                        MailAccess.AccessReason = "User don't input rason";

                    MailList.Add(MailAccess);
                }

                #endregion


                // 按照 AccessApprover、AccessOwner、ReportName 进行排序
                MailList.Sort();
                List<MailAccessContent> Temp = new List<MailAccessContent>();
                string LastApprover = string.Empty;

                for (int i = 0; i < MailList.Count; i++)
                {
                    if (LastApprover.Equals(MailList[i].AccessApprover_GID))
                        Temp.Add(MailList[i]);
                    else
                    {
                        if (i == 0)
                            Temp.Add(MailList[i]);
                        else
                        {
                            MailHelper.SendMail_GetReportAccess_Multiple(Temp);
                            Temp = new List<MailAccessContent>();
                            Temp.Add(MailList[i]);
                        }
                    }

                    if (i == MailList.Count - 1)
                        MailHelper.SendMail_GetReportAccess_Multiple(Temp);

                    LastApprover = MailList[i].AccessApprover_GID;
                }

                ReturnMsg.OpResult = true;
                ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_EmailSent");
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        [HttpPost]
        // POST: api/ToolBox_Api/Get_ReportAccess_Multiple
        public JsonResult<OperationMessage> Get_Catagory_BySubCatagoryId([FromBody] int catagoryId)
        {
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;
                List<v_MenuList> Report_CategoryList = CacheConfig.Get<List<v_MenuList>>(CacheConfig.CacheType.Report_CategoryList);
                //o.ml_Main_DeptID == CurrentUser.Dept_ID &&
                Report_CategoryList = Report_CategoryList.Where(o => (o.ml_Main_AccessLevel == 0 && o.ml_Sub_AccessLevel == 0 && o.ml_Third_AccessLevel == 0 && o.ml_Sub_ID == catagoryId)).ToList();

                ReturnMsg.OpResult = true;
                ReturnMsg.MenuList = Report_CategoryList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        #endregion

        #region QuickLink 相关

        /// <summary>
        /// 添加新快速链接
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Add_QuickLink
        public JsonResult<OperationMessage> Add_QuickLink()
        {
            OperationMessage ReturnMsg = new OperationMessage();
            v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;

            try
            {
                #region 获取表单数据

                t_QuickLink NewQuickLink = new t_QuickLink();
                NewQuickLink.ql_GUID = HttpContext.Current.Request["input_quickLinkGUID"].ToString();
                NewQuickLink.ql_m_DeptID = int.Parse(HttpContext.Current.Request["input_deptID"].ToString());
                NewQuickLink.ql_m_ID = int.Parse(HttpContext.Current.Request["select_Type"].ToString());
                NewQuickLink.ql_Name = HttpContext.Current.Request["input_quickLinkName"].ToString();
                NewQuickLink.ql_Description = string.IsNullOrWhiteSpace(HttpContext.Current.Request["input_description"].ToString()) ? "N/A" : HttpContext.Current.Request["input_description"].ToString();
                NewQuickLink.ql_Linkage = HttpContext.Current.Request["input_linkage"].ToString();
                NewQuickLink.ql_Status = int.Parse(HttpContext.Current.Request["input_enabled"].ToString());
                NewQuickLink.ql_AccessLevel = 0;
                NewQuickLink.ql_Sort = int.Parse(HttpContext.Current.Request["input_sort"].ToString());
                string FilePath = HttpContext.Current.Request["input_picPath"].ToString();
                MoveStore store = new MoveStore(FilePath);
                store.Pre = AppConfig.PreURL;
                store.Bucket = "Img/QuickLinkImg";
                #endregion

                #region 提交数据

                var oss = new OSSImage();
                var canMove = oss.CanMove(store);
                using (var scope = new TransactionScope())
                {
                    NewQuickLink.ql_PicPath = canMove.Success ? canMove.VirtualPath : "N/A";

                    ReturnMsg.OpResult = DBHelper_Content.Add_QuickLink(NewQuickLink);

                    var flag = false;
                    if (ReturnMsg.OpResult)
                    {
                        if (canMove.Success)
                        {
                            var ossRes = oss.Move(store);
                            if (ossRes.Success)
                            {
                                scope.Complete();
                                flag = true;
                            }
                        }
                        else
                        {
                            scope.Complete();
                            flag = true;
                        }
                    }

                    if (flag)
                    {
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Creation_Successed");
                    }
                    else
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Creation_Failed");

                    t_OprationLog log = new t_OprationLog
                    {
                        Type = "新增QuickLink",
                        Text = CurrentUser.User_GID + "(" + CurrentUser.User_Name_CH + ")" + " 新增了一个QuickLink：" + NewQuickLink.ql_Name,
                        CreateUser = CurrentUser.User_GID,
                        CreateDate = System.DateTime.Now,
                        Status = 0
                    };
                    DBHelper_Content.Add_Log(log);
                }

                #endregion
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        /// <summary>
        /// 更新快速链接信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Update_QuickLink
        public JsonResult<OperationMessage> Update_QuickLink()
        {
            OperationMessage ReturnMsg = new OperationMessage();
            v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;

            try
            {
                #region 获取表单数据
                t_QuickLink TargetQuickLink = new t_QuickLink();
                TargetQuickLink.ql_ID = int.Parse(HttpContext.Current.Request["input_quickLinkID"].ToString());
                TargetQuickLink.ql_GUID = HttpContext.Current.Request["input_quickLinkGUID"].ToString();
                TargetQuickLink.ql_m_DeptID = int.Parse(HttpContext.Current.Request["input_deptID"].ToString());
                TargetQuickLink.ql_m_ID = int.Parse(HttpContext.Current.Request["select_Type"].ToString());
                TargetQuickLink.ql_Name = HttpContext.Current.Request["input_quickLinkName"].ToString();
                TargetQuickLink.ql_Description = string.IsNullOrWhiteSpace(HttpContext.Current.Request["input_description"].ToString()) ? "N/A" : HttpContext.Current.Request["input_description"].ToString();

                TargetQuickLink.ql_Linkage = HttpContext.Current.Request["input_linkage"].ToString();
                TargetQuickLink.ql_Status = int.Parse(HttpContext.Current.Request["input_enabled"].ToString());
                TargetQuickLink.ql_AccessLevel = 0;
                TargetQuickLink.ql_Sort = int.Parse(HttpContext.Current.Request["input_sort"].ToString());
                string FilePath = HttpContext.Current.Request["input_picPath"].ToString();
                if (string.IsNullOrEmpty(FilePath)) FilePath = "N/A";
                MoveStore store = new MoveStore(FilePath);
                store.Bucket = "Img/ReportImg";
                store.Pre = AppConfig.PreURL;
                TargetQuickLink.ql_PicPath = FilePath;
                #endregion

                #region 提交数据
                var oss = new OSSImage();
                var oldReport = DBHelper_Content.Get_QuickLink_ByQuickLinkID(TargetQuickLink.ql_ID);
                var oldReportPic = oldReport.ql_PicPath;

                if (string.IsNullOrEmpty(oldReportPic)) oldReportPic = "N/A";
                var isSamePic = FilePath.Equals(oldReportPic, StringComparison.OrdinalIgnoreCase);
                if (!isSamePic)
                {
                    var canMove = oss.CanMove(store);
                    TargetQuickLink.ql_PicPath = canMove.Success ? canMove.VirtualPath : "N/A";
                }
                using (var scope = new TransactionScope())
                {
                    ReturnMsg.OpResult = DBHelper_Content.Update_QuickLink(TargetQuickLink);

                    if (ReturnMsg.OpResult)
                    {
                        if (!isSamePic)
                        {
                            var ossRes = oss.Move(store);
                            if (ossRes.Success) scope.Complete();
                        }
                        else scope.Complete();
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Modification_Successed");
                    }
                    else
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Modification_Failed");

                    t_OprationLog log = new t_OprationLog
                    {
                        Type = "更新QuickLink",
                        Text = CurrentUser.User_GID + "(" + CurrentUser.User_Name_CH + ")" + " 更新了QuickLink：" + TargetQuickLink.ql_Name,
                        CreateUser = CurrentUser.User_GID,
                        CreateDate = System.DateTime.Now,
                        Status = 0
                    };
                    DBHelper_Content.Add_Log(log);
                }
                #endregion
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        #endregion

        #region Customization 相关

        /// <summary>
        /// 添加新自定义内容
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Add_Customization
        public JsonResult<OperationMessage> Add_Customization()
        {
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                #region 获取表单数据
                t_Customization NewCustomization = new t_Customization();
                NewCustomization.c_GUID = HttpContext.Current.Request["input_customizationGUID"].ToString();
                NewCustomization.c_m_DeptID = int.Parse(HttpContext.Current.Request["input_deptID"].ToString());

                // 判断 Category 是从选择还是从输入获取
                bool Category_IsSelect = bool.Parse(HttpContext.Current.Request["input_Category_IsSelect"].ToString());
                if (Category_IsSelect)
                    NewCustomization.c_Category = HttpContext.Current.Request["select_Category"].ToString().Trim();
                else
                    NewCustomization.c_Category = HttpContext.Current.Request["input_Category"].ToString().Trim();

                NewCustomization.c_Name = HttpContext.Current.Request["input_customizationName"].ToString();
                NewCustomization.c_Description = string.IsNullOrWhiteSpace(HttpContext.Current.Request["input_description"].ToString()) ? "N/A" : HttpContext.Current.Request["input_description"].ToString();
                NewCustomization.c_Owner = (HttpContext.Current.Session["CurrentUser"] as v_User).User_GID;


                string FilePath = HttpContext.Current.Request["input_picPath"].ToString();
                MoveStore store = new MoveStore(FilePath);
                store.Bucket = "Img/CustomizationImg";
                store.Pre = AppConfig.PreURL;

                NewCustomization.c_Linkage = HttpContext.Current.Request["input_linkage"].ToString();
                NewCustomization.c_Sort = int.Parse(HttpContext.Current.Request["input_sort"].ToString());

                #endregion

                #region 提交数据
                var oss = new OSSImage();
                var canMove = oss.CanMove(store);
                using (var scope = new TransactionScope())
                {
                    NewCustomization.c_PicPath = canMove.Success ? canMove.VirtualPath : "N/A";

                    ReturnMsg.OpResult = DBHelper_Content.Add_Customization(NewCustomization);

                    if (ReturnMsg.OpResult)
                    {
                        if (canMove.Success)
                        {
                            var ossRes = oss.Move(store);
                            if (ossRes.Success) scope.Complete();
                        }
                        else scope.Complete();

                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Creation_Successed");
                    }
                    else
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Creation_Failed");

                }
                #endregion
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        /// <summary>
        /// 更新自定义内容信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Update_Customization
        public JsonResult<OperationMessage> Update_Customization()
        {
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                #region 获取表单数据
                t_Customization TargetCustomization = new t_Customization();
                TargetCustomization.c_ID = int.Parse(HttpContext.Current.Request["input_customizationID"].ToString());
                TargetCustomization.c_GUID = HttpContext.Current.Request["input_customizationGUID"].ToString();
                TargetCustomization.c_m_DeptID = int.Parse(HttpContext.Current.Request["input_deptID"].ToString());

                // 判断 Category 是从选择还是从输入获取
                bool Category_IsSelect = bool.Parse(HttpContext.Current.Request["input_Category_IsSelect"].ToString());
                if (Category_IsSelect)
                    TargetCustomization.c_Category = HttpContext.Current.Request["select_Category"].ToString().Trim();
                else
                    TargetCustomization.c_Category = HttpContext.Current.Request["input_Category"].ToString().Trim();

                TargetCustomization.c_Name = HttpContext.Current.Request["input_customizationName"].ToString();
                TargetCustomization.c_Description = string.IsNullOrWhiteSpace(HttpContext.Current.Request["input_description"].ToString()) ? "N/A" : HttpContext.Current.Request["input_description"].ToString();
                TargetCustomization.c_Owner = (HttpContext.Current.Session["CurrentUser"] as v_User).User_GID;
                //TargetCustomization.c_Admin = HttpContext.Current.Request["select_Admin"].ToString();

                TargetCustomization.c_Linkage = HttpContext.Current.Request["input_linkage"].ToString();
                TargetCustomization.c_Sort = int.Parse(HttpContext.Current.Request["input_sort"].ToString());

                string FilePath = HttpContext.Current.Request["input_picPath"].ToString();
                if (string.IsNullOrEmpty(FilePath)) FilePath = "N/A";
                MoveStore store = new MoveStore(FilePath);
                store.Bucket = "Img/CustomizationImg";
                store.Pre = AppConfig.PreURL;
                TargetCustomization.c_PicPath = FilePath;

                #endregion

                #region 提交数据
                var oss = new OSSImage();
                var oldCustomization = DBHelper_Content.Get_Customization_ByCustomizationID(TargetCustomization.c_ID);
                var oldReportPic = oldCustomization.c_PicPath;
                if (string.IsNullOrEmpty(oldReportPic)) oldReportPic = "N/A";
                var isSamePic = FilePath.Equals(oldReportPic, StringComparison.OrdinalIgnoreCase);
                if (!isSamePic)
                {
                    var canMove = oss.CanMove(store);
                    TargetCustomization.c_PicPath = canMove.Success ? canMove.VirtualPath : "N/A";
                }
                using (var scope = new TransactionScope())
                {
                    ReturnMsg.OpResult = DBHelper_Content.Update_Customization(TargetCustomization);

                    if (ReturnMsg.OpResult)
                    {
                        if (!isSamePic)
                        {
                            var ossRes = oss.Move(store);
                            if (ossRes.Success) scope.Complete();
                        }
                        else scope.Complete();

                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Modification_Successed");
                    }
                    else
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Modification_Failed");
                }
                #endregion
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        #endregion

        #region UserRole 相关

        /// <summary>
        /// 添加新的人员角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Add_UserRole
        public JsonResult<OperationMessage> Add_UserRole()
        {
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                #region 获取表单数据

                t_UserRole NewUserRole = new t_UserRole();
                NewUserRole.ur_e_Dept_ID = int.Parse(HttpContext.Current.Request["select_Dept"].ToString());
                NewUserRole.ur_User_GID = HttpContext.Current.Request["select_User"].ToString();
                NewUserRole.ur_a_ID = int.Parse(HttpContext.Current.Request["select_Role"].ToString());

                #endregion

                #region 提交数据

                ReturnMsg = DBHelper_User.Add_UserRole(NewUserRole);

                if (ReturnMsg.OpResult)
                {
                    ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Creation_Successed");
                    CacheConfig.Set(CacheConfig.CacheType.UserRoleList);
                }
                else
                {
                    if (ReturnMsg.EXT01.Equals("Existed"))
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_UserRole_Existed");
                    else
                        ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Creation_Failed");
                }

                #endregion
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        /// <summary>
        /// 更新人员角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Update_UserRole
        public JsonResult<OperationMessage> Update_UserRole()
        {
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                #region 获取表单数据

                t_UserRole TargetUserRole = new t_UserRole();
                TargetUserRole.ur_ID = int.Parse(HttpContext.Current.Request["input_userRoleID"].ToString());
                TargetUserRole.ur_e_Dept_ID = int.Parse(HttpContext.Current.Request["input_Dept"].ToString());
                TargetUserRole.ur_User_GID = HttpContext.Current.Request["input_User"].ToString();
                TargetUserRole.ur_a_ID = int.Parse(HttpContext.Current.Request["select_Role"].ToString());

                #endregion

                #region 提交数据

                ReturnMsg.OpResult = DBHelper_User.Update_UserRole(TargetUserRole);

                if (ReturnMsg.OpResult)
                {
                    ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Modification_Successed");
                    CacheConfig.Set(CacheConfig.CacheType.UserRoleList);
                }
                else
                    ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Modification_Failed");

                #endregion
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        /// <summary>
        /// 删除人员角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Delete_UserRole
        public JsonResult<OperationMessage> Delete_UserRole([FromBody] int ur_ID)
        {
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                ReturnMsg.OpResult = DBHelper_User.Delete_UserRole_ByID(ur_ID);

                if (ReturnMsg.OpResult)
                {
                    ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Deletion_Successed");
                    CacheConfig.Set(CacheConfig.CacheType.UserRoleList);
                }
                else
                    ReturnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Deletion_Failed");
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                ReturnMsg.OpResult = false;
                ReturnMsg.OpMsg = ex.Message;
            }

            JsonResult<OperationMessage> ReturnMsg_Json = Json<OperationMessage>(ReturnMsg);
            return ReturnMsg_Json;
        }

        [HttpPost]
        public JsonResult<bool> CheckUser()
        {
            v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;
            bool user = true;

            if (CurrentUser.User_Name_CH == "Guest")
            {
                user = false;
            }
            JsonResult<bool> ReturnMsg_Json = Json<bool>(user);

            return ReturnMsg_Json;

        }

        #endregion

        #region Process 相关

        [HttpPost]
        public JsonResult<OperationMessage> Delete_ProcessLinkage_ByID([FromBody] int pl_ID)
        {
            OperationMessage message = new OperationMessage();
            try
            {
                t_ProcessLinkage targetLinkage = DBHelper_Content.Get_ProcessLinkage_ByID(pl_ID);
                message.OpResult = DBHelper_Content.Delete_ProcessLinkage(targetLinkage);
                if (message.OpResult)
                {
                    message.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Deletion_Successed");
                }
                else
                {
                    message.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Deletion_Failed");
                }
            }
            catch (Exception exception)
            {
                EventlogHelper.AddLog(exception.Message);
                message.OpResult = false;
                message.OpMsg = exception.Message;
            }
            return base.Json<OperationMessage>(message);
        }


        [HttpPost]
        public JsonResult<OperationMessage> Add_ProcessLinkage()
        {
            OperationMessage message = new OperationMessage();
            try
            {
                t_ProcessLinkage newProcessLinkage = new t_ProcessLinkage
                {
                    pl_pn_ID = new int?(int.Parse(HttpContext.Current.Request["input_nodeID"].ToString())),
                    pl_Type = new int?(int.Parse(HttpContext.Current.Request["input_processLinkageType"].ToString())),
                    pl_Name = HttpContext.Current.Request["input_Name"].ToString(),
                    pl_Linkage = HttpContext.Current.Request["input_linkage"].ToString(),
                    pl_Sort = new int?(int.Parse(HttpContext.Current.Request["input_sort"].ToString()))
                };
                message.OpResult = DBHelper_Content.Add_ProcessLinkage(newProcessLinkage);
                if (message.OpResult)
                {
                    message.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Creation_Successed");
                }
                else
                {
                    message.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Creation_Failed");
                }
            }
            catch (Exception exception)
            {
                EventlogHelper.AddLog(exception.Message);
                message.OpResult = false;
                message.OpMsg = exception.Message;
            }
            return base.Json<OperationMessage>(message);
        }

        [HttpPost]
        public JsonResult<OperationMessage> Update_ProcessLinkage()
        {
            OperationMessage message = new OperationMessage();
            try
            {
                t_ProcessLinkage targetProcessLinkage = new t_ProcessLinkage
                {
                    pl_ID = int.Parse(HttpContext.Current.Request["input_processLinkageID"].ToString()),
                    pl_pn_ID = new int?(int.Parse(HttpContext.Current.Request["input_nodeID"].ToString())),
                    pl_Type = new int?(int.Parse(HttpContext.Current.Request["input_processLinkageType"].ToString())),
                    pl_Name = HttpContext.Current.Request["input_Name"].ToString(),
                    pl_Linkage = HttpContext.Current.Request["input_linkage"].ToString(),
                    pl_Sort = new int?(int.Parse(HttpContext.Current.Request["input_sort"].ToString()))
                };
                message.OpResult = DBHelper_Content.Update_ProcessLinkage(targetProcessLinkage);
                if (message.OpResult)
                {
                    message.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Modification_Successed");
                }
                else
                {
                    message.OpMsg = LanguageHelper.InnerLang(HttpContext.Current, "txt_Modification_Failed");
                }
            }
            catch (Exception exception)
            {
                EventlogHelper.AddLog(exception.Message);
                message.OpResult = false;
                message.OpMsg = exception.Message;
            }
            return base.Json<OperationMessage>(message);
        }


        [HttpPost]
        public JsonResult<t_Attachment> Get_Attachment_ByID([FromBody] int attachment_ID)
        {
            try
            {
                t_Attachment attachment = DBHelper_Content.Get_Attachment_ByID(attachment_ID);
                return base.Json<t_Attachment>(attachment);
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                return null;
            }
        }

        [HttpPost]
        public JsonResult<List<object>> Get_ProcessLinkage_ByNodeID([FromBody] int node_ID)
        {
            try
            {
                v_ProcessNode node = (from o in CacheConfig.Get<List<v_ProcessNode>>(CacheConfig.CacheType.ProcessNodeList)
                                      where o.pn_ID == node_ID
                                      select o).FirstOrDefault<v_ProcessNode>();
                //List<t_ProcessLinkage> list = DBHelper_Content.Get_ProcessLinkage_ByNodeID(node_ID);
                v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;
                string gid = CurrentUser.User_GID;
                List<ProcessLinkageModel> list = DBHelper_Content.Get_ProcessLinkage_ByNodeID(node_ID, gid);

                List<object> list2 = new List<object> {
                    node,
                    list
                };
                return base.Json<List<object>>(list2);
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                return null;
            }
        }



        /// <summary>
        /// 根据菜单ID获取节点
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Get_Process_Nodes_ByMenuID
        public JsonResult<List<t_ProcessNode>> Get_Process_Nodes_ByMenuID([FromBody] int menu_ID)
        {
            try
            {
                List<t_ProcessNode> NodeList = DBHelper_Content.Get_ProcessNodes_ByMenuID(menu_ID);

                JsonResult<List<t_ProcessNode>> Result = Json<List<t_ProcessNode>>(NodeList);
                return Result;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 根据Department ID获取节点
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Get_ProcessTopology_ByDeptID
        public JsonResult<List<Object>> Get_ProcessTopology_ByDeptID()
        {
            try
            {
                int dept_ID = Int32.Parse(HttpContext.Current.Request["dept_ID"].ToString());
                int menu_ID = Int32.Parse(HttpContext.Current.Request["menu_ID"].ToString());
                // && o.pn_m_ID == menu_ID

                //List<v_ProcessNode> list = CacheConfig.Get<List<v_ProcessNode>>(CacheConfig.CacheType.ProcessNodeList).Where(o => o.pn_m_DeptID == dept_ID).ToList();

                //List<t_ProcessConnection> list2 = CacheConfig.Get<List<t_ProcessConnection>>(CacheConfig.CacheType.ProcessConnectionList).Where(o => o.pc_m_DeptID == dept_ID).ToList();

                List<v_ProcessNode> list = CacheConfig.Get<List<v_ProcessNode>>(CacheConfig.CacheType.ProcessNodeList).Where(o => o.pn_m_DeptID != null).ToList();

                List<t_ProcessConnection> list2 = CacheConfig.Get<List<t_ProcessConnection>>(CacheConfig.CacheType.ProcessConnectionList).Where(o => o.pc_m_DeptID != null).ToList();


                List<object> list3 = new List<object> {
                    list,
                    list2
                };

                return base.Json<List<object>>(list3);

            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 根据菜单ID获取链接
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // POST: api/ToolBox_Api/Get_Process_Connections_ByMenuID
        public JsonResult<List<t_ProcessConnection>> Get_Process_Connections_ByMenuID([FromBody] int menu_ID)
        {
            try
            {
                List<t_ProcessConnection> ConnectionList = DBHelper_Content.Get_Process_Connections_ByMenuID(menu_ID);

                JsonResult<List<t_ProcessConnection>> Result = Json<List<t_ProcessConnection>>(ConnectionList);
                return Result;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 根据节点ID获取相关文件
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        // POST: api/ToolBox_Api/Get_Process_Attachments_ByNodeID
        //public JsonResult<List<t_ProcessAttachment>> Get_Process_Attachments_ByNodeID([FromBody]int node_ID)
        //{
        //    try
        //    {
        //        List<t_ProcessAttachment> AttachmentList = DBHelper_Content.Get_Process_Attachments_ByNodeID(node_ID);

        //        JsonResult<List<t_ProcessAttachment>> Result = Json<List<t_ProcessAttachment>>(AttachmentList);
        //        return Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        EventlogHelper.AddLog(ex.Message);
        //        return null;
        //    }
        //}

        #endregion

        #region Search
        public JsonResult<string[]> GetSearchList(string searchStr)
        {
            try
            {
                string[] searchModel = DBHelper_Content.GetNameList(searchStr);
                Array.Sort(searchModel);
                return base.Json<string[]>(searchModel);
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                return null;
            }

        }

        [HttpGet]
        public JsonResult<AutoCompleteModel> AutoSearchList(string keywords)
        {
            try
            {
                List<SearchModels> searchModel = DBHelper_Content.AutoNameList(keywords);
                var s = Newtonsoft.Json.JsonConvert.SerializeObject(searchModel);

                var model = new AutoCompleteModel
                {
                    type = "success",
                    code = 0,
                    content = searchModel
                };

                var b = Json(model);
                return Json(model);
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                return null;
            }

        }


        #endregion

        #region 用户名字匹配
        [HttpGet]
        public JsonResult<object> GetUserByName(string keywords)
        {
            try
            {
                List<v_User> res = DBHelper_Content.ContainsUsers(keywords);

                object model = new
                {
                    type = "success",
                    code = 0,
                    content = res.Select(a => new
                    {
                        gid = a.User_GID,
                        name = a.User_Name_EN
                    }).ToList()
                };

                return Json(model);
            }
            catch (Exception exception1)
            {
                EventlogHelper.AddLog(exception1.Message);
                return null;
            }

        }
        #endregion
    }
}
