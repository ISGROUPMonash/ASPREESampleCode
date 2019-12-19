using Aspree.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Aspree.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly Services.WebApiHandler _webApi;
        public BaseController()
        {
            _webApi = new Services.WebApiHandler();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (this.User != null && this.User.Identity.IsAuthenticated)
            {
                this.LoggedInUser = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.LoggedInUser>(this.User.Identity.Name);
                ViewBag.AccessToken = Session["AccessToken"];


                string projectDisplayName_Base = string.Empty;
                if (Session["ProjectId"] != null)
                {
                    ViewBag.ProjectId = Session["ProjectId"].ToString();

                    #region Get project
                    var projectStatus = _webApi.Get("Project/GetProjectBasicDetails/" + ViewBag.ProjectId);
                    if(projectStatus.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        new AccountController().ClearSession();
                        filterContext.Result = new RedirectResult("/Account/Login");
                    }
                    if (projectStatus.MessageType == "Success")
                    {
                        ProjectBasicDetailsViewModel project = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectBasicDetailsViewModel>(projectStatus.Content);
                        ViewBag.ProjectLogo = !string.IsNullOrEmpty(project.ProjectLogo) ? project.ProjectLogo : string.Empty;
                        ViewBag.ProjectColor = !string.IsNullOrEmpty(project.ProjectColor) ? project.ProjectColor : string.Empty;
                        
                        this.LoggedInUser.ProjectDisplayName = !string.IsNullOrEmpty(project.ProjectDisplayName) ? project.ProjectDisplayName : project.Name;
                        this.LoggedInUser.ProjectGuid = project.Guid != null ? project.Guid : Guid.Empty;
                        this.LoggedInUser.ProjectDisplayNameTextColour = !string.IsNullOrEmpty(project.ProjectDisplayNameTextColour) ? project.ProjectDisplayNameTextColour : string.Empty;
                    }
                    #endregion
                }
                ViewBag.LoggedInUser = this.LoggedInUser;
            }
        }

        protected void SetErrorMessage(string message)
        {
            TempData["Error"] = message;
        }

        protected void SetSuccessMessage(string message)
        {
            TempData["Success"] = message;
        }

        protected LoggedInUser LoggedInUser { get; set; }

        protected string BaseUrl
        {
            get
            {
                return $"{Request.Url.Scheme}://{Request.Url.Authority}";
            }
        }


        public bool WriteLog(string logMessage,
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string caller = null)
        {
            try
            {
                logMessage = "Line:" + lineNumber + "#Caller:" + caller + "\t " + logMessage + "\t " + GetLocalIPAddress();
                System.IO.FileStream objFilestream = new System.IO.FileStream(string.Format("{0}\\{1}", Server.MapPath("~/"), "log-aspree-web.log"), System.IO.FileMode.Append, System.IO.FileAccess.Write);
                System.IO.StreamWriter objStreamWriter = new System.IO.StreamWriter((System.IO.Stream)objFilestream);
                objStreamWriter.WriteLine(logMessage);
                objStreamWriter.Close();
                objFilestream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string GetLocalIPAddress()
        {

            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return ("No network adapters with an IPv4 address in the system!");
        }
    }
}