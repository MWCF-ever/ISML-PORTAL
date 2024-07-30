using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.Utilities.Mail
{
    public interface IMaillSend
    {
        /// <summary>
        /// 启动发送
        /// </summary>
        /// <param name="item">发送内容</param>
        /// <param name="parameter">参数 支持匿名函数  用于替换，格式 $$.属性名</param>
        /// <returns></returns>
        bool Send(MailItem item, object parameter = null);

        /// <summary>
        /// 重置Item内容
        /// </summary>
        Action<MailItem> ResetItemAction { get; set; }
    }
}
