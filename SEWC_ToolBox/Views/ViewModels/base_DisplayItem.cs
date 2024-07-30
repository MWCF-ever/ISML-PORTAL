using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEWC_ToolBox.ViewModels
{
    public abstract class base_DisplayItem
    {
        public int base_ID { get; set; }
        public string base_GUID { get; set; }
        public Nullable<int> base_DeptID { get; set; }
        public string base_Name { get; set; }
        public string base_Description { get; set; }
        public string base_PicPath { get; set; }
        public string base_Linkage { get; set; }
        public Nullable<int> base_Sort { get; set; }

        public abstract bool Add();
        public abstract bool Edit();
        public abstract bool Delete();
    }
}