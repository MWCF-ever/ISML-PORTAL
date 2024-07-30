using SEWC_NetDevLib.SEWC_NetLibExtend;
using SEWC_ToolBox.DAL.DBHelpers;
using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox.DAL.SecondModels;
using SEWC_ToolBox.Utilities.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace SEWC_ToolBox.Utilities.Helpers
{

    public static class MailHelper
    {
        public static string GetAppSettings(string key)
        {
            var val = ConfigurationManager.AppSettings[key];
            return val == null ? string.Empty : val.ToString();
        }

        //[Obsolete]
        //public static bool SendMail_GetReportAccess_Single_Bak(MailAccessContent MailAccess, List<string> EmailPicList)
        //{
        //    try
        //    {
        //        string reportLinkage = MailAccess.Report_Linkage;
        //        string FromMail = ConfigurationManager.AppSettings["PublicMailSender"].ToString();
        //        string ToMail = MailAccess.AccessApprover_Email + "," + MailAccess.Applicant_Email;
        //        string CCMail = "";
        //        string addCCMail = "";

        //        if (ITSuportConfig(reportLinkage))
        //        {
        //            addCCMail = DBHelper_Content.Get_ReportAccessManager();
        //        }


        //        //if (MailAccess.AccessOwner_Email == MailAccess.Applicant_Email)
        //        //{
        //        //    CCMail = MailAccess.AccessOwner_Email + "," + addCCMail;
        //        //}
        //        //else
        //        //{
        //        //    CCMail = MailAccess.AccessOwner_Email + "," + addCCMail;
        //        //}
        //        CCMail = MailAccess.AccessOwner_Email + "," + addCCMail;

        //        // Report Errors 时需要重新制定接受邮件和 CC 的人
        //        if (MailAccess.Template_Name == "Report_Errors")
        //        {
        //            addCCMail = ConfigurationManager.AppSettings["CCEmail"].ToString();
        //            ToMail = MailAccess.ReportAdmin_Email + "," + MailAccess.Applicant_Email;
        //            if (MailAccess.ReportOwner_Email == MailAccess.ReportCrOwner_Email)
        //            {
        //                CCMail = MailAccess.ReportOwner_Email + "," + addCCMail;
        //            }
        //            else
        //            {
        //                CCMail = MailAccess.ReportOwner_Email + "," + MailAccess.ReportCrOwner_Email + "," + addCCMail;
        //            }
        //            v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;
        //            CCMail = CCMail + "," + CurrentUser.User_Email;
        //            // FromMail = CurrentUser.User_Email;
        //        }
        //        else
        //        {
        //            if (DBHelper_Content.CheckRemoveList(MailAccess.Applicant_GID, MailAccess.Report_Linkage))
        //            {
        //                MailAccess.Template_Name = "Report_GetAccess_Single_Special";
        //            }
        //        }

        //        CCMail = CCMail.Trim(',');
        //        t_MailTemplate CurTemplate = DBHelper_MailTemplate.Get_MailTemplate_ByName(MailAccess.Template_Name);
        //        string MailTitle = CurTemplate.t_mt_Title + MailAccess.Report_Name;
        //        string MailBody = CurTemplate.t_mt_Content;

        //        #region 生成参数，并替换模板内容

        //        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        //        Parameters.Add("Applicant_Name", MailAccess.Applicant_Name);
        //        Parameters.Add("Applicant_GID", MailAccess.Applicant_GID);
        //        Parameters.Add("Applicant_Email", MailAccess.Applicant_Email);
        //        Parameters.Add("Applicant_JobTitle", MailAccess.Applicant_JobTitle);

        //        Parameters.Add("Report_Admin_En", MailAccess.ReportAdmin_Name_EN);
        //        Parameters.Add("Report_Admin_CH", MailAccess.ReportAdmin_Name_CH);

        //        Parameters.Add("Report_Name", "<a href='" + MailAccess.Report_Linkage + "'>" + MailAccess.Report_Name + "</a>");
        //        Parameters.Add("Report_Owner", MailAccess.ReportOwner_Name);
        //        Parameters.Add("ReportOwner_Name_CH", MailAccess.ReportOwner_Name_CH);
        //        Parameters.Add("Report_AccessOwner", MailAccess.AccessOwner_Name);

        //        Parameters.Add("ReportCrOwner_Name_EN", MailAccess.ReportCrOwner_Name_EN);
        //        Parameters.Add("ReportCrOwner_Name_CH", MailAccess.ReportCrOwner_Name_CH);


        //        if (MailAccess.AccessApprover_Name_EN != null)
        //        {
        //            Parameters.Add("AccessApprover_Name_EN", MailAccess.AccessApprover_Name_EN.Replace(",", string.Empty));
        //        }
        //        Parameters.Add("AccessApprover_Name_CH", MailAccess.AccessApprover_Name_CH);
        //        Parameters.Add("AccessReason", MailAccess.AccessReason);

        //        MailBody = PublicHelper.GenerateMailContent(MailBody, Parameters);

        //        #endregion

        //        /*#region 测试信息覆盖

        //        string IP = NetHelper.LANIP;
        //        string hostName = Dns.GetHostName();
        //        IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
        //        IP = ipEntry.AddressList[1].ToString();

        //        if (IP != "")
        //        {
        //            MailBody = "测试系统不会发送邮件到以下实际接收人<br /><br />Actual ToMail: " + ToMail + "<br /><br />" + "Actual CCMail: " + CCMail + "<br /><br />" + MailBody;
        //            ToMail = "yupeng-lan.ext@siemens.com";
        //            CCMail = "";
        //        }

        //        #endregion*/

        //        NetHelper.SendEmail(FromMail, ToMail, CCMail, MailTitle, MailBody, true, EmailPicList, "mail-cn.siemens.net");

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //[Obsolete]
        //public static bool SendMail_GetReportAccess_Multiple_Bak(List<MailAccessContent> MailList, List<string> EmailPicList)
        //{
        //    try
        //    {
        //        if (MailList.Count != 1)
        //        {
        //            string FromMail = ConfigurationManager.AppSettings["PublicMailSender"].ToString();
        //            string ToMail = MailList[0].AccessApprover_Email + "," + MailList[0].Applicant_Email;
        //            string CCMail = "";
        //            List<string> AccessOwner_EmailList = new List<string>();
        //            var flag = false;

        //            foreach (var item in MailList)
        //            {
        //                flag = DBHelper_Content.CheckRemoveList(MailList[0].Applicant_GID, item.Report_Linkage);
        //                if (flag)
        //                {
        //                    break;
        //                }
        //            }
        //            if (flag)
        //            {
        //                MailList[0].Template_Name = "Report_GetAccess_Multiple_Special";
        //            }


        //            t_MailTemplate CurTemplate = DBHelper_MailTemplate.Get_MailTemplate_ByName(MailList[0].Template_Name);
        //            string MailTitle = CurTemplate.t_mt_Title;
        //            string MailBody = CurTemplate.t_mt_Content;


        //            #region 生成参数，并替换模板内容

        //            Dictionary<string, string> Parameters = new Dictionary<string, string>();

        //            Parameters.Add("Applicant_Name", MailList[0].Applicant_Name);
        //            Parameters.Add("Applicant_GID", MailList[0].Applicant_GID);
        //            Parameters.Add("Applicant_Email", MailList[0].Applicant_Email);
        //            Parameters.Add("Applicant_JobTitle", MailList[0].Applicant_JobTitle);

        //            Parameters.Add("AccessApprover_Name_EN", MailList[0].AccessApprover_Name_EN.Replace(",", string.Empty));
        //            Parameters.Add("AccessApprover_Name_CH", MailList[0].AccessApprover_Name_CH);
        //            Parameters.Add("AccessReason", MailList[0].AccessReason);

        //            StringBuilder ReportContentList = new StringBuilder();
        //            string Last_AccessOwner = string.Empty;

        //            for (int i = 0; i < MailList.Count; i++)
        //            {
        //                MailAccessContent MailAccess = MailList[i];

        //                if (!AccessOwner_EmailList.Contains(MailAccess.AccessOwner_Email))
        //                    AccessOwner_EmailList.Add(MailAccess.AccessOwner_Email);

        //                if (Last_AccessOwner.Equals(MailAccess.AccessOwner_GID))
        //                {
        //                    ReportContentList.Append("<a href='" + MailAccess.Report_Linkage + "'><span>" + MailAccess.Report_Name + "</span></a><br />");
        //                }
        //                else
        //                {
        //                    if (i == 0)
        //                    {
        //                        ReportContentList.Append("<tr>");
        //                        ReportContentList.Append("<td><span>" + MailAccess.AccessOwner_Name + "</span></td>");
        //                        ReportContentList.Append("<td>");
        //                        ReportContentList.Append("<br /><a href='" + MailAccess.Report_Linkage + "'><span>" + MailAccess.Report_Name + "</span></a><br />");
        //                    }
        //                    else
        //                    {
        //                        ReportContentList.Append("<br /></td></tr>");

        //                        ReportContentList.Append("<tr>");
        //                        ReportContentList.Append("<td><span>" + MailAccess.AccessOwner_Name + "</span></td>");
        //                        ReportContentList.Append("<td>");
        //                        ReportContentList.Append("<br /><a href='" + MailAccess.Report_Linkage + "'><span>" + MailAccess.Report_Name + "</span></a><br />");
        //                    }
        //                }

        //                if (i == MailList.Count - 1)
        //                {
        //                    ReportContentList.Append("<br /></td></tr>");
        //                }

        //                Last_AccessOwner = MailAccess.AccessOwner_GID;
        //            }

        //            Parameters.Add("ReportContentList", ReportContentList.ToString());

        //            #endregion

        //            CCMail += string.Join(",", AccessOwner_EmailList);
        //            CCMail += "," + DBHelper_Content.Get_ReportAccessManager();
        //            CCMail = CCMail.Trim(',');
        //            MailBody = PublicHelper.GenerateMailContent(CurTemplate.t_mt_Content, Parameters);

        //            #region 测试信息覆盖

        //            string IP = NetHelper.LANIP;
        //            string hostName = Dns.GetHostName();
        //            IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
        //            IP = ipEntry.AddressList[1].ToString();

        //            if (IP != ConfigurationManager.AppSettings["ProdIP"])
        //            {
        //                MailBody = "测试系统不会发送邮件到以下实际接收人<br /><br />Actual ToMail: " + ToMail + "<br /><br />" + "Actual CCMail: " + CCMail + "<br /><br />" + MailBody;
        //                ToMail = ConfigurationManager.AppSettings["EmailAddress"];
        //                CCMail = "";
        //            }

        //            #endregion

        //            NetHelper.SendEmail(FromMail, ToMail, CCMail, MailTitle, MailBody, true, EmailPicList, "mail-cn.siemens.net");

        //            return true;
        //        }
        //        else
        //        {
        //            MailList[0].Template_Name = "Report_GetAccess_Single";
        //            return SendMail_GetReportAccess_Single(MailList[0], EmailPicList);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



        //public static bool SendMail_GetReportAccess_Single(MailAccessContent MailAccess, List<string> EmailPicList)
        //{
        //    try
        //    {
        //        string reportLinkage = MailAccess.Report_Linkage;
        //        string FromMail = GetAppSettings("PublicMailSender");
        //        //ConfigurationManager.AppSettings["PublicMailSender"].ToString();
        //        string ToMail = MailAccess.AccessApprover_Email + "," + MailAccess.Applicant_Email;
        //        string CCMail = "";
        //        string addCCMail = "";

        //        if (ITSuportConfig(reportLinkage))
        //        {
        //            addCCMail = DBHelper_Content.Get_ReportAccessManager();
        //        }

        //        CCMail = MailAccess.AccessOwner_Email + "," + addCCMail;

        //        // Report Errors 时需要重新制定接受邮件和 CC 的人
        //        if (MailAccess.Template_Name == "Report_Errors")
        //        {
        //            addCCMail = ConfigurationManager.AppSettings["CCEmail"].ToString();
        //            ToMail = MailAccess.ReportAdmin_Email + "," + MailAccess.Applicant_Email;
        //            if (MailAccess.ReportOwner_Email == MailAccess.ReportCrOwner_Email)
        //            {
        //                CCMail = MailAccess.ReportOwner_Email + "," + addCCMail;
        //            }
        //            else
        //            {
        //                CCMail = MailAccess.ReportOwner_Email + "," + MailAccess.ReportCrOwner_Email + "," + addCCMail;
        //            }
        //            v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;
        //            CCMail = CCMail + "," + CurrentUser.User_Email;
        //            // FromMail = CurrentUser.User_Email;
        //        }
        //        else
        //        {
        //            if (DBHelper_Content.CheckRemoveList(MailAccess.Applicant_GID, MailAccess.Report_Linkage))
        //            {
        //                MailAccess.Template_Name = "Report_GetAccess_Single_Special";
        //            }
        //        }

        //        CCMail = CCMail.Trim(',');
        //        t_MailTemplate CurTemplate = DBHelper_MailTemplate.Get_MailTemplate_ByName(MailAccess.Template_Name);
        //        string MailTitle = CurTemplate.t_mt_Title + MailAccess.Report_Name;
        //        string MailBody = CurTemplate.t_mt_Content;

        //        #region 生成参数，并替换模板内容

        //        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        //        Parameters.Add("Applicant_Name", MailAccess.Applicant_Name);
        //        Parameters.Add("Applicant_GID", MailAccess.Applicant_GID);
        //        Parameters.Add("Applicant_Email", MailAccess.Applicant_Email);
        //        Parameters.Add("Applicant_JobTitle", MailAccess.Applicant_JobTitle);

        //        Parameters.Add("Report_Admin_En", MailAccess.ReportAdmin_Name_EN);
        //        Parameters.Add("Report_Admin_CH", MailAccess.ReportAdmin_Name_CH);

        //        Parameters.Add("Report_Name", "<a href='" + MailAccess.Report_Linkage + "'>" + MailAccess.Report_Name + "</a>");
        //        Parameters.Add("Report_Owner", MailAccess.ReportOwner_Name);
        //        Parameters.Add("ReportOwner_Name_CH", MailAccess.ReportOwner_Name_CH);
        //        Parameters.Add("Report_AccessOwner", MailAccess.AccessOwner_Name);

        //        Parameters.Add("ReportCrOwner_Name_EN", MailAccess.ReportCrOwner_Name_EN);
        //        Parameters.Add("ReportCrOwner_Name_CH", MailAccess.ReportCrOwner_Name_CH);


        //        if (MailAccess.AccessApprover_Name_EN != null)
        //        {
        //            Parameters.Add("AccessApprover_Name_EN", MailAccess.AccessApprover_Name_EN.Replace(",", string.Empty));
        //        }
        //        Parameters.Add("AccessApprover_Name_CH", MailAccess.AccessApprover_Name_CH);
        //        Parameters.Add("AccessReason", MailAccess.AccessReason);

        //        MailBody = PublicHelper.GenerateMailContent(MailBody, Parameters);

        //        #endregion

        //        var mailItem = new MailItem();
        //        mailItem.AddImages(EmailPicList);
        //        mailItem.Subject = MailTitle;
        //        mailItem.Body = MailBody;
        //        mailItem.Token = GetAppSettings("MailToken"); //AppConfig.MailToken;
        //        mailItem.Url = GetAppSettings("MailUrl"); //AppConfig.MailUrl;
        //        mailItem.Form = FromMail;

        //        mailItem.AddTo(ToMail);
        //        mailItem.AddCc(CCMail);
        //        var mailApi = MailFac.GetMailHttp();
        //        var isDev = GetAppSettings("IsDev").Equals("true", StringComparison.OrdinalIgnoreCase);

        //        mailApi.ResetItemAction = (item) =>
        //        {
        //            //测试环境情况下重置发送内容
        //            if (!isDev) return;
        //            item.ClearCc();
        //            item.ClearTo();
        //            var tos = (GetAppSettings("EmailAddress") ?? string.Empty)
        //                .Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries)
        //                .Where(a => !string.IsNullOrEmpty(a))
        //                .ToList();
        //            if (tos.Count == 0)
        //            {
        //                tos.Add("taihong.huang.ext@siemens.com");
        //            }
        //            foreach (var to in tos)
        //            {
        //                item.AddTo(to);
        //            }
        //        };
        //        return mailApi.Send(mailItem);
        //        //NetHelper.SendEmail(FromMail, ToMail, CCMail, MailTitle, MailBody, true, EmailPicList, "mail-cn.siemens.net");

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public static void SendMail_GetReportAccess_SingleNew(MailAccessContent MailAccess)
        {
            try
            {
                string reportLinkage = MailAccess.Report_Linkage;
                string ToMail = MailAccess.AccessApprover_Email + "," + MailAccess.Applicant_Email;
                string CCMail = "";
                string addCCMail = "";
                CCMail = MailAccess.AccessOwner_Email + "," + addCCMail;

                // Report Errors 时需要重新制定接受邮件和 CC 的人
                if (MailAccess.Template_Name == "Report_Errors")
                {
                    ToMail = MailAccess.ReportAdmin_Email + "," + MailAccess.Applicant_Email;
                    if (MailAccess.ReportOwner_Email == MailAccess.ReportCrOwner_Email)
                    {
                        CCMail = MailAccess.ReportOwner_Email + "," + addCCMail;
                    }
                    else
                    {
                        CCMail = MailAccess.ReportOwner_Email + "," + MailAccess.ReportCrOwner_Email + "," + addCCMail;
                    }
                    v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;
                    CCMail = CCMail + "," + CurrentUser.User_Email;
                }

                CCMail = CCMail.Trim(',');
                t_MailTemplate CurTemplate = DBHelper_MailTemplate.Get_MailTemplate_ByName(MailAccess.Template_Name);
                string MailTitle = CurTemplate.t_mt_Title + MailAccess.Report_Name;
                string MailBody = CurTemplate.t_mt_Content;

                #region 生成参数，并替换模板内容

                Dictionary<string, string> Parameters = new Dictionary<string, string>();

                Parameters.Add("Applicant_Name", MailAccess.Applicant_Name);
                Parameters.Add("Applicant_GID", MailAccess.Applicant_GID);
                Parameters.Add("Applicant_Email", MailAccess.Applicant_Email);
                Parameters.Add("Applicant_JobTitle", MailAccess.Applicant_JobTitle);

                Parameters.Add("Report_Admin_En", MailAccess.ReportAdmin_Name_EN);
                Parameters.Add("Report_Admin_CH", MailAccess.ReportAdmin_Name_CH);

                Parameters.Add("Report_Name", "<a href='" + MailAccess.Report_Linkage + "'>" + MailAccess.Report_Name + "</a>");
                Parameters.Add("Report_Owner", MailAccess.ReportOwner_Name);
                Parameters.Add("ReportOwner_Name_CH", MailAccess.ReportOwner_Name_CH);
                Parameters.Add("Report_AccessOwner", MailAccess.AccessOwner_Name);

                Parameters.Add("ReportCrOwner_Name_EN", MailAccess.ReportCrOwner_Name_EN);
                Parameters.Add("ReportCrOwner_Name_CH", MailAccess.ReportCrOwner_Name_CH);


                if (MailAccess.AccessApprover_Name_EN != null)
                {
                    Parameters.Add("AccessApprover_Name_EN", MailAccess.AccessApprover_Name_EN.Replace(",", string.Empty));
                }
                Parameters.Add("AccessApprover_Name_CH", MailAccess.AccessApprover_Name_CH);
                Parameters.Add("AccessReason", MailAccess.AccessReason);

                MailBody = PublicHelper.GenerateMailContent(MailBody, Parameters);

                #endregion

                SendEmail(ToMail, CCMail, MailTitle, MailBody);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendMail_GetReportAccess_Multiple(List<MailAccessContent> MailList)
        {
            try
            {
                if (MailList.Count != 1)
                {
                    string ToMail = MailList[0].AccessApprover_Email + "," + MailList[0].Applicant_Email;
                    string CCMail = "";
                    List<string> AccessOwner_EmailList = new List<string>();

                    t_MailTemplate CurTemplate = DBHelper_MailTemplate.Get_MailTemplate_ByName(MailList[0].Template_Name);
                    string MailTitle = CurTemplate.t_mt_Title;
                    string MailBody = CurTemplate.t_mt_Content;
                    #region 生成参数，并替换模板内容

                    Dictionary<string, string> Parameters = new Dictionary<string, string>();

                    Parameters.Add("Applicant_Name", MailList[0].Applicant_Name);
                    Parameters.Add("Applicant_GID", MailList[0].Applicant_GID);
                    Parameters.Add("Applicant_Email", MailList[0].Applicant_Email);
                    Parameters.Add("Applicant_JobTitle", MailList[0].Applicant_JobTitle);

                    Parameters.Add("AccessApprover_Name_EN", MailList[0].AccessApprover_Name_EN.Replace(",", string.Empty));
                    Parameters.Add("AccessApprover_Name_CH", MailList[0].AccessApprover_Name_CH);
                    Parameters.Add("AccessReason", MailList[0].AccessReason);

                    StringBuilder ReportContentList = new StringBuilder();
                    string Last_AccessOwner = string.Empty;

                    for (int i = 0; i < MailList.Count; i++)
                    {
                        MailAccessContent MailAccess = MailList[i];

                        if (!AccessOwner_EmailList.Contains(MailAccess.AccessOwner_Email))
                            AccessOwner_EmailList.Add(MailAccess.AccessOwner_Email);

                        if (Last_AccessOwner.Equals(MailAccess.AccessOwner_GID))
                        {
                            ReportContentList.Append("<a href='" + MailAccess.Report_Linkage + "'><span>" + MailAccess.Report_Name + "</span></a><br />");
                        }
                        else
                        {
                            if (i == 0)
                            {
                                ReportContentList.Append("<tr>");
                                ReportContentList.Append("<td><span>" + MailAccess.AccessOwner_Name + "</span></td>");
                                ReportContentList.Append("<td>");
                                ReportContentList.Append("<br /><a href='" + MailAccess.Report_Linkage + "'><span>" + MailAccess.Report_Name + "</span></a><br />");
                            }
                            else
                            {
                                ReportContentList.Append("<br /></td></tr>");

                                ReportContentList.Append("<tr>");
                                ReportContentList.Append("<td><span>" + MailAccess.AccessOwner_Name + "</span></td>");
                                ReportContentList.Append("<td>");
                                ReportContentList.Append("<br /><a href='" + MailAccess.Report_Linkage + "'><span>" + MailAccess.Report_Name + "</span></a><br />");
                            }
                        }

                        if (i == MailList.Count - 1)
                        {
                            ReportContentList.Append("<br /></td></tr>");
                        }

                        Last_AccessOwner = MailAccess.AccessOwner_GID;
                    }

                    Parameters.Add("ReportContentList", ReportContentList.ToString());

                    #endregion
                    CCMail += string.Join(",", AccessOwner_EmailList);
                    CCMail += "," + DBHelper_Content.Get_ReportAccessManager();
                    CCMail = CCMail.Trim(',');
                    MailBody = PublicHelper.GenerateMailContent(CurTemplate.t_mt_Content, Parameters);
                    SendEmail(ToMail, CCMail, MailTitle, MailBody);
                }
                else
                {
                    MailList[0].Template_Name = "Report_GetAccess_Single";
                    SendMail_GetReportAccess_SingleNew(MailList[0]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            smtp.Send(mail);
        }

        /// <summary>
        /// Config ITSuport  Email
        /// </summary>
        /// <param name="ToMail"></param>
        /// <param name="reportLinkage"></param>
        /// <returns></returns>
        private static bool ITSuportConfig(string reportLinkage)
        {
            try
            {
                bool checkLink = reportLinkage.Contains("https://intranet.sewc-bireport.siemens.net/");

                if (checkLink)
                {
                    return true;
                }
                else
                {
                    return reportLinkage.Contains("https://intranet.dfpdsalesbi.siemens.com/");
                }

            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}