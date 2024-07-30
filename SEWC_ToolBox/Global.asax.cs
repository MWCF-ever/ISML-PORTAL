using SEWC_NetDevLib.SEWC_NetLibExtend;
using System;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace SEWC_ToolBox_Project
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                string VersionNo = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                Application.Add("VersionNo", VersionNo);

                AreaRegistration.RegisterAllAreas();
                GlobalConfiguration.Configure(WebApiConfig.Register);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
                BundleTable.EnableOptimizations = false;
                // 加载缓存数据
                CacheConfig.Initial();
            }
            catch(Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
            }
        }

        public override void Init()
        {
            PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            base.Init();
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }


        public void Application_AuthenticateRequest()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                CustomPrincipal principal = new CustomPrincipal(authTicket.Name);
                HttpContext.Current.User = principal;
            }
        }
    }
}
