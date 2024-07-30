using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SEWC_ToolBox.Utilities.Helpers
{
    public class RegexHelper
    {
        /// <summary>
        /// 移除富文本可能跨站攻击的标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveXSS(string content)
        {
            Dictionary<string, string> regexs = new Dictionary<string, string>();
            regexs.Add("<script['\\s\\S]*?</script>", string.Empty);
            regexs.Add("<a['\\s\\S]*?</a>", string.Empty);
            regexs.Add("<ifarme['\\s\\S]*?</ifarme>", string.Empty);
            regexs.Add("on[^>| ]+?=[\"']?[^>]+?['\"]?>", ">");
            foreach (var regex in regexs)
            {
                content = Regex.Replace(content, regex.Key, regex.Value, RegexOptions.IgnoreCase);
            }

            return content;
        }

        public static bool IsMatch(string input, string pattern)
        {
            return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(input);
        }
    }
}
