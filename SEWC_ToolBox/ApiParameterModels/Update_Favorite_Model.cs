using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEWC_ToolBox.ApiParameterModels
{
    public class Update_Favorite_Model
    {
        public bool isAdd { get; set; }
        public int f_TypeID { get; set; }
        public int f_ObjectID { get; set; }
    }
}