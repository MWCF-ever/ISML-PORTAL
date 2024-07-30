using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.Utilities.Upload.Entity
{
    public class FileStore
    {
        private string _pre;
        private string _bucket;

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string Ext { get; private set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 内容
        /// </summary>
        public byte[] Body { get; private set; }

        /// <summary>
        /// 内容大小
        /// </summary>
        public long Size { get; private set; }

        /// <summary>
        /// 辅助路径 如：UploadTemp
        /// </summary>
        public string Bucket { get { return _bucket; } set { _bucket = value.Trim('/', '\\'); } }

        /// <summary>
        /// 路径前缀 默认无
        /// </summary>
        public string Pre { get { return _pre; } set { _pre = value.Trim('/', '\\'); } }


        public FileStore(string fullName, MemoryStream stream) : this(fullName, stream.ToArray())
        {

        }

        public FileStore(string fullName, byte[] buffer)
        {
            this.FullName = fullName;
            Ext = Path.GetExtension(fullName);
            Name = Path.GetFileName(fullName);

            this.Body = buffer;
            Size = buffer.Length;
        }


        public long GetKb()
        {
            return (this.Body == null ? 0 : Body.Length) / 1024;
        }
    }
}
