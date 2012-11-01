using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Obscured.Holdr.BLL;

namespace Obscured.Holdr.Web.Filter
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
            var rawUrl = filterContext.HttpContext.Request.RawUrl;
            var urlEncodedUrl = HttpUtility.UrlEncode(rawUrl, Encoding.UTF8);
            filterContext.Result = new RedirectResult("/Admin/Logon?returnUrl=" + urlEncodedUrl);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var httpCookie = httpContext.Request.Cookies.Get("elmah");
            if (httpCookie == null)
            {
                return false;
            }
            var cookieValue = httpCookie.Value;
            var hashKey = ConfigurationManager.AppSettings["hashKey"];
            IEncryptionService encryptionService = new EncryptionService();
            const string compareString = "root|MacB00k84";
            var hash = encryptionService.GetMd5HashWithKey(compareString, hashKey);
            var pwHash = encryptionService.GetMd5HashWithKey("MacB00k84", hashKey);
            var builder = new StringBuilder();
            builder.Append(pwHash).Append(hash);
            var encyptedData = Convert.FromBase64String(cookieValue);
            var decryptedData = ProtectedData.Unprotect(encyptedData, null, DataProtectionScope.CurrentUser);
            var plainText = Encoding.UTF8.GetString(decryptedData);
            return plainText.Equals(builder.ToString());
        }
    }
}