using SEWC_ToolBox_Project.Filters;
using System.Web.Mvc;

namespace SEWC_ToolBox_Project
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // 注释掉默认HandledError属性，使用我们自定义的CustomHandleError属性，并指定错误信息展示页面
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomHandleErrorAttribute() { View = "~/Views/Error/CustomError.cshtml" });
        }
    }
}
