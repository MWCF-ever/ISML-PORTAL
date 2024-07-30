using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.Utilities.Upload.Entity
{
    public class MoveStore
    {
        private string _key;
        private string _pre;
        private string _bucket;

        /// <summary>
        /// 被移动文件路径
        /// </summary>
        public string File { get; private set; }

        /// <summary>
        /// 辅助路径 如：UploadTemp
        /// </summary>
        public string Bucket { get { return _bucket; } set { _bucket = value.Trim('/', '\\'); } }

        /// <summary>
        /// 路径前缀 默认无
        /// </summary>
        public string Pre { get { return _pre; } set { _pre = value.Trim('/', '\\'); } }

        /// <summary>
        /// 文件key
        /// </summary>
        public string Key { get { return _key; } }


        public MoveStore(string file)
        {
            this.File = file;
            this._key = Guid.NewGuid().ToString("N");
        }

        public string GetExt()
        {
            return Path.GetExtension(File);
        }

    }
}
