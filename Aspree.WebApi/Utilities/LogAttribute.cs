using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Aspree.WebApi.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class LogAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var baseController = actionContext.ControllerContext.Controller as Controllers.BaseController;
            if (baseController.User.Identity.IsAuthenticated)
            {
                var claimsIdentity = baseController.User.Identity as ClaimsIdentity;
                baseController.LoggedInUserId = new Guid(claimsIdentity.Claims.First(c => c.Type == "UserId").Value);
                baseController.LoggedInUserTenantId = new Guid(claimsIdentity.Claims.First(c => c.Type == "TenantId").Value);

                if (!actionContext.Request.RequestUri.AbsoluteUri.ToLower().Contains("authenticationtype") && !new Provider.Provider.LoginUserProvider().CheckIsUserApprovedBySystemAdmin(baseController.LoggedInUserId))
                {

                    // HttpContext currentContext = HttpContext.Current;
                    baseController.Request.Headers.Clear();
                    throw new Core.UnauthorizedException("Unauthorized access.");
                }



            }
            else
            { 


                HttpContext httpContext = HttpContext.Current;
                string authHeader = httpContext.Request.Headers["Authorization"];

                if (!string.IsNullOrEmpty(authHeader))
                {
                    //TODO:: Temp Fix for unautorized tockens API
                    if (authHeader == "Bearer" || authHeader == "Bearer undefined")
                        return;

                    var basicString = authHeader.Length <= 5 ? authHeader : authHeader.Substring(0, 5);
                    basicString = !string.IsNullOrEmpty(basicString) ? basicString.ToLower() : basicString;
                    basicString = basicString.Length > 1 ? basicString.First().ToString().ToUpper() + basicString.Substring(1) : basicString;
                    authHeader = authHeader.Length >= 5 ? basicString + authHeader.Substring(5) : authHeader;

                    if (authHeader != null && authHeader.StartsWith("Basic"))
                    {
                        //Extract credentials
                        string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();

                        string usernamePassword = string.Empty;
                        int seperatorIndex;
                        try
                        {
                            //the coding should be iso or you could use ASCII and UTF-8 decoder
                            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                            usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                            seperatorIndex = usernamePassword.IndexOf(':');
                        }
                        catch (Exception exception)
                        {
                            throw new Core.UnauthorizedException("Unauthorized access.");
                        }

                        try
                        {
                            string username = usernamePassword.Substring(0, seperatorIndex);
                            string password = usernamePassword.Substring(seperatorIndex + 1);

                            var result = new Provider.Provider.LoginUserProvider().CheckLoginCredentialsFromAPI(username, password);
                            if (result != null)
                            {
                                baseController.LoggedInUserId = result.Guid;
                                baseController.LoggedInUserTenantId = result.TenantId;
                            }
                            else
                            {
                                throw new Core.UnauthorizedException("Unauthorized access.");
                              
                            }
                        }
                        catch (Exception exception)
                        {
                            throw new Core.UnauthorizedException("Unauthorized access.");
                        }
                    }
                    else
                    {
                        ////Handle what happens if that isn't the case
                        throw new System.Web.Http.HttpResponseException(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                        {
                            Content = new System.Net.Http.StringContent("Unauthorized access."),
                            ReasonPhrase = "Unauthorized access."
                        });
                        
                    }
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

        }
    }
}