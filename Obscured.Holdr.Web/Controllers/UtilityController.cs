using System.Collections.Generic;
using System.Web.Mvc;

namespace Obscured.Holdr.Web.Controllers
{
    public class UtilityController : Controller
    {
        public ActionResult Ip()
        {
            var jsonObj = new
            {
                ip = Request.UserHostAddress,
                hostname = Request.UserHostName,
                agent = Request.UserAgent,
                vars = new Dictionary<string, string>()
            };
            
            foreach (var s in Request.ServerVariables.AllKeys)
            {
                jsonObj.vars.Add(s, Request.ServerVariables[s]);
            }

            return Json(jsonObj, JsonRequestBehavior.AllowGet);
        }
    }
}