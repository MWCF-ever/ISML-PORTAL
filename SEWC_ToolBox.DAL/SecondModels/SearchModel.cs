using SEWC_ToolBox.DAL.EFs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.DAL.SecondModels
{
    public class SearchModel
    {
        public List<t_ProcessLinkage> DocumentList { get; set; }
        public List<ReportModel> ReprotList { get; set; }

    }

    public class SearchModels
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class AutoCompleteModel
    {
        public string type { get; set; }

        public int code { get; set; }

        public List<SearchModels> content { get; set; }

    }

    public class JsonModel
    {
        public int code { get; set; }
        public string msg { get; set; }
        public int count { get; set; }
        public Object data { get; set; }

    }
}
