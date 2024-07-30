using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SEWC_ToolBox.Utilities
{
    public class StringHtml
    {
        /// <summary>
        /// 删除所有的HTML标签
        /// </summary>
        /// <param name="html">内容</param>
        /// <returns></returns>
        public static string DelHTML(string html)
        {
            var output = string.Empty;
            if (!string.IsNullOrEmpty(html))
            {
                html = System.Web.HttpUtility.HtmlDecode(html);
                Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
                output = regex.Replace(html, "");
            }

            return output;
        }
    }
}
