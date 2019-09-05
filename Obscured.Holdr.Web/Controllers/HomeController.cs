using System;
using System.Web.Mvc;
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
            if (!string.IsNullOrEmpty(category))
                categoryFolder = category + "\\";

            if(string.IsNullOrEmpty(image))
            {
                var imagePath = Server.MapPath("/Content/Images/") + categoryFolder;
                var imageFile = imageService.GetImageRandom(width, height, imagePath);
                return File(imageFile, "image/jpeg");
            }

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
