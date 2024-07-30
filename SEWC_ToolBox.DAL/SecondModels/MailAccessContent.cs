using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEWC_ToolBox.DAL.SecondModels
{
    public class MailAccessContent : IComparable<MailAccessContent>
    {
        public string Template_Name { get; set; }

        public string Applicant_Name { get; set; }
        public string Applicant_GID { get; set; }
        public string Applicant_Email { get; set; }
        public string Applicant_JobTitle { get; set; }

        public string Report_Name { get; set; }
        public string Report_Linkage { get; set; }

        public string ReportOwner_Name { get; set; }
        public string ReportOwner_Name_CH { get; set; }
        public string ReportOwner_Email { get; set; }
        public string AccessOwner_Name { get; set; }
        public string AccessOwner_GID { get; set; }
        public string AccessOwner_Email { get; set; }
        public string AccessApprover_Name_EN { get; set; }
        public string AccessApprover_Name_CH { get; set; }
        public string ReportAdmin_GID { get; set; }
        public string ReportAdmin_Email { get; set; }
        public string ReportAdmin_Name_EN { get; set; }
        public string ReportAdmin_Name_CH { get; set; }
        public string ReportCrOwner_GID { get; set; }
        public string ReportCrOwner_Email { get; set; }
        public string ReportCrOwner_Name_EN { get; set; }
        public string ReportCrOwner_Name_CH { get; set; }
        public string AccessApprover_GID { get; set; }
        public string AccessApprover_Email { get; set; }
        public string AccessReason { get; set; }

        //重写的CompareTo方法，根据Id排序
        public int CompareTo(MailAccessContent Cur)
        {
            int result = 0;

            if (Cur == null)
                result = 0;

            // 对 AccessApprover 进行比较
            result = this.AccessApprover_Name_EN.CompareTo(Cur.AccessApprover_Name_EN);

            // 如果 AccessApprover 相同再对 AccessOwner 进行比较
            if (result == 0)
                result = this.AccessOwner_Name.CompareTo(Cur.AccessOwner_Name);

            // 如果 AccessOwner 相同再对 Report
            if (result == 0)
                result = this.Report_Name.CompareTo(Cur.Report_Name);

            return result;
        }
    }
}