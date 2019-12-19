using Aspree.ExtensionClasses;
using Aspree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aspree.Controllers
{
    //[CustomAuthorizeAttribute(Roles = "Definition Admin, System Admin, Project Admin, Data Entry Supervisor, Data Entry Operator")]
    [CustomAuthorizeAttribute]
    public class HomeController : BaseController
    {
        private readonly WebApiHandler _webApi;

        public HomeController()
        {
            _webApi = new WebApiHandler();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult ListOfProject()
        {
            ViewBag.LoggedInUserGuid = this.LoggedInUser.Guid;
            ViewBag.LoggedInUserRoles = this.LoggedInUser.Roles.FirstOrDefault();
            ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            var projects = new List<Core.ViewModels.FormDataEntryProjectsViewModel>();

            var activityResponse = new Utility.ResponseMessage();

            bool isTesturipath = Request.Url.AbsoluteUri.ToLower().Contains(Utility.ConfigSettings.TestSiteKeyword);
            if (isTesturipath)
            {
                activityResponse = _webApi.Get("Review/GetAllDeployedProject");
            }
            else
            {
                activityResponse = _webApi.Get("ProjectDeploy/GetAllDeployedProject");
            }
            if (activityResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/LogOut");
            }

            if (activityResponse.MessageType == "Success")
            {
                projects = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.FormDataEntryProjectsViewModel>>(activityResponse.Content);
            }

            TempData["SummaryPageParticipantId"] = null;
            TempData["SummaryPageFormId"] = null;
            TempData["SummaryPageGuid"] = null;

            var applicationrole = _webApi.Get("User/" + this.LoggedInUser.Guid);
            if (applicationrole.MessageType == "Success")
            {
                var urole = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.UserLoginViewModel>(applicationrole.Content);
                if (urole != null && urole.RoleName == Aspree.Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " "))
                    ViewBag.LoggedInUserRoles = urole.RoleName;
            }
            return View(projects);
        }

        [HttpPost]
        public JsonResult ListOfProject(Guid projectGuid)
        {
            #region re-login according to project
            var user = _webApi.Get("User/" + this.LoggedInUser.Guid);
            var userDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.UserLoginViewModel>(user.Content);
            var userId_ProjectId = userDetail.Guid + "_" + projectGuid;



            string absolutePath = string.Empty;
            try
            {
                bool isTesturipath = Request.Url.AbsoluteUri.ToLower().Contains(Utility.ConfigSettings.TestSiteKeyword);
                absolutePath = isTesturipath ? "test/" : "";
            }
            catch (Exception exc) { WriteLog("AbsoluteUri exception::" + exc); }
            var response = new Utility.ResponseMessage();
            if (absolutePath == "test/")
            {
                response = _webApi.InternalLogin("InternalProjectLogin", userId_ProjectId, "password", true);
            }
            else
            {
                response = _webApi.InternalLogin("InternalProjectLogin", userId_ProjectId);
            }
            if (response.MessageType == "Success")
            {
                var accessToken = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.AccessToken>(response.Content);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(new Core.ViewModels.LoggedInUser
                {
                    Email = accessToken.email,
                    Guid = accessToken.guid,
                    Name = accessToken.name,
                    TenantId = accessToken.tenantId,
                    Roles = !string.IsNullOrEmpty(accessToken.roles) ? accessToken.roles.Split(',').ToList() : "Admin".Split(',').ToList()
                });
                Session["AccessToken"] = accessToken.access_token;
                System.Web.Security.FormsAuthentication.SetAuthCookie(json, false);
            }
            #region Get project
            var projectStatus = _webApi.Get("FormDataEntry/" + projectGuid);
            if (projectStatus.MessageType == "Success")
            {
                var project = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.FormDataEntryViewModel>(projectStatus.Content);
                string logo = string.Empty;
                string color = string.Empty;
                try
                {
                    var projectLogo = project.FormDataEntryVariable != null ? project.FormDataEntryVariable.FirstOrDefault(x => x.VariableName == "ProjectLogo") : null;
                    logo = projectLogo != null ? projectLogo.SelectedValues : string.Empty;
                    Session["ProjectLogo"] = !string.IsNullOrEmpty(logo) ? logo : null;
                }
                catch (Exception exc) { }
                try
                {
                    var projectColor = project.FormDataEntryVariable != null ? project.FormDataEntryVariable.FirstOrDefault(x => x.VariableName == "ProjectColor") : null;
                    color = projectColor != null ? projectColor.SelectedValues : string.Empty;
                    Session["ProjectColor"] = !string.IsNullOrEmpty(color) ? color : null;
                }
                catch (Exception exc) { }

                Session["ProjectId"] = projectGuid;
            }
            else
            {
                
            }
            #endregion
            #endregion
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RedirectSuccess(string message)
        {
            SetSuccessMessage(message);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}