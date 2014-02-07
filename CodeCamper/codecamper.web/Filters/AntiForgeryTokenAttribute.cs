using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Web.Helpers;
using System.Net.Http;
using System.Net;

namespace codecamper.web.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AntiForgeryTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            var headerToken = actionContext
                .Request
                .Headers
                .Where(z => z.Key.Equals(AntiForgeryConfig.CookieName, StringComparison.OrdinalIgnoreCase))
                .Select(z => z.Value)
                .SelectMany(z => z)
                .FirstOrDefault();

            var cookieToken = actionContext
                .Request
                .Headers
                .GetCookies()
                .Select(c => c[AntiForgeryConfig.CookieName])
                .FirstOrDefault();

            if (cookieToken == null || headerToken == null)
            {
                actionContext.Response =
                    actionContext.Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                actionContext.Response.ReasonPhrase = "Missing token null";
                return;
            }

            try
            {
                AntiForgery.Validate(cookieToken.Value, headerToken);
            }
            catch
            {
                actionContext.Response =
                    actionContext.Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                actionContext.Response.ReasonPhrase = "Invalid token";
                return;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}