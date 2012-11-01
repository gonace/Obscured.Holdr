using System;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Web.Mvc;
using System.Web.Routing;
using Obscured.Holdr.Service;

namespace Obscured.Holdr.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public FileContentResult Generate(int width, int height, string image, string category)
        {
            var imageService = ImageService.Instance();
            var categoryFolder = "boobs\\";
            if (!String.IsNullOrEmpty(category))
                categoryFolder = category + "\\";

            if(String.IsNullOrEmpty(image))
            {
                var imagePath = Server.MapPath("/Content/Images/") + categoryFolder;
                var imageFile = imageService.GetImageRandom(width, height, imagePath);
                return File(imageFile, "image/jpeg");
            }
            else
            {
                try
                {
                    byte[] imageFile;
                    var imgPath = Server.MapPath("/Content/Images/") + categoryFolder + image + ".jpg";
                    var tmpFile = imageService.GetImageByName(width, height, imgPath);
                    if (tmpFile.Length > 0)
                    {
                        imageFile = tmpFile;
                    }
                    else
                    {
                        var imagePath = Server.MapPath("/Content/Images/") + categoryFolder;
                        imageFile = imageService.GetImageRandom(width, height, imagePath);
                    }

                    return File(imageFile, "image/jpeg");
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error: " + ex.Message);
                }
            }
        }
    }
}
