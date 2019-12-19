using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Aspree.Models;
using System.Data.Entity;
using System.Collections.Generic;
using System.Web.Security;
using Aspree.Services;
using Okta.AspNet;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json;
using Aspree.ExtensionClasses;

namespace Aspree.Controllers
{
    [CustomAuthorizeAttribute]
    public class AccountController : BaseController
    {
        private readonly WebApiHandler _webApi;

        public AccountController()
        {
            _webApi = new WebApiHandler();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(Guid guid)
        {
            bool isTestSite = false;
            try
            {
                isTestSite = Request.Url.AbsoluteUri.ToLower().Contains(Utility.ConfigSettings.TestSiteKeyword);
            }
            catch (Exception exc) { WriteLog("AbsoluteUri exception::" + exc); }

            Utility.ResponseMessage userResponse = new Utility.ResponseMessage();
            if (isTestSite)
            {
                userResponse = _webApi.Get("Review/GetUserByTempGuid/" + guid.ToString());
            }
            else
            {
                userResponse = _webApi.Get("user/GetUserByTempGuid/" + guid.ToString());
            }

            if (userResponse.MessageType == "Success")
            {
                var user = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.UserLoginViewModel>(userResponse.Content);
                if (!string.IsNullOrEmpty(user.Answer))
                {
                    FormsAuthentication.SignOut();
                    SetErrorMessage("Your account is already verified. Please login.");
                    return RedirectToRoute("Default");
                }
                guid = user.Guid;
            }
            else
            {
                FormsAuthentication.SignOut();
                SetErrorMessage("This link is either expired or you have already verified. Please login  or click on forgot password");
                return Redirect("/");
            }
            var queResponse = _webApi.Get("securityquestion");
            if (queResponse.MessageType == "Success")
            {
                ViewBag.SecurityQuestions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.SecurityQuestionViewModel>>(queResponse.Content);
            }
            var model = new Aspree.Core.ViewModels.ResetPassword()
            {
                Guid = guid,
            };
            ViewBag.Guid = guid;
            return View(model);
        }


        [CustomAuthorizeAttribute(Roles = "Definition Admin, System Admin, Project Admin")]
        public ActionResult ManageProfile()
        {
            var user = new Models.UserViewModel();
            var roles = new List<Core.ViewModels.RoleModel>();
            var queResponse = _webApi.Get("securityquestion");
            if (queResponse.MessageType == "Success")
            {
                ViewBag.SecurityQuestions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.SecurityQuestionViewModel>>(queResponse.Content);
            }
            var userResponse = _webApi.Get("User/" + this.LoggedInUser.Guid.ToString());
            if (userResponse.MessageType == "Success")
            {
                var userEdit = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.UserLoginViewModel>(userResponse.Content);
                user.UserGuid = userEdit.Guid;
                user.Address = userEdit.Address;
                user.Email = userEdit.Email;
                user.FirstName = userEdit.FirstName;
                user.LastName = userEdit.LastName;
                user.Mobile = userEdit.Mobile;
                user.RoleId = userEdit.RoleId;
                user.UserId = userEdit.Id;
                user.Answer = userEdit.Answer;
                user.QuestionId = userEdit.SecurityQuestionId;
                user.UserName = userEdit.UserName;
                user.Status = userEdit.Status != null ? (int)userEdit.Status : (int)Core.Enum.Status.InActive;
                user.IsUserApprovedBySystemAdmin = userEdit.IsUserApprovedBySystemAdmin;
            }
            var response = _webApi.Get("Role");
            if (response.MessageType == "Success")
            {
                roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.RoleModel>>(response.Content).ToList();

                List<SelectListItem> items = new List<SelectListItem>();

                foreach (var r in roles.Where(r => r.IsSystemRole))
                {
                    items.Add(new SelectListItem { Text = r.Name, Value = r.Guid.ToString() });
                }
                user.Role = items;

                user.TenantId = this.LoggedInUser.TenantId;

                return View(user);
            }

            return View(user);
        }


        // GET: /Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string returnUrl)
        {
            if (returnUrl != null && returnUrl.Contains("logout"))
                returnUrl = null;
            if (this.User != null && this.User.Identity.IsAuthenticated)
            {
                return Redirect("/Home/ListOfProject");
            }
            var authenticationEndpointRespons = _webApi.Get("AuthenticationType");
            if (authenticationEndpointRespons.MessageType == "Success")
            {
                List<Core.ViewModels.AuthenticationTypeViewModel> allAuthTypes = JsonConvert.DeserializeObject<List<Core.ViewModels.AuthenticationTypeViewModel>>(authenticationEndpointRespons.Content);
                ViewBag.AuthenticationTypes = allAuthTypes.Where(x => x.Status == (int)Core.Enum.Status.Active).ToList();
            }
            ViewBag.ReturnUrl = returnUrl;


            if (Request.Cookies["AspreeLogin"] != null)
            {
                LoginViewModel model = new LoginViewModel();
                model.Email = Request.Cookies["AspreeLogin"].Values["username"];
                model.RememberMe = true;
                return View(model);
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult ThankYou()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.Password = model.Password.Trim();
            string absolutePath = string.Empty;
            try
            {
                bool isTesturipath = Request.Url.AbsoluteUri.ToLower().Contains(Utility.ConfigSettings.TestSiteKeyword);
                absolutePath = isTesturipath ? "test/" : "";
            }
            catch (Exception exc) { WriteLog("AbsoluteUri exception::" + exc); }
            ViewBag.ReturnUrl = returnUrl;

            var loginHandler = new Services.WebApiHandler();
            var response = new Utility.ResponseMessage();
            if (absolutePath == "test/")
            {
                response = loginHandler.Login(model.Email, model.Password, "password", true);
            }
            else
            {
                response = loginHandler.Login(model.Email, model.Password);
            }
            if (response.MessageType == "Success")
            {
                var accessToken = JsonConvert.DeserializeObject<Core.ViewModels.AccessToken>(response.Content);
                var json = JsonConvert.SerializeObject(new Core.ViewModels.LoggedInUser
                {
                    Email = accessToken.email,
                    Guid = accessToken.guid,
                    Name = accessToken.name,
                    TenantId = accessToken.tenantId,
                    Roles = !string.IsNullOrEmpty(accessToken.roles) ? accessToken.roles.Split(',').ToList() : "Admin".Split(',').ToList()
                });
                Session["AccessToken"] = accessToken.access_token;
                FormsAuthentication.SetAuthCookie(json, model.RememberMe);

                if (model.RememberMe)
                {
                    HttpCookie cookie = new HttpCookie("AspreeLogin");
                    cookie.Values.Add("username", model.Email);
                    cookie.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    if (Request.Cookies["AspreeLogin"] != null)
                    {
                        if (Request.Cookies["AspreeLogin"].Values["username"] == model.Email)
                        {
                            HttpCookie myCookie = new HttpCookie("AspreeLogin");
                            myCookie.Expires = DateTime.Now.AddDays(-1d);
                            Response.Cookies.Add(myCookie);
                        }
                    }
                }


                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return Redirect("/Home/ListOfProject");
            }
            else if (model.Password == "{{okta}}")
            {
                TempData["Error"] = "Institutional Login Failed, User is not registered in the system.";
                return Redirect("/Account/Login");
            }
            else if (model.Password == "{{GoogleLogin}}")
            {
                TempData["Error"] = "Google Login Failed, User is not registered in the system.";
                return Redirect("/Account/Login");
            }
            else
            {
                var authenticationEndpointRespons = _webApi.Get("AuthenticationType");
                if (authenticationEndpointRespons.MessageType == "Success")
                {
                    ViewBag.AuthenticationTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.AuthenticationTypeViewModel>>(authenticationEndpointRespons.Content);
                }

                var getUserStatusRespons = new Utility.ResponseMessage();
                if (absolutePath == "test/")
                {
                    getUserStatusRespons = _webApi.Get("user/GetUserStatus/" + model.Email + "/true/");
                }
                else
                {
                    getUserStatusRespons = _webApi.Get("user/GetUserStatus/" + model.Email + "/false/");
                }
                int status = 0;
                if (getUserStatusRespons.MessageType == "Success")
                {
                    status = JsonConvert.DeserializeObject<int>(getUserStatusRespons.Content);
                }

                if (status == (int)Core.Enum.Status.Locked)
                {
                    TempData["Error"] = "Your account has been locked. Please contact a system administrator for assistance.";
                }
                else
                {
                    ModelState.AddModelError("Password", "Invalid username or password");
                }
            }
            return View(model);
        }

        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }


        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = SignInStatus.Success; //await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }


        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new Models.RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            return View();
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword(Guid guid)
        {
            bool isTestSite = false;
            try
            {
                isTestSite = Request.Url.AbsoluteUri.ToLower().Contains(Utility.ConfigSettings.TestSiteKeyword);
            }
            catch (Exception exc) { WriteLog("AbsoluteUri exception::" + exc); }

            Utility.ResponseMessage userResponse = new Utility.ResponseMessage();
            if (isTestSite)
            {
                userResponse = _webApi.Get("Review/GetUserByTempGuid/" + guid.ToString());
            }
            else
            {
                userResponse = _webApi.Get("user/GetUserByTempGuid/" + guid.ToString());
            }
            var user = new Core.ViewModels.UserLoginViewModel();
            if (userResponse.MessageType == "Success")
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.UserLoginViewModel>(userResponse.Content);
                if (user.Password != null)
                {
                    FormsAuthentication.SignOut();
                    SetErrorMessage("This link is either expired or you have already set it. Please login  or click on forgot password.");
                    return Redirect("/account/login");
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                SetErrorMessage("This link is either expired or you have already set it. Please login  or click on forgot password");
                return Redirect("/account/login");
            }

            ViewBag.Guid = user.Guid;
            ViewBag.QuestionGuid = user.SecurityQuestionId;
            var queResponse = _webApi.Get("securityquestion");
            if (queResponse.MessageType == "Success")
            {
                ViewBag.SecurityQuestions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.SecurityQuestionViewModel>>(queResponse.Content);
            }
            var model = new Aspree.Core.ViewModels.ResetPassword()
            {
                Answer = user.Answer,
                Guid = user.Guid,
                QuestionGuid = user.SecurityQuestionId
            };
            return View(model);
        }


        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            return View();
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(Guid provider, string returnUrl)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                var authenticationEndpointRespons = _webApi.Get("AuthenticationType/" + provider);
                if (authenticationEndpointRespons.MessageType == "Success")
                {
                    var AuthenticationData = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.AuthenticationTypeViewModel>(authenticationEndpointRespons.Content);
                    var redirecturl = string.Empty;
                    switch (AuthenticationData.AuthType)
                    {
                        case (int)Aspree.Core.Enum.AuthenticationTypes.OpenID_Connect:
                            redirecturl = $"{AuthenticationData.AuthorizeEndpoint}authorize?client_id={AuthenticationData.ClientId}&response_type=code&scope={AuthenticationData.Scope}&redirect_uri={HttpUtility.UrlEncode(BaseUrl + "/account/externallogincallback")}&state={AuthenticationData.State}";
                            if (AuthenticationData.AuthorizeEndpoint.ToLower().Contains("google"))
                            {
                                redirecturl = $"{AuthenticationData.AuthorizeEndpoint}auth?client_id={AuthenticationData.ClientId}&response_type=code&scope={AuthenticationData.Scope}&redirect_uri={HttpUtility.UrlEncode(BaseUrl + "/account/externallogincallback")}&state={AuthenticationData.State}";
                            }

                            break;
                        default:
                            break;
                    }
                    return Redirect(redirecturl);
                }
            }

