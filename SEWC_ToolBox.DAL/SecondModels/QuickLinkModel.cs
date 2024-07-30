using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.DAL.SecondModels
{
    public class QuickLinkModel
    {
        public int ql_ID { get; set; }
        public string ql_GUID { get; set; }
        public Nullable<int> ql_m_DeptID { get; set; }
        public Nullable<int> ql_m_ID { get; set; }
        public string ql_m_ActionName { get; set; }
        public string ql_Name { get; set; }
        public string ql_Description { get; set; }
        public string ql_PicPath { get; set; }
        public string ql_Linkage { get; set; }
        public Nullable<int> ql_Status { get; set; }
        public Nullable<int> ql_AccessLevel { get; set; }
        public Nullable<int> ql_Sort { get; set; }
        public string Favorite_IsAdded { get; set; }
        public string Location { get; set; }

    }
}
