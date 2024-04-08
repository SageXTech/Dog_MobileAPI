using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace DogAPI.Areas.Admin.Data
{
    public class SessionExpire : ActionFilterAttribute, IRequiresSessionState
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["UserId"] == null)
            {
                FormsAuthentication.SignOut();
                //var routeValues = new RouteValueDictionary();
                //routeValues["controller"] = "Login";
                //routeValues["action"] = "Login";
                //filterContext.Result = new RedirectToRouteResult(routeValues);
                filterContext.Result = new RedirectResult("~/Admin/Login/Login");
                return;
            }
        }
    }
}