using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEWC_ToolBox_Project;
using SEWC_ToolBox_Project.Controllers;

namespace SEWC_ToolBox_Project.UnitTests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.HomePage() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void PersonalInformation()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.PersonalInformation() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
