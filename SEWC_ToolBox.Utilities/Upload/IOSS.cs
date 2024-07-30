using SEWC_ToolBox.Utilities.Upload.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.Utilities.Upload
{
    public interface IOSS
    {
        void Setting(int maxSize, string[] allowExts);

        OssResult Upload(FileStore store);

        OssResult Upload(FileStore store, string savePath);

        OssResult CanMove(MoveStore store);

        OssResult Move(MoveStore store);


        bool Delete(string file);
    }
}
