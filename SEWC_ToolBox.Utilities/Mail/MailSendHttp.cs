using SEWC_ToolBox.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;

namespace SEWC_ToolBox.Utilities.Mail
{
    public class MailSendHttp : IMaillSend
    {
        private static object LOCK_OBJ = new object();

        public Action<MailItem> ResetItemAction { get; set; }

        private string GetBase64(string image)
        {
            var base64 = string.Empty;
            FileInfo fInfo = new FileInfo(image);
            if (fInfo.Exists)
            {
                //var extension = "jpg";
                using (var fs = fInfo.OpenRead())
                {
                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    base64 = Convert.ToBase64String(buffer);
                }
                //暂时不知道什么原因，Outlook不能显示base64的GIF图，在这里强制整成jpg
                //base64 = $"data:image/{fInfo.Extension.Trim('.')};base64,{base64}";
                base64 = $"data:image/jpg;base64,{base64}";
            }
            return base64;
        }
        private string FormatTepleme(string body, List<string> images, object parameter)
        {
            var content = body;
            if (parameter != null)
            {
                content = PublicHelper.GenerateMailTemplateContent(body, parameter);
            }

            if (images != null && images.Count > 0)
            {
                for (int i = 0; i < images.Count; i++)
                {
                    var image = images[i];
                    var base64 = GetBase64(image);
                    content = content.Replace($"cid:attach{i}", base64);
                }
            }

            return content;
        }

        public virtual bool Send(MailItem item, object parameter = null)
        {
            ResetItemAction?.Invoke(item);
            var res = false;
            var body = FormatTepleme(item.Body, item.Images, parameter);
            lock (LOCK_OBJ)
            {
                var postData = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    tonken = item.Token,
                    from = item.Form,
                    to = item.Tos,
                    cc = item.Ccs,
                    title = item.Subject,
                    body = body
                });

                #region
                /*StringBuilder temps = new StringBuilder();
                temps.AppendLine("{");

                temps.AppendFormat("\"tonken\": \"{0}\"", item.Token);
                temps.AppendLine(",");

                temps.AppendFormat("\"from\": \"{0}\"", item.Form);
                temps.AppendLine(",");

                temps.AppendFormat("\"to\": \"{0}\"", item.Tos);
                temps.AppendLine(",");

                temps.AppendFormat("\"cc\": \"{0}\"", item.Ccs);
                temps.AppendLine(",");

                temps.AppendFormat("\"title\": \"{0}\"", item.Subject);
                temps.AppendLine(",");

                temps.AppendFormat("\"body\": \"{0}\"", body);
                temps.AppendLine();

                temps.AppendLine("}");*/
                #endregion

                using (HttpClient client = new HttpClient())
                {
                    StringContent content = new StringContent(postData);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    var httpRes = client.PostAsync(item.Url, content).GetAwaiter().GetResult();

                    if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        res = true;
                    }
                }
            }

            return res;
        }
    }
}
