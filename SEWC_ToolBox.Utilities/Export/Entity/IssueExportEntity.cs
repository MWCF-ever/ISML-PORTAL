using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.Utilities.Export.Entity
{
    public class IssueExportEntity
    {

        [Description("Id")]
        public int ID { get; set; }

        [Description("Report Title")]
        public string ReportTitle { get; set; }

        [Description("Admin User")]
        public string AdminUser { get; set; }

        [Description("Create User")]
        public string CreateUser { get; set; }

        [Description("Issue")]
        public string Issue { get; set; }

        [Description("Create Time")]
        public string CreteTime { get; set; }

        [Description("Reason")]
        public string Reason { get; set; }

        [Description("Solution")]
        public string Solution { get; set; }

        [Description("Hanldle User")]
        public string HandleUser { get; set; }

        [Description("Handle Time")]
        public string HandleTime { get; set; }

        [Description("State")]
        public string State { get; set; }
    }
}