            return Redirect("/home/ListOfProject");
        }


        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            return View();
        }

        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> ExternalLoginCallback(string code)
        {
            string state = Request.QueryString["state"];
            Utility.ResponseMessage authenticationTypeResponse = _webApi.Get("AuthenticationType/GetAuthenticationTypeByState/" + state);
            Core.ViewModels.AuthenticationTypeViewModel authTypeMaster = new Core.ViewModels.AuthenticationTypeViewModel();

            string passwordType = "{{GoogleLogin}}";

            if (authenticationTypeResponse.MessageType == "Success")
            {
                authTypeMaster = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.AuthenticationTypeViewModel>(authenticationTypeResponse.Content);

                string authType = authTypeMaster != null ? authTypeMaster.AuthorizeEndpoint : string.Empty;
                if (authType.ToLower().Contains("google"))
                {
                    passwordType = "{{GoogleLogin}}";
                }
                else if (authType.ToLower().Contains("okta"))
                {
                    passwordType = "{{okta}}";
                }
            }

            var token = new Core.ViewModels.OktaAccessToken();
            var user = new Core.ViewModels.OktaUserViewModel();

            var tokenResponse = _webApi.GetGoogleToken(code, $"{BaseUrl}/account/externallogincallback", authTypeMaster.ClientId, authTypeMaster.ClientSecret, authTypeMaster.TokenEndpoint);
            if (tokenResponse.MessageType == "Success")
            {
                token = JsonConvert.DeserializeObject<Core.ViewModels.OktaAccessToken>(tokenResponse.Content);
            }
            else
            {
                TempData["Error"] = "Institutional Login Failed";
                return Redirect("/Account/Login");
            }

            var userResponse = _webApi.GetGoogleUserInfo("userinfo", token.access_token, authTypeMaster.UserinfoEndpoint);

            if (userResponse.MessageType == "Success")
            {
                user = JsonConvert.DeserializeObject<Core.ViewModels.OktaUserViewModel>(userResponse.Content);
            }
            else
            {
                TempData["Error"] = "Institutional Login Failed";
                return Redirect("/Account/Login");
            }

            string authenticationUsername = string.Empty;
            switch (authTypeMaster.AuthenticationProviderClaim)
            {
                case "sub":
                    authenticationUsername = user.sub;
                    break;
                case "email":
                    authenticationUsername = user.email;
                    break;
                case "preferred_username":
                    authenticationUsername = user.preferred_username;
                    break;
                case "given_name":
                    authenticationUsername = user.given_name;
                    break;
                case "family_name":
                    authenticationUsername = user.family_name;
                    break;
                case "zoneinfo":
                    authenticationUsername = user.zoneinfo;
                    break;
                case "phone_number":
                    authenticationUsername = user.phone_number;
                    break;
                case "name":
                    authenticationUsername = user.name;
                    break;
                case "updated_at":
                    authenticationUsername = Convert.ToString(user.updated_at);
                    break;
                default:
                    break;
            }
            return await Login(new LoginViewModel { Email = authenticationUsername, Password = passwordType }, "");
        }
 
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff(bool expired = false)
        {
            FormsAuthentication.SignOut();
            Session["AccessToken"] = null;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(
                    CookieAuthenticationDefaults.AuthenticationType,
                    OktaDefaults.MvcAuthenticationType);
            }

            //Session.Abandon();
            if (expired)
            {
                TempData["Error"] = "Your browser session has been expired.";
            }
            else
            {
                TempData["Success"] = "You have logged out successfully.";
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult LogOut(bool expired = false)
        {
            FormsAuthentication.SignOut();
            Session["AccessToken"] = null;
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            this.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            this.Response.Cache.SetNoStore();

            this.Response.Cache.AppendCacheExtension("no-store, must-revalidate");
            this.Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
            this.Response.AppendHeader("Expires", "0"); // Proxies.


            if (Request.Cookies["AspreeLogin"] != null)
            { 
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(
                    CookieAuthenticationDefaults.AuthenticationType,
                    OktaDefaults.MvcAuthenticationType);
            }

            //Session.Abandon();
            if (expired)
            {
                TempData["Error"] = "Your browser session has been expired.";
            }
            else
            {
                TempData["Success"] = "You have logged out successfully.";
            }
            return RedirectToRoute("Default");
        }

        public ActionResult PostLogout()
        {
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }


        public bool ClearSession()
        {
            try
            {
                FormsAuthentication.SignOut();
                Session["AccessToken"] = null;
                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();
                this.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
                this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                this.Response.Cache.SetNoStore();
                this.Response.Cache.AppendCacheExtension("no-store, must-revalidate");
                this.Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
                this.Response.AppendHeader("Expires", "0"); // Proxies.
                return true;
            }
            catch (Exception exception)
            {
                WriteLog(exception.Message);
                return false;
            }
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}