using SEWC_ToolBox.Controllers;
using SEWC_ToolBox.DAL.DBHelpers;
using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox.DAL.EFs.EnumCollect;
using SEWC_ToolBox.DAL.SecondModels;
using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using SEWC_ToolBox.Utilities.Helpers;
using SEWC_ToolBox.Utilities.Mail;
using SEWC_ToolBox.Languages;
using System.Configuration;
using SEWC_ToolBox.Utilities.Export.Entity;
using SEWC_ToolBox.Utilities.Export;
using SEWC_ToolBox.Utilities;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using static SEWC_NetDevLib.SEWC_NetLibExtend.NetHelper;
using System.Net.Security;

namespace SEWC_ToolBox_Project.Controllers
{
    [Authorize]
    public class IssueController : ToolBoxBaseController
    {

        protected override string OverrideContollerName => "SSML";

        /// <summary>
        /// 查询Issue列表集合
        /// </summary>
        /// <param name="reportTitle"></param>
        /// <param name="questionName"></param>
        /// <param name="state"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Index(string reportTitle = "", string createUser = "", short state = 0, int limit = 12, int page = 1)
        {
            ViewBag.Main_ActionName = "Management";
            if (!Request.IsAjaxRequest())
            {
                return View();
            }

            var dbResult = DBHelper_Content.GetIssuePageList(page, limit, reportTitle, createUser, state);

            var json = new JsonResult();

            json.Data = new
            {
                data = dbResult.ToArray(),
                code = 0,
                msg = "",
                count = dbResult.Total
            };
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //ViewBag.IsCommonUser= AppConfig
            return json;
        }

        ///// <summary>
        ///// 撤销接口
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult Revoke(int id)
        //{
        //    OperationMessage returnMsg = new OperationMessage();
        //    var model = DBHelper_Content.IssueFirst(id);

        //    v_User CurrentUser = HttpContext.Session["CurrentUser"] as v_User;

        //    if (CurrentUser.User_GID == model.CreateUser)
        //    {
        //        if (model.State == (short)IssueState.Revoke)
        //        {
        //            returnMsg.OpResult = true;
        //            returnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext, "txt_IssueRevokeSuccess");
        //        }
        //        else if (model.State == (short)IssueState.Solved)
        //        {
        //            returnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext, "txt_IssueRevokeFaild_C1");
        //            //"问题已经处理，不能进行撤销";
        //        }
        //        else if (model.State == (short)IssueState.Panding)
        //        {
        //            returnMsg.OpResult = DBHelper_Content.IssueSetState(model.id, (short)IssueState.Revoke);
        //            if (returnMsg.OpResult)
        //            {
        //                returnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext, "txt_IssueRevokeSuccess");
        //            }
        //            else
        //            {
        //                returnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext, "txt_IssueRevokeFaild");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        returnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext, "txt_NoPermission");
        //    }

        //    var res = new JsonResult();
        //    res.Data = returnMsg;

        //    return res;
        //}


        /// <summary>
        ///// 激活接口
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult Activation(int id)
        //{
        //    OperationMessage returnMsg = new OperationMessage();
        //    var model = DBHelper_Content.IssueFirst(id);

        //    v_User CurrentUser = HttpContext.Session["CurrentUser"] as v_User;

        //    //可以处理人的集合
        //    var handles = AppConfig.IssueHandleUsers;

        //    //只有管理员才可以激活
        //    if (CurrentUser.User_GID == model.AdminOwner
        //        || handles.Contains(CurrentUser.User_GID))
        //    {
        //        if (model.State == (short)IssueState.Panding)
        //        {
        //            returnMsg.OpResult = true;
        //            returnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext, "txt_IssueActionSuccess");
        //        }
        //        else if (model.State == (short)IssueState.Revoke)
        //        {
        //            returnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext, "txt_IssueActionFaild_C1");
        //            //"问题已经处撤销，不能从新激活";
        //        }
        //        else if (model.State == (short)IssueState.Solved)
        //        {
        //            returnMsg.OpResult = DBHelper_Content.IssueSetState(model.id, (short)IssueState.Panding);
        //            if (returnMsg.OpResult)
        //            {
        //                returnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext, "txt_IssueActionSuccess");
        //            }
        //            else
        //            {
        //                returnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext, "txt_IssueActionFaild");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        returnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext, "txt_NoPermission");
        //    }

