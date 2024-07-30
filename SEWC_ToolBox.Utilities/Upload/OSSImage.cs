using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.Utilities.Upload
{
    public class OSSImage : OSSFile
    {
        protected override string[] AllowExts { get; set; }

        protected override int MaxSize { get; set; }


        public OSSImage() : base()
        {
            MaxSize = Convert.ToInt32(ConfigurationManager.AppSettings["UploadImgMaxSize"]);
            AllowExts = ConfigurationManager.AppSettings["UploadImgExts"].Split(',');
            NameBlackList= ConfigurationManager.AppSettings["UploadNameBlackList"].Split(',');
        }
    }
}
