using SEWC_NetDevLib.SEWC_NetLibExtend;
using SEWC_ToolBox.DAL.DBHelpers;
using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox.DAL.SecondModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace SEWC_ToolBox.Utilities.Helpers
{

    public static class MailHelper
    {
        public static bool SendMail_GetReportAccess_Single(MailAccessContent MailAccess, List<string> EmailPicList)
        {
            try
            {
                string FromMail = ConfigurationManager.AppSettings["PublicMailSender"].ToString();
                string ToMail =  MailAccess.AccessApprover_Email;
                string CCMail = MailAccess.AccessOwner_Email + "," + MailAccess.Applicant_Email + "," + DBHelper_Content.Get_ReportAccessManager();
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

                Parameters.Add("Report_Name", "<a href='" + MailAccess.Report_Linkage + "'>" + MailAccess.Report_Name + "</a>");
                Parameters.Add("Report_Owner", MailAccess.ReportOwner_Name);
                Parameters.Add("Report_AccessOwner", MailAccess.AccessOwner_Name);
                Parameters.Add("AccessApprover_Name_EN", MailAccess.AccessApprover_Name_EN.Replace(",", string.Empty));
                Parameters.Add("AccessApprover_Name_CH", MailAccess.AccessApprover_Name_CH);
                Parameters.Add("AccessReason", MailAccess.AccessReason);

                MailBody = PublicHelper.GenerateMailContent(MailBody, Parameters);

                #endregion
                
                #region 测试信息覆盖

                string IP = NetHelper.LANIP;

                if (IP != "139.24.142.213")
                {
                    MailBody = "测试系统不会发送邮件到以下实际接收人<br /><br />Actual ToMail: " + ToMail + "<br /><br />" + "Actual CCMail: " + CCMail + "<br /><br />" + MailBody;
                    ToMail = "qian.ding.qd@siemens.com";
                    CCMail = "";
                }

                #endregion

                NetHelper.SendEmail(FromMail, ToMail, CCMail, MailTitle, MailBody, false, EmailPicList);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SendMail_GetReportAccess_Multiple(List<MailAccessContent> MailList, List<string> EmailPicList)
        {
            try
            {
                if (MailList.Count != 1)
                {
                    string FromMail = ConfigurationManager.AppSettings["PublicMailSender"].ToString();
                    string ToMail = MailList[0].AccessApprover_Email;
                    string CCMail = MailList[0].Applicant_Email + ",";
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
                    CCMail = CCMail.Trim(',');
                    MailBody = PublicHelper.GenerateMailContent(CurTemplate.t_mt_Content, Parameters);
                    
                    #region 测试信息覆盖

                    string IP = NetHelper.LANIP;

                    if (IP != "139.24.142.213")
                    {
                        MailBody = "测试系统不会发送邮件到以下实际接收人<br /><br />Actual ToMail: " + ToMail + "<br /><br />" + "Actual CCMail: " + CCMail + "<br /><br />" + MailBody;
                        ToMail = "qian.ding.qd@siemens.com";
                        CCMail = "";
                    }

                    #endregion

                    NetHelper.SendEmail(FromMail, ToMail, CCMail, MailTitle, MailBody, false, EmailPicList);

                    return true;
                }
                else
                {
                    MailList[0].Template_Name = "Report_GetAccess_Single";
                    return SendMail_GetReportAccess_Single(MailList[0], EmailPicList);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}