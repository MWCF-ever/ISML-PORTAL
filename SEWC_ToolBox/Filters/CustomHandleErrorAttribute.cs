using SEWC_NetDevLib.SEWC_NetLibExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEWC_ToolBox_Project.Filters
{
    public class CustomHandleErrorAttribute: HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            // 获取异常信息
            string excepitonInfo = filterContext.Exception.ToString();
            
            // 记录日志
            EventlogHelper.AddLog(excepitonInfo);
        }
    }
}