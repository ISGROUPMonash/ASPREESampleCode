using Aspree.Core.ViewModels;
using Aspree.Provider.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Aspree.WebApi.Startup))]
namespace Aspree.WebApi
{

    /// <summary>
    /// 
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public static void Configuration(IAppBuilder app)
        {
            var oauthProvider = new OAuthAuthorizationServerProvider
            {
                OnGrantResourceOwnerCredentials = async context =>
                {

                    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                    //bool isTesturipath = context.Request.Uri.AbsoluteUri.ToLower().Contains("uds-test");
                    bool isTesturipath = context.Request.Uri.AbsoluteUri.ToLower().Contains(Utilities.ConfigSettings.TestSiteKeyword);

                    Core.ViewModels.RoleViewModel result = null;// new Core.ViewModels.RoleViewModel();
                    if (isTesturipath)
                    {
                        if (context.UserName == "InternalProjectLogin" && context.Password.Contains("_"))
                        {
                            result = new LoginUserProvider().CheckInternalProjectLoginCredentials(context.UserName, context.Password);
                        }
                        else
                        {
                            result = new LoginUserProvider().CheckLoginCredentials(context.UserName, context.Password, true);
                        }
                    }
                    else
                    {
                        if (context.UserName == "InternalProjectLogin" && context.Password.Contains("_"))
                        {
                            result = new LoginUserProvider().CheckInternalProjectLoginCredentials(context.UserName, context.Password);
                        }
                        else
                        {
                            result = new LoginUserProvider().CheckLoginCredentials(context.UserName, context.Password);
                        }
                    }
                    if (result != null)
                    {
                        var claimsIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                        claimsIdentity.AddClaim(new Claim("UserId", result.Guid.ToString()));
                        claimsIdentity.AddClaim(new Claim("Email", result.Email));
                        claimsIdentity.AddClaim(new Claim("TenantId", result.TenantId.ToString()));
                        var roles = new List<string>();
                        foreach (var role in result.Roles)
                        {
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                            roles.Add(role);
                        }
                        var props = new AuthenticationProperties(new Dictionary<string, string>
                        {
                            {
                                "roles", string.Join("," ,roles)
                            },
                            {
                                "name" , result.Name
                            },
                            {
                                "guid" , result.Guid.ToString()
                            },
                            {
                                "tenantId" , result.TenantId.ToString()
                            },
                            {
                                "email" , result.Email
                            },
                        });
                        context.Validated(new AuthenticationTicket(claimsIdentity, props));
                        return;
                    }
                    context.Rejected();
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                },
                OnValidateClientAuthentication = async context =>
                {
                    string clientId;
                    string clientSecret;
                    if (context.TryGetBasicCredentials(out clientId, out clientSecret))
                    {
                        if (clientId == "client" && clientSecret == "secret")
                        {
                            context.Validated();
                        }
                    }
                },
                OnTokenEndpoint = context =>
                {
                    foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
                    {
                        context.AdditionalResponseParameters.Add(property.Key, property.Value);
                    }
                    return Task.FromResult<object>(null);
                }
            };
            var oauthOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/v1/accesstoken"),
                Provider = oauthProvider,
                AuthorizationCodeExpireTimeSpan = TimeSpan.FromMinutes(1),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(30),
                SystemClock = new SystemClock(),
            };
            app.UseOAuthAuthorizationServer(oauthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
