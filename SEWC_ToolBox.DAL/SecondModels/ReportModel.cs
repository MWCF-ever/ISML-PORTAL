using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.DAL.SecondModels
{
    public class ReportModel
    {
        public int r_ID { get; set; }
        public Nullable<int> r_m_DeptID { get; set; }
        public Nullable<int> r_m_ID_Category { get; set; }
        public string Menu_CategoryName { get; set; }
        public string Menu_SubCategoryName { get; set; }
        public string Menu_CategoryActionName { get; set; }
        public Nullable<int> r_m_ID_Frequency { get; set; }
        public string Menu_FrequencyName { get; set; }
        public string Menu_FrequencyActionName { get; set; }
        public string r_Name { get; set; }
        public string r_Description { get; set; }
        public string r_Owner { get; set; }
        public string r_Admin { get; set; }
        public string r_AdminEmail { get; set; }
        public string User_Owner { get; set; }
        public string User_CrOwner { get; set; }
        public string User_CrEmail { get; set; }
        public string User_OwnerEmail { get; set; }
        public string r_AccessOwner { get; set; }
        public string User_AccessOwner { get; set; }
        public string User_AccessOwnerEmail { get; set; }
        public string r_PicPath { get; set; }
        public string r_Linkage { get; set; }
        public Nullable<int> r_Status { get; set; }
        public Nullable<int> r_AccessLevel { get; set; }
        public Nullable<int> r_Sort { get; set; }
        public int r_cs_ClickTimes { get; set; }
        public Nullable<DateTime> r_lr_LastReloadTime { get; set; }
        public Nullable<int> Menu_e_ID { get; set; }
        public string Favorite_IsAdded { get; set; }

        public string CreateUser { get; set; }
        public string Location { get; set; }

    }
}
