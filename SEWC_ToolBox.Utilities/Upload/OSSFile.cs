using SEWC_ToolBox.Utilities.Helpers;
using SEWC_ToolBox.Utilities.Upload.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SEWC_ToolBox.Utilities.Upload
{
    public class OSSFile : IOSS
    {
        protected virtual string[] AllowExts { get; set; }

        protected virtual int MaxSize { get; set; }

        protected virtual string[] NameBlackList { get; set; }

        public OSSFile()
        {
            MaxSize = 50 * 1024 * 1024;
            AllowExts = new string[] { };
        }

        protected OssResult Check(FileStore store)
        {
            OssResult result = new OssResult();
            result.Success = false;
            AllowExts = new string[] { ".PNG", ".JPG", ".JPEG" };
            //校验文件后缀和内容
            var flag = FileTypeCheck.IsValidFileExtension(store.FullName, store.Body, AllowExts);
            if (flag)
            {
                if (MaxSize * 1024 < store.GetKb())
                {
                    result.Message = "文件过大";
                }
                else
                {
                    var isMatch = false;
                    foreach (var blackName in NameBlackList)
                    {
                        isMatch = RegexHelper.IsMatch(store.Name, blackName);
                        if (isMatch)
                        {
                            break;
                        }
                    }
                    if (isMatch) { result.Message = "上传文件名不合法"; }
                    else
                        result.Success = true;
                }
            }
            else
            {
                result.Message = "非法文件";
            }

            return result;
        }

        public virtual OssResult Upload(FileStore store)
        {
            var bucket = store.Bucket.Trim('/').Trim('\\');
            var path = $"/{bucket}/{Guid.NewGuid():N}{store.Ext}";
            var res = Upload(store, $"/{store.Pre}{path}");
            res.VirtualPath = path;

            return res;
        }

        public virtual OssResult Upload(FileStore store, string savePath)
        {
            OssResult result;
            //校验文件后缀&内容&大小
            result = Check(store);
            //验证通过
            if (result.Success)
            {
                //保存到目录下
                var file = savePath;

                var absPath = HttpContext.Current.Server.MapPath(file);

                FileInfo fInfo = new FileInfo(absPath);

                if (!fInfo.Directory.Exists)
                {
                    fInfo.Directory.Create();
                }

                using (var fs = fInfo.Open(FileMode.Create, FileAccess.Write))
                {
                    fs.Write(store.Body, 0, store.Body.Length);
                    fs.Flush();
                }

                result.Success = true;
                result.SavePath = file;
                result.VirtualPath = file;
                result.AbsPath = absPath;
            }
            return result;
        }

        public virtual bool Exist(string file)
        {
            var absPath = HttpContext.Current.Server.MapPath(file);

            return File.Exists(absPath);
        }

        public virtual OssResult CanMove(MoveStore store)
        {
            OssResult result = new OssResult();

            var savePath = $"/{store.Pre}{store.File}";
            var absPath = HttpContext.Current.Server.MapPath($"/{store.Pre}{store.File}");
            //File.WriteAllText(absPath + ".txt", absPath);
            var newFile = $"/{store.Bucket}/{store.Key}{store.GetExt()}";
            FileInfo fInfo = new FileInfo(absPath);
            if (fInfo.Exists)
            {
                result.VirtualPath = newFile;
                result.AbsPath = fInfo.FullName;
                result.SavePath = savePath;
                result.Success = true;
            }
            else result.Success = false;

            return result;
        }


        public virtual OssResult Move(MoveStore store)
        {
            OssResult result = new OssResult();

            var absOldPath = HttpContext.Current.Server.MapPath($"/{store.Pre}{store.File}");
            var newFile = $"/{store.Bucket}/{store.Key}{store.GetExt()}";
            var savePath = $"/{store.Pre}{newFile}";
            var absSaveFile = HttpContext.Current.Server.MapPath(savePath);

            FileInfo fInfo = new FileInfo(absOldPath);

            if (fInfo.Exists)
            {
                var newFileInfo = new FileInfo(absSaveFile);
                if (!newFileInfo.Directory.Exists) newFileInfo.Directory.Create();

                fInfo.CopyTo(absSaveFile, true);
                try
                {
                    fInfo.Delete();
                    result.VirtualPath = newFile;
                    result.SavePath = savePath;
                    result.AbsPath = absSaveFile;
                    result.Success = true;
                }
                catch (Exception) { }
            }
            else
            {
                result.Message = "移动失败";
            }

            return result;
        }

        public virtual bool Delete(string file)
        {
            var res = false;
            var absPath = HttpContext.Current.Server.MapPath(file);
            FileInfo fInfo = new FileInfo(absPath);
            if (fInfo.Exists)
            {
                fInfo.Delete();
                res = true;
            }

            return res;
        }

        public void Setting(int maxSize, string[] allowExts)
        {
            this.MaxSize = maxSize;
            AllowExts = allowExts;
        }

    }
}
