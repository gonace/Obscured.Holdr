using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Obscured.Holdr.Web.Controllers;

namespace Obscured.Holdr.Web.Tests.Controllers
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
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual("Welcome to ASP.NET MVC!", result.ViewBag.Message);
        }
    }
}
