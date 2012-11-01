using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Obscured.Holdr.Web.Areas.Admin.Models;
using Obscured.Holdr.BLL;

namespace Obscured.Holdr.Web.Areas.Admin.Controllers
{
    public class LogonController : Controller
    {
        //
        // GET: /Admin/Logon/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LogonModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserName.Equals("root") && model.Password.Equals("MacB00k84"))
                {
                    IEncryptionService encryptionService = new EncryptionService();
                    var hashKey = ConfigurationManager.AppSettings["hashKey"];
                    var compositeData = model.UserName + "|" + model.Password;
                    var hash = encryptionService.GetMd5HashWithKey(compositeData, hashKey);
                    var passHash = encryptionService.GetMd5HashWithKey(model.Password, hashKey);
                    var builder = new StringBuilder();
                    builder.Append(passHash).Append(hash);
                    var cookie = new HttpCookie("elmah") { Path = "/", Expires = DateTime.Now.AddHours(2) };
                    var data = builder.ToString();
                    var protectedData = ProtectedData.Protect(Encoding.UTF8.GetBytes(data), null, DataProtectionScope.CurrentUser);
                    cookie.Value = Convert.ToBase64String(protectedData);
                    Response.AppendCookie(cookie);
                    return RedirectToAction("Index", "Elmah");
                }
            }
            ModelState.AddModelError("", "Fel användarnamn och/eller lösenord");
            return View(model);
        }
    }
}
