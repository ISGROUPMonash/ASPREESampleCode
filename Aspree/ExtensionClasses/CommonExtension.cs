using Aspree.Core.ViewModels.MongoViewModels;
using Aspree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aspree.ExtensionClasses
{
    public class CommonExtension
    {
    }
    public class SearchPageViewTransection
    {
        public MongoSearchPageReturnViewModel MongoSearchPage(Utility.ResponseMessage response, Core.ViewModels.LoggedInUser LoggedInUser)
        {
            if (response.MessageType == "Success")
            {
                MongoSearchPageReturnViewModel model = new MongoSearchPageReturnViewModel();
                var project = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.MongoViewModels.ProjectDeployViewModel>(response.Content);
                model._ViewProjectName = project.ProjectName;

                var role = project.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == LoggedInUser.Guid);

                model._ViewProjectUserRole = (role != null ? role.Role : null);

                var ActivityList = new Dictionary<Guid, string>();

                List<Core.ViewModels.MongoViewModels.FormsMongo> forms = new List<Core.ViewModels.MongoViewModels.FormsMongo>();
                foreach (var activity in project.ProjectActivitiesList.Where(x => x.IsDefaultActivity == (int)Core.Enum.DefaultActivityType.Default))
                {
                    ActivityList.Add(activity.ActivityGuid, activity.ActivityName);
                    forms.AddRange(activity.FormsListMongo.Where(x => x.IsDefaultForm == (int)Core.Enum.DefaultFormType.Default));
                }
                model.RecruitmentStartDate = project.RecruitmentStartDate;
                model.RecruitmentEndDate = project.RecruitmentEndDate;
                model.ProjectEthicsApproval = project.ProjectEthicsApproval;
                model._ViewMongoFormsList = forms;
                model._ViewMongoActivityList = ActivityList;
                model._ViewProjectGuid = project.ProjectGuid;

                return model;
            }
            return null;
        }
        public SQLSearchPageReturnViewModel SQLSearchPage(Utility.ResponseMessage response, Core.ViewModels.LoggedInUser LoggedInUser, int flag = 0)
        {
            if (flag == 0)
            {
                if (response.MessageType == "Success")
                {
                    SQLSearchPageReturnViewModel model = new SQLSearchPageReturnViewModel();
                    var project = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.ProjectViewModel>(response.Content);
                    var role = project.ProjectStaffMembersRoles.FirstOrDefault(x => x.UserGuid == LoggedInUser.Guid);

                    model._ViewProjectGuid = project.Guid;
                    model._ViewProjectName = project.ProjectName;
                    model._ViewProjectUserRole = (role != null ? role.ProjectUserRoleName : null);

                    try { model.RecruitmentStartDate = project.RecruitmentStartDate; } catch (Exception ex) { }
                    try { model.RecruitmentEndDate = project.RecruitmentEndDate; } catch (Exception ex) { }

                    return model;
                }
            }
            else
            {
                if (response.MessageType == "Success")
                {
                    SQLSearchPageReturnViewModel model = new SQLSearchPageReturnViewModel();

                    var forms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.FormViewModel>>(response.Content);
                    model._ViewSQLFormsList = forms;
                    return model;
                }
            }
            return null;
        }

        public SummaryViewModel MongoSummaryPage(Utility.ResponseMessage response)
        {
            if (response.MessageType == "Success")
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.MongoViewModels.SummaryViewModel>(response.Content);
                if (result.EntityType != "Person")
                {
                    var summaryPageActivityTypeList = result.SummaryPageActivityTypeList.FirstOrDefault(x => x.ActivityName == "Project Linkage");
                    result.SummaryPageActivityTypeList.Remove(summaryPageActivityTypeList);
                }
                return result;
            }
            return null;
        }
        public SummaryViewModel SQLSummaryPage(Utility.ResponseMessage response)
        {
            if (response.MessageType == "Success")
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.MongoViewModels.SummaryViewModel>(response.Content);
            }
            return null;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        // @Override
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                string loginUrl = "/"; // Default Login Url 
                filterContext.Result = new RedirectResult(loginUrl);
            }
        }
    }
}