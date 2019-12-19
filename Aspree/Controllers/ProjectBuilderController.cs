using Aspree.ExtensionClasses;
using Aspree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aspree.Controllers
{
    [CustomAuthorizeAttribute(Roles = "Project Admin, System Admin")]
    public class ProjectBuilderController : BaseController
    {

        private readonly WebApiHandler _webApi;

        public ProjectBuilderController()
        {
            _webApi = new WebApiHandler();
        }

        // GET: ProjectBuilder
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Variables(Guid? guid = null)
        {
            if (guid == null)
            {
                if (Session["ProjectId"] != null)
                {
                    guid = new Guid(Session["ProjectId"].ToString());
                }
                else
                {
                    Session["ProjectId"] = null;
                    return Redirect("/Home/ListOfProject");
                }
            }
            Session["ProjectId"] = guid.ToString();

            //get project name by guid
            var categories = new List<Core.ViewModels.VariableCategoryViewModel>();
            var projectBuilderVariables = new Core.ViewModels.ProjectBuilderVariablesViewModel();
            var roleResponse = _webApi.Get("Variable/ProjectBuilderVariables/" + guid);
            if (roleResponse.MessageType == "Success")
            {
                projectBuilderVariables = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.ProjectBuilderVariablesViewModel>(roleResponse.Content);
                categories = projectBuilderVariables.VariableCategory.OrderBy(i => i.IsDefaultVariableCategory).ThenBy(i => i.CategoryName).ToList();
                ViewBag.Roles = projectBuilderVariables.Role;
                ViewBag.VariableTypes = projectBuilderVariables.VariableType;
                ViewBag.ValidationRules = projectBuilderVariables.ValidationRule;

                ViewBag.LookupVariablesPreviewViewModelList = projectBuilderVariables.LookupVariablesPreviewViewModelList;
            }
            var dateFormate = from Core.Enum.DateFormat e in Enum.GetValues(typeof(Core.Enum.DateFormat))
                              select new
                              {
                                  Id = e.ToString().Replace("_", ""),
                                  Name = e.ToString().Replace("_", "-")
                              };
            ViewBag.DateFormat = new SelectList(dateFormate, "Id", "Name");

            return View("~/Views/Variables/Index.cshtml", categories);
        }

        public ActionResult Forms(Guid? guid = null)
        {
            if (guid == null)
            {
                Session["ProjectId"] = null;
                SetErrorMessage("Please select project.");
                return Redirect("/Home/ListOfProject");
            }
            //get project name by guid
            var categories = new List<Core.ViewModels.FormCategoryViewModel>();
            var projectBuilderForms = new Core.ViewModels.ProjectBuilderFormsViewModel();
            var roleResponse = _webApi.Get("Form/ProjectBuilderForms/" + guid);
            if (roleResponse.MessageType == "Success")
            {
                projectBuilderForms = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.ProjectBuilderFormsViewModel>(roleResponse.Content);
                categories = projectBuilderForms.FormCategory.OrderBy(cn => cn.IsDefaultFormCategory).ThenBy(cn => cn.CategoryName).ToList();
                ViewBag.VariableCategories = projectBuilderForms.VariableCategories.OrderBy(vc => vc.IsDefaultVariableCategory).ThenBy(vc => vc.CategoryName).ToList();
                ViewBag.Roles = projectBuilderForms.Roles;
                ViewBag.EntityTypes = projectBuilderForms.EntityTypes;
            }
            var currentUser = _webApi.Get("User/" + this.LoggedInUser.Guid);
            if (currentUser.MessageType == "Success")
            {
                var currentUserDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.UserLoginViewModel>(currentUser.Content);
                ViewBag.CreatedBy = currentUserDetail.Id;
            }
            return View(categories);
        }

        public ActionResult Activities(Guid? guid = null)
        {
            if (guid == null)
            {
                Session["ProjectId"] = null;
                SetErrorMessage("Please select project.");
                return Redirect("/Home/ListOfProject");
            }
            //get project name by guid
            var categories = new List<Core.ViewModels.ActivityCategoryViewModel>();
            var projectBuilderActivity = new Core.ViewModels.ProjectBuilderActivityViewModel();
            var roleResponse = _webApi.Get("Activity/ProjectBuilderActivities/" + guid);
            if (roleResponse.MessageType == "Success")
            {
                projectBuilderActivity = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.ProjectBuilderActivityViewModel>(roleResponse.Content);
                categories = projectBuilderActivity.ActivityCategory.Where(x => x.CategoryName == "Default")
                     .Concat(projectBuilderActivity.ActivityCategory.Where(x => x.CategoryName != "Default")
                     .OrderBy(x => x.CategoryName)).ToList();
                ViewBag.FormCategories = projectBuilderActivity.FormCategory.OrderBy(fc => fc.IsDefaultFormCategory).ThenBy(fc => fc.CategoryName).ToList();
                ViewBag.Roles = projectBuilderActivity.Roles;
                ViewBag.EntityTypes = projectBuilderActivity.EntityTypes;
            }
            return View(categories);
        }

        public ActionResult Scheduling(Guid? guid = null)
        {
            if (guid == null)
            {
                Session["ProjectId"] = null;
                SetErrorMessage("Please select project.");
                return Redirect("/Home/ListOfProject");
            }
            //get project name by guid
            var categories = new List<Core.ViewModels.ActivityCategoryViewModel>();
            var activityResponse = _webApi.Get("ActivityCategory");

            if (activityResponse.MessageType == "Success")
            {
                categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.ActivityCategoryViewModel>>(activityResponse.Content);
            }

            var offsetType = from Core.Enum.SchedulingOffsetType e in Enum.GetValues(typeof(Core.Enum.SchedulingOffsetType))
                             select new
                             {
                                 Id = (int)e,
                                 Name = e.ToString()
                             };
            ViewBag.OffsetType = new SelectList(offsetType, "Id", "Name");
            return View(categories);
        }

        public ActionResult PreviewScheduling(Guid? guid = null)
        {
            if (guid == null)
            {
                Session["ProjectId"] = null;
                SetErrorMessage("Please select project.");
                return Redirect("/Home/ListOfProject");
            }
            //get project name by guid
            var categories = new List<Core.ViewModels.ActivityCategoryViewModel>();
            var activityResponse = _webApi.Get("ActivityCategory");

            if (activityResponse.MessageType == "Success")
            {
                categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.ActivityCategoryViewModel>>(activityResponse.Content);
            }
            var allActivity = new List<Core.ViewModels.ActivityViewModel>();
            var allActivityResponse = _webApi.Get("Activity");

            if (allActivityResponse.MessageType == "Success")
            {
                allActivity = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.ActivityViewModel>>(allActivityResponse.Content);
                ViewBag.AllActivity = allActivity;
            }
            return View(categories);
        }

        public ActionResult Deployment(Guid? guid = null)
        {
            if (guid == null)
            {
                Session["ProjectId"] = null;
                SetErrorMessage("Please select project.");
                return Redirect("/Home/ListOfProject");
            }
            //get project name by guid
            #region get project name by guid
            {
                var projectName = _webApi.Get("Project/" + guid);
                if (projectName.MessageType == "Success")
                {
                    var projname = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.ProjectViewModel>(projectName.Content);
                    ViewBag.ProjectName = projname.ProjectName;

                    var role = projname.ProjectStaffMembersRoles.FirstOrDefault(x => x.UserGuid == this.LoggedInUser.Guid);
                    if (role != null)
                    {
                        Session["ProjectUserRole"] = role.ProjectUserRoleName;
                    }
                    else
                    {
                        Session["ProjectUserRole"] = null;
                    }
                }
            }
            ViewBag.ProjectUserRole = this.LoggedInUser.Roles.FirstOrDefault();
            #endregion
            var categories = new List<Core.ViewModels.ActivityCategoryViewModel>();
            var activityResponse = _webApi.Get("ActivityCategory");

            if (activityResponse.MessageType == "Success")
            {
                categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.ActivityCategoryViewModel>>(activityResponse.Content);
            }

            return View(categories);
        }

        //[Route("Test/ProjectBuilder/DeploymentPreview/{guid}")]
        public ActionResult DeploymentPreview(Guid? guid = null)
        {
            List<Core.ViewModels.RoleModel> model = new List<Core.ViewModels.RoleModel>();
            var roleResponse = _webApi.Get("Role");
            if (roleResponse.MessageType == "Success")
            {
                ViewBag.Roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.RoleModel>>(roleResponse.Content);
                model = ViewBag.Roles;
            }
            var projectResponse = _webApi.Get("Project/" + guid);
            if (projectResponse.MessageType == "Success")
            {
                List<Core.ViewModels.RoleModel> rolemodel = new List<Core.ViewModels.RoleModel>();
                var projectRoles = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.ProjectViewModel>(projectResponse.Content);
                foreach (var role in projectRoles.ProjectStaffMembersRoles)
                {
                    var d = model.FirstOrDefault(x => x.Guid == role.RoleGuid);
                    if (d != null)
                    {
                        rolemodel.Add(d);
                    }
                }
                ViewBag.Roles = rolemodel;
            }
            return View();
        }

        //[Route("Test/ProjectBuilder/DeploymentPreviewDashboard/{guids}")]
        public ActionResult DeploymentPreviewDashboard(string guids = null)
        {
            ViewBag.LoggedInUserGuid = this.LoggedInUser.Guid;
            var e = guids.Split(',');
            List<string> sGuids = guids.Split(',').ToList();
            List<Guid> GuidList = sGuids.ConvertAll(Guid.Parse);
            var roleResponse = _webApi.Get("Project");
            if (roleResponse.MessageType == "Success")
            {
                ViewBag.AllProjects = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.ProjectViewModel>>(roleResponse.Content);
            }
            ViewBag.ProjectId = GuidList[0];
            ViewBag.ProjectRoleId = GuidList[1];
            ViewBag.ProjectUserId = GuidList[2];
            Session["PreviewProjectId"] = GuidList[0];
            Session["PreviewProjectRoleId"] = GuidList[1];
            Session["PreviewProjectUserId"] = GuidList[2];
            return View();
        }

        //[Route("Test/ProjectBuilder/DeploymentPreviewActivities/{guid}")]
        public ActionResult DeploymentPreviewActivities(Guid? guid = null)
        {
            if (guid == null)
            {
                Session["PreviewProjectId"] = null;
                Session["PreviewProjectRoleId"] = null;
                Session["PreviewProjectUserId"] = null;
                SetErrorMessage("Please select project.");
                return Redirect("/Home/ListOfProject");
            }
            //get project name by guid
            var categories = new List<Core.ViewModels.ActivityViewModel>();
            var activityResponse = _webApi.Get("Activity");
            if (activityResponse.MessageType == "Success")
            {
                categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.ActivityViewModel>>(activityResponse.Content);
            }
            var activities = new List<Core.ViewModels.ActivityViewModel>();
            categories.Where(c => c.ProjectId == guid && c.PreviewSchedulingDone == true).ToList().ForEach(x =>
            {
                activities.Add(x);
            });
            ViewBag.ProjectUserRole = Session["PreviewProjectRoleId"].ToString();
            ViewBag.AllActivity = activities;
            return View();
        }

        //[Route("Test/ProjectBuilder/DeploymentPreviewForms/{guid}")]
        public ActionResult DeploymentPreviewForms(Guid? guid = null)
        {
            if (guid == null)
            {
                Session["PreviewProjectId"] = null;
                Session["PreviewProjectRoleId"] = null;
                Session["PreviewProjectUserId"] = null;
                SetErrorMessage("Please select project.");
                return Redirect("/Home/ListOfProject");
            }
            if (Session["PreviewProjectId"] == null)
            {
                Session["PreviewProjectId"] = null;
                Session["PreviewProjectRoleId"] = null;
                Session["PreviewProjectUserId"] = null;
                SetErrorMessage("Please select project.");
                return Redirect("/Home/ListOfProject");
            }
            //get project name by guid
            var categories = new Core.ViewModels.ActivityViewModel();
            var activityResponse = _webApi.Get("Activity/" + guid);

            if (activityResponse.MessageType == "Success")
            {
                categories = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.ActivityViewModel>(activityResponse.Content);
            }
            var forms = new List<Core.ViewModels.FormActivityViewModel>();
            categories.Forms.Where(c => c.Status == "Published").ToList().ForEach(x => { forms.Add(x); });
            ViewBag.ActivityForm = forms[0];
            ViewBag.AllActivityForms = forms;
            return View();
        }
    }
}