        //    var res = new JsonResult();
        //    res.Data = returnMsg;

        //    return res;
        //}

        [HttpGet]
        public ActionResult View(int id)
        {
            ViewBag.Main_ActionName = "Management";
            var ent = DBHelper_Content.IssueFirst(id);

            return View(ent);
        }

        [HttpGet]
        public ActionResult Handle(int id)
        {
            var ent = DBHelper_Content.IssueFirst(id);

            return View(ent);
        }

        //处理Issue
        [HttpPost]
        public ActionResult Handle(t_ReportIssue ent)
        {
            OperationMessage returnMsg = new OperationMessage();
            v_User CurrentUser = HttpContext.Session["CurrentUser"] as v_User;

            var oldIssue = DBHelper_Content.IssueFirst(ent.id);
            if (oldIssue != null)
            {
                //可以处理人的集合
                var Handles = AppConfig.IssueHandleUsers;

                //创建人  admin 通用用户
                if (oldIssue.Groups == (short)IssueGroup.SCM && Handles.Contains(CurrentUser.User_GID)
                    || CurrentUser.User_GID == oldIssue.AdminOwner
                    || CurrentUser.User_GID == oldIssue.CreateUser)
                {
                    var sendMail = false;
                    if (ent.State == (short)IssueState.Solved)
                    {
                        oldIssue.HandleUser = CurrentUser.User_GID;
                        oldIssue.HandleUserName = CurrentUser.User_Name_EN;
                        oldIssue.HandleTime = DateTime.Now;
                        oldIssue.State = (short)IssueState.Solved;
                        sendMail = true;
                    }
                    oldIssue.AnswerDetail = RegexHelper.RemoveXSS(System.Web.HttpUtility.HtmlDecode(ent.AnswerDetail));
                    oldIssue.AnswerReason = RegexHelper.RemoveXSS(ent.AnswerReason);

                    var flag = DBHelper_Content.IssueUpdate(oldIssue);
                    returnMsg.OpResult = flag;

                    returnMsg.OpMsg = flag ?
                        LanguageHelper.InnerLang(HttpContext, "txt_Success")
                        : LanguageHelper.InnerLang(HttpContext, "txt_Failed");

                    if (flag && sendMail)
                    {
                        //发送邮件
                        HandleSendMail(oldIssue);
                    }
                }
                else
                {
                    returnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext, "txt_NoPermission");
                }
            }
            else
            {
                returnMsg.OpMsg = LanguageHelper.InnerLang(HttpContext, "txt_Exception");
            }

            var res = new JsonResult();
            res.Data = returnMsg;

            return res;
        }

        [HttpGet]
        public ActionResult Export()
        {
            var datas = DBHelper_Content.GetIssueList();

            var outputs = (datas ?? new List<t_ReportIssue>()).Select(a => new IssueExportEntity
            {
                ID = a.id,
                ReportTitle = a.ReportTitle,
                AdminUser = $"{a.AdminOwnerName}（{a.AdminOwner}）",
                CreateUser = $"{a.CreateUserName}（{a.CreateUser}）",
                CreteTime = a.Createtime.ToString("yyyy-MM-dd HH:mm:ss"),
                HandleTime = a.State == 1 ? string.Empty : a.HandleTime.HasValue ? a.HandleTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                HandleUser = a.State == 1 ? string.Empty :
                    (!string.IsNullOrEmpty(a.HandleUserName) ? a.HandleUserName : "") + (!string.IsNullOrEmpty(a.HandleUser) ? $"{a.HandleUser}" : string.Empty),
                Issue = StringHtml.DelHTML(a.Issues),
                Reason = a.AnswerReason,
                State = a.State == 1 ? "pending" : "solved",
                Solution = StringHtml.DelHTML(a.AnswerDetail)
            }).ToList();
            var buffer = ExcelHelper.Export(outputs, false);

            return File(buffer, "application/vnd.ms-excel", $"{DateTime.Now.ToString("yyyyMMddHHmm")}_issue.xlsx");
        }


