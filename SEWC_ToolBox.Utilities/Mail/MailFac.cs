using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.Utilities.Mail
{
    public class MailFac
    {
        public static IMaillSend GetMailHttp()
        {
            return new MailSendHttp();
        }
    }
}
