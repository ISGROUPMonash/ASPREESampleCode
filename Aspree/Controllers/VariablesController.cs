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
    public class VariablesController : BaseController
    {

        private readonly WebApiHandler _webApi;

        public VariablesController()
        {
            _webApi = new WebApiHandler();
        }

        // GET: Variables
        public ActionResult Index()
        {

            var categories = new List<Core.ViewModels.VariableCategoryViewModel>();
            var projectBuilderVariables = new Core.ViewModels.ProjectBuilderVariablesViewModel();
            var roleResponse = _webApi.Get("Variable/ProjectBuilderVariables/"+ Session["ProjectId"]);
            if (roleResponse.MessageType == "Success")
            {
                projectBuilderVariables = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.ProjectBuilderVariablesViewModel>(roleResponse.Content);
                categories = projectBuilderVariables.VariableCategory.ToList();
                ViewBag.Roles = projectBuilderVariables.Role;
                ViewBag.VariableTypes = projectBuilderVariables.VariableType;
                ViewBag.ValidationRules = projectBuilderVariables.ValidationRule;
            }
            return View(categories);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(Guid guid)
        {
            return View();
        }

    }
}