using SEWC_ToolBox.Utilities.Upload;
using SEWC_ToolBox.Utilities.Upload.Entity;
using SEWC_ToolBox_Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

public class UploadImageHandler : Handler
{
    public UploadConfig UploadConfig { get; private set; }
    public UploadResult Result { get; private set; }

    public UploadImageHandler(HttpContext context, UploadConfig config)
        : base(context)
    {
        this.UploadConfig = config;
        this.Result = new UploadResult() { State = UploadState.Unknown };
    }

    public override void Process()
    {
        string uploadFileName;
        byte[] uploadFileBytes;
        if (UploadConfig.Base64)
        {
            uploadFileName = UploadConfig.Base64Filename;
            uploadFileBytes = Convert.FromBase64String(Request[UploadConfig.UploadFieldName]);
        }
        else
        {
            var file = Request.Files[UploadConfig.UploadFieldName];
            uploadFileName = file.FileName;
            uploadFileBytes = new byte[file.ContentLength];
            try
            {
                file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);
            }
            catch (Exception)
            {
                Result.State = UploadState.NetworkError;
                WriteResult();
                return;
            }
        }
        IOSS oss = new OSSImage();
        oss.Setting(UploadConfig.SizeLimit, UploadConfig.AllowExtensions);
        var savePath = PathFormatter.Format(uploadFileName, UploadConfig.PathFormat);
        FileStore store = new FileStore(uploadFileName, uploadFileBytes);
        savePath = $"{AppConfig.PreURL}{savePath}";
        var res = oss.Upload(store, savePath);

        if (res.Success)
        {
            Result.OriginFileName = res.getFileName();
            Result.Url = res.SavePath;
            Result.State = UploadState.Success;
        }
        else
        {
            Result.State = UploadState.FileAccessError;
            Result.ErrorMessage = "Failure";
        }
        WriteResult();
    }

    private void WriteResult()
    {
        this.WriteJson(new
        {
            state = GetStateMessage(Result.State),
            url = Result.Url,
            title = Result.OriginFileName,
            original = Result.OriginFileName,
            error = Result.ErrorMessage
        });
    }

    private string GetStateMessage(UploadState state)
    {
        switch (state)
        {
            case UploadState.Success:
                return "SUCCESS";
            case UploadState.FileAccessError:
                return "文件访问出错，请检查写入权限";
            case UploadState.SizeLimitExceed:
                return "文件大小超出服务器限制";
            case UploadState.TypeNotAllow:
                return "不允许的文件格式";
            case UploadState.NetworkError:
                return "网络错误";
        }
        return "未知错误";
    }

    private bool CheckFileType(string filename)
    {
        var fileExtension = Path.GetExtension(filename).ToLower();
        return UploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension);
    }

    private bool CheckFileSize(int size)
    {
        return size < UploadConfig.SizeLimit;
    }
}