        /// <summary>
        /// 处理成功之后发送邮件
        /// </summary>
        private void HandleSendMail(t_ReportIssue inEnt)
        {
            t_Report TargetReport = DBHelper_Content.Get_Report_ByReportID(inEnt.r_Id);

            v_User CurrentUser = HttpContext.Session["CurrentUser"] as v_User;
            v_User ReportOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_Owner);
            v_User ReportAccessOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_AccessOwner);
            v_User reportAdmin = DBHelper_User.Get_User_ByGID(TargetReport.r_Admin);
            v_User reportCrOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_CROwner);
            //v_User AccessApprover = DBHelper_User.Get_User_ByGID(ReportAccessOwner.LineManager_GID);
            v_User createOwner = DBHelper_User.Get_User_ByGID(inEnt.CreateUser);

            var reportTemplate = "Report_Errors_Reply";
            t_MailTemplate CurTemplate = DBHelper_MailTemplate.Get_MailTemplate_ByName(reportTemplate);
            string MailTitle = CurTemplate.t_mt_Title + TargetReport.r_Name;
            string MailBody = CurTemplate.t_mt_Content;

            #region 生成参数，并替换模板内容

            Dictionary<string, string> Parameters = new Dictionary<string, string>();

            Parameters.Add("Applicant_Name", CurrentUser.User_Name_EN);
            Parameters.Add("Applicant_GID", CurrentUser.User_GID);
            Parameters.Add("Applicant_Email", CurrentUser.User_Email);
            Parameters.Add("Applicant_JobTitle", CurrentUser.Job_Function);
            Parameters.Add("Report_Admin_En", createOwner.User_Name_EN);
            Parameters.Add("Report_Admin_CH", createOwner.User_Name_CH);
            Parameters.Add("Report_Name", "<a href='" + TargetReport.r_Linkage + "'>" + TargetReport.r_Name + "</a>");
            Parameters.Add("Report_Owner", ReportOwner.User_Name_EN);
            Parameters.Add("ReportOwner_Name_CH", ReportOwner.User_Name_CH);
            Parameters.Add("Report_AccessOwner", ReportAccessOwner.User_Name_EN);
            //Parameters.Add("AccessApprover_Name_EN", AccessApprover.User_Name_EN);
            //Parameters.Add("AccessApprover_Name_CH", AccessApprover.User_Name_EN);
            Parameters.Add("AccessReason", inEnt.Issues);
            Parameters.Add("ErrorReason", inEnt.AnswerReason);
            Parameters.Add("ErrorSolution", inEnt.AnswerDetail);

            MailBody = PublicHelper.GenerateMailContent(MailBody, Parameters);

            #endregion
            string ToMail = reportAdmin.User_Email + "," + createOwner.User_Email;
            string CCMail = CurrentUser.User_Email + "," + ReportOwner.User_Email + "," + reportCrOwner.User_Email;
            SendEmail(ToMail, CCMail, MailTitle, MailBody);

            t_OprationLog log = new t_OprationLog
            {
                Type = "报表报错回复",
                Text = CurrentUser.User_GID + "(" + CurrentUser.User_Name_CH + ")" + " 向报表" + TargetReport.r_Name + "错误给予了解决方案",
                CreateUser = CurrentUser.User_GID,
                CreateDate = System.DateTime.Now,
                Status = 0
            };
            DBHelper_Content.Add_Log(log);
        }

        public static void SendEmail(string to, string cc, string obj, string content)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = delegate (object obj6, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; };
            var smtp = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 60000,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(@"UNOVPTSGMY", @"0f=y-XhkSM-1j=kg-=lvp6PvX="),
                Port = 25,
                Host = "139.25.239.160",
                EnableSsl = true
            };
            var mail = new MailMessage { From = new MailAddress("IT_Application.ssml@siemens.com") };
            if (to != "")
            {
                string[] tos = to.Split(',');
                for (int i = 0; i < tos.Length; i++)
                {
                    mail.To.Add(new MailAddress(tos[i]));
                }
            }
            if (cc != "")
            {
                string[] ccs = cc.Split(',');
                for (int i = 0; i < ccs.Length; i++)
                {
                    mail.CC.Add(new MailAddress(ccs[i]));
                }
            }
            //mail.Bcc.Add(new MailAddress("chaoming.sun@siemens.com"));
            mail.Subject = obj;
            mail.Body = content;
            mail.IsBodyHtml = true;
            //smtp.Send(mail);
        }

        //private void HandleSendMail(t_ReportIssue inEnt)
        //{
        //    t_Report TargetReport = DBHelper_Content.Get_Report_ByReportID(inEnt.r_Id);

        //    v_User CurrentUser = HttpContext.Session["CurrentUser"] as v_User;
        //    v_User ReportOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_Owner);
        //    v_User ReportAccessOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_AccessOwner);
        //    v_User reportAdmin = DBHelper_User.Get_User_ByGID(TargetReport.r_Admin);
        //    v_User reportCrOwner = DBHelper_User.Get_User_ByGID(TargetReport.r_CROwner);
        //    v_User AccessApprover = DBHelper_User.Get_User_ByGID(ReportAccessOwner.LineManager_GID);
        //    v_User createOwner = DBHelper_User.Get_User_ByGID(inEnt.CreateUser);

        //    var reportTemplate = "Report_Errors_Reply";
        //    t_MailTemplate CurTemplate = DBHelper_MailTemplate.Get_MailTemplate_ByName(reportTemplate);
        //    string MailTitle = CurTemplate.t_mt_Title + TargetReport.r_Name;


        //    var mailApi = MailFac.GetMailHttp();
        //    if (AppConfig.IsDev)
        //    {
        //        mailApi.ResetItemAction = (item) =>
        //        {
        //            item.ClearCc();
        //            item.ClearTo();
        //            item.AddTo("taihong.huang.ext@siemens.com");
        //        };
        //    }
        //    var mailItem = new MailItem();
        //    mailItem.AddImage(HttpContext.Server.MapPath($"{AppConfig.PreURL}/Img/Email/EmailLogo.gif"));
        //    mailItem.AddImage(HttpContext.Server.MapPath($"{AppConfig.PreURL}/Img/Email/EmailHead.jpg"));
        //    mailItem.Subject = MailTitle;
        //    mailItem.Body = CurTemplate.t_mt_Content;
        //    mailItem.Token = AppConfig.MailToken;
        //    mailItem.Url = AppConfig.MailUrl;
        //    mailItem.Form = AppConfig.PublicMailSender;

        //    mailItem.AddTo(reportAdmin.User_Email);
        //    mailItem.AddTo(createOwner.User_Email);

        //    mailItem.AddCc(CurrentUser.User_Email);
        //    mailItem.AddCc(ReportOwner.User_Email);
        //    mailItem.AddCc(reportCrOwner.User_Email);
        //    mailItem.AddCc(AppConfig.ChunYanMail);


        //    var param = new
        //    {
        //        Applicant_Name = CurrentUser.User_Name_EN,
        //        Applicant_GID = CurrentUser.User_GID,
        //        Applicant_Email = CurrentUser.User_Email,
        //        Applicant_JobTitle = CurrentUser.Job_Function,
        //        //Report_Admin_En = reportAdmin.User_Name_EN,
        //        //Report_Admin_CH = reportAdmin.User_Name_CH,
        //        Report_Admin_En = createOwner.User_Name_EN,
        //        Report_Admin_CH = createOwner.User_Name_CH,
        //        Report_Name = "<a href='" + TargetReport.r_Linkage + "'>" + TargetReport.r_Name + "</a>",
        //        Report_Owner = ReportOwner.User_Name_EN,
        //        ReportOwner_Name_CH = ReportOwner.User_Name_CH,
        //        Report_AccessOwner = ReportAccessOwner.User_Name_EN,
        //        AccessApprover_Name_EN = AccessApprover != null ? AccessApprover.User_Name_EN.Replace(",", string.Empty) : "",
        //        AccessApprover_Name_CH = AccessApprover != null ? AccessApprover.User_Name_CH.Replace(",", string.Empty) : "",
        //        AccessReason = inEnt.Issues,
        //        ErrorReason = inEnt.AnswerReason,
        //        ErrorSolution = inEnt.AnswerDetail
        //    };

        //    var flag = mailApi.Send(mailItem, param);

        //    if (flag)
        //    {
        //        t_OprationLog log = new t_OprationLog
        //        {
        //            Type = "报表报错回复",
        //            Text = CurrentUser.User_GID + "(" + CurrentUser.User_Name_CH + ")" + " 向报表" + TargetReport.r_Name + "错误给予了解决方案",
        //            CreateUser = CurrentUser.User_GID,
        //            CreateDate = System.DateTime.Now,
        //            Status = 0
        //        };
        //        DBHelper_Content.Add_Log(log);
        //    }
        //}
    }
}