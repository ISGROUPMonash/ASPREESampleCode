using Aspree.Core.Enum;
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
    public class SummaryController : BaseController
    {
        private readonly WebApiHandler _webApi;
        private readonly ExtensionClasses.SearchPageViewTransection _searchPageViewTransection;
        public SummaryController()
        {
            _webApi = new WebApiHandler();
            _searchPageViewTransection = new ExtensionClasses.SearchPageViewTransection();
        }

        public ActionResult Index(string participant = null, Guid? formId = null, Guid? guid = null)
        {
            if (guid != null)
            {
                try
                {
                    if (guid != new Guid(Session["ProjectId"].ToString()))
                    {
                        Session["ProjectId"] = guid;
                    }
                }
                catch (Exception exGuid) { }
            }

            #region store last search result
            if (participant == null && formId == null && guid == null)
            {
                participant = TempData.Peek("SummaryPageParticipantId") as String;
                formId = TempData.Peek("SummaryPageFormId") as System.Guid?;
                guid = TempData.Peek("SummaryPageGuid") as Guid?;

                if (participant == null && guid == null)
                {
                    if (Session["ProjectId"] != null)
                    {
                        TempData["SetSearchPageErrorMessage"] = "Please search/create an entity.";
                        return Redirect("/Search/Index/" + Session["ProjectId"].ToString());
                    }
                    else
                    {
                        TempData["SetSearchPageErrorMessage"] = "Please search/create an entity.";
                        return Redirect("/Search");
                    }
                }
            }
            if (guid == null)
            {
                SetErrorMessage("Please select project.");
                return Redirect("/Home/ListOfProject");
            }
            TempData["SummaryPageParticipantId"] = participant;
            TempData["SummaryPageFormId"] = formId;
            TempData["SummaryPageGuid"] = guid;

            TempData.Keep("SummaryPageParticipantId");
            TempData.Keep("SummaryPageFormId");
            TempData.Keep("SummaryPageGuid");
            #endregion

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
                #region test site
                if (participant != null && participant != null)
                {
                    Session["ProjectId"] = guid.ToString();
                    ViewBag.ProjectId = guid.ToString();

                    var projectname = _webApi.Get("Review/GetSummaryDetails/" + guid + "/" + participant);
                    if (projectname.MessageType == "Success")
                    {
                        ViewBag.ProjectStaffMembersRoles = this.LoggedInUser.Roles.FirstOrDefault();
                        Core.ViewModels.MongoViewModels.SummaryViewModel projectData = _searchPageViewTransection.MongoSummaryPage(projectname);
                        ViewBag.CurrentProjectId = guid;

                        try
                        {
                            if (projectData.EntityType != EntityTypes.Project.ToString())
                            {
                                if (System.IO.File.Exists(projectData.EntityProfileImage))
                                {
                                    var stream = System.IO.File.ReadAllBytes(projectData.EntityProfileImage);
                                    projectData.EntityProfileImage = "data:image/jpeg;base64," + Convert.ToBase64String(stream);
                                }
                            }
                        }
                        catch (Exception e)
                        { }

                        return PartialView("_SummaryMongo", projectData);
                    }
                }
                #endregion
            }
            else
            {
                #region live site
                int status = 0;
                int currentEntityUserlogin = 0;
                var projectStatus = _webApi.Get("FormDataEntry/" + guid);
                if (projectStatus.MessageType == "Success")
                {
                    var project = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.FormDataEntryViewModel>(projectStatus.Content);
                    status = project != null ? (project.ProjectDeployStatus != null ? (int)project.ProjectDeployStatus : 0) : 0;
                    try { currentEntityUserlogin = project != null ? Convert.ToInt32(project.ThisUserId) : 0; } catch (Exception exv) { }
                }

                #region check entity exestence location
                var checkEntityLocationResponse = _webApi.Get("Search/CheckEntityExistenceLocation/" + participant);
                if (checkEntityLocationResponse.MessageType == "Success")
                {
                    string entitySavedLocation = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(checkEntityLocationResponse.Content);
                    if (entitySavedLocation != null)
                    {
                        if (entitySavedLocation == Core.Enum.AspreeDatabaseType.Mongo.ToString())
                        {
                            status = (int)Core.Enum.ProjectStatusTypes.Published;
                        }
                        else if (entitySavedLocation == Core.Enum.AspreeDatabaseType.SQL.ToString())
                        {
                            status = (int)Core.Enum.ProjectStatusTypes.Draft;
                        }
                    }
                }
                #endregion

                if (status == (int)Core.Enum.ProjectStatusTypes.Published && absolutePath != "test/")
                {
                    #region Mongo-summary-page
                    if (participant != null && participant != null)
                    {
                        Session["ProjectId"] = guid.ToString();
                        ViewBag.ProjectId = guid.ToString();

                        var projectname = _webApi.Get("MongoDB_Summary/GetSummaryDetails/" + guid + "/" + participant);
                        if (projectname.MessageType == "Success")
                        {
                            ViewBag.ProjectStaffMembersRoles = this.LoggedInUser.Roles.FirstOrDefault();
                            Core.ViewModels.MongoViewModels.SummaryViewModel projectData = _searchPageViewTransection.MongoSummaryPage(projectname);
                            ViewBag.CurrentProjectId = guid;


                            try
                            {
                                if (projectData.EntityType != EntityTypes.Project.ToString())
                                {
                                    if (System.IO.File.Exists(projectData.EntityProfileImage))
                                    {
                                        var stream = System.IO.File.ReadAllBytes(projectData.EntityProfileImage);
                                        projectData.EntityProfileImage = "data:image/jpeg;base64," + Convert.ToBase64String(stream);
                                    }
                                }
                            }
                            catch (Exception e)
                            { }

                            return PartialView("_SummaryMongo", projectData);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region SQL-summary-page
                    #region API- get project information
                    if (guid != null)
                    {
                        var projectname = _webApi.Get("Project/" + guid);
                        if (projectname.MessageType == "Success")
                        {
                            Core.ViewModels.ProjectViewModel project = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.ProjectViewModel>(projectname.Content);
                            ViewBag.ProjectName = project.ProjectName;

                            Guid? projectStaffMembersRoleGuid = project.ProjectStaffMembersRoles.FirstOrDefault(x => x.UserGuid == this.LoggedInUser.Guid) != null ? project.ProjectStaffMembersRoles.FirstOrDefault(x => x.UserGuid == this.LoggedInUser.Guid).RoleGuid : null;
                            if (projectStaffMembersRoleGuid == null)
                            {
                                string rolename = this.LoggedInUser.Roles.FirstOrDefault();
                                projectStaffMembersRoleGuid = project.ProjectStaffMembersRoles.Where(x => x.ProjectUserRoleName == rolename).Select(x => x.RoleGuid).FirstOrDefault();
                            }


                            try
                            {
                                string projectStaffMembersRoleName = project.ProjectStaffMembersRoles.FirstOrDefault(x => x.UserGuid == this.LoggedInUser.Guid) != null ? project.ProjectStaffMembersRoles.FirstOrDefault(x => x.UserGuid == this.LoggedInUser.Guid).ProjectUserRoleName : null;
                                if (projectStaffMembersRoleName == null)
                                {
                                    string rolename = this.LoggedInUser.Roles.FirstOrDefault();
                                    projectStaffMembersRoleName = rolename;
                                }
                                ViewBag.ProjectStaffMemberRoleName = projectStaffMembersRoleName;
                            }
                            catch (Exception rlEx) { }

                            ViewBag.ProjectStaffMembersRoles = projectStaffMembersRoleGuid;

                        }
                        Session["ProjectId"] = guid.ToString();
                        ViewBag.ProjectId = guid.ToString();
                    }
                    #endregion

                    if (formId == null)
                    {
                        return Redirect("/Search/Index/");
                    }

                    #region API- get form-details
                    string formName = string.Empty;
                    Core.ViewModels.FormViewModel formVM = new Core.ViewModels.FormViewModel();
                    var formResponse = _webApi.Get("Form/" + formId);
                    if (formResponse.MessageType == "Success")
                    {
                        formVM = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.FormViewModel>(formResponse.Content);
                        formName = formVM.FormTitle;
                        if (formName == "Participant Registration")
                        {
                            ViewBag.EntityType = "Participant";
                        }
                        else if (formName == "Person Registration")
                        {
                            ViewBag.EntityType = "Person";
                        }
                        else if (formName == "Place/Group Registration")
                        {
                            ViewBag.EntityType = "Place/Group";
                        }
                        else if (formName == "Project Registration")
                        {
                            ViewBag.EntityType = "Project";
                        }
                        else if (formName == DefaultFormName.Project_Linkage.ToString().Replace("_", " "))
                        {
                            ViewBag.EntityType = "Person";
                        }
                    }
                    #endregion

                    #region API- get entity details by entity id
                    DateTime entityCreationDate = new DateTime();
                    System.Guid _thisActivityGuid = Guid.Empty;
                    List<Core.ViewModels.FormDataEntryVariableViewModel> user = new List<Core.ViewModels.FormDataEntryVariableViewModel>();
                    var userResult = new Utility.ResponseMessage();
                    if (absolutePath == "test/")
                    {
                        WriteLog("Web.SummaryController: call api- \"Test/FormDataEntry/GetFormDataEntryByEntId.\"");
                        userResult = _webApi.Get("Test/FormDataEntry/GetFormDataEntryByEntId/" + guid + "/" + formId + "/" + participant);
                    }
                    else
                    {
                        WriteLog("Web.SummaryController: call api- \"FormDataEntry/GetFormDataEntryByEntId.\"");
                        userResult = _webApi.Get("FormDataEntry/GetFormDataEntryByEntId/" + guid + "/" + formId + "/" + participant);
                    }


                    string sysAppr = null;
                    if (userResult.MessageType == "Success")
                    {
                        user = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FormDataEntryVariableViewModel>>(userResult.Content);

                        ViewBag.SearchId = user.Where(x => x.VariableName == DefaultsVariables.EntID.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                        ViewBag.FirstName = user.Where(x => x.VariableName == DefaultsVariables.FirstName.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                        ViewBag.Surname = user.Where(x => x.VariableName == DefaultsVariables.Name.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                        ViewBag.DOB = user.Where(x => x.VariableName == DefaultsVariables.DOB.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                        ViewBag.Gender = user.Where(x => x.VariableName == DefaultsVariables.Gender.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                        ViewBag.Email = user.Where(x => x.VariableName == DefaultsVariables.Email.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                        ViewBag.Phone = user.Where(x => x.VariableName == "Phone").Select(x => x.SelectedValues).FirstOrDefault();
                        ViewBag.State = user.Where(x => x.VariableName == "State").Select(x => x.SelectedValues).FirstOrDefault();
                        ViewBag.Suburb = user.Where(x => x.VariableName == "Suburb").Select(x => x.SelectedValues).FirstOrDefault();

                        ViewBag.Profession = user.Where(x => x.VariableName == DefaultsVariables.PerSType.ToString()).Select(x => x.SelectedValues).FirstOrDefault();

                        string midName = user.FirstOrDefault(x => x.VariableName == "MiddleName") != null ? user.FirstOrDefault(x => x.VariableName == "MiddleName").SelectedValues : string.Empty;
                        if (!string.IsNullOrEmpty(midName) && formName == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                        {
                            ViewBag.FirstName = ViewBag.FirstName + " " + midName;
                        }
                        
                        #region Current entity details
                        try
                        {
                            UserLoginViewModel currentEntityDB = null;
                            var currentEntity = _webApi.Get("user/GetUserById/" + user.FirstOrDefault().SelectedValues_int);
                            if (currentEntity.MessageType == "Success")
                            {
                                currentEntityDB = Newtonsoft.Json.JsonConvert.DeserializeObject<UserLoginViewModel>(userResult.Content);
                                if (string.IsNullOrEmpty(ViewBag.Phone))
                                {
                                    ViewBag.Phone = currentEntityDB != null ? currentEntityDB.Mobile : null;
                                }
                                if (string.IsNullOrEmpty(ViewBag.Address))
                                {
                                    ViewBag.Address = currentEntityDB != null ? currentEntityDB.Address : null;
                                }
                                if (string.IsNullOrEmpty(ViewBag.Email))
                                {
                                    ViewBag.Email = currentEntityDB != null ? currentEntityDB.Email : null;
                                }
                            }

                            if (formVM.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                            {
                                var projectLogo = user.FirstOrDefault(x => x.VariableName == DefaultsVariables.ProjectLogo.ToString());
                                string projectLogoSelectedValus = projectLogo != null ? projectLogo.SelectedValues : null;
                                ViewBag.EntityProfileImage = !string.IsNullOrEmpty(projectLogoSelectedValus) ? projectLogoSelectedValus : null;
                            }
                            else
                            {
                                string baseImagepath = System.Configuration.ConfigurationManager.AppSettings["ProfileImageBasePath"].ToString();
                                var imgPath = System.IO.Path.Combine(baseImagepath + "/", currentEntityDB.Guid.ToString() + ".jpg");
                                if (System.IO.File.Exists(imgPath))
                                {
                                    var stream = System.IO.File.ReadAllBytes(imgPath);
                                    ViewBag.EntityProfileImage = "data:image/jpeg;base64," + Convert.ToBase64String(stream);
                                }
                            }
                        }
                        catch (Exception exc) { }
                        #endregion

                        ViewBag.EntityCreatedDate = user.Select(x => x.CreatedDate).FirstOrDefault();
                        _thisActivityGuid = user.Select(x => x.ActivityGuid).FirstOrDefault() != null ? (Guid)user.Select(x => x.ActivityGuid).FirstOrDefault() : Guid.Empty;
                        entityCreationDate = (DateTime)ViewBag.EntityCreatedDate;

                        ViewBag.Gender = (ViewBag.Gender == "1" ? "Male" : ViewBag.Gender == "2" ? "Female" : ViewBag.Gender == "3" ? "Other" : string.Empty);
                        ViewBag.Profession = (ViewBag.Profession == "1" ? "Medical Practitioner/Allied Health" : (ViewBag.Profession == "2" ? "Non-Medical Practitioner" : ViewBag.Profession == string.Empty));

                        System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
                        Core.ViewModels.FormDataEntryVariableViewModel formStatus = user.FirstOrDefault(x => x.FormGuid == formId);
                        ViewBag.FormStatus = formStatus;
                        sysAppr = user.FirstOrDefault(x => x.VariableName == DefaultsVariables.SysAppr.ToString()) != null ? user.FirstOrDefault(x => x.VariableName == DefaultsVariables.SysAppr.ToString()).SelectedValues : null;
                    }
                    #endregion

                    #region API- get entity type id
                    var tempResponse = formResponse;
                    Guid EntityTypeGuid = Guid.Empty;
                    List<Core.ViewModels.EntityTypeViewModel> EntityTypes = new List<Core.ViewModels.EntityTypeViewModel>();
                    var entityTypeResponse = _webApi.Get("EntityType");
                    if (entityTypeResponse.MessageType == "Success")
                    {
                        EntityTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.EntityTypeViewModel>>(tempResponse.Content);
                        Core.ViewModels.EntityTypeViewModel ent = EntityTypes.FirstOrDefault(x => x.Name == ViewBag.EntityType);
                        EntityTypeGuid = ent != null ? ent.Guid : Guid.Empty;
                    }
                    #endregion

                    List<Core.ViewModels.SchedulingViewModel> UserActivity = new List<Core.ViewModels.SchedulingViewModel>();
                    List<Core.ViewModels.ActivityViewModel> allActivity = new List<Core.ViewModels.ActivityViewModel>();
                    var allActivityResponse = _webApi.Get("Scheduling/GetAllScheduledActivityByProjectId/" + guid);
                    if (allActivityResponse.MessageType == "Success")
                    {
                        List<Core.ViewModels.SchedulingViewModel> allActivity1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.SchedulingViewModel>>(allActivityResponse.Content);
                        if (ViewBag.EntityType == "Place/Group")
                        {
                            try
                            {
                                #region for specific entity type
                                var str = new string[] { "Person", "Hospital", "Practice/Clinic", "Laboratory", "Medical Imaging", "Research facility/University", "Healthcare Group", "Government Organisation", "Industry Group", "Consumer Group", "Activity Venue", "Vehicle", "MAC", "Ethics Committee", "API" };
                                var entType = user.Where(x => x.VariableName == "EntType").Select(x => x.SelectedValues).FirstOrDefault();
                                int i = !String.IsNullOrEmpty(entType) ? Convert.ToInt32(entType) : 0;
                                string vald = str[i - 1];

                                Core.ViewModels.EntityTypeViewModel ent = EntityTypes.FirstOrDefault(x => x.Name.ToLower() == vald.ToLower());
                                EntityTypeGuid = ent != null ? ent.Guid : Guid.Empty;
                                UserActivity = allActivity1.Where(x => x.EntityTypes.Contains(EntityTypeGuid)).ToList();
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                #region if not match with any entType
                                try
                                {
                                    List<string> enames = new List<string>();
                                    enames.Add("Person"); enames.Add("Participant"); enames.Add("Project");
                                    var elist = EntityTypes.Where(x => !enames.Contains(x.Name)).Select(x => x.Guid).ToList();
                                    if (elist.Count() > 0)
                                    {
                                        UserActivity = allActivity1.Where(x => elist.Contains(x.EntityTypes.FirstOrDefault())).ToList();
                                    }
                                }
                                catch (Exception exx) { }
                                #endregion
                            }
                        }
                        else
                            UserActivity = allActivity1.Where(x => x.EntityTypes.Contains(EntityTypeGuid)).ToList();

                        if (absolutePath == "test/")
                        {
                            UserActivity = UserActivity.Where(x => x.Status == (int)Core.Enum.ActivityDeploymentStatus.Pushed || x.Status == (int)Core.Enum.ActivityDeploymentStatus.Deployed).ToList();
                        }
                        else
                        {
                            UserActivity = UserActivity.Where(x => x.Status == (int)Core.Enum.ActivityDeploymentStatus.Deployed).ToList();
                        }
                    }
                    var allSummaryPageActivityResponse = new Utility.ResponseMessage();
                    if (absolutePath == "test/")
                    {
                        WriteLog("Web.SummaryController: gat all test site summary page activity.");
                        allSummaryPageActivityResponse = _webApi.Get("Review/GetAllSummaryPageActivity/" + participant + "/" + guid);
                    }
                    else
                    {
                        WriteLog("Web.SummaryController: gat all live site summary page activity.");
                        allSummaryPageActivityResponse = _webApi.Get("Activity/GetAllSummaryPageActivity/" + participant + "/" + guid);
                    }

                    if (allSummaryPageActivityResponse.MessageType == "Success")
                    {
                        List<Core.ViewModels.AddSummaryPageActivityViewModel> sumarypageactivitylist = new List<Core.ViewModels.AddSummaryPageActivityViewModel>();
                        sumarypageactivitylist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.AddSummaryPageActivityViewModel>>(allSummaryPageActivityResponse.Content);
                        ViewBag.SummaryPageActivityList = sumarypageactivitylist.OrderByDescending(x => x.ActivityDate).ThenByDescending(c => c.CreatedDate).ToList();

                        #region apply permission on activities  forms
                        string roleName = this.LoggedInUser.Roles.FirstOrDefault();

                        List<AddSummaryPageActivityViewModel> listSummaryPageActivityViewModel_ONE_LIST = new List<AddSummaryPageActivityViewModel>();
                        foreach (var activity in ViewBag.SummaryPageActivityList as List<AddSummaryPageActivityViewModel>)
                        {
                            if (roleName == RoleTypes.System_Admin.ToString().Replace("_", " ")
                                || roleName == RoleTypes.Project_Admin.ToString().Replace("_", " ")
                                )
                            {
                                if (activity.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration)
                                    && roleName == RoleTypes.Project_Admin.ToString().Replace("_", " "))
                                {

                                }
                                else
                                {
                                    listSummaryPageActivityViewModel_ONE_LIST.Add(activity);
                                }
                            }
                            else
                            {
                                if (activity.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Linkage)
                                    || activity.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                                {
                                    if (activity.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Linkage))
                                    {
                                        activity.Forms = new List<FormActivityViewModel>();
                                        listSummaryPageActivityViewModel_ONE_LIST.Add(activity);
                                    }
                                }
                                else
                                {
                                    listSummaryPageActivityViewModel_ONE_LIST.Add(activity);
                                }
                            }
                        }
                        ViewBag.SummaryPageActivityList = listSummaryPageActivityViewModel_ONE_LIST;
                        #endregion
                        List<Guid> removeList = new List<Guid>();
                        foreach (Core.ViewModels.SchedulingViewModel activities in UserActivity)
                        {
                            DateTime offsetDate = entityCreationDate;
                            if (activities.ScheduledToBeCompleted == (int)Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity)
                            {
                                Int16 count = 0;
                                if (activities.OffsetCount != null)
                                    count = Convert.ToInt16(activities.OffsetCount);
                                switch (activities.OffsetType)
                                {
                                    case (int)Core.Enum.SchedulingOffsetType.Day:
                                        offsetDate = offsetDate.AddDays(count);
                                        break;
                                    case (int)Core.Enum.SchedulingOffsetType.Weeks:
                                        offsetDate = offsetDate.AddDays(count * 7);
                                        break;
                                    case (int)Core.Enum.SchedulingOffsetType.Month:
                                        offsetDate = offsetDate.AddMonths(count);
                                        break;
                                    case (int)Core.Enum.SchedulingOffsetType.Year:
                                        offsetDate = offsetDate.AddYears(count);
                                        break;
                                    default:
                                        break;
                                }

                                if (offsetDate > DateTime.UtcNow)
                                {
                                    if (activities.OtherActivity == _thisActivityGuid)
                                    {
                                        removeList.Add(activities.ActivityId);
                                    }
                                }
                            }
                            if (activities.ActivityAvailableForCreation == (int)Core.Enum.SchedulingActivityAvailableForCreation.Only_if_specified_activity_had_already_been_created)
                            {
                                Core.ViewModels.AddSummaryPageActivityViewModel isAvaliable = sumarypageactivitylist.FirstOrDefault(x => x.ActivityId == activities.SpecifiedActivity && x.ProjectId == guid);
                                if (isAvaliable == null)
                                {
                                    removeList.Add(activities.ActivityId);
                                }
                            }
                            if (activities.ActivityAvailableForCreation == (int)Core.Enum.SchedulingActivityAvailableForCreation.Based_on_calendar_month_before_or_after_scheduled_date)
                            {
                                if (activities.ScheduledToBeCompleted == (int)Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity)
                                {
                                    int? start = activities.CreationWindowOpens;
                                    int? ends = activities.CreationWindowClose;
                                    try
                                    {
                                        DateTime acDate = offsetDate;
                                        DateTime edate = acDate.AddMonths((int)ends);
                                        DateTime sdate = acDate.AddMonths(-(int)start);
                                        if (sdate > DateTime.UtcNow || edate < DateTime.UtcNow)
                                        {
                                            removeList.Add(activities.ActivityId);
                                        }
                                        else
                                        {
                                            removeList.Remove(activities.ActivityId);
                                        }
                                    }
                                    catch (Exception exc) { }
                                }
                                else
                                {
                                    int? start = activities.CreationWindowOpens;
                                    int? ends = activities.CreationWindowClose;
                                    try
                                    {
                                        DateTime acDate = (DateTime)activities.ScheduleDate;
                                        DateTime edate = acDate.AddMonths((int)ends);
                                        DateTime sdate = acDate.AddMonths(-(int)start);
                                        if (sdate > DateTime.UtcNow || edate < DateTime.UtcNow)
                                        {
                                            removeList.Add(activities.ActivityId);
                                        }
                                    }
                                    catch (Exception exc) { }
                                }
                            }
                            if (activities.ActivityAvailableForCreation == (int)Core.Enum.SchedulingActivityAvailableForCreation.Based_on_days_before_or_after_scheduled_date)
                            {
                                if (activities.ScheduledToBeCompleted == (int)Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity)
                                {
                                    int? start = activities.CreationWindowOpens;
                                    int? ends = activities.CreationWindowClose;
                                    try
                                    {
                                        DateTime acDate = offsetDate;
                                        DateTime edate = acDate.AddDays((int)ends);
                                        DateTime sdate = acDate.AddDays(-(int)start);
                                        if (sdate > DateTime.UtcNow || edate < DateTime.UtcNow)
                                        {
                                            removeList.Add(activities.ActivityId);
                                        }
                                        else
                                        {
                                            removeList.Remove(activities.ActivityId);
                                        }
                                    }
                                    catch (Exception exc) { }
                                }
                                else
                                {
                                    int? start = activities.CreationWindowOpens;
                                    int? ends = activities.CreationWindowClose;
                                    try
                                    {
                                        DateTime acDate = (DateTime)activities.ScheduleDate;
                                        DateTime edate = acDate.AddDays((int)ends);
                                        DateTime sdate = acDate.AddDays(-(int)start);

                                        if (sdate > DateTime.UtcNow || edate < DateTime.UtcNow)
                                        {
                                            removeList.Add(activities.ActivityId);
                                        }
                                    }
                                    catch (Exception exc) { }
                                }
                            }
                            var loggedInUserRole = LoggedInUser.Roles.FirstOrDefault();
                            if (!activities.RolesToCreateActivity_Name.Any(s => s == loggedInUserRole))
                            {
                                if (activities.RolesToCreateActivity_Name.Count() > 0)
                                    removeList.Add(activities.ActivityId);
                            }
                            if (activities.RoleToCreateActivityRegardlessScheduled_Name.Any(s => LoggedInUser.Roles.Contains(s)))
                            {
                                removeList.RemoveAll(activity => activity == activities.ActivityId);
                            }
                        }
                        if (removeList.Count() > 0)
                            UserActivity = UserActivity.Where(x => !removeList.Contains(x.ActivityId)).ToList();

                        #region projectbuilder restriction
                        {
                            if (string.IsNullOrEmpty(sysAppr) || sysAppr == "0")
                            {
                                UserActivity = new List<SchedulingViewModel>();
                            }
                        }
                        #endregion
                        var ecrd = ViewBag.SummaryPageActivityList as List<AddSummaryPageActivityViewModel>;
                        if (ViewBag.EntityType == EntityTypesListInDB.Person.ToString())
                        {
                            AddSummaryPageActivityViewModel addSummaryPageActivityViewModel = ecrd.FirstOrDefault(x => x.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration));
                            ViewBag.EntityCreatedDate = addSummaryPageActivityViewModel != null ? addSummaryPageActivityViewModel.ActivityDate : ViewBag.EntityCreatedDate;
                        }
                        else if (ViewBag.EntityType == EntityTypesListInDB.Participant.ToString())
                        {
                            AddSummaryPageActivityViewModel addSummaryPageActivityViewModel = ecrd.FirstOrDefault(x => x.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration));
                            ViewBag.EntityCreatedDate = addSummaryPageActivityViewModel != null ? addSummaryPageActivityViewModel.ActivityDate : ViewBag.EntityCreatedDate;
                        }
                        else if (ViewBag.EntityType == EntityTypesListInDB.Place__Group.ToString().ToString().Replace("__", "/"))
                        {
                            AddSummaryPageActivityViewModel addSummaryPageActivityViewModel = ecrd.FirstOrDefault(x => x.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration));
                            ViewBag.EntityCreatedDate = addSummaryPageActivityViewModel != null ? addSummaryPageActivityViewModel.ActivityDate : ViewBag.EntityCreatedDate;
                        }
                        else if (ViewBag.EntityType == EntityTypesListInDB.Project.ToString())
                        {
                            AddSummaryPageActivityViewModel addSummaryPageActivityViewModel = ecrd.FirstOrDefault(x => x.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration));
                            ViewBag.EntityCreatedDate = addSummaryPageActivityViewModel != null ? addSummaryPageActivityViewModel.ActivityDate : ViewBag.EntityCreatedDate;
                        }
                        return PartialView("_SummarySQL", UserActivity);
                    }
                    #endregion
                }
                #endregion
            }
            SetErrorMessage("Please select project.");
            return View();
        }

        public ActionResult PerformActivity(Guid? guid = null)
        {
            var categories = new Core.ViewModels.ActivityViewModel();
            var activityResponse = _webApi.Get("Activity/" + guid);

            if (activityResponse.MessageType == "Success")
            {
                categories = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.ActivityViewModel>(activityResponse.Content);
            }
            ViewBag.ActivityName = categories.ActivityName;
            ViewBag.ActivityGuid = categories.Guid;
            var forms = new List<Core.ViewModels.FormActivityViewModel>();
            categories.Forms.Where(c => c.Status == "Published").ToList().ForEach(x => { forms.Add(x); });

            var formIds = categories.Forms.Select(x => x.Id).ToList();

            var formList = new List<Core.ViewModels.FormViewModel>();
            var formListResponse = _webApi.Post("Form/GetFormsByGuidList/", formIds);

            if (formListResponse.MessageType == "Success")
            {
                formList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.FormViewModel>>(formListResponse.Content);
                ViewBag.AllActivityForms = formList;
            }
            return View();
        }
    }
}