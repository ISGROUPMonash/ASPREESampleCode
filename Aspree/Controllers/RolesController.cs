using Aspree.Core.ViewModels;
using Aspree.ExtensionClasses;
using Aspree.Services;
using Aspree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace Aspree.Controllers
{
    [CustomAuthorizeAttribute(Roles = "Definition Admin, System Admin")]
    public class RolesController : BaseController
    {
        private readonly WebApiHandler _webApi;

        public RolesController()
        {
            _webApi = new WebApiHandler();
        }
        // GET: Roles
        public ActionResult Index()
        {
            var roles = new List<Core.ViewModels.PrivilegeSmallViewModel>();
            var response = _webApi.Get("Privilege");
            if (response.MessageType == "Success")
            {
                roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.PrivilegeSmallViewModel>>(response.Content);
                return View(roles);
            }
            return View(roles);
        }


        [HttpGet]
        public ActionResult Edit(Guid guid)
        {
            var roles = new List<Core.ViewModels.PrivilegeSmallViewModel>();
            var role = new Core.ViewModels.NewRoleModel();
            ViewBag.Guid = guid;
            var roleResponse = _webApi.Get("Role/" + guid.ToString());
            if (roleResponse.MessageType == "Success")
            {
                var editRole = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.RoleModel>(roleResponse.Content);
                role.Name = editRole.Name;
                role.Privileges = editRole.Privileges == null ? new List<Guid>() : editRole.Privileges;
            }
            return View(role);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid guid, EditRoleModel editRoleModel)
        {
            var response1 = _webApi.Get("Privilege");

            if (response1.MessageType == "Success")
            {
                ViewBag.Roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.PrivilegeSmallViewModel>>(response1.Content);
            }

            if (!ModelState.IsValid)
            {
                return View(editRoleModel);
            }

            var role = new Core.ViewModels.RoleModel();
            var roleResponse = _webApi.Put("Role/" + guid.ToString(), editRoleModel);

            if (roleResponse.MessageType == "Success")
            {
                TempData["Success"] = "Role was modified successfully.";
                return Redirect("/Roles");
            }
            else
            {
                if (roleResponse.Content.Contains("key"))
                {
                    ModelState.AddModelError("Name", "Role name already exists.");
                    return View(editRoleModel);
                }
                else
                {
                    var response = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(roleResponse.Content);
                    TempData["Error"] = response.message;
                    return View(editRoleModel);
                }
            }
        }

        // POST: Roles/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid guid)
        {
            var role = new Core.ViewModels.RoleModel();
            var response = _webApi.Delete("Role/" + guid.ToString());
            return Json(response);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var role = new Core.ViewModels.NewRoleModel();
            role.TenantId = this.LoggedInUser.TenantId;
            return View(role);
        }

        // POST: Roles/Create
        [HttpPost]
        public ActionResult Create(NewRoleModel newRoleModel)
        {
            var response1 = _webApi.Get("Privilege");
            if (response1.MessageType == "Success")
            {
                ViewBag.Roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.PrivilegeSmallViewModel>>(response1.Content);
            }
            if (!ModelState.IsValid)
            {
                return View(newRoleModel);
            }
            var roleResponse = _webApi.Post("Role", newRoleModel);
            if (roleResponse.MessageType == "Success")
            {
                TempData["Success"] = "Role was created successfully.";
                return Redirect("/Roles");
            }
            else
            {
                if (roleResponse.Content.Contains("key"))
                {
                    ModelState.AddModelError("Name", "Role name already exists.");
                    return View(newRoleModel);
                }
                else
                {
                    var response = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(roleResponse.Content);
                    TempData["Error"] = response.message;
                    return View(newRoleModel);
                }
            }
        }
    }
}