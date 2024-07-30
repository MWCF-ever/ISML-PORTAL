using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.Utilities.Upload.Entity
{
    public class OssResult
    {
        public bool Success { get; set; }


        public string Message { get; set; }

        /// <summary>
        /// 保存的虚拟路径，直接可访问，
        /// </summary>
        public string SavePath { get; set; }

        /// <summary>
        /// 对项目的虚拟路径，子站点模式不能直接访问
        /// </summary>
        public virtual string VirtualPath { get; set; }

        /// <summary>
        /// 物理路径
        /// </summary>
        public virtual string AbsPath { get; set; }

        public string getFileName()
        {
            return Path.GetFileName(SavePath);
        }
    }
}
