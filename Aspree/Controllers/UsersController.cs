using Aspree.ExtensionClasses;
using Aspree.Models;
using Aspree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace Aspree.Controllers
{
    [CustomAuthorizeAttribute(Roles = "Definition Admin, System Admin")]
    public class UsersController : BaseController
    {
        private readonly WebApiHandler _webApi;
        public UsersController()
        {
            _webApi = new WebApiHandler();
        }


        // GET: User
        public ActionResult Index()
        {
            UserViewModel user = new Models.UserViewModel();
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
                return View(user);
            }

            return View(user);
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            UserViewModel user = new Models.UserViewModel();
            var roles = new List<Core.ViewModels.RoleModel>();
            var response = _webApi.Get("Role");

            if (response.MessageType == "Success")
            {
                roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.RoleModel>>(response.Content).ToList();

                List<SelectListItem> items = new List<SelectListItem>();

                foreach (var r in roles.Where(r => !r.DateDeactivated.HasValue && r.IsSystemRole).ToArray())
                {
                    items.Add(new SelectListItem { Text = r.Name, Value = r.Guid.ToString() });
                }
                user.Role = items;
                user.TenantId = this.LoggedInUser.TenantId;
                return View(user);
            }

            return View(user);
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(UserViewModel newUser)
        {
            if (!ModelState.IsValid)
            {
                return View(newUser);
            }

            var user = new Core.ViewModels.NewUserViewModel()
            {
                Address = newUser.Address,
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Mobile = newUser.Mobile,
                RoleId = newUser.RoleId,
                TenantId = newUser.TenantId
            };

            var roles = new List<Core.ViewModels.RoleModel>();
            var roleResponse = _webApi.Get("Role");

            if (roleResponse.MessageType == "Success")
            {
                roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.RoleModel>>(roleResponse.Content).ToList();

                List<SelectListItem> items = new List<SelectListItem>();

                foreach (var r in roles.Where(r => !r.DateDeactivated.HasValue && r.IsSystemRole).ToArray())
                {
                    items.Add(new SelectListItem { Text = r.Name, Value = r.Guid.ToString() });
                }
                newUser.Role = items;
                newUser.TenantId = this.LoggedInUser.TenantId;
           }

            var userResponse = _webApi.Post("User", user);
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

        // GET: User/Edit/5
        public ActionResult Edit(Guid guid)
        {
            var user = new Models.UserViewModel();

            var roles = new List<Core.ViewModels.RoleModel>();
            
            var userResponse = _webApi.Get("User/" + guid.ToString());
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
            }
            var response = _webApi.Get("Role");
            if (response.MessageType == "Success")
            {
                roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.RoleModel>>(response.Content).ToList();

                List<SelectListItem> items = new List<SelectListItem>();

                foreach (var r in roles.Where(r => !r.DateDeactivated.HasValue && r.IsSystemRole).ToArray())
                {
                    items.Add(new SelectListItem { Text = r.Name, Value = r.Guid.ToString() });
                }
                user.Role = items;
                user.TenantId = this.LoggedInUser.TenantId;

                return View(user);
            }

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid guid, UserViewModel editUser)
        {
            if (!ModelState.IsValid)
            {
                return View(editUser);
            }

            var user = new Core.ViewModels.EditUserViewModel()
            {
                Address = editUser.Address,
                Email = editUser.Email,
                FirstName = editUser.FirstName,
                LastName = editUser.LastName,
                Mobile = editUser.Mobile,
                RoleId = editUser.RoleId,
                TenantId = editUser.TenantId,
            };

            var userResponse = _webApi.Put("User/" + guid.ToString() , user);
            if (userResponse.MessageType == "Success")
            {
                TempData["Success"] = "User was modified successfully.";
                return Redirect("/Users");
            }
            else
            {
                if (userResponse.Content.Contains("key"))
                {
                    ModelState.AddModelError("Email", "Email Id already exists.");
                    return View(editUser);
                }
                else
                {
                    var response = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(userResponse.Content);
                    TempData["Error"] = response.message;
                    return View(editUser);
                }
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid guid, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult UserCreate()
        {
            UserViewModel user = new Models.UserViewModel();

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
                return View(user);
            }

            return View(user);
        }
    }
}
