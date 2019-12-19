using Aspree.Core.ViewModels;
using Aspree.ExtensionClasses;
using Aspree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aspree.Controllers
{
    [CustomAuthorizeAttribute(Roles = "System Admin")]
    public class ProjectsController : BaseController
    {

        private readonly WebApiHandler _webApi;
        public ProjectsController()
        {
            _webApi = new WebApiHandler();
        }

        // GET: Projects
        public ActionResult Index()
        {
            return View();
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            ProjectViewModel project = new ProjectViewModel();

            var roleResponse = _webApi.Get("Role");
            var roles = new List<RoleModel>();
            if (roleResponse.MessageType == "Success")
            {
                roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RoleModel>>(roleResponse.Content).ToList();
                ViewBag.ProjectRoles = roles;
            }

            Guid roleGuid = new Guid();
            foreach (var role in roles)
            {
                if (role.Name == "Project Admin")
                {
                    project.RoleId = role.Id;
                    roleGuid = role.Guid;
                    project.RoleGuid = roleGuid;
                    break;
                }
            }
            var users = new List<UserLoginViewModel>();
            var response = _webApi.Get("User");
            if (response.MessageType == "Success")
            {
                users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserLoginViewModel>>(response.Content).ToList();
                List<SelectListItem> items = new List<SelectListItem>();
                List<SelectListItem> projectUserList = new List<SelectListItem>();
                foreach (var r in users.Where(r => !r.DateDeactivated.HasValue).ToArray())
                {
                    projectUserList.Add(new SelectListItem { Text = r.FirstName + " " + r.LastName, Value = r.Guid.ToString() });
                    if (r.RoleId == roleGuid)
                        items.Add(new SelectListItem { Text = r.FirstName + " " + r.LastName, Value = r.Guid.ToString() });
                }
                project.UserList = projectUserList;
                project.TenantId = this.LoggedInUser.TenantId;
                return View(project);
            }

            return View(project);
        }

        // POST: Projects/Create
        [HttpPost]
        public ActionResult Create(ProjectViewModel newUser)
        {
            if (!ModelState.IsValid)
            {
                return View(newUser);
            }
            var user = new Core.ViewModels.ProjectViewModel()
            {
                ProjectName = newUser.ProjectName,
                Version = newUser.Version,
                ProjectUserId = newUser.ProjectUserId,
                TenantId = newUser.TenantId
            };
            var userResponse = _webApi.Post("Project", user);
            if (userResponse.MessageType == "Success")
            {
                TempData["Success"] = "User was created successfully.";
                return Redirect("/Users");
            }
            else
            {
                if (userResponse.Content.Contains("key"))
                {
                    ModelState.AddModelError("Email", "Email Id already exists.");
                    return View(newUser);
                }
                else
                {
                    var response = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(userResponse.Content);
                    TempData["Error"] = response.message;
                    return View(newUser);
                }
            }
        }

        // GET: Projects/Create
        [HttpGet]
        public ActionResult Edit(Guid guid)
        {
            ProjectViewModel project = new ProjectViewModel();
            var roleResponse = _webApi.Get("Role");
            var roles = new List<RoleModel>();
            if (roleResponse.MessageType == "Success")
            {
                roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RoleModel>>(roleResponse.Content).ToList();
                ViewBag.ProjectRoles = roles;
            }
            var d = roles.Where(x => x.Name == "Project Admin").FirstOrDefault();
            Guid roleGuid = new Guid();
            foreach (var role in roles)
            {
                if (role.Name == "Project Admin")
                {
                    project.RoleId = role.Id;
                    roleGuid = role.Guid;
                    project.RoleGuid = roleGuid;
                    break;
                }
            }
            var users = new List<UserLoginViewModel>();
            var response = _webApi.Get("User");
            if (response.MessageType == "Success")
            {
                users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserLoginViewModel>>(response.Content).ToList();
                List<SelectListItem> items = new List<SelectListItem>();
                List<SelectListItem> projectUserList = new List<SelectListItem>();
                foreach (var r in users.Where(r => !r.DateDeactivated.HasValue).ToArray())
                {
                    projectUserList.Add(new SelectListItem { Text = r.FirstName + " " + r.LastName, Value = r.Guid.ToString() });
                    if (r.RoleId == roleGuid)
                        items.Add(new SelectListItem { Text = r.FirstName + " " + r.LastName, Value = r.Guid.ToString() });
                }
                project.UserList = projectUserList;

                project.TenantId = this.LoggedInUser.TenantId;

                var projectResponse = _webApi.Get("Project/" + guid);
                if (projectResponse.MessageType == "Success")
                {
                    var currentProject = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectViewModel>(projectResponse.Content);
                    project.ProjectName = currentProject.ProjectName;
                    project.ProjectUserId = currentProject.ProjectUsers.Select(x => x.Guid).FirstOrDefault();
                    project.ProjectStatusId = currentProject.ProjectStatusId;
                    project.State = currentProject.State;
                    project.Version = currentProject.Version;
                    project.Guid = currentProject.Guid;
                    project.ProjectStaffMembersRoles = currentProject.ProjectStaffMembersRoles;
                }
                return View(project);
            }
            return View(project);
        }
    }
}