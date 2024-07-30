using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Configuration;
using System.Web;

namespace SEWC_ToolBox_Project
{
    public class AppConfig
    {
        static ConcurrentDictionary<string, Object> CACHES = new ConcurrentDictionary<string, Object>();
        public static string[] IssueHandleUsers
        {
            get
            {
                var items = CACHES.GetOrAdd("IssueHandleUsers", (key) =>
                {
                    return ConfigurationManager.AppSettings[key].ToString();
                });

                string[] res = new string[0];

                if (items != null)
                {
                    res = items.ToString().Split(new String[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
                }

                return res ?? new string[0];
            }
        }

        /// <summary>
        /// 春燕管理源
        /// </summary>
        public static string ChunYanMail
        {
            get
            {
                var items = CACHES.GetOrAdd("CCEmail", (key) =>
                {
                    return ConfigurationManager.AppSettings[key].ToString();
                });

                return items.ToString();
            }
        }

        /// <summary>
        /// 邮箱发布接口
        /// </summary>
        public static string MailUrl
        {
            get
            {
                var items = CACHES.GetOrAdd("MailUrl", (key) =>
                {
                    return ConfigurationManager.AppSettings[key].ToString();
                });

                return items.ToString();
            }
        }

        /// <summary>
        /// 邮件发送Token
        /// </summary>
        public static string MailToken
        {
            get
            {
                var items = CACHES.GetOrAdd("MailToken", (key) =>
                {
                    return ConfigurationManager.AppSettings[key].ToString();
                });

                return items.ToString();
            }
        }

        /// <summary>
        /// 公开邮件发送人
        /// </summary>
        public static string PublicMailSender
        {
            get
            {
                var items = CACHES.GetOrAdd("PublicMailSender", (key) =>
                {
                    return ConfigurationManager.AppSettings[key].ToString();
                });

                return items.ToString();
            }
        }

        /// <summary>
        /// 是否是调试模式
        /// </summary>
        public static bool IsDev
        {
            get
            {
                var flag = CACHES.GetOrAdd("IsDev", (key) =>
                {
                    var k = ConfigurationManager.AppSettings[key];
                    if (k == null)
                    {
                        return false;
                    }
                    return Convert.ToBoolean(k.ToString());
                });

                return (bool)flag;
            }
        }

        /// <summary>
        /// 网站访问前缀
        /// </summary>
        public static string PreURL
        {
            get
            {
                var items = CACHES.GetOrAdd("PreUrl", (key) =>
                {
                    return ConfigurationManager.AppSettings[key].ToString();
                });
                var res = items.ToString();
                if (string.IsNullOrEmpty(res))
                {
                    res = "";
                }
                else
                {
                    res = $"/{res.Trim('/')}";
                }

                return res;
            }
        }

    }
}