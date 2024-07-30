using SEWC_NetDevLib.SEWC_NetLibExtend;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SEWC_ToolBox.Languages
{
    public enum LangType
    {
        zh,
        en
    }

    public static class LanguageHelper
    {
        /// <summary>
        /// 在外边的 Html 中直接使用
        /// </summary>
        /// <param name="htmlhelper"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Lang(this HtmlHelper htmlhelper, string key)
        {
            return GetLangString(htmlhelper.ViewContext.HttpContext, key);
        }

        /// <summary>
        /// 在外边的 Html 中直接使用，对 JS 进行输出字符串
        /// </summary>
        /// <param name="htmlhelper"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string LangOutJsVar(this HtmlHelper htmlhelper, string key)
        {
            string langstr = GetLangString(htmlhelper.ViewContext.HttpContext, key);
            return string.Format("var {0} = '{1}'", key, langstr);
        }

        /// <summary>
        /// 在 C# 中使用
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string InnerLang(HttpContextBase httpContext, string key)
        {
            return GetLangString(httpContext, key);
        }

        public static string InnerLang(HttpContext httpContext, string key)
        {
            return GetLangString(httpContext, key);
        }

        private static string GetLangString(HttpContextBase httpContext, string key)
        {
            // Session 无值时默认语言英语
            LangType langtype = LangType.en;
            if (httpContext.Session["CurrentLanguage"] != null)
            {
                langtype = (LangType)httpContext.Session["CurrentLanguage"];
            }

            return LangResourceFileProvider.GetLangString(key, langtype);
        }

        private static string GetLangString(HttpContext httpContext, string key)
        {
            // Session 无值时默认语言英语
            LangType langtype = LangType.en;
            if (httpContext.Session["CurrentLanguage"] != null)
            {
                langtype = (LangType)httpContext.Session["CurrentLanguage"];
            }

            return LangResourceFileProvider.GetLangString(key, langtype);
        }
    }

    public static class LangResourceFileProvider
    {
        private static ResourceManager rm { get; set; }

        static LangResourceFileProvider()
        {
            rm = new ResourceManager("SEWC_ToolBox.Languages.Resource.lang", Assembly.GetExecutingAssembly());
        }
        
        public static string GetLangString(string Key, LangType CurrentLang)
        {
            string result = string.Empty;

            try
            {
                string culture = string.Empty;

                switch (CurrentLang)
                {
                    case LangType.zh: culture = "zh-CN"; break;
                    case LangType.en: culture = "en-US"; break;
                    default: culture = "en-US"; break;
                }

                Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
                result = rm.GetString(Key);
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                result = Key;
            }

            return result;
        }
    }
}