using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.Utilities.Mail
{
    public class MailItem
    {
        public string Url { get; set; }

        public string Token { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsHtml { get; set; } = true;

        public string Form { get; set; }

        public string Tos { get { return string.Join(",", this.ToLis); } }
        public string Ccs { get { return string.Join(",", this.CcLis); } }

        public List<string> Images { get; private set; } = new List<string>();

        public List<string> ToLis { get; private set; } = new List<string>();
        public List<string> CcLis { get; private set; } = new List<string>();

        public void ClearCc()
        {
            CcLis.Clear();
        }

        public void ClearTo()
        {
            ToLis.Clear();
        }


        public void AddTo(string to)
        {
            if (!ToLis.Contains(to))
            {
                this.ToLis.Add(to);
            }
        }

        public void AddCc(string cc)
        {
            if (!CcLis.Contains(cc))
            {
                this.CcLis.Add(cc);
            }
        }

        public void AddImage(string img)
        {
            if (!Images.Contains(img))
            {
                this.Images.Add(img);
            }
        }
        public void AddImages(List<string> imgs)
        {
            foreach (var img in imgs)
            {
                AddImage(img);
            }
        }
    }
}
