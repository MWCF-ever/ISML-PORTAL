using System.Reflection;
using System.Web;
using System.Web.Optimization;

namespace SEWC_ToolBox_Project
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            string VersionNo = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            // 自定义脚本打包
            bundles.Add(new ScriptBundle("~/Scripts/CustomScripts?v=" + VersionNo).Include(
                        "~/Scripts/CustomScripts/*.js"));
            bundles.Add(new ScriptBundle("~/Scripts/CustomScripts/Home?v=" + VersionNo).Include(
                        "~/Scripts/CustomScripts/Home/Index.js"));
            bundles.Add(new ScriptBundle("~/Scripts/CustomScripts/SCM?v=" + VersionNo).Include(
                        "~/Scripts/CustomScripts/SCM/*.js"));
            bundles.Add(new ScriptBundle("~/Scripts/CustomScripts/ToolBoxBase?v=" + VersionNo).Include(
                        "~/Scripts/CustomScripts/ToolBoxBase/ToolBoxBase.js"));
            bundles.Add(new ScriptBundle("~/Scripts/CustomScripts/ToolBoxFunctional?v=" + VersionNo).Include(
                        "~/Scripts/CustomScripts/ToolBoxFunctional/ToolBoxFunctional.js"));
            bundles.Add(new ScriptBundle("~/Scripts/CustomScripts/ToolBoxBase/Organization?v=" + VersionNo).Include(
                        "~/Scripts/CustomScripts/ToolBoxBase/Orginization.js"));
            bundles.Add(new ScriptBundle("~/Scripts/CustomScripts/ToolBoxFunctional/Reporting?v=" + VersionNo).Include(
                        "~/Scripts/CustomScripts/ToolBoxFunctional/Reporting_Function.js"));
            bundles.Add(new ScriptBundle("~/Scripts/CustomScripts/ToolBoxFunctional/QuickLink?v=" + VersionNo).Include(
                        "~/Scripts/CustomScripts/ToolBoxFunctional/QuickLink_Function.js"));
            bundles.Add(new ScriptBundle("~/Scripts/CustomScripts/ToolBoxFunctional/GetAccess?v=" + VersionNo).Include(
                        "~/Scripts/CustomScripts/ToolBoxFunctional/GetAccess_Function.js"));
            bundles.Add(new ScriptBundle("~/Scripts/CustomScripts/ToolBoxFunctional/Customization?v=" + VersionNo).Include(
                        "~/Scripts/CustomScripts/ToolBoxFunctional/Customization_Function.js"));
            bundles.Add(new ScriptBundle("~/Scripts/CustomScripts/ToolBoxFunctional/UserRole?v=" + VersionNo).Include(
                        "~/Scripts/CustomScripts/ToolBoxFunctional/UserRole_Function.js"));
            bundles.Add(new ScriptBundle("~/Scripts/CustomScripts/ToolBoxFunctional/ProcessLinkage?v=" + VersionNo).Include(
                        "~/Scripts/CustomScripts/ToolBoxFunctional/ProcessLinkage_Function.js"));
            bundles.Add(new ScriptBundle("~/Content/ueditor/config?v=" + VersionNo).Include(
                        "~/Content/ueditor/ueditor.config.js"));
            bundles.Add(new ScriptBundle("~/Content/ueditor/api?v=" + VersionNo).Include(
                        "~/Content/ueditor/editor_api.js"));
            bundles.Add(new ScriptBundle("~/Content/ueditor/lang?v=" + VersionNo).Include(
                         "~/Content/ueditor/lang/zh-cn/zh-cn.js"));

            bundles.Add(new ScriptBundle("~/Scripts/CustomScripts/Topology?v=" + VersionNo).Include(
                        "~/Scripts/CustomScripts/Topology/*.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Clipboard?v=" + VersionNo).Include(
                      "~/Scripts/clipboard/clipboard.min.js"));

            // 自定义样式打包
            bundles.Add(new StyleBundle("~/Styles/General?v=" + VersionNo).Include(
                      "~/Styles/General.css"));
            bundles.Add(new StyleBundle("~/Styles/ToolBox?v=" + VersionNo).Include(
                      "~/Styles/ToolBox.css"));

            // 第三方脚本和样式打包
            bundles.Add(new ScriptBundle("~/ThirdParty/layui?v=" + VersionNo).Include(
                        "~/ThirdParty/layui/layui.all.js"));
            bundles.Add(new StyleBundle("~/ThirdParty/layui/css?v=" + VersionNo).Include(
                      "~/ThirdParty/layui/css/layui.css"));
            bundles.Add(new StyleBundle("~/ThirdParty/layui/font?v=" + VersionNo).Include(
          "~/ThirdParty/layui/font/iconfont.woff"));

            bundles.Add(new ScriptBundle("~/ThirdParty/vis?v=" + VersionNo).Include(
                        "~/ThirdParty/vis/vis.js"));
            bundles.Add(new StyleBundle("~/ThirdParty/vis/css?v=" + VersionNo).Include(
                      "~/ThirdParty/vis/vis.css"));
        }
    }
}
