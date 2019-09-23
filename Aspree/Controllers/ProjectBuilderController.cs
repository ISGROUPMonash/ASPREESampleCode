using Aspree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aspree.Controllers
{
    [AuthorizeAttribute(Roles = "Project Admin, System Admin")]
    public class ProjectBuilderController : BaseController
    {
        /// <summary>
        /// Common class which is used to call web api 
        /// </summary>
        private readonly WebApiHandler _webApi;

        public ProjectBuilderController()
        {
            _webApi = new WebApiHandler();
        }

        // GET: ProjectBuilder
        /// <summary>
        /// Web controller which get all the variables and their respective details from current project.
        /// </summary>
        /// <param name="guid">Current selected project after login</param>
        /// <returns>Return model</returns>
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
    }
}