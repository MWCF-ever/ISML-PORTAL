using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEWC_ToolBox_Project.Controllers
{
    public class ErrorController : BaseController
    {
        public ViewResult CustomError()
        {
            return View("CustomError");
        }

        public ViewResult NotFound()
        {
            ViewBag.Title = "404 Not Found";

            return View("NotFound");
        }
    }
}