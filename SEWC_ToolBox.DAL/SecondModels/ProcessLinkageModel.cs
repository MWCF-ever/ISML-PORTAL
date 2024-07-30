using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.DAL.SecondModels
{
    public class ProcessLinkageModel
    {
        public int pl_ID { get; set; }
        public Nullable<int> pl_pn_ID { get; set; }
        public Nullable<int> pl_Type { get; set; }
        public string pl_Name { get; set; }
        public string pl_Linkage { get; set; }
        public Nullable<int> pl_Sort { get; set; }
        public string Favorite { get; set; }
        public string Location { get; set; }

    }
}
