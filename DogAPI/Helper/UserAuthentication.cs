using DogsEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DogAPI.Helper
{
    public class UserAuthentication: AuthorizationFilterAttribute
    {
        DogCRMEntities dbContext = new DogCRMEntities();
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                try
                {
                    string authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                    //string decodedAuthenticationToken= Encoding.UTF8.GetString(
                    //    Convert.FromBase64String(authenticationToken));
                    string decodedAuthenticationToken = authenticationToken;
                    string[] keyAuthTokenArray = decodedAuthenticationToken.Split(':');
                    string authToken = keyAuthTokenArray[0];
                    int userId = Convert.ToInt32(keyAuthTokenArray[1]);
                    if (dbContext.UserTokens.Any(cust => cust.UniqueId == authToken && cust.UserId == userId))
                    {
                        Thread.CurrentPrincipal = new GenericPrincipal(
                           new GenericIdentity(userId.ToString()), null);
                    }
                    else
                    {
                        actionContext.Response = actionContext.Request
                            .CreateResponse(HttpStatusCode.Unauthorized);
                    }
                }
                catch (Exception Ex)
                {
                    actionContext.Response = actionContext.Request
                            .CreateResponse(HttpStatusCode.InternalServerError, Ex.ToString());
                }
            }
        }
    }
}