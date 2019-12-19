using Aspree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
namespace Aspree.Controllers.Mongo
{
    public class SearchController : BaseController
    {
        private readonly WebApiHandler _webApi;
        public SearchController()
        {
            _webApi = new WebApiHandler();
        }
        [Route("Mongo/Search/Index/{guid}")]
        public ActionResult Index(Guid? guid)
        {
            if (guid != null)
            {
                WriteLog("Web.SearchController: start.");
                var projectname = _webApi.Get("ProjectDeploy/GetProjectMongo/" + guid);
                if (projectname.MessageType == "Success")
                {
                    
                    var project = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.MongoViewModels.ProjectDeployViewModel>(projectname.Content);
                    ViewBag.ProjectName = project.ProjectName;

                    var role = project.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == this.LoggedInUser.Guid);
                    if (role != null)
                    {
                        Session["ProjectUserRole"] = role.Role;
                    }
                    else
                    {
                        Session["ProjectUserRole"] = null;
                    }

                    var ActivityList = new Dictionary<Guid, string>();

                    List<Core.ViewModels.MongoViewModels.FormsMongo> forms = new List<Core.ViewModels.MongoViewModels.FormsMongo>();
                    foreach (var activity in project.ProjectActivitiesList.Where(x=>x.IsDefaultActivity == (int)Core.Enum.DefaultActivityType.Default))
                    {
                        ActivityList.Add(activity.ActivityGuid, activity.ActivityName);
                        forms.AddRange(activity.FormsListMongo.Where(x => x.IsDefaultForm == (int)Core.Enum.DefaultFormType.Default));
                    }

                    ViewBag.SearchForms = forms;
                    ViewBag.ActivityList = ActivityList;
                }
                Session["ProjectId"] = guid.ToString();
                Session["ProjectName"] = ViewBag.ProjectName;
                ViewBag.ProjectId = guid.ToString();
            }

            //if (Session["ProjectId"] != null)
            //{
            //    var allForms = _webApi.Get("Form/GetProjectDefaultForms/" + Session["ProjectId"].ToString());
            //    if (allForms.MessageType == "Success")
            //    {
            //        var forms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.FormViewModel>>(allForms.Content);
            //        ViewBag.SearchForms = forms;
            //    }
            //}
            //else
            //{
            //    Session["ProjectId"] = null;
            //    Session["ProjectName"] = null;
            //    //return Redirect("/ProjectBuilder");
            //    SetErrorMessage("Please select project.");
            //    return Redirect("/Home/ListOfProject");
            //}

            return View("~/Views/Mongo/Search/Index.cshtml", guid);
            //return View("~/View/Mongo/Search/Index.cshtml");
        }
    }
}