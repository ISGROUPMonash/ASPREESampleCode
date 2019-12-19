using Aspree.Core.ViewModels;
using Aspree.Core.ViewModels.MongoViewModels;
using Aspree.ExtensionClasses;
using Aspree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aspree.Controllers
{
    [CustomAuthorizeAttribute]
    public class SearchController : BaseController
    {
        private readonly WebApiHandler _webApi;
        private readonly ExtensionClasses.SearchPageViewTransection _searchPageViewTransection;
        public SearchController()
        {
            _webApi = new WebApiHandler();
            _searchPageViewTransection = new ExtensionClasses.SearchPageViewTransection();
        }

        public ActionResult Index(Guid? guid)
        {
            WriteLog("search page  guid==" + guid);
            if (guid == null && Session["ProjectId"] == null)
            {
                WriteLog("search page  Session[ProjectId]==" + Session["ProjectId"]);
                SetErrorMessage("Please select project.");
                return Redirect("/Home/ListOfProject");
            }
            if (guid == null)
            {
                WriteLog("search page  guid == null");
                if (Session["ProjectId"] != null)
                    guid = new Guid(Session["ProjectId"].ToString());
            }

            if (guid != null)
            {
                WriteLog("search page  guid != null" + guid);
                #region test-site search page
                #region url checking
                string absolutePath = string.Empty;
                try
                {
                    bool isTesturipath = Request.Url.AbsoluteUri.ToLower().Contains(Utility.ConfigSettings.TestSiteKeyword);
                    absolutePath = isTesturipath ? "test/" : "";
                }
                catch (Exception exc)
                { }
                #endregion
                if (absolutePath == "test/")
                {
                    WriteLog("search page");
                    #region test site search page
                    var projectname = _webApi.Get("Review/GetProjectMongo/" + guid);
                    WriteLog("search page  guid != null" + projectname.Content);
                    if (projectname.MessageType == "Success")
                    {
                        MongoSearchPageReturnViewModel model = _searchPageViewTransection.MongoSearchPage(projectname, this.LoggedInUser);
                        ViewBag.ProjectName = model._ViewProjectName;
                        ViewBag.SearchForms = model._ViewMongoFormsList;
                        ViewBag.ActivityList = model._ViewMongoActivityList;
                        ViewBag.ProjectId = model._ViewProjectGuid;
                        Session["ProjectUserRole"] = model._ViewProjectUserRole;
                        Session["ProjectId"] = model._ViewProjectGuid;
                        try { ViewBag.RecruitmentStartDate = model.RecruitmentStartDate != null ? model.RecruitmentStartDate.Value.ToString("yyyy-MM-dd") : ""; } catch (Exception ex) { }
                        try { ViewBag.RecruitmentEndDate = model.RecruitmentEndDate != null ? model.RecruitmentEndDate.Value.ToString("yyyy-MM-dd") : ""; } catch (Exception ex) { }

                        ViewBag.ProjectEthicsApproval = model.ProjectEthicsApproval;

                        if (TempData["SetSearchPageErrorMessage"] != null)
                        {
                            TempData["SetSearchPageErrorMessage"] = null;
                            SetErrorMessage("Please search for an entity.");
                        }
                        return PartialView("_SearchMongo", model);
                    }
                    #endregion
                }
                else
                {
                    #region live site search page
                    int status = 0;
                    var projectStatus = _webApi.Get("FormDataEntry/" + guid);
                    if (projectStatus.MessageType == "Success")
                    {
                        var project = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.FormDataEntryViewModel>(projectStatus.Content);
                        status = project != null ? (project.ProjectDeployStatus != null ? (int)project.ProjectDeployStatus : 0) : 0;

                        #region Does this project have ethics approval
                        try
                        {
                            FormDataEntryVariableViewModel does_This_ProjectHaveEthicsApproval = project.FormDataEntryVariable.FirstOrDefault(x => x.VariableName == Core.Enum.DefaultsVariables.Ethics.ToString());
                            if (does_This_ProjectHaveEthicsApproval != null)
                            {
                                int? ethicsApproval = !string.IsNullOrEmpty(does_This_ProjectHaveEthicsApproval.SelectedValues) ? Convert.ToInt32(does_This_ProjectHaveEthicsApproval.SelectedValues) : (int?)null;
                                ViewBag.ProjectEthicsApproval = ethicsApproval != null ? (ethicsApproval == 1 ? true : false) : (bool?)null;
                            }
                        }
                        catch (Exception ethicsApproval) { }
                        #endregion
                    }

                    if (status == (int)Core.Enum.ProjectStatusTypes.Published)
                    {
                        #region Mongo-search-page
                        var projectname = _webApi.Get("ProjectDeploy/GetProjectMongo/" + guid);
                        if (projectname.MessageType == "Success")
                        {
                            MongoSearchPageReturnViewModel model = _searchPageViewTransection.MongoSearchPage(projectname, this.LoggedInUser);
                            ViewBag.ProjectName = model._ViewProjectName;
                            ViewBag.SearchForms = model._ViewMongoFormsList;
                            ViewBag.ActivityList = model._ViewMongoActivityList;
                            ViewBag.ProjectId = model._ViewProjectGuid;
                            Session["ProjectUserRole"] = model._ViewProjectUserRole;
                            Session["ProjectId"] = model._ViewProjectGuid;
                            try { ViewBag.RecruitmentStartDate = model.RecruitmentStartDate != null ? model.RecruitmentStartDate.Value.ToString("yyyy-MM-dd") : ""; } catch (Exception ex) { }
                            try { ViewBag.RecruitmentEndDate = model.RecruitmentEndDate != null ? model.RecruitmentEndDate.Value.ToString("yyyy-MM-dd") : ""; } catch (Exception ex) { }

                            if (TempData["SetSearchPageErrorMessage"] != null)
                            {
                                TempData["SetSearchPageErrorMessage"] = null;
                                SetErrorMessage("Please search for an entity.");
                            }
                            return PartialView("_SearchMongo", model);
                        }
                        #endregion
                    }
                    else
                    {
                        #region SQL-search-page
                        SQLSearchPageReturnViewModel model = new SQLSearchPageReturnViewModel();
                        var projectSQL = _webApi.Get("Project/" + guid);
                        if (projectSQL.MessageType == "Success")
                        {
                            SQLSearchPageReturnViewModel modelProj = _searchPageViewTransection.SQLSearchPage(projectSQL, this.LoggedInUser, 0);
                            Session["ProjectId"] = modelProj._ViewProjectGuid;
                            Session["ProjectUserRole"] = modelProj._ViewProjectUserRole;

                            model._ViewProjectGuid = modelProj._ViewProjectGuid;
                            model._ViewProjectName = modelProj._ViewProjectName;
                            model._ViewProjectUserRole = modelProj._ViewProjectUserRole;

                            try { ViewBag.RecruitmentStartDate = modelProj.RecruitmentStartDate != null ? modelProj.RecruitmentStartDate.Value.ToString("yyyy-MM-dd") : ""; } catch (Exception ex) { }
                            try { ViewBag.RecruitmentEndDate = modelProj.RecruitmentEndDate != null ? modelProj.RecruitmentEndDate.Value.ToString("yyyy-MM-dd") : ""; } catch (Exception ex) { }

                        }
                        if (Session["ProjectId"] != null)
                        {
                            var allForms = _webApi.Get("Form/GetProjectDefaultForms/" + Session["ProjectId"].ToString());
                            if (allForms.MessageType == "Success")
                            {
                                SQLSearchPageReturnViewModel modelForm = _searchPageViewTransection.SQLSearchPage(allForms, this.LoggedInUser, 1);
                                model._ViewSQLFormsList = modelForm._ViewSQLFormsList;
                            }
                        }
                        if (TempData["SetSearchPageErrorMessage"] != null)
                        {
                            TempData["SetSearchPageErrorMessage"] = null;
                            SetErrorMessage("Please search for an entity.");
                        }
                        return PartialView("_SearchSQL", model);
                        #endregion
                    }
                    #endregion
                }
                #endregion
            }
            SetErrorMessage("Please select project.");
            return Redirect("/Home/ListOfProject");
        }
        public ActionResult SearchResult(string participant, Guid formId, Guid? guid = null)
        {
            if (guid != null)
            {
                var projectname = _webApi.Get("Project/" + guid);
                if (projectname.MessageType == "Success")
                {
                    var project = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.ProjectViewModel>(projectname.Content);
                    ViewBag.ProjectName = project.ProjectName;
                }
                Session["ProjectId"] = guid.ToString();
                ViewBag.ProjectId = guid.ToString();
            }
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

            var user = new List<Core.ViewModels.FormDataEntryVariableViewModel>();

            var userResult = _webApi.Get("FormDataEntry/GetFormDataEntryByEntId/" + guid + "/" + formId + "/" + participant);

            if (userResult.MessageType == "Success")
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.FormDataEntryVariableViewModel>>(userResult.Content);

                ViewBag.FirstName = user.Where(x => x.VariableId == 14).Select(x => x.SelectedValues).FirstOrDefault();
                ViewBag.Surname = user.Where(x => x.VariableId == 13).Select(x => x.SelectedValues).FirstOrDefault();
                ViewBag.DOB = user.Where(x => x.VariableId == 40).Select(x => x.SelectedValues).FirstOrDefault();
                ViewBag.Gender = user.Where(x => x.VariableId == 41).Select(x => x.SelectedValues).FirstOrDefault();

                ViewBag.Email = user.Where(x => x.VariableId == 35).Select(x => x.SelectedValues).FirstOrDefault();
                ViewBag.ParticipantData = user;
            }
            return View(categories);
        }
    }
}