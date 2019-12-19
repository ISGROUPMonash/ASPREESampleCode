  using Aspree.ExtensionClasses;
using Aspree.Models;
using Aspree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aspree.Controllers
{
    public class SystemAdminToolsController : BaseController
    {
        private readonly WebApiHandler _webApi;
        public SystemAdminToolsController()
        {
            _webApi = new WebApiHandler();
        }
        // GET: SystemAdminTools
        public ActionResult Index()
        {
            return View();
        }
        [CustomAuthorizeAttribute(Roles = "System Admin")]
        public ActionResult ManagesProjects()
        {
            if (Session["ProjectId"] != null)
            {
                ViewBag.ProjectId = Session["ProjectId"].ToString();

                var roles = new List<Core.ViewModels.FormViewModel>();
                var response = _webApi.Get("Form/GetProjectDefaultForms/" + ViewBag.ProjectId);
                if (response.MessageType == "Success")
                {
                    roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.FormViewModel>>(response.Content);
                    var form = roles.FirstOrDefault(x => x.FormTitle == "Project Registration");
                    ViewBag.FormId = form != null ? form.Guid : new Guid();
                }
            }
            else
            {
                SetErrorMessage("Please select project.");
                return Redirect("/Home/ListOfProject");
            }
            return View("~/Views/Projects/Index.cshtml");
        }
        [CustomAuthorizeAttribute(Roles = "System Admin")]
        public ActionResult ManageCategories(int? path = 0)
        {
            if (path == 2)
            {
                ViewBag.UrlData = "2";
                
                return View("~/Views/Category/Activities.cshtml");
            }
            else if (path == 1)
            {
                ViewBag.UrlData = "1";
                return View("~/Views/Category/Forms.cshtml");
            }
            else
            {
                return View("~/Views/Category/Variables.cshtml");
            }
        }

        public ActionResult ManageRoles()
        {
            var roles = new List<Core.ViewModels.PrivilegeSmallViewModel>();
            var response = _webApi.Get("Privilege");
            if (response.MessageType == "Success")
            {
                roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.PrivilegeSmallViewModel>>(response.Content);
                return View("~/Views/Roles/Index.cshtml",roles);
            }
            
            return View("~/Views/Roles/Index.cshtml",roles);
        }
        [CustomAuthorizeAttribute(Roles = "System Admin")]
        public ActionResult ManageEntities(int? path = 0)
        {
            if (path == 1)
            {
                return View("~/Views/Entities/Configuration.cshtml");
            }
            else
            {
                return View("~/Views/Entities/Index.cshtml");
            }           
        }
        [CustomAuthorizeAttribute(Roles = "System Admin")]
        public ActionResult ManageUsers()
        {
            UserViewModel user = new UserViewModel();

            var roles = new List<Core.ViewModels.RoleModel>();
            var response = _webApi.Get("Role");

            if (response.MessageType == "Success")
            {
                roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.RoleModel>>(response.Content).ToList();

                List<SelectListItem> items = new List<SelectListItem>();

                foreach (var r in roles)
                {
                    items.Add(new SelectListItem { Text = r.Name, Value = r.Guid.ToString() });
                }
                user.Role = items;
                user.TenantId = this.LoggedInUser.TenantId;

                return View("~/Views/Users/Index.cshtml",user);
            }
            return View("~/Views/Users/Index.cshtml", user);
        }

        [CustomAuthorizeAttribute(Roles = "System Admin")]
        public ActionResult ManageAuthentication()
        {
            var authenticationType = from Core.Enum.AuthenticationTypes e in Enum.GetValues(typeof(Core.Enum.AuthenticationTypes))
                             select new
                             {
                                 Id = (int)e,
                                 Name = e.ToString()
                             };
            ViewBag.AuthenticationType = new SelectList(authenticationType, "Id", "Name");
            return View();
        }
    }
}