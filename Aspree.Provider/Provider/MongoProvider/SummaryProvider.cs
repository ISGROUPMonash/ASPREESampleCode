using Aspree.Core.Enum;
using Aspree.Core.ViewModels;
using Aspree.Core.ViewModels.MongoViewModels;
using Aspree.Data;
using Aspree.Data.MongoDB;
using Aspree.Provider.Interface;
using Aspree.Provider.Interface.MongoProvider;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Provider.MongoProvider
{
    public class SummaryProvider : ISummaryProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities _dbContext;
        private readonly MongoDBContext _mongoDBContext;
        private readonly TestMongoDBContext _testMongoDBContext;
        public SummaryProvider(
            IUserLoginProvider userLoginProvider
            , AspreeEntities dbContext
            , MongoDBContext mongoDBContext
            , TestMongoDBContext testMongoDBContext)
        {
            this._userLoginProvider = userLoginProvider;
            this._dbContext = dbContext;
            this._mongoDBContext = mongoDBContext;
            this._testMongoDBContext = testMongoDBContext;
        }
        public SummaryViewModel GetSummaryDetails(Guid projectId, Int64 entityId, Guid LoggedInUser)
        {
            SummaryViewModel model = new SummaryViewModel();
            string roleName = string.Empty;
            var entity = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().FirstOrDefault(x => x.EntityNumber == entityId);// x.ProjectGuid == projectId &&
            if (entity != null)
            {
                var condition = MongoDB.Driver.Builders.Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectId);
                var project = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                bool isDifferentProject = false;
                if (project == null)
                {
                    isDifferentProject = true;
                    condition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, entity.ProjectGuid);
                    project = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                }
                model.ProjectName = project.ProjectName;
                model.ProjectGuid = project.ProjectGuid;
                model.EntityType = entity.EntityTypeName;
                model.EntityNumber = entity.EntityNumber.ToString("D7");
                model.CreatedDate = entity.CreatedDate;
                model.EntityUserName = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "FirstName") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "FirstName").SelectedValues : string.Empty;
                model.EntityUserSurname = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Name") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Name").SelectedValues : string.Empty;
                model.DOB = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "DOB") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "DOB").SelectedValues : string.Empty;
                model.Gender = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Gender") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Gender").SelectedValues : string.Empty;
                model.Email = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Email") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Email").SelectedValues : string.Empty;
                model.Phone = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Phone") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Phone").SelectedValues : string.Empty;
                model.State = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "State") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "State").SelectedValues : string.Empty;
                model.Suburb = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Suburb") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Suburb").SelectedValues : string.Empty;

                string midName = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "MiddleName") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "MiddleName").SelectedValues : string.Empty;
                if (!string.IsNullOrEmpty(midName) && entity.FormTitle == Core.Enum.EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                {
                    model.EntityUserName = model.EntityUserName + " " + midName;
                }
                model.Postcode = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Postcode.ToString()) != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Postcode.ToString()).SelectedValues : string.Empty;
                #region Current entity details
                try
                {
                    var currentEntity = _dbContext.UserLogins.FirstOrDefault(x => x.Id == entity.ThisUserId);
                    if (string.IsNullOrEmpty(model.Phone))
                    {
                        model.Phone = currentEntity != null ? currentEntity.Mobile : null;
                    }
                    if (string.IsNullOrEmpty(model.Address))
                    {
                        model.Address = currentEntity != null ? currentEntity.Address : null;
                    }
                    if (string.IsNullOrEmpty(model.Email))
                    {
                        model.Email = currentEntity != null ? currentEntity.Email : null;
                    }

                    if (entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                    {
                        var projectLogo = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ProjectLogo.ToString());
                        string projectLogoSelectedValus = projectLogo != null ? projectLogo.SelectedValues : null;
                        model.EntityProfileImage = !string.IsNullOrEmpty(projectLogoSelectedValus) ? projectLogoSelectedValus : null;
                    }
                    else if (entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                    {
                        var projectLogo = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PlaceProfilePicture.ToString());
                        string projectLogoSelectedValus = projectLogo != null ? projectLogo.SelectedValues : null;
                        model.EntityProfileImage = !string.IsNullOrEmpty(projectLogoSelectedValus) ? projectLogoSelectedValus : null;
                    }
                    else
                    {
                        string baseImagepath = System.Configuration.ConfigurationManager.AppSettings["ProfileImageBasePath"].ToString();
                        var imgPath = System.IO.Path.Combine(baseImagepath + "/", currentEntity.Guid.ToString() + ".jpg");
                        if (System.IO.File.Exists(imgPath))
                        {
                            model.EntityProfileImage = imgPath;
                        }
                    }

                }
                catch (Exception ex)
                { }
                #endregion
                #region Default variables display in left panel
                try
                {
                    var mdl = UpdateLeftPanelSummaryPage(projectId, entityId);
                    model.Email = mdl.Email;
                    model.Phone = mdl.Phone;
                    model.State = mdl.State;
                    model.Suburb = mdl.Suburb;
                    model.Postcode = mdl.Postcode;
                    model.Fax = mdl.Fax;
                    model.StrtNum = mdl.StrtNum;
                    model.StrtNum2 = mdl.StrtNum2;
                    model.StrtNme = mdl.StrtNme;
                    model.StrtNme2 = mdl.StrtNme2;
                }
                catch (Exception exDefaultVariablesList)
                { Console.WriteLine(exDefaultVariablesList.Message); }
                #endregion
                model.Gender = (model.Gender == "1" ? "Male" : model.Gender == "2" ? "Female" : model.Gender == "3" ? "Other" : string.Empty);
                model.Profession = entity.formDataEntryVariableMongoList.Where(x => x.VariableName == DefaultsVariables.PerSType.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                model.Profession = model.Profession == "1" ? "Medical Practitioner/Allied Health" : model.Profession == "2" ? "Non-Medical Practitioner" : string.Empty;

                if (entity.FormTitle == "Project Registration")
                {
                    model.EntityUserName = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Name") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Name").SelectedValues : string.Empty;
                    model.EntityUserSurname = string.Empty;
                }
                var summaryPageActivityForms = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindAll().AsQueryable().Where(x => x.PersonEntityId == entityId && x.DateDeactivated == null).AsQueryable();
                if (summaryPageActivityForms.Count() != 0)
                {
                    string[] defaultActivitiesName = new string[]
                    {
                        EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Linkage)
                    };
                    var defaultActivities = summaryPageActivityForms.Where(x => defaultActivitiesName.Contains(x.ActivityName)).ToList();
                    summaryPageActivityForms = summaryPageActivityForms.Where(x => x.ProjectGuid == projectId && !defaultActivitiesName.Contains(x.ActivityName));
                    var list1 = summaryPageActivityForms.ToList();
                    var list2 = defaultActivities.ToList();
                    var result = list1.Concat(list2).OrderBy(x => x.ActivityDate).ToList();
                    summaryPageActivityForms = result.AsQueryable();
                }

                List<SummaryPageActivityViewModel> listSummaryPageActivityViewModel = new List<SummaryPageActivityViewModel>();
                summaryPageActivityForms.ToList().ForEach(y =>
                {
                    if (y.ActivityName == "Project Linkage")
                    {
                        var conditionEntity = Query.And(Query<FormDataEntryMongo>.EQ(w => w.SummaryPageActivityObjId, Convert.ToString(y.Id)));
                        var projectLinkageConditionEntity = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionEntity).AsQueryable().FirstOrDefault();
                        if (projectLinkageConditionEntity != null)
                        {
                            var projectguid = projectLinkageConditionEntity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "LnkPro")
                                                  != null
                                                  ? projectLinkageConditionEntity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "LnkPro").SelectedValues
                                                  : null;
                            Guid projectGuid = !string.IsNullOrEmpty(projectguid) ? new Guid(projectguid) : Guid.Empty;
                            var allProjects = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").FindAll().AsQueryable().OrderByDescending(x => x.ProjectDeployDate).AsQueryable();
                            var thisproject = allProjects.FirstOrDefault(x => x.ProjectGuid == projectGuid);

                            if (thisproject == null)
                            {
                                var sqlProject = _dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == projectGuid);
                                if (sqlProject != null)
                                {
                                    thisproject = new ProjectDeployViewModel();
                                    thisproject.ProjectName = sqlProject.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == "Name")
                                    != null ? sqlProject.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == "Name").SelectedValues : null;
                                    thisproject.ProjectGuid = sqlProject.Guid;
                                }
                            }
                            y.LinkedProjectName = thisproject != null ? thisproject.ProjectName : null;
                            y.LinkedProjectGuid = thisproject != null ? thisproject.ProjectGuid : (Guid?)null;
                        }
                    }
                    listSummaryPageActivityViewModel.Add(y);
                });
                summaryPageActivityForms = listSummaryPageActivityViewModel.AsQueryable();

                #region apply permission on activities  forms
                roleName = string.Empty;
                try
                {
                    var roleLoggedinProj = _dbContext.ProjectStaffMemberRoles.FirstOrDefault(x => x.FormDataEntry.Guid == projectId && x.UserLogin.Guid == LoggedInUser);
                    if (roleLoggedinProj == null)
                    {
                        var userlogin = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == LoggedInUser);
                        if (userlogin != null)
                        {
                            var role = userlogin.UserRoles.FirstOrDefault();
                            roleName = role != null ? role.Role.Name : string.Empty;
                        }
                    }
                    else
                    {
                        try { roleName = roleLoggedinProj.Role.Name; } catch (Exception exc) { }
                    }
                }
                catch (Exception excRole)
                {
                    ProjectStaffMongo roleCurrentUserole = project.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == LoggedInUser);
                    roleName = roleCurrentUserole != null ? roleCurrentUserole.Role : string.Empty;
                }
                List<SummaryPageActivityViewModel> listSummaryPageActivityViewModel_ONE_LIST = new List<SummaryPageActivityViewModel>();
                SummaryPageActivityViewModel listSummaryPageActivityViewModel_OBJ = new SummaryPageActivityViewModel();
                foreach (var activity in listSummaryPageActivityViewModel)
                {
                    listSummaryPageActivityViewModel_OBJ = new SummaryPageActivityViewModel();
                    listSummaryPageActivityViewModel_OBJ = activity;
                    ActivitiesMongo projectActivity = null;
                    if (activity.ProjectVersion != project.ProjectInternalVersion)
                    {
                        var conditionOtherVersion = Query.And(Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectId), Query<ProjectDeployViewModel>.EQ(r => r.ProjectInternalVersion, activity.ProjectVersion));
                        var projectOtherVersion = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionOtherVersion).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                        if (projectOtherVersion != null)
                        {
                            projectActivity = projectOtherVersion.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == activity.ActivityGuid);
                        }
                        else
                        {
                            projectActivity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == activity.ActivityGuid);
                        }
                    }
                    else
                    {
                        projectActivity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == activity.ActivityGuid);
                    }
                    List<Guid> removedFormGuid = new List<Guid>();
                    foreach (var form in activity.SummaryPageActivityFormsList)
                    {
                        int count = 0;
                        int totalVariables = 0;
                        if (projectActivity != null)
                        {
                            try
                            {
                                FormsMongo frm = projectActivity.FormsListMongo.FirstOrDefault(x => x.FormGuid == form.FormGuid);
                                if (frm != null)
                                {
                                    IQueryable<VariablesMongo> variables = frm.VariablesListMongo.AsQueryable();
                                    totalVariables = variables.Count();
                                    variables.ToList().ForEach(v =>
                                    {
                                        var variable = v.VariableRoleListMongo.Where(x => x.RoleName == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                                        if (variable == null)
                                        {
                                            count++;
                                        }
                                        else
                                        {
                                            if (v.DependentVariableId != null)
                                            {
                                                var parentVar = variables.FirstOrDefault(x => x.VariableId == v.DependentVariableId);
                                                var isParentVar = parentVar.VariableRoleListMongo.Where(x => x.RoleName == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                                                if (isParentVar == null) { count++; }
                                            }
                                        }
                                    });
                                }
                                if (count > 0)
                                {
                                    if (totalVariables == count)
                                        removedFormGuid.Add(form.FormGuid);
                                }
                            }
                            catch (Exception exc)
                            {
                            }
                        }
                        else
                        {
                            IMongoQuery checkProjectCondition;
                            if (activity.ProjectVersion != project.ProjectInternalVersion)
                            {
                                checkProjectCondition = Query.And(Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, activity.ProjectGuid), Query<ProjectDeployViewModel>.EQ(r => r.ProjectInternalVersion, activity.ProjectVersion));
                            }
                            else
                            {
                                checkProjectCondition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, activity.ProjectGuid);
                            }
                            var checkProject = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(checkProjectCondition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                            if (checkProject != null)
                            {
                                try
                                {
                                    var chkActivity = checkProject.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == activity.ActivityGuid);
                                    if (chkActivity != null)
                                    {
                                        FormsMongo frm = chkActivity.FormsListMongo.FirstOrDefault(x => x.FormGuid == form.FormGuid);
                                        if (frm != null)
                                        {
                                            IQueryable<VariablesMongo> variables = frm.VariablesListMongo.AsQueryable();
                                            totalVariables = variables.Count();
                                            variables.ToList().ForEach(v =>
                                            {
                                                var variable = v.VariableRoleListMongo.Where(x => x.RoleName == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                                                if (variable == null)
                                                {
                                                    count++;
                                                }
                                                else
                                                {
                                                    if (v.DependentVariableId != null)
                                                    {
                                                        var parentVar = variables.FirstOrDefault(x => x.VariableId == v.DependentVariableId);
                                                        var isParentVar = parentVar.VariableRoleListMongo.Where(x => x.RoleName == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                                                        if (isParentVar == null) { count++; }
                                                    }
                                                }
                                            });
                                        }
                                        if (count > 0)
                                        {
                                            if (totalVariables == count)
                                                removedFormGuid.Add(form.FormGuid);
                                        }
                                    }
                                }
                                catch (Exception exc)
                                {
                                }
                            }
                            else
                            {
                                try
                                {
                                    var frm = _dbContext.Forms.FirstOrDefault(x => x.Guid == form.FormGuid);
                                    if (frm != null)
                                    {
                                        frm.FormVariables.Count();
                                        frm.FormVariables.ToList().ForEach(v =>
                                        {
                                            var variable = v.FormVariableRoles.Where(x => x.Role.Name == roleName && x.CanView == true).FirstOrDefault();
                                            if (variable == null)
                                            {
                                                count++;
                                            }
                                            else
                                            {
                                                if (v.DependentVariableId != null)
                                                {
                                                    var parentVar = frm.FormVariables.FirstOrDefault(x => x.VariableId == v.DependentVariableId);
                                                    var isParentVar = parentVar.FormVariableRoles.Where(x => x.Role.Name == roleName && x.CanView == true).FirstOrDefault();
                                                    if (isParentVar == null) { count++; }
                                                }
                                            }
                                        });
                                    }
                                    if (count > 0)
                                    {
                                        if (totalVariables == count)
                                            removedFormGuid.Add(form.FormGuid);
                                    }
                                }
                                catch (Exception exceSQL)
                                {
                                }
                            }
                        }
                    }

                    if (removedFormGuid.Count() > 0)
                    {
                        listSummaryPageActivityViewModel_OBJ.SummaryPageActivityFormsList.RemoveAll(x => removedFormGuid.Contains(x.FormGuid));
                    }

                    listSummaryPageActivityViewModel_ONE_LIST.Add(listSummaryPageActivityViewModel_OBJ);
                }
                summaryPageActivityForms = listSummaryPageActivityViewModel_ONE_LIST.AsQueryable();
                #endregion
                model.SummaryPageActivitiesList = summaryPageActivityForms.OrderByDescending(x => x.ActivityDate).ToList();
                #region Activity Completed By drop-down
                model.SummaryPageProjectUsersList = new List<SummaryPageProjectUser>();
                SummaryPageProjectUser summaryPageProjectUser = new SummaryPageProjectUser();
                if (entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                    || entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
                    || entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)
                    )
                {
                    var projectStaffList = _userLoginProvider.GetProjectAllUsers(projectId);
                    projectStaffList.ToList().ForEach(stf =>
                    {
                        summaryPageProjectUser = new SummaryPageProjectUser
                        {
                            UserName = stf.FirstName + " " + stf.LastName,
                            UserGuid = stf.Guid,
                            UserId = stf.Id,
                        };
                        model.SummaryPageProjectUsersList.Add(summaryPageProjectUser);
                    });
                }
                else if (entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                {
                    try
                    {
                        var allSAUsers = _dbContext.UserRoles.Where(x => x.Role.Name == RoleTypes.System_Admin.ToString().Replace("_", " ") && x.UserLogin.UserTypeId != (int)UsersLoginType.Test && x.UserLogin.IsUserApprovedBySystemAdmin && x.UserLogin.Status == (int)Core.Enum.Status.Active).OrderBy(x => x.UserLogin.FirstName).ToList();
                        allSAUsers.ForEach(sa_user =>
                        {
                            model.SummaryPageProjectUsersList.Add(new SummaryPageProjectUser
                            {
                                UserName = sa_user.UserLogin.FirstName + " " + sa_user.UserLogin.LastName,
                                UserGuid = sa_user.UserLogin.Guid,
                                UserId = sa_user.UserLogin.Id,
                            });
                        });

                    }
                    catch (Exception exc) { }
                }
                #endregion

                model.SummaryPageActivityTypeList = new List<SummaryPageActivityTypes>();
                SummaryPageActivityTypes scheduledActivity = new SummaryPageActivityTypes();

                if (isDifferentProject)
                {
                    ActivitiesMongo projectLinkageActivity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityName == DefaultFormName.Project_Linkage.ToString().Replace("_", " "));
                    project = new ProjectDeployViewModel();
                    project.ProjectActivitiesList = new List<ActivitiesMongo>();

                    try
                    {
                        project.ProjectActivitiesList.Add(projectLinkageActivity);
                    }
                    catch (Exception exc) { }
                }
                int i = 0;
                string EntityTypeName = null;
                List<Core.ViewModels.EntityTypeViewModel> EntityTypes = _dbContext.EntityTypes.Select(et => new Core.ViewModels.EntityTypeViewModel
                {
                    Guid = et.Guid,
                    Id = et.Id,
                    Name = et.Name,
                    TenantId = et.TenantId != null ? et.Tenant.Guid : (Guid?)null,
                }).ToList();

                Core.ViewModels.EntityTypeViewModel ent = EntityTypes.FirstOrDefault(x => x.Name == model.EntityType);
                EntityTypeName = ent != null ? ent.Name : null;

                if (model.EntityType == "Place/Group")
                {
                    try
                    {
                        #region for specific entity type
                        var str = new string[] { "Person", "Hospital", "Practice/Clinic", "Laboratory", "Medical Imaging", "Research facility/University", "Healthcare Group", "Government Organisation", "Industry Group", "Consumer Group", "Activity Venue", "Vehicle", "MAC", "Ethics Committee", "API" };
                        var entType = entity.formDataEntryVariableMongoList.Where(x => x.VariableName == "EntType").Select(x => x.SelectedValues).FirstOrDefault();
                        int ii = !String.IsNullOrEmpty(entType) ? Convert.ToInt32(entType) : 0;
                        string vald = str[ii - 1];
                        Core.ViewModels.EntityTypeViewModel entNme = EntityTypes.FirstOrDefault(x => x.Name.ToLower() == vald.ToLower());
                        EntityTypeName = entNme != null ? entNme.Name : null;
                        var allPlaceType = project.ProjectActivitiesList.Where(x => x.ActivityEntityTypeName == EntityTypesListInDB.Place__Group.ToString().Replace("__", "/"));
                        project.ProjectActivitiesList = project.ProjectActivitiesList.Where(x => x.ActivityEntityTypeName.Contains(EntityTypeName)).ToList();
                        project.ProjectActivitiesList.AddRange(allPlaceType);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        #region if not match with any entType
                        try
                        {
                            List<string> enames = new List<string>();
                            enames.Add("Person"); enames.Add("Participant"); enames.Add("Project");
                            var elist = EntityTypes.Where(x => !enames.Contains(x.Name)).Select(x => x.Name).ToList();
                            if (elist.Count() > 0)
                            {
                                project.ProjectActivitiesList = project.ProjectActivitiesList.Where(x => elist.Contains(x.ActivityEntityTypeName)).ToList();
                            }
                        }
                        catch (Exception exx) { }
                        #endregion
                    }
                }
                else
                {
                    project.ProjectActivitiesList = project.ProjectActivitiesList.Where(x => x.ActivityEntityTypeName.Contains(EntityTypeName)).ToList();
                }

                var idsToRemove = new List<Guid>();
                foreach (var activity in project.ProjectActivitiesList.Where(x => x.DateDeactivated == null))
                {
                    if (!activity.CanCreatedMultipleTime)
                    {
                        var isAdded = model.SummaryPageActivitiesList.Where(x => x.ActivityGuid == activity.ActivityGuid).FirstOrDefault();
                        if (isAdded != null) continue;
                    }

                    Guid ActivityAdd = Guid.Empty;

                    scheduledActivity = new SummaryPageActivityTypes();
                    DateTime entityCreatedDate = activity.ScheduleDate ?? Convert.ToDateTime(activity.ScheduleDate);
                    #region question 1: When is the activity scheduled to be completed? 
                    if (activity.ScheduledToBeCompleted == Core.Enum.ScheduledToBeCompleted.Unscheduled.ToString())
                    {
                        ActivityAdd = activity.ActivityGuid;
                    }
                    else if (activity.ScheduledToBeCompleted == Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity.ToString())
                    {
                        #region Offset from another activity
                        SummaryPageActivityViewModel isAvaliable = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == activity.OtherActivityName && x.ProjectGuid == projectId);
                        if (isAvaliable != null)
                        {
                            ActivityAdd = activity.ActivityGuid;

                            //[ASPMONASH-225] Commented offest logic based on [ASPMONASH-225]
                            #region [ASPMONASH-225] Removed offest logic based on [ASPMONASH-225]
                            //Int16 count = 0;
                            //if (activity.OffsetCount != null)
                            //    count = Convert.ToInt16(activity.OffsetCount);
                            //DateTime acDate = isAvaliable.ActivityDate;
                            //switch (activity.OffsetTypeName)
                            //{
                            //    case "Day":
                            //        acDate = acDate.AddDays(count);
                            //        break;
                            //    case "Weeks":
                            //        acDate = acDate.AddDays(count * 7);
                            //        break;
                            //    case "Month":
                            //        acDate = acDate.AddMonths(count);
                            //        break;
                            //    case "Year":
                            //        acDate = acDate.AddYears(count);
                            //        break;
                            //    default:
                            //        break;
                            //}
                            //if (acDate.Date > DateTime.UtcNow.Date)
                            //{
                            //    //PC
                            //    //idsToRemove.Add(activity.ActivityGuid);
                            //    //LM
                            //    ActivityAdd = activity.ActivityGuid;
                            //}
                            //else
                            //{
                            //    ActivityAdd = activity.ActivityGuid;
                            //} 
                            #endregion
                        }
                        #endregion
                    }
                    #endregion

                    #region question 2: When should the activity be available for creation on the summary page?
                    if (activity.ActivityAvailableForCreation == Core.Enum.SchedulingActivityAvailableForCreation.Always_available.ToString())
                    {
                        ActivityAdd = activity.ActivityGuid;
                    }
                    else if (activity.ActivityAvailableForCreation == Core.Enum.SchedulingActivityAvailableForCreation.Only_if_specified_activity_had_already_been_created.ToString())
                    {
                        #region Only if specified activity had already been created
                        SummaryPageActivityViewModel isAvaliable = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == activity.SpecifiedActivityName && x.ProjectGuid == projectId);
                        if (isAvaliable != null)
                        {
                            ActivityAdd = activity.ActivityGuid;

                            //[ASPMONASH-225] Commented offest logic based on [ASPMONASH-225]
                            #region [ASPMONASH-225] Removed offest logic based on [ASPMONASH-225]
                            //if (activity.ScheduledToBeCompleted == Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity.ToString())
                            //{
                            //    SummaryPageActivityViewModel isOffsetAvaliable = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == activity.OtherActivityName && x.ProjectGuid == projectId);
                            //    if (isOffsetAvaliable != null)
                            //    {
                            //        Int16 count = 0;
                            //        if (activity.OffsetCount != null)
                            //            count = Convert.ToInt16(activity.OffsetCount);
                            //        DateTime acDate = isOffsetAvaliable.ActivityDate;
                            //        switch (activity.OffsetTypeName)
                            //        {
                            //            case "Day":
                            //                acDate = acDate.AddDays(count);
                            //                break;
                            //            case "Weeks":
                            //                acDate = acDate.AddDays(count * 7);
                            //                break;
                            //            case "Month":
                            //                acDate = acDate.AddMonths(count);
                            //                break;
                            //            case "Year":
                            //                acDate = acDate.AddYears(count);
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (acDate.Date > DateTime.UtcNow.Date)
                            //        {
                            //            //PC
                            //            //idsToRemove.Add(activity.ActivityGuid);
                            //            //LM
                            //            ActivityAdd = activity.ActivityGuid;
                            //        }
                            //        else
                            //        {
                            //            ActivityAdd = activity.ActivityGuid;
                            //        }
                            //    }
                            //    ActivityAdd = activity.ActivityGuid;
                            //}
                            //else
                            //{
                            //    ActivityAdd = activity.ActivityGuid;
                            //} 
                            #endregion
                        }
                        else
                        {
                            ActivityAdd = Guid.Empty;
                        }
                        #endregion
                    }
                    else if (activity.ActivityAvailableForCreation == Core.Enum.SchedulingActivityAvailableForCreation.Based_on_calendar_month_before_or_after_scheduled_date.ToString())
                    {
                        #region Based on calendar month before or after scheduled date
                        if (activity.ScheduledToBeCompleted == Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity.ToString())
                        {
                            SummaryPageActivityViewModel isAvaliable = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == activity.OtherActivityName && x.ProjectGuid == projectId);
                            if (isAvaliable != null)
                            {
                                int? start = activity.CreationWindowOpens;
                                int? ends = activity.CreationWindowClose;
                                try
                                {
                                    DateTime acDate = isAvaliable.ActivityDate;
                                    Int16 count = 0;
                                    if (activity.OffsetCount != null)
                                        count = Convert.ToInt16(activity.OffsetCount);
                                    switch (activity.OffsetTypeName)
                                    {
                                        case "Day":
                                            acDate = acDate.AddDays(count);
                                            break;
                                        case "Weeks":
                                            acDate = acDate.AddDays(count * 7);
                                            break;
                                        case "Month":
                                            acDate = acDate.AddMonths(count);
                                            break;
                                        case "Year":
                                            acDate = acDate.AddYears(count);
                                            break;
                                        default:
                                            break;
                                    }
                                    DateTime edate = acDate.AddMonths((int)ends);
                                    DateTime sdate = acDate.AddMonths(-(int)start);
                                    if (sdate.Date <= DateTime.UtcNow.ToLocalTime().Date && edate.Date >= DateTime.UtcNow.ToLocalTime().Date)
                                    {
                                        ActivityAdd = activity.ActivityGuid;
                                    }
                                    else
                                    {
                                        ActivityAdd = Guid.Empty;
                                    }
                                    #endregion
                                }
                                catch (Exception exc) { }
                            }
                        }
                        else
                        {
                            int? start = activity.CreationWindowOpens;
                            int? ends = activity.CreationWindowClose;
                            try
                            {
                                DateTime acDate = (DateTime)activity.ScheduleDate;
                                DateTime edate = acDate.AddMonths((int)ends);
                                DateTime sdate = acDate.AddMonths(-(int)start);
                                if (sdate.Date <= DateTime.UtcNow.ToLocalTime().Date && edate.Date >= DateTime.UtcNow.ToLocalTime().Date)
                                {
                                    ActivityAdd = activity.ActivityGuid;
                                }
                                else
                                {
                                    ActivityAdd = Guid.Empty;
                                }

                            }
                            catch (Exception exc) { }
                        }
                        #endregion
                    }
                    else if (activity.ActivityAvailableForCreation == Core.Enum.SchedulingActivityAvailableForCreation.Based_on_days_before_or_after_scheduled_date.ToString())
                    {
                        #region Based on days before or after scheduled date

                        if (activity.ScheduledToBeCompleted == Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity.ToString())
                        {

                            SummaryPageActivityViewModel isAvaliable = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == activity.OtherActivityName && x.ProjectGuid == projectId);
                            if (isAvaliable != null)
                            {

                                DateTime acDate = isAvaliable.ActivityDate;
                                Int16 count = 0;
                                if (activity.OffsetCount != null)
                                    count = Convert.ToInt16(activity.OffsetCount);
                                switch (activity.OffsetTypeName)
                                {
                                    case "Day":
                                        acDate = acDate.AddDays(count);
                                        break;
                                    case "Weeks":
                                        acDate = acDate.AddDays(count * 7);
                                        break;
                                    case "Month":
                                        acDate = acDate.AddMonths(count);
                                        break;
                                    case "Year":
                                        acDate = acDate.AddYears(count);
                                        break;
                                    default:
                                        break;
                                }
                                int? start = activity.CreationWindowOpens;
                                int? ends = activity.CreationWindowClose;
                                try
                                {
                                    DateTime edate = acDate.AddDays((int)ends);
                                    DateTime sdate = acDate.AddDays(-(int)start);

                                    if (sdate.Date <= DateTime.UtcNow.ToLocalTime().Date && edate.Date >= DateTime.UtcNow.ToLocalTime().Date)
                                    {
                                        ActivityAdd = activity.ActivityGuid;
                                    }
                                    else
                                    {
                                        ActivityAdd = Guid.Empty;
                                    }
                                }
                                catch (Exception exc) { }
                            }
                        }
                        else
                        {
                            int? start = activity.CreationWindowOpens;
                            int? ends = activity.CreationWindowClose;
                            try
                            {
                                DateTime acDate = (DateTime)activity.ScheduleDate;
                                DateTime edate = acDate.AddDays((int)ends);
                                DateTime sdate = acDate.AddDays(-(int)start);

                                if (sdate.Date <= DateTime.UtcNow.ToLocalTime().Date && edate.Date >= DateTime.UtcNow.ToLocalTime().Date)
                                {
                                    ActivityAdd = activity.ActivityGuid;
                                }
                                else
                                {
                                    ActivityAdd = Guid.Empty;
                                }
                            }
                            catch (Exception exc) { }
                        }
                        #endregion
                    }

                    #region question 3: Which roles should be able to create the activity as scheduled?
                    var role = string.Empty;
                    ProjectStaffMongo loggedUser = null;
                    if (project != null && project.ProjectStaffListMongo == null)
                    {
                        try
                        {
                            var stf = _dbContext.ProjectStaffMemberRoles.FirstOrDefault(x => x.UserLogin.Guid == LoggedInUser && x.FormDataEntry.Guid == projectId);
                            loggedUser = new ProjectStaffMongo()
                            {
                                Role = stf.Role.Name,
                                StaffGuid = LoggedInUser,
                                StaffName = stf.UserLogin.FirstName + " " + stf.UserLogin.LastName,
                            };
                        }
                        catch (Exception exc) { }
                    }
                    else
                    {
                        loggedUser = project.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == LoggedInUser);
                    }
                    if (loggedUser == null)
                    {
                        try
                        {
                            var user = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == LoggedInUser);
                            if (user != null)
                            {
                                role = user.UserRoles.FirstOrDefault().Role.Name;
                            }
                        }
                        catch (Exception ec)
                        { }
                    }

                    if (loggedUser != null)
                    {
                        role = loggedUser.Role;
                    }
                    if (!activity.RolesToCreateActivity.Any(s => s == role))
                    {
                        if (activity.RolesToCreateActivity.Count() > 0)
                            ActivityAdd = Guid.Empty;
                    }
                    #endregion

                    #region question 4: Which roles should be able to create the activity regardless of schedule?
                    if (activity.RoleToCreateActivityRegardlessScheduled.Any(s => role.Contains(s)))
                    {
                        ActivityAdd = activity.ActivityGuid;
                    }
                    #endregion
                    if (ActivityAdd != Guid.Empty)
                    {
                        scheduledActivity.ActivityGuid = activity.ActivityGuid;
                        scheduledActivity.ActivityId = activity.ActivityId;
                        scheduledActivity.ActivityName = activity.ActivityName;
                        scheduledActivity.IsDefaultActivity = activity.IsDefaultActivity;
                        model.SummaryPageActivityTypeList.Add(scheduledActivity);
                    }
                }
                #region PC
                if (idsToRemove.Count > 0)
                {
                    foreach (var item in idsToRemove)
                    {
                        var currActivity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == item);
                        if (currActivity != null)
                        {
                            project.ProjectActivitiesList.Remove(currActivity);
                        }
                        var currActivity1 = model.SummaryPageActivityTypeList.FirstOrDefault(x => x.ActivityGuid == item);
                        if (currActivity1 != null)
                        {
                            model.SummaryPageActivityTypeList.Remove(currActivity1);
                        }
                    }
                }
                #endregion

                #region apply role based permission on Activititty type list//apply permission on forms
                if (roleName == string.Empty)
                {
                    var userlogin = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == LoggedInUser);
                    if (userlogin != null)
                    {
                        var role = userlogin.UserRoles.FirstOrDefault();
                        roleName = role != null ? role.Role.Name : null;
                    }
                }

                List<SummaryPageActivityTypes> listSummaryPageActivityTypeList = new List<SummaryPageActivityTypes>();
                SummaryPageActivityViewModel summaryPageActivityType_OBJ = new SummaryPageActivityViewModel();
                #region ASPMONAS Sprint 13-- [ASPMONASH -225]   Remove as client requirment   
                //model. = listSummaryPageActivityTypeLister;
                #endregion
                #endregion

                if (entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration) || entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                {
                    string sysAppr = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.SysAppr.ToString()) != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.SysAppr.ToString()).SelectedValues : null;
                    if (string.IsNullOrEmpty(sysAppr) || sysAppr == "0")
                    {
                        //remove from activity type drop-down
                        //var projectLinkageActivityType = model.SummaryPageActivityTypeList.FirstOrDefault(x => x.ActivityName == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Linkage));
                        //model.SummaryPageActivityTypeList.Remove(projectLinkageActivityType);
                        model.SummaryPageActivityTypeList = new List<SummaryPageActivityTypes>();


                        string[] summaryPageDefaultActivities = new string[] {
                            EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
                            ,EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                            ,EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)
                            ,EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration)
                        };

                        //remove from added activities list
                        model.SummaryPageActivitiesList.RemoveAll(activity => !summaryPageDefaultActivities.Contains(activity.ActivityName));
                    }
                }




                model.SummaryPageActivitiesList = model.SummaryPageActivitiesList.OrderByDescending(x => x.ActivityDate.Date).ThenByDescending(c => c.CreatedDate).ToList();

                //[ASPMONAS Sprint 12]  Ticket no: ASPMONASH-210
                string[] allowedRole = new string[] {
                    RoleTypes.System_Admin.ToString().Replace("_"," ")
                    , RoleTypes.Project_Admin.ToString().Replace("_"," ")
                };
                if (!allowedRole.Contains(roleName))
                {
                    model.SummaryPageActivitiesList.RemoveAll(x => x.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Linkage));
                }


                #region Update entity registration date
                if (model.EntityType == EntityTypesListInDB.Person.ToString())
                {
                    SummaryPageActivityViewModel addSummaryPageActivityViewModel = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration));
                    model.CreatedDate = addSummaryPageActivityViewModel != null ? addSummaryPageActivityViewModel.ActivityDate : model.CreatedDate;
                }
                else if (model.EntityType == EntityTypesListInDB.Participant.ToString())
                {
                    SummaryPageActivityViewModel addSummaryPageActivityViewModel = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration));
                    model.CreatedDate = addSummaryPageActivityViewModel != null ? addSummaryPageActivityViewModel.ActivityDate : model.CreatedDate;
                }
                else if (model.EntityType == EntityTypesListInDB.Place__Group.ToString().ToString().Replace("__", "/"))
                {
                    SummaryPageActivityViewModel addSummaryPageActivityViewModel = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration));
                    model.CreatedDate = addSummaryPageActivityViewModel != null ? addSummaryPageActivityViewModel.ActivityDate : model.CreatedDate;
                }
                else if (model.EntityType == EntityTypesListInDB.Project.ToString())
                {
                    SummaryPageActivityViewModel addSummaryPageActivityViewModel = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration));
                    model.CreatedDate = addSummaryPageActivityViewModel != null ? addSummaryPageActivityViewModel.ActivityDate : model.CreatedDate;
                }
                #endregion
                return model;
            }
            return null;
        }
        public SummaryViewModel TestEnvironment_GetSummaryDetails(Guid projectId, Int64 entityId, Guid LoggedInUser)
        {
            SummaryViewModel model = new SummaryViewModel();
            var entity = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().FirstOrDefault(x => x.EntityNumber == entityId);// x.ProjectGuid == projectId &&
            if (entity != null)
            {
                var condition = MongoDB.Driver.Builders.Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectId);
                var project = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                bool isDifferentProject = false;
                if (project == null)
                {
                    isDifferentProject = true;
                    condition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, entity.ProjectGuid);
                    project = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                }
                model.ProjectName = project.ProjectName;
                model.ProjectGuid = project.ProjectGuid;
                model.EntityType = entity.EntityTypeName;
                model.EntityNumber = entity.EntityNumber.ToString("D7");
                model.CreatedDate = entity.CreatedDate;
                model.EntityUserName = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "FirstName") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "FirstName").SelectedValues : string.Empty;
                model.EntityUserSurname = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Name") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Name").SelectedValues : string.Empty;
                model.DOB = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "DOB") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "DOB").SelectedValues : string.Empty;
                model.Gender = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Gender") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Gender").SelectedValues : string.Empty;
                model.Email = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Email") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Email").SelectedValues : string.Empty;
                model.Phone = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Phone") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Phone").SelectedValues : string.Empty;
                model.State = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "State") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "State").SelectedValues : string.Empty;
                model.Suburb = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Suburb") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Suburb").SelectedValues : string.Empty;
                model.Gender = (model.Gender == "1" ? "Male" : model.Gender == "2" ? "Female" : model.Gender == "3" ? "Other" : string.Empty);
                model.Profession = entity.formDataEntryVariableMongoList.Where(x => x.VariableName == DefaultsVariables.PerSType.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                model.Profession = model.Profession == "1" ? "Medical Practitioner/Allied Health" : model.Profession == "2" ? "Non-Medical Practitioner" : string.Empty;
                if (entity.FormTitle == "Project Registration")
                {
                    model.EntityUserName = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Name") != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "Name").SelectedValues : string.Empty;
                    model.EntityUserSurname = string.Empty;
                }
                #region Current entity details
                try
                {
                    var currentEntity = _dbContext.UserLogins.FirstOrDefault(x => x.Id == entity.ThisUserId);
                    if (string.IsNullOrEmpty(model.Phone))
                    {
                        model.Phone = currentEntity != null ? currentEntity.Mobile : null;
                    }
                    if (string.IsNullOrEmpty(model.Address))
                    {
                        model.Address = currentEntity != null ? currentEntity.Address : null;
                    }
                    if (string.IsNullOrEmpty(model.Email))
                    {
                        model.Email = currentEntity != null ? currentEntity.Email : null;
                    }
                    if (entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                    {
                        var projectLogo = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ProjectLogo.ToString());
                        string projectLogoSelectedValus = projectLogo != null ? projectLogo.SelectedValues : null;
                        model.EntityProfileImage = !string.IsNullOrEmpty(projectLogoSelectedValus) ? projectLogoSelectedValus : null;
                    }
                    else if (entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                    {
                        var projectLogo = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PlaceProfilePicture.ToString());
                        string projectLogoSelectedValus = projectLogo != null ? projectLogo.SelectedValues : null;
                        model.EntityProfileImage = !string.IsNullOrEmpty(projectLogoSelectedValus) ? projectLogoSelectedValus : null;
                    }
                    else
                    {
                        string baseImagepath = System.Configuration.ConfigurationManager.AppSettings["ProfileImageBasePath"].ToString();
                        var imgPath = System.IO.Path.Combine(baseImagepath + "/", currentEntity.Guid.ToString() + ".jpg");
                        if (System.IO.File.Exists(imgPath))
                        {
                            model.EntityProfileImage = imgPath;
                        }
                    }

                }
                catch (Exception ex)
                { }
                #endregion

                #region Default variables display in left panel
                try
                {
                    IMongoQuery conditionUserEntitiesCustomForms = Query.And(Query<FormDataEntryMongo>.EQ(q => q.ParentEntityNumber, entityId), Query<FormDataEntryMongo>.EQ(q => q.ProjectGuid, projectId));
                    IQueryable<FormDataEntryMongo> userEntitiesCustomForms = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionUserEntitiesCustomForms).AsQueryable();
                    string[] defaultVariablesForLeftPanel = {
                        DefaultsVariables.Email.ToString()
                        , DefaultsVariables.Phone.ToString()
                        , DefaultsVariables.Fax.ToString()
                        , DefaultsVariables.StrtNum.ToString()
                        , DefaultsVariables.StrtNum2.ToString()
                        , DefaultsVariables.StrtNme.ToString()
                        , DefaultsVariables.StrtNme2.ToString()
                        , DefaultsVariables.Suburb.ToString()
                        , DefaultsVariables.State.ToString()
                        , DefaultsVariables.Postcode.ToString()
                    };
                    List<string> defaultEmailValues = new List<string>();
                    List<string> defaultPhoneValues = new List<string>();
                    List<string> defaultFaxValues = new List<string>();
                    List<string> defaultStrtNumValues = new List<string>();
                    List<string> defaultStrt2NumValues = new List<string>();
                    List<string> defaultStrtNmeValues = new List<string>();
                    List<string> defaultStrtNme2Values = new List<string>();
                    List<string> defaultSuburbValues = new List<string>();
                    List<string> defaultStateValues = new List<string>();
                    List<string> defaultPostcodeValues = new List<string>();

                    defaultEmailValues.Add(model.Email);
                    defaultPhoneValues.Add(model.Phone);
                    defaultSuburbValues.Add(model.Suburb);
                    defaultStateValues.Add(model.State);
                    foreach (var customForm in userEntitiesCustomForms)
                    {
                        bool frmDefaultVariables = customForm.formDataEntryVariableMongoList.Any(c => defaultVariablesForLeftPanel.Contains(c.VariableName));
                        if (frmDefaultVariables)
                        {
                            FormDataEntryVariableMongo emailObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.Email.ToString());
                            if (emailObj != null)
                            {
                                defaultEmailValues.Add(!string.IsNullOrEmpty(emailObj.SelectedValues) ? emailObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo phoneObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.Phone.ToString());
                            if (phoneObj != null)
                            {
                                defaultPhoneValues.Add(!string.IsNullOrEmpty(phoneObj.SelectedValues) ? phoneObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo faxObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.Fax.ToString());
                            if (faxObj != null)
                            {
                                defaultFaxValues.Add(!string.IsNullOrEmpty(faxObj.SelectedValues) ? faxObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo strtNumObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.StrtNum.ToString());
                            if (strtNumObj != null)
                            {
                                defaultStrtNumValues.Add(!string.IsNullOrEmpty(strtNumObj.SelectedValues) ? strtNumObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo strtNum2Obj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.StrtNum2.ToString());
                            if (strtNum2Obj != null)
                            {
                                defaultStrt2NumValues.Add(!string.IsNullOrEmpty(strtNum2Obj.SelectedValues) ? strtNum2Obj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo strtNmeObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.StrtNme.ToString());
                            if (strtNmeObj != null)
                            {
                                defaultStrtNmeValues.Add(!string.IsNullOrEmpty(strtNmeObj.SelectedValues) ? strtNmeObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo strtNme2Obj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.StrtNme2.ToString());
                            if (strtNme2Obj != null)
                            {
                                defaultStrtNme2Values.Add(!string.IsNullOrEmpty(strtNme2Obj.SelectedValues) ? strtNme2Obj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo suburbObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.Suburb.ToString());
                            if (suburbObj != null)
                            {
                                defaultSuburbValues.Add(!string.IsNullOrEmpty(suburbObj.SelectedValues) ? suburbObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo stateObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.State.ToString());
                            if (stateObj != null)
                            {
                                defaultStateValues.Add(!string.IsNullOrEmpty(stateObj.SelectedValues) ? stateObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo postcodeObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.Postcode.ToString());
                            if (postcodeObj != null)
                            {
                                defaultPostcodeValues.Add(!string.IsNullOrEmpty(postcodeObj.SelectedValues) ? postcodeObj.SelectedValues : string.Empty);
                            }
                        }
                    }
                    defaultEmailValues = defaultEmailValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultPhoneValues = defaultPhoneValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultSuburbValues = defaultSuburbValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultStateValues = defaultStateValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();

                    defaultFaxValues = defaultFaxValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultStrtNumValues = defaultStrtNumValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultStrt2NumValues = defaultStrt2NumValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultStrtNmeValues = defaultStrtNmeValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultStrtNmeValues = defaultStrtNme2Values.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultPostcodeValues = defaultPostcodeValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();

                    model.Email = string.Join(":", defaultEmailValues);
                    model.Phone = string.Join(":", defaultPhoneValues);
                    model.State = string.Join(":", defaultStateValues);
                    model.Suburb = string.Join(":", defaultSuburbValues);
                    model.Postcode = string.Join(":", defaultPostcodeValues);
                    model.Fax = string.Join(":", defaultFaxValues);
                    model.StrtNum = string.Join(":", defaultStrtNumValues);
                    model.StrtNum2 = string.Join(":", defaultStrt2NumValues);
                    model.StrtNme = string.Join(":", defaultStrtNmeValues);
                    model.StrtNme2 = string.Join(":", defaultStrtNmeValues);

                }
                catch (Exception exDefaultVariablesList)
                { Console.WriteLine(exDefaultVariablesList.Message); }
                #endregion
                string[] defaultActivitiesName = new string[]
                    {
                        EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Linkage)
                    };
                var summaryPageActivityForms = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindAll().AsQueryable().Where(x => x.PersonEntityId == entityId && x.DateDeactivated == null).AsQueryable();
                if (summaryPageActivityForms.Count() != 0)
                {
                    var defaultActivities = summaryPageActivityForms.Where(x => defaultActivitiesName.Contains(x.ActivityName)).ToList();
                    summaryPageActivityForms = summaryPageActivityForms.Where(x => x.ProjectGuid == projectId && !defaultActivitiesName.Contains(x.ActivityName));
                    var list1 = summaryPageActivityForms.ToList();
                    var list2 = defaultActivities.ToList();
                    var result = list1.Concat(list2).OrderBy(x => x.ActivityDate).ToList();
                    summaryPageActivityForms = result.AsQueryable();
                }
                List<SummaryPageActivityViewModel> listSummaryPageActivityViewModel = new List<SummaryPageActivityViewModel>();
                summaryPageActivityForms.ToList().ForEach(y =>
                {
                    if (y.ActivityName == "Project Linkage")
                    {
                        var conditionEntity = Query.And(Query<FormDataEntryMongo>.EQ(w => w.SummaryPageActivityObjId, Convert.ToString(y.Id)));
                        var projectLinkageConditionEntity = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionEntity).AsQueryable().FirstOrDefault();
                        if (projectLinkageConditionEntity != null)
                        {
                            var projectguid = projectLinkageConditionEntity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "LnkPro")
                                                  != null
                                                  ? projectLinkageConditionEntity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "LnkPro").SelectedValues
                                                  : null;
                            Guid projectGuid = !string.IsNullOrEmpty(projectguid) ? new Guid(projectguid) : Guid.Empty;
                            var allProjects = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").FindAll().AsQueryable().OrderByDescending(x => x.ProjectDeployDate).AsQueryable();
                            var thisproject = allProjects.FirstOrDefault(x => x.ProjectGuid == projectGuid);

                            if (thisproject == null)
                            {
                                var sqlProject = _dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == projectGuid);
                                if (sqlProject != null)
                                {
                                    thisproject = new ProjectDeployViewModel();
                                    thisproject.ProjectName = sqlProject.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == "Name")
                                    != null ? sqlProject.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == "Name").SelectedValues : null;
                                    thisproject.ProjectGuid = sqlProject.Guid;
                                }
                            }

                            y.LinkedProjectName = thisproject != null ? thisproject.ProjectName : null;
                            y.LinkedProjectGuid = thisproject != null ? thisproject.ProjectGuid : (Guid?)null;
                        }
                    }

                    listSummaryPageActivityViewModel.Add(y);
                });
                summaryPageActivityForms = listSummaryPageActivityViewModel.AsQueryable();

                #region apply permission summary page added activities forms 
                string roleName = string.Empty;
                try
                {
                    var roleLoggedinProj = _dbContext.ProjectStaffMemberRoles.FirstOrDefault(x => x.FormDataEntry.Guid == projectId && x.UserLogin.Guid == LoggedInUser);
                    if (roleLoggedinProj == null)
                    {
                        var userlogin = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == LoggedInUser);
                        if (userlogin != null)
                        {
                            var role = userlogin.UserRoles.FirstOrDefault();
                            roleName = role != null ? role.Role.Name : string.Empty;
                        }
                    }
                    else
                    {
                        try { roleName = roleLoggedinProj.Role.Name; } catch (Exception exc) { }
                    }
                }
                catch (Exception excRole)
                {
                    ProjectStaffMongo roleCurrentUserole = project.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == LoggedInUser);
                    roleName = roleCurrentUserole != null ? roleCurrentUserole.Role : string.Empty;
                }

                List<SummaryPageActivityViewModel> listSummaryPageActivityViewModel_ONE_LIST = new List<SummaryPageActivityViewModel>();
                SummaryPageActivityViewModel listSummaryPageActivityViewModel_OBJ = new SummaryPageActivityViewModel();
                foreach (var activity in listSummaryPageActivityViewModel)
                {
                    listSummaryPageActivityViewModel_OBJ = new SummaryPageActivityViewModel();
                    listSummaryPageActivityViewModel_OBJ = activity;
                    ActivitiesMongo projectActivity = null;
                    if (defaultActivitiesName.Contains(activity.ActivityName))
                    {
                        projectActivity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityName == activity.ActivityName);
                    }
                    else
                    {
                        if (activity.ProjectVersion != project.ProjectInternalVersion)
                        {
                            var conditionOtherVersion = Query.And(Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectId), Query<ProjectDeployViewModel>.EQ(r => r.ProjectInternalVersion, activity.ProjectVersion));
                            var projectOtherVersion = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionOtherVersion).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                            projectActivity = projectOtherVersion.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == activity.ActivityGuid);
                        }
                        else
                        {
                            projectActivity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == activity.ActivityGuid);
                        }
                    }
                    List<Guid> removedFormGuid = new List<Guid>();
                    foreach (var form in activity.SummaryPageActivityFormsList)
                    {
                        int count = 0;
                        int totalVariables = 0;
                        if (projectActivity != null)
                        {
                            try
                            {
                                FormsMongo frm = projectActivity.FormsListMongo.FirstOrDefault(x => x.FormGuid == form.FormGuid);
                                if (frm != null)
                                {
                                    IQueryable<VariablesMongo> variables = frm.VariablesListMongo.AsQueryable();
                                    totalVariables = variables.Count();
                                    variables.ToList().ForEach(v =>
                                    {
                                        var variable = v.VariableRoleListMongo.Where(x => x.RoleName == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                                        if (variable == null)
                                        {
                                            count++;
                                        }
                                        else
                                        {
                                            if (v.DependentVariableId != null)
                                            {
                                                var parentVar = variables.FirstOrDefault(x => x.VariableId == v.DependentVariableId);
                                                var isParentVar = parentVar.VariableRoleListMongo.Where(x => x.RoleName == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                                                if (isParentVar == null) { count++; }
                                            }
                                        }
                                    });
                                }
                                if (count > 0)
                                {
                                    if (totalVariables == count)
                                        removedFormGuid.Add(form.FormGuid);
                                }
                            }
                            catch (Exception exc)
                            { }
                        }
                        else
                        {
                            IMongoQuery checkProjectCondition;
                            if (activity.ProjectVersion != project.ProjectInternalVersion)
                            {
                                checkProjectCondition = Query.And(Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, activity.ProjectGuid), Query<ProjectDeployViewModel>.EQ(r => r.ProjectInternalVersion, activity.ProjectVersion));
                            }
                            else
                            {
                                checkProjectCondition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, activity.ProjectGuid);
                            }
                            var checkProject = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(checkProjectCondition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                            if (checkProject != null)
                            {
                                try
                                {
                                    var chkActivity = checkProject.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == activity.ActivityGuid);
                                    if (chkActivity != null)
                                    {
                                        FormsMongo frm = chkActivity.FormsListMongo.FirstOrDefault(x => x.FormGuid == form.FormGuid);
                                        if (frm != null)
                                        {
                                            IQueryable<VariablesMongo> variables = frm.VariablesListMongo.AsQueryable();
                                            totalVariables = variables.Count();
                                            variables.ToList().ForEach(v =>
                                            {
                                                var variable = v.VariableRoleListMongo.Where(x => x.RoleName == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                                                if (variable == null)
                                                {
                                                    count++;
                                                }
                                                else
                                                {
                                                    if (v.DependentVariableId != null)
                                                    {
                                                        var parentVar = variables.FirstOrDefault(x => x.VariableId == v.DependentVariableId);
                                                        var isParentVar = parentVar.VariableRoleListMongo.Where(x => x.RoleName == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                                                        if (isParentVar == null) { count++; }
                                                    }
                                                }
                                            });
                                        }
                                        if (count > 0)
                                        {
                                            if (totalVariables == count)
                                                removedFormGuid.Add(form.FormGuid);
                                        }
                                    }
                                }
                                catch (Exception exc)
                                {
                                }
                            }
                            else
                            {
                                try
                                {
                                    var frm = _dbContext.Forms.FirstOrDefault(x => x.Guid == form.FormGuid);
                                    if (frm != null)
                                    {
                                        frm.FormVariables.Count();
                                        frm.FormVariables.ToList().ForEach(v =>
                                        {
                                            var variable = v.FormVariableRoles.Where(x => x.Role.Name == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                                            if (variable == null)
                                            {
                                                count++;
                                            }
                                            else
                                            {
                                                if (v.DependentVariableId != null)
                                                {
                                                    var parentVar = frm.FormVariables.FirstOrDefault(x => x.VariableId == v.DependentVariableId);
                                                    var isParentVar = parentVar.FormVariableRoles.Where(x => x.Role.Name == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                                                    if (isParentVar == null) { count++; }
                                                }
                                            }
                                        });
                                    }
                                    if (count > 0)
                                    {
                                        if (totalVariables == count)
                                            removedFormGuid.Add(form.FormGuid);
                                    }
                                }
                                catch (Exception exceSQL)
                                { }
                            }
                        }
                    }
                    if (removedFormGuid.Count() > 0)
                    {
                        listSummaryPageActivityViewModel_OBJ.SummaryPageActivityFormsList.RemoveAll(x => removedFormGuid.Contains(x.FormGuid));
                    }
                    listSummaryPageActivityViewModel_ONE_LIST.Add(listSummaryPageActivityViewModel_OBJ);
                }
                summaryPageActivityForms = listSummaryPageActivityViewModel_ONE_LIST.AsQueryable();
                #endregion
                model.SummaryPageActivitiesList = summaryPageActivityForms.OrderByDescending(x => x.ActivityDate).ToList();

                #region Activity Completed By drop-down
                model.SummaryPageProjectUsersList = new List<SummaryPageProjectUser>();
                SummaryPageProjectUser summaryPageProjectUser = new SummaryPageProjectUser();
                if (entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                    || entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
                    || entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)
                    )
                {
                    var projectStaffList = _userLoginProvider.TestEnvironment_GetProjectAllUsers(projectId);
                    projectStaffList.ToList().ForEach(stf =>
                    {
                        summaryPageProjectUser = new SummaryPageProjectUser
                        {
                            UserName = stf.FirstName + " " + stf.LastName,
                            UserGuid = stf.Guid,
                            UserId = stf.Id,
                        };
                        model.SummaryPageProjectUsersList.Add(summaryPageProjectUser);
                    });
                }
                else if (entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                {
                    try
                    {
                        var allSAUsers = _dbContext.UserRoles.Where(x => x.Role.Name == RoleTypes.System_Admin.ToString().Replace("_", " ") && x.UserLogin.UserTypeId == (int)UsersLoginType.Test && x.UserLogin.IsUserApprovedBySystemAdmin && x.UserLogin.Status == (int)Core.Enum.Status.Active).OrderBy(x => x.UserLogin.FirstName).ToList();
                        allSAUsers.ForEach(sa_user =>
                        {
                            model.SummaryPageProjectUsersList.Add(new SummaryPageProjectUser
                            {
                                UserName = sa_user.UserLogin.FirstName + " " + sa_user.UserLogin.LastName,
                                UserGuid = sa_user.UserLogin.Guid,
                                UserId = sa_user.UserLogin.Id,
                            });
                        });
                    }
                    catch (Exception exc) { }
                }
                #endregion

                model.SummaryPageActivityTypeList = new List<SummaryPageActivityTypes>();
                SummaryPageActivityTypes scheduledActivity = new SummaryPageActivityTypes();

                if (isDifferentProject)
                {
                    ActivitiesMongo projectLinkageActivity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityName == DefaultFormName.Project_Linkage.ToString().Replace("_", " "));
                    project = new ProjectDeployViewModel();
                    project.ProjectActivitiesList = new List<ActivitiesMongo>();

                    try
                    {
                        project.ProjectActivitiesList.Add(projectLinkageActivity);
                    }
                    catch (Exception exc) { }
                }
                int i = 0;
                string EntityTypeName = null;
                List<Core.ViewModels.EntityTypeViewModel> EntityTypes = _dbContext.EntityTypes.Select(et => new Core.ViewModels.EntityTypeViewModel
                {
                    Guid = et.Guid,
                    Id = et.Id,
                    Name = et.Name,
                    TenantId = et.TenantId != null ? et.Tenant.Guid : (Guid?)null,
                }).ToList();
                Core.ViewModels.EntityTypeViewModel ent = EntityTypes.FirstOrDefault(x => x.Name == model.EntityType);
                EntityTypeName = ent != null ? ent.Name : null;
                if (model.EntityType == "Place/Group")
                {
                    try
                    {
                        #region for specific entity type
                        var str = new string[] { "Person", "Hospital", "Practice/Clinic", "Laboratory", "Medical Imaging", "Research facility/University", "Healthcare Group", "Government Organisation", "Industry Group", "Consumer Group", "Activity Venue", "Vehicle", "MAC", "Ethics Committee", "API" };
                        var entType = entity.formDataEntryVariableMongoList.Where(x => x.VariableName == "EntType").Select(x => x.SelectedValues).FirstOrDefault();
                        int ii = !String.IsNullOrEmpty(entType) ? Convert.ToInt32(entType) : 0;
                        string vald = str[ii - 1];

                        Core.ViewModels.EntityTypeViewModel entNme = EntityTypes.FirstOrDefault(x => x.Name.ToLower() == vald.ToLower());
                        EntityTypeName = entNme != null ? entNme.Name : null;

                        var allPlaceType = project.ProjectActivitiesList.Where(x => x.ActivityEntityTypeName == EntityTypesListInDB.Place__Group.ToString().Replace("__", "/"));
                        project.ProjectActivitiesList = project.ProjectActivitiesList.Where(x => x.ActivityEntityTypeName.Contains(EntityTypeName)).ToList();
                        project.ProjectActivitiesList.AddRange(allPlaceType);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        #region if not match with any entType
                        try
                        {
                            List<string> enames = new List<string>();
                            enames.Add("Person"); enames.Add("Participant"); enames.Add("Project");
                            var elist = EntityTypes.Where(x => !enames.Contains(x.Name)).Select(x => x.Name).ToList();
                            if (elist.Count() > 0)
                            {
                                project.ProjectActivitiesList = project.ProjectActivitiesList.Where(x => elist.Contains(x.ActivityEntityTypeName)).ToList();
                            }
                        }
                        catch (Exception exx) { }
                        #endregion
                    }
                }
                else
                {
                    project.ProjectActivitiesList = project.ProjectActivitiesList.Where(x => x.ActivityEntityTypeName.Contains(EntityTypeName)).ToList();
                }

                var idsToRemove = new List<Guid>();
                foreach (var activity in project.ProjectActivitiesList)
                {
                    if (!activity.CanCreatedMultipleTime)
                    {
                        var isAdded = model.SummaryPageActivitiesList.Where(x => x.ActivityGuid == activity.ActivityGuid).FirstOrDefault();
                        if (isAdded != null) continue;
                    }
                    Guid ActivityAdd = Guid.Empty;
                    scheduledActivity = new SummaryPageActivityTypes();
                    DateTime entityCreatedDate = entity.CreatedDate;

                    #region question 1: When is the activity scheduled to be completed? 
                    if (activity.ScheduledToBeCompleted == Core.Enum.ScheduledToBeCompleted.Unscheduled.ToString())
                    {
                        ActivityAdd = activity.ActivityGuid;
                    }
                    else if (activity.ScheduledToBeCompleted == Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity.ToString())
                    {
                        #region Offset from another activity

                        SummaryPageActivityViewModel isAvaliable = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == activity.OtherActivityName && x.ProjectGuid == projectId);
                        if (isAvaliable != null)
                        {
                            ActivityAdd = activity.ActivityGuid;
                            //[ASPMONASH-225] Removed offest logic based on [ASPMONASH-225]
                            #region [ASPMONASH-225] Removed offest logic based on [ASPMONASH-225]
                            //Int16 count = 0;
                            //if (activity.OffsetCount != null)
                            //    count = Convert.ToInt16(activity.OffsetCount);
                            //DateTime acDate = isAvaliable.ActivityDate;
                            //switch (activity.OffsetTypeName)
                            //{
                            //    case "Day":
                            //        acDate = acDate.AddDays(count);
                            //        break;
                            //    case "Weeks":
                            //        acDate = acDate.AddDays(count * 7);
                            //        break;
                            //    case "Month":
                            //        acDate = acDate.AddMonths(count);
                            //        break;
                            //    case "Year":
                            //        acDate = acDate.AddYears(count);
                            //        break;
                            //    default:
                            //        break;
                            //}
                            //if (acDate.Date > DateTime.UtcNow.Date)
                            //{
                            //    //PC
                            //    //idsToRemove.Add(activity.ActivityGuid);
                            //    //LM
                            //    //ActivityAdd = Guid.Empty;
                            //    ActivityAdd = activity.ActivityGuid;
                            //}
                            //else
                            //{
                            //    ActivityAdd = activity.ActivityGuid;
                            //} 
                            #endregion
                        }
                        #endregion

                    }
                    #endregion

                    #region question 2: When should the activity be available for creation on the summary page?
                    if (activity.ActivityAvailableForCreation == Core.Enum.SchedulingActivityAvailableForCreation.Always_available.ToString())
                    {
                        ActivityAdd = activity.ActivityGuid;
                    }
                    else if (activity.ActivityAvailableForCreation == Core.Enum.SchedulingActivityAvailableForCreation.Only_if_specified_activity_had_already_been_created.ToString())
                    {
                        #region Only if specified activity had already been created
                        SummaryPageActivityViewModel isAvaliable = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == activity.SpecifiedActivityName && x.ProjectGuid == projectId);
                        if (isAvaliable != null)
                        {
                            ActivityAdd = activity.ActivityGuid;
                            //[ASPMONASH-225] Removed offest logic based on [ASPMONASH-225]
                            #region [ASPMONASH-225] Removed offest logic based on [ASPMONASH-225]
                            //if (activity.ScheduledToBeCompleted == Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity.ToString())
                            //{
                            //    SummaryPageActivityViewModel isOffsetAvaliable = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == activity.OtherActivityName && x.ProjectGuid == projectId);
                            //    if (isOffsetAvaliable != null)
                            //    {
                            //        Int16 count = 0;
                            //        if (activity.OffsetCount != null)
                            //            count = Convert.ToInt16(activity.OffsetCount);
                            //        DateTime acDate = isOffsetAvaliable.ActivityDate;
                            //        switch (activity.OffsetTypeName)
                            //        {
                            //            case "Day":
                            //                acDate = acDate.AddDays(count);
                            //                break;
                            //            case "Weeks":
                            //                acDate = acDate.AddDays(count * 7);
                            //                break;
                            //            case "Month":
                            //                acDate = acDate.AddMonths(count);
                            //                break;
                            //            case "Year":
                            //                acDate = acDate.AddYears(count);
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (acDate.Date > DateTime.UtcNow.Date)
                            //        {
                            //            //PC
                            //            //idsToRemove.Add(activity.ActivityGuid);
                            //            //LM
                            //            //ActivityAdd = Guid.Empty;
                            //            ActivityAdd = activity.ActivityGuid;
                            //        }
                            //        else
                            //        {
                            //            ActivityAdd = activity.ActivityGuid;
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ActivityAdd = activity.ActivityGuid;
                            //} 
                            #endregion
                        }
                        else
                        {
                            ActivityAdd = Guid.Empty;
                        }
                        #endregion
                    }
                    else if (activity.ActivityAvailableForCreation == Core.Enum.SchedulingActivityAvailableForCreation.Based_on_calendar_month_before_or_after_scheduled_date.ToString())
                    {
                        #region Based on calendar month before or after scheduled date
                        if (activity.ScheduledToBeCompleted == Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity.ToString())
                        {
                            SummaryPageActivityViewModel isAvaliable = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == activity.OtherActivityName && x.ProjectGuid == projectId);
                            if (isAvaliable != null)
                            {
                                int? start = activity.CreationWindowOpens;
                                int? ends = activity.CreationWindowClose;
                                try
                                {
                                    DateTime acDate = isAvaliable.ActivityDate;

                                    Int16 count = 0;
                                    if (activity.OffsetCount != null)
                                        count = Convert.ToInt16(activity.OffsetCount);
                                    switch (activity.OffsetTypeName)
                                    {
                                        case "Day":
                                            acDate = acDate.AddDays(count);
                                            break;
                                        case "Weeks":
                                            acDate = acDate.AddDays(count * 7);
                                            break;
                                        case "Month":
                                            acDate = acDate.AddMonths(count);
                                            break;
                                        case "Year":
                                            acDate = acDate.AddYears(count);
                                            break;
                                        default:
                                            break;
                                    }
                                    DateTime edate = acDate.AddMonths((int)ends);
                                    DateTime sdate = acDate.AddMonths(-(int)start);
                                    if (sdate.Date <= DateTime.UtcNow.ToLocalTime().Date && edate.Date >= DateTime.UtcNow.ToLocalTime().Date)
                                    {
                                        ActivityAdd = activity.ActivityGuid;
                                    }
                                    else
                                    {
                                        ActivityAdd = Guid.Empty;
                                    }
                                }
                                catch (Exception exc) { }
                            }
                        }
                        else
                        {
                            int? start = activity.CreationWindowOpens;
                            int? ends = activity.CreationWindowClose;
                            try
                            {
                                DateTime acDate = (DateTime)activity.ScheduleDate;
                                DateTime edate = acDate.AddMonths((int)ends);
                                DateTime sdate = acDate.AddMonths(-(int)start);
                                if (sdate.Date <= DateTime.UtcNow.ToLocalTime().Date && edate.Date >= DateTime.UtcNow.ToLocalTime().Date)
                                {
                                    ActivityAdd = activity.ActivityGuid;
                                }
                                else
                                {
                                    ActivityAdd = Guid.Empty;
                                }
                            }
                            catch (Exception exc) { }
                        }
                        #endregion
                    }
                    else if (activity.ActivityAvailableForCreation == Core.Enum.SchedulingActivityAvailableForCreation.Based_on_days_before_or_after_scheduled_date.ToString())
                    {
                        #region Based on days before or after scheduled date

                        if (activity.ScheduledToBeCompleted == Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity.ToString())
                        {
                            SummaryPageActivityViewModel isAvaliable = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == activity.OtherActivityName && x.ProjectGuid == projectId);
                            if (isAvaliable != null)
                            {
                                DateTime acDate = isAvaliable.ActivityDate;
                                Int16 count = 0;
                                if (activity.OffsetCount != null)
                                    count = Convert.ToInt16(activity.OffsetCount);
                                switch (activity.OffsetTypeName)
                                {
                                    case "Day":
                                        acDate = acDate.AddDays(count);
                                        break;
                                    case "Weeks":
                                        acDate = acDate.AddDays(count * 7);
                                        break;
                                    case "Month":
                                        acDate = acDate.AddMonths(count);
                                        break;
                                    case "Year":
                                        acDate = acDate.AddYears(count);
                                        break;
                                    default:
                                        break;
                                }
                                int? start = activity.CreationWindowOpens;
                                int? ends = activity.CreationWindowClose;
                                try
                                {
                                    DateTime edate = acDate.AddDays((int)ends);
                                    DateTime sdate = acDate.AddDays(-(int)start);
                                    if (sdate.Date <= DateTime.UtcNow.ToLocalTime().Date && edate.Date >= DateTime.UtcNow.ToLocalTime().Date)
                                    {
                                        ActivityAdd = activity.ActivityGuid;
                                    }
                                    else
                                    {
                                        ActivityAdd = Guid.Empty;
                                    }
                                }
                                catch (Exception exc) { }
                            }
                        }
                        else
                        {
                            int? start = activity.CreationWindowOpens;
                            int? ends = activity.CreationWindowClose;
                            try
                            {
                                DateTime acDate = (DateTime)activity.ScheduleDate;
                                DateTime edate = acDate.AddDays((int)ends);
                                DateTime sdate = acDate.AddDays(-(int)start);
                                if (sdate.Date <= DateTime.UtcNow.ToLocalTime().Date && edate.Date >= DateTime.UtcNow.ToLocalTime().Date)
                                {
                                    ActivityAdd = activity.ActivityGuid;
                                }
                                else
                                {
                                    ActivityAdd = Guid.Empty;
                                }
                            }
                            catch (Exception exc) { }
                        }
                        #endregion
                    }
                    #endregion

                    #region question 3: Which roles should be able to create the activity as scheduled?
                    var role = string.Empty;
                    ProjectStaffMongo loggedUser = null;
                    if (project != null && project.ProjectStaffListMongo == null)
                    {
                        try
                        {
                            var stf = _dbContext.ProjectStaffMemberRoles.FirstOrDefault(x => x.UserLogin.Guid == LoggedInUser && x.FormDataEntry.Guid == projectId);
                            loggedUser = new ProjectStaffMongo()
                            {
                                Role = stf.Role.Name,
                                StaffGuid = LoggedInUser,
                                StaffName = stf.UserLogin.FirstName + " " + stf.UserLogin.LastName,
                            };
                        }
                        catch (Exception exc) { }
                    }
                    else
                    {
                        loggedUser = project.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == LoggedInUser);
                    }
                    if (loggedUser == null)
                    {
                        try
                        {
                            var user = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == LoggedInUser);
                            if (user != null)
                            {
                                role = user.UserRoles.FirstOrDefault().Role.Name;
                            }
                        }
                        catch (Exception ec)
                        { }
                    }

                    if (loggedUser != null)
                    {
                        role = loggedUser.Role;
                    }
                    if (!activity.RolesToCreateActivity.Any(s => s == role))
                    {
                        if (activity.RolesToCreateActivity.Count() > 0)
                            ActivityAdd = Guid.Empty;
                    }
                    #endregion

                    #region question 4: Which roles should be able to create the activity regardless of schedule?
                    if (activity.RoleToCreateActivityRegardlessScheduled.Any(s => role.Contains(s)))
                    {
                        ActivityAdd = activity.ActivityGuid;
                    }
                    #endregion

                    if (ActivityAdd != Guid.Empty)
                    {
                        scheduledActivity.ActivityGuid = activity.ActivityGuid;
                        scheduledActivity.ActivityId = activity.ActivityId;
                        scheduledActivity.ActivityName = activity.ActivityName;
                        scheduledActivity.IsDefaultActivity = activity.IsDefaultActivity;
                        model.SummaryPageActivityTypeList.Add(scheduledActivity);
                    }
                }
                #region PC to remove activity
                if (idsToRemove.Count > 0)
                {
                    foreach (var item in idsToRemove)
                    {
                        var currActivity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == item);
                        if (currActivity != null)
                        {
                            project.ProjectActivitiesList.Remove(currActivity);
                        }
                        var currActivity1 = model.SummaryPageActivityTypeList.FirstOrDefault(x => x.ActivityGuid == item);
                        if (currActivity1 != null)
                        {
                            model.SummaryPageActivityTypeList.Remove(currActivity1);
                        }
                    }
                }
                #endregion

                #region apply role based permission on Activititty type list
                #region apply permission on forms
                if (roleName == string.Empty)
                {
                    var userlogin = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == LoggedInUser);
                    if (userlogin != null)
                    {
                        var role = userlogin.UserRoles.FirstOrDefault();
                        roleName = role != null ? role.Role.Name : null;
                    }
                }
                List<SummaryPageActivityTypes> listSummaryPageActivityTypeList = new List<SummaryPageActivityTypes>();
                SummaryPageActivityViewModel summaryPageActivityType_OBJ = new SummaryPageActivityViewModel();
                #endregion
                #endregion
                if (entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration) || entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                {
                    string sysAppr = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.SysAppr.ToString()) != null ? entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.SysAppr.ToString()).SelectedValues : null;
                    if (string.IsNullOrEmpty(sysAppr) || sysAppr == "0")
                    {
                        model.SummaryPageActivityTypeList = new List<SummaryPageActivityTypes>();
                        string[] summaryPageDefaultActivities = new string[] {
                            EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
                            ,EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                            ,EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)
                            ,EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration)
                        };
                        model.SummaryPageActivitiesList.RemoveAll(activity => !summaryPageDefaultActivities.Contains(activity.ActivityName));
                    }
                }
                model.SummaryPageActivitiesList = model.SummaryPageActivitiesList.OrderByDescending(x => x.ActivityDate.Date).ThenByDescending(c => c.CreatedDate).ToList();
                #region Update entity registration date
                if (model.EntityType == EntityTypesListInDB.Person.ToString())
                {
                    SummaryPageActivityViewModel addSummaryPageActivityViewModel = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration));
                    model.CreatedDate = addSummaryPageActivityViewModel != null ? addSummaryPageActivityViewModel.ActivityDate : model.CreatedDate;
                }
                else if (model.EntityType == EntityTypesListInDB.Participant.ToString())
                {
                    SummaryPageActivityViewModel addSummaryPageActivityViewModel = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration));
                    model.CreatedDate = addSummaryPageActivityViewModel != null ? addSummaryPageActivityViewModel.ActivityDate : model.CreatedDate;
                }
                else if (model.EntityType == EntityTypesListInDB.Place__Group.ToString().ToString().Replace("__", "/"))
                {
                    SummaryPageActivityViewModel addSummaryPageActivityViewModel = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration));
                    model.CreatedDate = addSummaryPageActivityViewModel != null ? addSummaryPageActivityViewModel.ActivityDate : model.CreatedDate;
                }
                else if (model.EntityType == EntityTypesListInDB.Project.ToString())
                {
                    SummaryPageActivityViewModel addSummaryPageActivityViewModel = model.SummaryPageActivitiesList.FirstOrDefault(x => x.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration));
                    model.CreatedDate = addSummaryPageActivityViewModel != null ? addSummaryPageActivityViewModel.ActivityDate : model.CreatedDate;
                }
                #endregion
                return model;
            }
            return null;
        }

        public SummaryPageActivityViewModel AddSummaryPageActivity(SummaryPageActivityViewModel model, Guid loggedInUser)
        {
            var conditionProject = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, model.ProjectGuid);
            var project = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionProject).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
            var activity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == model.ActivityGuid);
            bool canRepeat = activity != null ? activity.CanCreatedMultipleTime : false;

            SummaryPageActivityViewModel summarypageActivityDocument = null;

            if (!canRepeat)
                summarypageActivityDocument = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindAll().AsQueryable().FirstOrDefault(x => x.ProjectGuid == model.ProjectGuid && x.ActivityGuid == model.ActivityGuid && x.PersonEntityId == model.PersonEntityId && x.DateDeactivated == null);

            if (summarypageActivityDocument == null)
            {
                var createdBy = _userLoginProvider.GetByGuid(loggedInUser);

                #region linkage from summary page                
                var activityCompletedBy = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == model.ActivityCompletedByGuid);
                SummaryPageActivityViewModel summaryPageActivityViewModel = new SummaryPageActivityViewModel();
                summaryPageActivityViewModel.ActivityId = activity.ActivityId;
                summaryPageActivityViewModel.ActivityGuid = activity.ActivityGuid;
                summaryPageActivityViewModel.ActivityName = activity.ActivityName;
                summaryPageActivityViewModel.ActivityCompletedById = activityCompletedBy.Id;
                summaryPageActivityViewModel.ActivityCompletedByGuid = activityCompletedBy.Guid;
                summaryPageActivityViewModel.ActivityCompletedByName = activityCompletedBy.FirstName + " " + activityCompletedBy.LastName;
                summaryPageActivityViewModel.ActivityDate = model.ActivityDate.Date;

                summaryPageActivityViewModel.ProjectGuid = project.ProjectGuid;
                summaryPageActivityViewModel.ProjectName = project.ProjectName;
                summaryPageActivityViewModel.PersonEntityId = model.PersonEntityId;
                summaryPageActivityViewModel.CreatedByName = createdBy.FirstName + " " + createdBy.LastName;
                summaryPageActivityViewModel.CreatedDate = DateTime.UtcNow;

                summaryPageActivityViewModel.SummaryPageActivityFormsList = new List<SummaryPageActivityForms>();
                SummaryPageActivityForms from = new SummaryPageActivityForms();

                ProjectStaffMongo roleCurrentUserole = project.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == loggedInUser);
                string roleName = roleCurrentUserole != null ? roleCurrentUserole.Role : string.Empty;
                if (roleName == string.Empty)
                {
                    var userlogin = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == loggedInUser);
                    if (userlogin != null)
                    {
                        var role = userlogin.UserRoles.FirstOrDefault();
                        roleName = role != null ? role.Role.Name : null;
                    }
                }
                List<Guid> removedFormGuid = new List<Guid>();
                foreach (var item in activity.FormsListMongo)
                {
                    #region permission                    
                    try
                    {
                        int count = 0;
                        int totalVariables = 0;
                        IQueryable<VariablesMongo> variables = item.VariablesListMongo.AsQueryable();
                        totalVariables = variables.Count();
                        variables.ToList().ForEach(v =>
                        {
                            var variable = v.VariableRoleListMongo.Where(x => x.RoleName == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                            if (variable == null) { count++; }
                            else
                            {
                                if (v.DependentVariableId != null)
                                {
                                    var parentVar = variables.FirstOrDefault(x => x.VariableId == v.DependentVariableId);
                                    var isParentVar = parentVar.VariableRoleListMongo.Where(x => x.RoleName == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                                    if (isParentVar == null) { count++; }
                                }
                            }
                        });
                        if (count > 0)
                        {
                            if (totalVariables == count)
                                removedFormGuid.Add(item.FormGuid);
                        }
                    }
                    catch (Exception exc) { Console.WriteLine(exc); }
                    #endregion

                    summaryPageActivityViewModel.SummaryPageActivityFormsList.Add(new SummaryPageActivityForms()
                    {
                        FormGuid = item.FormGuid,
                        FormId = item.FormId,
                        FormTitle = item.FormTitle,
                        FormStatusId = (int)Core.Enum.FormStatusTypes.Not_entered,
                        FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), (int)Core.Enum.FormStatusTypes.Not_entered),
                    });
                }

                summaryPageActivityViewModel.ProjectVersion = project.ProjectInternalVersion;
                var summaryPageActivity = _mongoDBContext._database.GetCollection<MongoDB.Bson.BsonDocument>("SummaryPageActivity");
                var summaryPageActivityResult = summaryPageActivity.Insert(summaryPageActivityViewModel);

                summaryPageActivityViewModel.SummaryPageActivityFormsList.RemoveAll(x => removedFormGuid.Contains(x.FormGuid));
                return summaryPageActivityViewModel;
                #endregion
            }
            else
            {
                throw new Core.AlreadyExistsException("activity already added.");
            }
        }
        public SummaryPageActivityViewModel EditSummaryPageActivity(SummaryPageActivityViewModel model, Guid loggedInUser)
        {
            var summaryPageActivityCondition = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, model.Id);
            var summaryPageActivityDetails = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindOne(summaryPageActivityCondition);
            if (summaryPageActivityDetails != null)
            {
                var activityCompletedBy = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == model.ActivityCompletedByGuid);
                var modifiedBy = _userLoginProvider.GetByGuid(loggedInUser);

                summaryPageActivityDetails.ActivityDate = model.ActivityDate.Date;
                summaryPageActivityDetails.ActivityCompletedById = activityCompletedBy.Id;
                summaryPageActivityDetails.ActivityCompletedByGuid = activityCompletedBy.Guid;
                summaryPageActivityDetails.ActivityCompletedByName = activityCompletedBy.FirstName + " " + activityCompletedBy.LastName;

                summaryPageActivityDetails.Id = model.Id;
                summaryPageActivityDetails.ModifiedDate = DateTime.UtcNow;
                summaryPageActivityDetails.ModifiedBy = modifiedBy.Guid;
                summaryPageActivityDetails.ModifiedByName = modifiedBy.FirstName + " " + modifiedBy.LastName;

                //Mongo Query  
                var CarObjectId = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, model.Id);
                // Document Collections  
                var collection = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                // Document Update which need Id and Data to Update  
                var result = collection.Update(CarObjectId, Update.Replace(summaryPageActivityDetails), MongoDB.Driver.UpdateFlags.None);

                return summaryPageActivityDetails;
            }
            else
            {
                throw new Core.AlreadyExistsException("Some error occured.");
            }
        }
        public SummaryPageActivityViewModel DeleteSummaryPageActivity(string id, Guid loggedInUser)
        {
            var summaryPageActivityCondition = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, new MongoDB.Bson.ObjectId(id));
            var summaryPageActivityDetails = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindOne(summaryPageActivityCondition);
            if (summaryPageActivityDetails != null)
            {
                if (summaryPageActivityDetails.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                || summaryPageActivityDetails.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
                || summaryPageActivityDetails.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)
                || summaryPageActivityDetails.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                {
                    throw new Core.NotFoundException("Can not delete default registration activities.");
                }

                var modifiedBy = _userLoginProvider.GetByGuid(loggedInUser);

                summaryPageActivityDetails.Id = new MongoDB.Bson.ObjectId(id);
                summaryPageActivityDetails.DateDeactivated = DateTime.UtcNow;
                summaryPageActivityDetails.DeactivatedBy = modifiedBy.Guid;
                summaryPageActivityDetails.DeactivatedByName = modifiedBy.FirstName + " " + modifiedBy.LastName;

                //Mongo Query  
                var CarObjectId = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, new MongoDB.Bson.ObjectId(id));
                // Document Collections  
                var collection = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                // Document Update which need Id and Data to Update  
                var result = collection.Update(CarObjectId, Update.Replace(summaryPageActivityDetails), MongoDB.Driver.UpdateFlags.None);

                return summaryPageActivityDetails;
            }
            else
            {
                throw new Core.NotFoundException("Activity not found.");
            }
        }

        public SummaryPageActivityViewModel TestEnvironment_AddSummaryPageActivity(SummaryPageActivityViewModel model, Guid loggedInUser)
        {
            var conditionProject = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, model.ProjectGuid);
            var project = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionProject).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
            var activity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == model.ActivityGuid);
            bool canRepeat = activity != null ? activity.CanCreatedMultipleTime : false;


            SummaryPageActivityViewModel summarypageActivityDocument = null;

            if (!canRepeat)
                summarypageActivityDocument = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindAll().AsQueryable().FirstOrDefault(x => x.ProjectGuid == model.ProjectGuid && x.ActivityGuid == model.ActivityGuid && x.PersonEntityId == model.PersonEntityId && x.DateDeactivated == null);

            if (summarypageActivityDocument == null)
            {
                var createdBy = _userLoginProvider.GetByGuid(loggedInUser);
                #region linkage from summary page                
                var activityCompletedBy = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == model.ActivityCompletedByGuid);
                SummaryPageActivityViewModel summaryPageActivityViewModel = new SummaryPageActivityViewModel();
                summaryPageActivityViewModel.ActivityId = activity.ActivityId;
                summaryPageActivityViewModel.ActivityGuid = activity.ActivityGuid;
                summaryPageActivityViewModel.ActivityName = activity.ActivityName;
                summaryPageActivityViewModel.ActivityCompletedById = activityCompletedBy.Id;
                summaryPageActivityViewModel.ActivityCompletedByGuid = activityCompletedBy.Guid;
                summaryPageActivityViewModel.ActivityCompletedByName = activityCompletedBy.FirstName + " " + activityCompletedBy.LastName;
                summaryPageActivityViewModel.ActivityDate = model.ActivityDate.Date;

                summaryPageActivityViewModel.ProjectGuid = project.ProjectGuid;
                summaryPageActivityViewModel.ProjectName = project.ProjectName;
                summaryPageActivityViewModel.PersonEntityId = model.PersonEntityId;
                summaryPageActivityViewModel.CreatedByName = createdBy.FirstName + " " + createdBy.LastName;
                summaryPageActivityViewModel.CreatedDate = DateTime.UtcNow;

                summaryPageActivityViewModel.SummaryPageActivityFormsList = new List<SummaryPageActivityForms>();
                SummaryPageActivityForms from = new SummaryPageActivityForms();


                ProjectStaffMongo roleCurrentUserole = project.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == loggedInUser);
                string roleName = roleCurrentUserole != null ? roleCurrentUserole.Role : string.Empty;
                if (roleName == string.Empty)
                {
                    var userlogin = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == loggedInUser);
                    if (userlogin != null)
                    {
                        var role = userlogin.UserRoles.FirstOrDefault();
                        roleName = role != null ? role.Role.Name : null;
                    }
                }
                List<Guid> removedFormGuid = new List<Guid>();
                foreach (var item in activity.FormsListMongo)
                {
                    #region permission                    
                    try
                    {
                        int count = 0;
                        int totalVariables = 0;
                        IQueryable<VariablesMongo> variables = item.VariablesListMongo.AsQueryable();
                        totalVariables = variables.Count();
                        variables.ToList().ForEach(v =>
                        {
                            var variable = v.VariableRoleListMongo.Where(x => x.RoleName == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                            if (variable == null) { count++; }
                            else
                            {
                                if (v.DependentVariableId != null)
                                {
                                    var parentVar = variables.FirstOrDefault(x => x.VariableId == v.DependentVariableId);
                                    var isParentVar = parentVar.VariableRoleListMongo.Where(x => x.RoleName == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                                    if (isParentVar == null) { count++; }
                                }
                            }
                        });
                        if (count > 0)
                        {
                            if (totalVariables == count)
                                removedFormGuid.Add(item.FormGuid);
                        }
                    }
                    catch (Exception exc) { Console.WriteLine(exc); }
                    #endregion
                    summaryPageActivityViewModel.SummaryPageActivityFormsList.Add(new SummaryPageActivityForms()
                    {
                        FormGuid = item.FormGuid,
                        FormId = item.FormId,
                        FormTitle = item.FormTitle,
                        FormStatusId = (int)Core.Enum.FormStatusTypes.Not_entered,
                        FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), (int)Core.Enum.FormStatusTypes.Not_entered),
                    });
                }
                summaryPageActivityViewModel.ProjectVersion = project.ProjectInternalVersion;
                var summaryPageActivity = _testMongoDBContext._database.GetCollection<MongoDB.Bson.BsonDocument>("SummaryPageActivity");
                var summaryPageActivityResult = summaryPageActivity.Insert(summaryPageActivityViewModel);

                summaryPageActivityViewModel.SummaryPageActivityFormsList.RemoveAll(x => removedFormGuid.Contains(x.FormGuid));

                return summaryPageActivityViewModel;
                #endregion
            }
            else
            {
                throw new Core.AlreadyExistsException("activity already added.");
            }
        }
        public SummaryPageActivityViewModel TestEnvironment_EditSummaryPageActivity(SummaryPageActivityViewModel model, Guid loggedInUser)
        {
            var summaryPageActivityCondition = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, model.Id);
            var summaryPageActivityDetails = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindOne(summaryPageActivityCondition);
            if (summaryPageActivityDetails != null)
            {
                var activityCompletedBy = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == model.ActivityCompletedByGuid);
                var modifiedBy = _userLoginProvider.GetByGuid(loggedInUser);

                summaryPageActivityDetails.ActivityDate = model.ActivityDate.Date;
                summaryPageActivityDetails.ActivityCompletedById = activityCompletedBy.Id;
                summaryPageActivityDetails.ActivityCompletedByGuid = activityCompletedBy.Guid;
                summaryPageActivityDetails.ActivityCompletedByName = activityCompletedBy.FirstName + " " + activityCompletedBy.LastName;

                summaryPageActivityDetails.Id = model.Id;
                summaryPageActivityDetails.ModifiedDate = DateTime.UtcNow;
                summaryPageActivityDetails.ModifiedBy = modifiedBy.Guid;
                summaryPageActivityDetails.ModifiedByName = modifiedBy.FirstName + " " + modifiedBy.LastName;

                //Mongo Query  
                var CarObjectId = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, model.Id);
                // Document Collections  
                var collection = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                // Document Update which need Id and Data to Update  
                var result = collection.Update(CarObjectId, Update.Replace(summaryPageActivityDetails), MongoDB.Driver.UpdateFlags.None);

                return summaryPageActivityDetails;
            }
            else
            {
                throw new Core.AlreadyExistsException("Some error occured.");
            }
        }
        public SummaryPageActivityViewModel TestEnvironment_DeleteSummaryPageActivity(string id, Guid loggedInUser)
        {
            var summaryPageActivityCondition = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, new MongoDB.Bson.ObjectId(id));
            var summaryPageActivityDetails = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindOne(summaryPageActivityCondition);
            if (summaryPageActivityDetails != null)
            {
                if (summaryPageActivityDetails.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                || summaryPageActivityDetails.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
                || summaryPageActivityDetails.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)
                || summaryPageActivityDetails.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                {
                    throw new Core.NotFoundException("Can not delete default registration activities.");
                }

                var modifiedBy = _userLoginProvider.GetByGuid(loggedInUser);

                summaryPageActivityDetails.Id = new MongoDB.Bson.ObjectId(id);
                summaryPageActivityDetails.DateDeactivated = DateTime.UtcNow;
                summaryPageActivityDetails.DeactivatedBy = modifiedBy.Guid;
                summaryPageActivityDetails.DeactivatedByName = modifiedBy.FirstName + " " + modifiedBy.LastName;

                //Mongo Query  
                var CarObjectId = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, new MongoDB.Bson.ObjectId(id));
                // Document Collections  
                var collection = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                // Document Update which need Id and Data to Update  
                var result = collection.Update(CarObjectId, Update.Replace(summaryPageActivityDetails), MongoDB.Driver.UpdateFlags.None);

                return summaryPageActivityDetails;
            }
            else
            {
                throw new Core.AlreadyExistsException("Some error occured.");
            }
        }
        public FormsMongo GetSummaryPageForm(Int64 entId, Guid formId, Guid activityId, Guid projectId, int p_Version, string summaryPageActivityId, Guid loggedInUserId, Guid currentProjectId)
        {
            bool isNewForm = true;
            var condition = Query<FormDataEntryMongo>.EQ(q => q.EntityNumber, entId);
            Form isDefaultForm = _dbContext.Forms.FirstOrDefault(x => x.Guid == formId);
            var entity = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(condition).AsQueryable().FirstOrDefault();
            if (p_Version != 0)
                entity.ProjectVersion = (entity.ProjectVersion != p_Version ? p_Version : entity.ProjectVersion);

            MongoDB.Driver.IMongoQuery conditionProject;
            conditionProject = Query.And(Query.EQ("ProjectGuid", projectId), Query.EQ("ProjectInternalVersion", entity.ProjectVersion));
            bool isRegistrationForm = isDefaultForm != null ? (isDefaultForm.IsDefaultForm == (int)Core.Enum.DefaultFormType.Default ? true : false) : false;
            if (isRegistrationForm) conditionProject = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectId);
            var thisProject = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionProject).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
            bool isSystemAdmin = false;
            var urole = _dbContext.UserRoles.FirstOrDefault(c => c.UserLogin.Guid == loggedInUserId);
            if (urole != null)
            {
                if (urole.Role == null)
                {
                    urole.Role = _dbContext.Roles.FirstOrDefault(c => c.Id == urole.RoleId);
                }
                if (urole.Role.Name == RoleTypes.System_Admin.ToString().Replace("_", " "))
                {
                    isSystemAdmin = true;
                }
            }
            if (thisProject == null)
            {
                if (isSystemAdmin)
                {
                    Form getProjectId = _dbContext.Forms.FirstOrDefault(c => c.Guid == formId);
                    if (getProjectId != null)
                    {
                        conditionProject = Query<ProjectDeployViewModel>.EQ(q => q.ProjectId, getProjectId.ProjectId);
                        thisProject = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionProject).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                    }
                }
            }
            if (thisProject != null)
            {
                var activity = thisProject.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == activityId);
                if (isRegistrationForm)
                    activity = thisProject.ProjectActivitiesList.FirstOrDefault(x => x.ActivityName == isDefaultForm.FormTitle);
                if (activity == null)
                {
                    if (isSystemAdmin)
                    {
                        Form getProjectId = _dbContext.Forms.FirstOrDefault(c => c.Guid == formId);
                        if (getProjectId != null)
                        {
                            conditionProject = Query<ProjectDeployViewModel>.EQ(q => q.ProjectId, getProjectId.ProjectId);
                            thisProject = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionProject).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                            activity = thisProject.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == activityId);
                        }
                    }
                }

                if (activity != null)
                {
                    var form = activity.FormsListMongo.FirstOrDefault(x => x.FormGuid == formId);
                    if (isRegistrationForm)
                        form = activity.FormsListMongo.FirstOrDefault(x => x.FormTitle == isDefaultForm.FormTitle);
                    if (form != null)
                        return ToSummaryPageFormModel(form, entId, isNewForm, activityId, summaryPageActivityId, loggedInUserId, projectId, currentProjectId);
                }
            }
            return null;
        }
        public FormsMongo TestEnvironment_GetSummaryPageForm(Int64 entId, Guid formId, Guid activityId, Guid projectId, int p_Version, string summaryPageActivityId, Guid loggedInUserId, Guid currentProjectId)
        {
            bool isNewForm = true;
            var condition = Query<FormDataEntryMongo>.EQ(q => q.EntityNumber, entId);
            Form isDefaultForm = _dbContext.Forms.FirstOrDefault(x => x.Guid == formId);
            var entity = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(condition).AsQueryable().FirstOrDefault();
            if (p_Version != 0)
                entity.ProjectVersion = (entity.ProjectVersion != p_Version ? p_Version : entity.ProjectVersion);
            MongoDB.Driver.IMongoQuery conditionProject;
            conditionProject = Query.And(Query.EQ("ProjectGuid", projectId), Query.EQ("ProjectInternalVersion", entity.ProjectVersion));
            bool isRegistrationForm = isDefaultForm != null ? (isDefaultForm.IsDefaultForm == (int)Core.Enum.DefaultFormType.Default ? true : false) : false;

            if (isRegistrationForm) conditionProject = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectId);


            var thisProject = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionProject).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();

            var activity = thisProject.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == activityId);

            if (isRegistrationForm)
                activity = thisProject.ProjectActivitiesList.FirstOrDefault(x => x.ActivityName == isDefaultForm.FormTitle);

            if (activity != null)
            {
                var form = activity.FormsListMongo.FirstOrDefault(x => x.FormGuid == formId);
                if (isRegistrationForm)
                    form = activity.FormsListMongo.FirstOrDefault(x => x.FormTitle == isDefaultForm.FormTitle);


                if (form != null)
                    return TestEnvironment_ToSummaryPageFormModel(form, entId, isNewForm, activityId, summaryPageActivityId, loggedInUserId, projectId, currentProjectId);
            }
            return null;
        }
        public FormsMongo ToSummaryPageFormModel(FormsMongo form, Int64 entId, bool isNewForm, Guid activityId, string summaryPageActivityId, Guid loggedInUserId, Guid projectId, Guid currentProjectId)
        {
            var conditionChild = Query.And(Query.EQ("ParentEntityNumber", entId), Query.EQ("FormGuid", form.FormGuid));
            string[] registrationFormsArray = new string[]
                    {
                        EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Linkage)
                    };

            bool isRegistrationForm = registrationFormsArray.Contains(form.FormTitle) ? true : false;

            if (!isRegistrationForm)
                conditionChild = Query.And(Query.EQ("ParentEntityNumber", entId), Query.EQ("FormGuid", form.FormGuid), Query.EQ("ActivityGuid", activityId), Query.EQ("SummaryPageActivityObjId", summaryPageActivityId));
            else
                conditionChild = Query.And(Query.EQ("ParentEntityNumber", entId), Query.EQ("FormTitle", form.FormTitle), Query.EQ("SummaryPageActivityObjId", summaryPageActivityId));
            var isCustomEntity = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionChild).AsQueryable().FirstOrDefault();
            if (isCustomEntity != null)
            {
                entId = isCustomEntity.EntityNumber;
                isNewForm = false;
            }
            else
            {
                if (form.FormTitle == "Person Registration"
                    || form.FormTitle == "Participant Registration"
                    || form.FormTitle == "Place/Group Registration"
                    || form.FormTitle == "Project Registration")
                {

                }
                else
                    entId = 0;
            }
            var condition = Query.And(Query.EQ("EntityNumber", entId), Query.EQ("SummaryPageActivityObjId", summaryPageActivityId));
            var entity = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(condition).AsQueryable().FirstOrDefault();

            List<FormDataEntryVariableMongo> userEntitiesVariable = new List<FormDataEntryVariableMongo>();
            if (entity != null)
            {
                userEntitiesVariable = entity.formDataEntryVariableMongoList;

            }
            string roleName = string.Empty;
            var proRole = _dbContext.ProjectStaffMemberRoles.FirstOrDefault(x => x.FormDataEntry.Guid == currentProjectId && x.UserLogin.Guid == loggedInUserId);
            if (proRole == null)
            {
                UserLogin userLogin = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == loggedInUserId);
                if (userLogin != null)
                {
                    Role role = userLogin.UserRoles.Select(x => x.Role).FirstOrDefault();
                    roleName = role != null ? role.Name : string.Empty;
                }
            }
            else
            {
                roleName = proRole != null ? proRole.Role.Name : string.Empty;
            }
            var temp = new FormVariableRoleMongo { RoleName = roleName, CanView = true };
            var modifiedBy = entity != null ? entity.ModifiedBy.HasValue ? _userLoginProvider.GetByGuid(entity.ModifiedBy.Value) : null : null;
            IMongoQuery conditionUserEntitiesCustomForms = Query.Or(
                 Query<FormDataEntryMongo>.EQ(q => q.FormTitle, EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                 , Query<FormDataEntryMongo>.EQ(q => q.FormTitle, EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                 , Query<FormDataEntryMongo>.EQ(q => q.FormTitle, EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                 , Query<FormDataEntryMongo>.EQ(q => q.FormTitle, EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                 );
            IQueryable<FormDataEntryMongo> allEntities = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionUserEntitiesCustomForms).AsQueryable();
            var teamValues = new FormsMongo()
            {
                FormId = form.FormId,
                FormGuid = form.FormGuid,
                FormCategoryName = form.FormCategoryName,
                FormTitle = form.FormTitle,
                FormVersion = form.FormVersion,
                IsDefaultForm = form.IsDefaultForm,
                FormEntityTypes = form.FormEntityTypes,
                VariablesListMongo = form.VariablesListMongo.OrderBy(x => x.VariableOrderNo).Where(x => x.VariableRoleListMongo.Contains(temp, new FormVariableRoleMongoComparer())).Select(variable => new VariablesMongo()
                {
                    VariableGuid = variable.VariableGuid,
                    VariableName = variable.VariableName,
                    VariableId = variable.VariableId,
                    HelpText = variable.HelpText,
                    IsRequired = variable.IsRequired,
                    MaxRange = variable.MaxRange,
                    MinRange = variable.MinRange,
                    Question = variable.Question,
                    VariableRequiredMessage = variable.VariableRequiredMessage,
                    ValueDescription = (variable.VariableName == DefaultsVariables.AuthenticationMethod.ToString() || variable.VariableName == DefaultsVariables.ProRole.ToString() || variable.VariableTypeName == VariableTypes.LKUP.ToString()) ? GetVariableValues(variable, currentProjectId, loggedInUserId, allEntities).Select(x => x.Value).FirstOrDefault() : variable.ValueDescription,
                    Values = (variable.VariableName == DefaultsVariables.AuthenticationMethod.ToString() || variable.VariableName == DefaultsVariables.ProRole.ToString() || variable.VariableTypeName == VariableTypes.LKUP.ToString()) ? GetVariableValues(variable, currentProjectId, loggedInUserId, allEntities).Select(x => x.Key).FirstOrDefault() : variable.Values,
                    VariableTypeName = variable.VariableTypeName,
                    LookupEntityTypeName = variable.LookupEntityTypeName,
                    LookupEntitySubtypeName = variable.LookupEntitySubtypeName,
                    VariableRoleListMongo = variable.VariableRoleListMongo,
                    VariableValidationRuleListMongo = variable.VariableValidationRuleListMongo,
                    DependentVariableId = variable.DependentVariableId,
                    DependentVariableName = variable.DependentVariableName,
                    IsDefaultVariable = variable.IsDefaultVariable,
                    IsSearchVisible = variable.IsSearchVisible,
                    ResponseOption = variable.ResponseOption,
                    SearchPageOrder = variable.SearchPageOrder,
                    SelectedValues = userEntitiesVariable.FirstOrDefault(x => x.VariableGuid == variable.VariableGuid) != null ? userEntitiesVariable.FirstOrDefault(x => x.VariableGuid == variable.VariableGuid).SelectedValues : string.Empty,
                    UserLoginGuid = entity != null ? entity.ThisUserGuid : (Guid?)null,
                    CanFutureDate = variable.CanFutureDate,
                    FormPageVariableId = variable.FormPageVariableId,
                    FileName = userEntitiesVariable.FirstOrDefault(x => x.VariableGuid == variable.VariableGuid) != null ? userEntitiesVariable.FirstOrDefault(x => x.VariableGuid == variable.VariableGuid).FileName : string.Empty,
                    LinkedProjectListWithGroupList = variable.VariableName == DefaultsVariables.LnkPro.ToString() ? GetLinkedProjectListWithGroup(variable, currentProjectId, loggedInUserId) : null,
                    IsBlank = variable.IsBlank,
                }).ToList(),
                TotalFormVariableCount = form.VariablesListMongo != null ? form.VariablesListMongo.Where(variable => variable.VariableTypeName != VariableTypes.Heading.ToString() && variable.VariableTypeName != VariableTypes.Other_Text.ToString().Replace("_", " ")).Count() : 0,
                FormDataEntryId = entity != null ? entity.Id.ToString() : null,
                ModifiedBy = modifiedBy != null ? modifiedBy.FirstName + " " + modifiedBy.LastName : entity != null ? entity.CreatedByName : string.Empty,
                ModifiedDate = entity != null ? entity.ModifiedDate != null ? entity.ModifiedDate?.ToLocalTime().ToString("dd-MMM-yyyy HH:mm:ss") : entity.CreatedDate.ToLocalTime().ToString("dd-MMM-yyyy HH:mm:ss") : string.Empty,
            };
            //TODO:: 
            //Check for selcted values in Values
            try
            {
                var currentSelection = teamValues.VariablesListMongo.FirstOrDefault(c => c.VariableName == DefaultsVariables.AuthenticationMethod.ToString()).SelectedValues;
                var CurrenSelectedDropDown = teamValues.VariablesListMongo.FirstOrDefault(c => c.VariableName == DefaultsVariables.AuthenticationMethod.ToString()).Values;
                //If not exists, 
                var IsExits = CurrenSelectedDropDown.Contains(currentSelection);
                if (!IsExits)
                {
                    //Get the current selcted values details from db collection
                    var newValueDiscription = _dbContext.LoginAuthTypeMasters.Where(c => c.Guid == new Guid(currentSelection)).Select(c => c.AuthTypeName).First();
                    //add the current values to dropdown collections Values and Descrptions
                    teamValues.VariablesListMongo.FirstOrDefault(c => c.VariableName == DefaultsVariables.AuthenticationMethod.ToString()).Values.Add(currentSelection);
                    teamValues.VariablesListMongo.FirstOrDefault(c => c.VariableName == DefaultsVariables.AuthenticationMethod.ToString()).ValueDescription.Add(newValueDiscription);

                }
            }
            catch (Exception ex)
            {


            }

            return teamValues;
        }
        public FormsMongo TestEnvironment_ToSummaryPageFormModel(FormsMongo form, Int64 entId, bool isNewForm, Guid activityId, string summaryPageActivityId, Guid loggedInUserId, Guid projectId, Guid currentProjectId)
        {
            var conditionChild = Query.And(Query.EQ("ParentEntityNumber", entId), Query.EQ("FormGuid", form.FormGuid));
            string[] registrationFormsArray = new string[]
                    {
                        EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Linkage)
                    };
            bool isRegistrationForm = registrationFormsArray.Contains(form.FormTitle) ? true : false;

            if (!isRegistrationForm)
                conditionChild = Query.And(Query.EQ("ParentEntityNumber", entId), Query.EQ("FormGuid", form.FormGuid), Query.EQ("ActivityGuid", activityId), Query.EQ("SummaryPageActivityObjId", summaryPageActivityId));
            else
                conditionChild = Query.And(Query.EQ("ParentEntityNumber", entId), Query.EQ("FormTitle", form.FormTitle), Query.EQ("SummaryPageActivityObjId", summaryPageActivityId));
            var isCustomEntity = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionChild).AsQueryable().FirstOrDefault();
            if (isCustomEntity != null)
            {
                entId = isCustomEntity.EntityNumber;
                isNewForm = false;
            }
            else
            {
                if (form.FormTitle == "Person Registration"
                    || form.FormTitle == "Participant Registration"
                    || form.FormTitle == "Place/Group Registration"
                    || form.FormTitle == "Project Registration")
                {

                }
                else
                    entId = 0;
            }
            var condition = Query.And(Query.EQ("EntityNumber", entId), Query.EQ("SummaryPageActivityObjId", summaryPageActivityId));
            var entity = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(condition).AsQueryable().FirstOrDefault();

            List<FormDataEntryVariableMongo> userEntitiesVariable = new List<FormDataEntryVariableMongo>();
            if (entity != null)
            {
                userEntitiesVariable = entity.formDataEntryVariableMongoList;

            }
            string roleName = string.Empty;
            var proRole = _dbContext.ProjectStaffMemberRoles.FirstOrDefault(x => x.FormDataEntry.Guid == currentProjectId && x.UserLogin.Guid == loggedInUserId);
            if (proRole == null)
            {
                UserLogin userLogin = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == loggedInUserId);
                if (userLogin != null)
                {
                    Role role = userLogin.UserRoles.Select(x => x.Role).FirstOrDefault();
                    roleName = role != null ? role.Name : string.Empty;
                }
            }
            else
            {
                roleName = proRole != null ? proRole.Role.Name : string.Empty;
            }
            var temp = new FormVariableRoleMongo { RoleName = roleName, CanView = true };
            var modifiedBy = entity != null ? entity.ModifiedBy.HasValue ? _userLoginProvider.GetByGuid(entity.ModifiedBy.Value) : null : null;
            IMongoQuery conditionUserEntitiesCustomForms = Query.Or(
               Query<FormDataEntryMongo>.EQ(q => q.FormTitle, EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
               , Query<FormDataEntryMongo>.EQ(q => q.FormTitle, EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
               , Query<FormDataEntryMongo>.EQ(q => q.FormTitle, EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
               , Query<FormDataEntryMongo>.EQ(q => q.FormTitle, EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
               );
            IQueryable<FormDataEntryMongo> allEntities = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionUserEntitiesCustomForms).AsQueryable();

            return new FormsMongo()
            {
                FormId = form.FormId,
                FormGuid = form.FormGuid,
                FormCategoryName = form.FormCategoryName,
                FormTitle = form.FormTitle,
                FormVersion = form.FormVersion,
                IsDefaultForm = form.IsDefaultForm,
                FormEntityTypes = form.FormEntityTypes,
                VariablesListMongo = form.VariablesListMongo.OrderBy(x => x.VariableOrderNo).Where(varRole => varRole.VariableRoleListMongo.Contains(temp, new FormVariableRoleMongoComparer())).Select(variable => new VariablesMongo()
                {
                    VariableGuid = variable.VariableGuid,
                    VariableName = variable.VariableName,
                    VariableId = variable.VariableId,
                    HelpText = variable.HelpText,
                    IsRequired = variable.IsRequired,
                    MaxRange = variable.MaxRange,
                    MinRange = variable.MinRange,
                    Question = variable.Question,
                    VariableRequiredMessage = variable.VariableRequiredMessage,
                    ValueDescription = (variable.VariableName == DefaultsVariables.AuthenticationMethod.ToString() || variable.VariableName == DefaultsVariables.ProRole.ToString() || variable.VariableTypeName == VariableTypes.LKUP.ToString()) ? TestEnvironment_GetVariableValues(variable, currentProjectId, loggedInUserId, allEntities).Select(x => x.Value).FirstOrDefault() : variable.ValueDescription,
                    Values = (variable.VariableName == DefaultsVariables.AuthenticationMethod.ToString() || variable.VariableName == DefaultsVariables.ProRole.ToString() || variable.VariableTypeName == VariableTypes.LKUP.ToString()) ? TestEnvironment_GetVariableValues(variable, currentProjectId, loggedInUserId, allEntities).Select(x => x.Key).FirstOrDefault() : variable.Values,
                    VariableTypeName = variable.VariableTypeName,
                    LookupEntityTypeName = variable.LookupEntityTypeName,
                    LookupEntitySubtypeName = variable.LookupEntitySubtypeName,
                    VariableRoleListMongo = variable.VariableRoleListMongo,
                    VariableValidationRuleListMongo = variable.VariableValidationRuleListMongo,
                    DependentVariableId = variable.DependentVariableId,
                    DependentVariableName = variable.DependentVariableName,
                    IsDefaultVariable = variable.IsDefaultVariable,
                    IsSearchVisible = variable.IsSearchVisible,
                    ResponseOption = variable.ResponseOption,
                    SearchPageOrder = variable.SearchPageOrder,
                    SelectedValues = userEntitiesVariable.FirstOrDefault(x => x.VariableGuid == variable.VariableGuid) != null ? userEntitiesVariable.FirstOrDefault(x => x.VariableGuid == variable.VariableGuid).SelectedValues : string.Empty,
                    UserLoginGuid = entity != null ? entity.ThisUserGuid : (Guid?)null,
                    CanFutureDate = variable.CanFutureDate,
                    FormPageVariableId = variable.FormPageVariableId,
                    LinkedProjectListWithGroupList = variable.VariableName == DefaultsVariables.LnkPro.ToString() ? TestEnvironment_GetLinkedProjectListWithGroup(variable, currentProjectId, loggedInUserId) : null,
                    IsBlank = variable.IsBlank,
                }).ToList(),
                TotalFormVariableCount = form.VariablesListMongo != null ? form.VariablesListMongo.Where(variable => variable.VariableTypeName != VariableTypes.Heading.ToString() && variable.VariableTypeName != VariableTypes.Other_Text.ToString().Replace("_", " ")).Count() : 0,
                FormDataEntryId = entity != null ? entity.Id.ToString() : null,
                ModifiedBy = modifiedBy != null ? modifiedBy.FirstName + " " + modifiedBy.LastName : entity != null ? entity.CreatedByName : string.Empty,
                ModifiedDate = entity != null ? entity.ModifiedDate != null ? entity.ModifiedDate?.ToString("dd-MMM-yyyy HH:mm:ss") : entity.CreatedDate.ToString("dd-MMM-yyyy HH:mm:ss") : string.Empty,
            };
        }

        public IDictionary<List<string>, List<string>> GetVariableValues(VariablesMongo variablesMongo, Guid projectId, Guid loggedInUserId, IQueryable<FormDataEntryMongo> allEntities)
        {
            if (variablesMongo.VariableName == DefaultsVariables.LnkPro.ToString())
            {
                return new Dictionary<List<string>, List<string>>();
            }
            List<string> valueList = new List<string>();
            List<string> descriptionList = new List<string>();
            if (variablesMongo.VariableName == DefaultsVariables.AuthenticationMethod.ToString())
            {
                valueList = new List<string>();
                descriptionList = new List<string>();
                var authTypeId = _dbContext.LoginAuthTypeMasters.Where(x => x.DateDeactivated == null).Select(x => new { id = x.Guid.ToString(), name = x.AuthTypeName }).ToList();
                valueList = authTypeId.Select(x => x.id).ToList();
                descriptionList = authTypeId.Select(x => x.name).ToList();
            }
            if (variablesMongo.VariableTypeName == VariableTypes.LKUP.ToString())
            {
                #region LKUP drop-down
                valueList = new List<string>();
                descriptionList = new List<string>();
                string suntype = string.Empty;
                if (variablesMongo.LookupEntitySubtypeName != null && variablesMongo.LookupEntitySubtypeName.Count() == 1)
                {
                    suntype = variablesMongo.LookupEntitySubtypeName.FirstOrDefault();
                }
                Guid lookupEntitySubtypeGuid = _dbContext.EntitySubTypes.Where(x => x.Name == suntype).Select(x => x.Guid).FirstOrDefault();
                #region MONGO entities
                try
                {
                    var mon = GetLookupVariablesPreview(variablesMongo, variablesMongo.LookupEntityTypeName, null, allEntities);
                    mon.ToList().ForEach(mngo =>
                    {
                        if (lookupEntitySubtypeGuid != Guid.Empty)
                        {
                            try
                            {
                                if (mngo.EntitySubtypeId == lookupEntitySubtypeGuid)
                                {
                                    valueList.Add(mngo.EntityName);
                                    descriptionList.Add(mngo.EntityName);
                                }
                            }
                            catch (Exception ef)
                            { }
                        }
                        else if (mngo.EntityName != null)
                        {
                            valueList.Add(mngo.EntityName);
                            descriptionList.Add(mngo.EntityName);
                        }
                    });
                }
                catch (Exception dd)
                { }
                #endregion
                #endregion
            }
            if (variablesMongo.VariableName == DefaultsVariables.ProRole.ToString())
            {
                valueList = new List<string>();
                descriptionList = new List<string>();
                var roles = _dbContext.Roles.Where(x => x.Name != Core.Enum.RoleTypes.Definition_Admin.ToString().Replace("_", " ")
                        && x.DateDeactivated == null && x.Name != Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " ")).OrderByDescending(x => x.Id).Select(x => new { id = x.Guid.ToString(), name = x.Name }).ToList();
                valueList = roles.Select(x => x.id).ToList();
                descriptionList = roles.Select(x => x.name).ToList();
            }
            if (variablesMongo.VariableName == DefaultsVariables.LnkPro.ToString())
            {
                #region LnkProj
                Int32 projectStaffMemberRoleId = _dbContext.ProjectStaffMemberRoles.Where(x => x.FormDataEntry.Guid == projectId && x.UserLogin.Guid == loggedInUserId).Select(x => x.RoleId).FirstOrDefault();
                var roleDB = _dbContext.Roles.FirstOrDefault(x => x.Id == projectStaffMemberRoleId);
                string roleName = roleDB != null ? roleDB.Name : string.Empty;
                if (roleName == string.Empty)
                {
                    var userlogin = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == loggedInUserId);
                    if (userlogin != null)
                    {
                        var role = userlogin.UserRoles.FirstOrDefault();
                        roleName = role != null ? role.Role.Name : null;
                    }
                }
                valueList = new List<string>();
                descriptionList = new List<string>();
                int entityTypeId = _dbContext.EntityTypes.Where(x => x.Name == Core.Enum.EntityTypes.Project.ToString()).Select(x => x.Id).FirstOrDefault();
                List<FormDataEntry> allProjects = _dbContext.FormDataEntries.Where(x =>
                            x.EntityId == entityTypeId
                            && x.Activity.ActivityName == DefaultFormName.Project_Registration.ToString().Replace("_", " ")
                            ).ToList();
                string[] allowedRole = new string[] {
                    RoleTypes.System_Admin.ToString().Replace("_"," ")
                    , RoleTypes.Project_Admin.ToString().Replace("_"," ")
                };
                if (roleName != Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " "))
                {
                    var projectStaffList = _dbContext.ProjectStaffMemberRoles.Where(x => x.UserLogin.Guid == loggedInUserId);
                    List<Guid> removeList = new List<Guid>();
                    List<FormDataEntry> removeList1 = new List<FormDataEntry>();
                    allProjects.ForEach(frm =>
                    {
                        var staff = projectStaffList.Where(x => x.ProjectId == frm.Id).FirstOrDefault();
                        string projectRoleName = staff != null ? staff.Role.Name : string.Empty;

                        if (!allowedRole.Contains(projectRoleName))
                        {
                            removeList.Add(frm.Guid);
                        }
                    });
                    allProjects.RemoveAll(x => removeList.Contains(x.Guid));
                }
                allProjects.ForEach(pr =>
                {
                    valueList.Add(pr.Guid.ToString());
                    var pName = pr.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.Name.ToString());
                    string name = string.Empty;
                    if (pName != null && !string.IsNullOrEmpty(pName.SelectedValues))
                    {
                        name = pName.SelectedValues;
                    }
                    else
                    {
                        pName = pr.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.ProjectDisplayName.ToString());
                        name = pName != null && !string.IsNullOrEmpty(pName.SelectedValues) ? pName.SelectedValues : string.Empty;
                    }
                    descriptionList.Add(name);
                });
                #endregion
            }
            IDictionary<List<string>, List<string>> resultValues = new Dictionary<List<string>, List<string>>();
            resultValues.Add(valueList, descriptionList);
            return resultValues;
        }

        public IDictionary<List<string>, List<string>> TestEnvironment_GetVariableValues(VariablesMongo variablesMongo, Guid projectId, Guid loggedInUserId, IQueryable<FormDataEntryMongo> allEntities)
        {
            if (variablesMongo.VariableName == DefaultsVariables.LnkPro.ToString())
            {
                return new Dictionary<List<string>, List<string>>();
            }
            List<string> valueList = new List<string>();
            List<string> descriptionList = new List<string>();
            if (variablesMongo.VariableName == DefaultsVariables.AuthenticationMethod.ToString())
            {
                valueList = new List<string>();
                descriptionList = new List<string>();
                var authTypeId = _dbContext.LoginAuthTypeMasters.Where(x => x.DateDeactivated == null).Select(x => new { id = x.Guid.ToString(), name = x.AuthTypeName }).ToList();
                valueList = authTypeId.Select(x => x.id).ToList();
                descriptionList = authTypeId.Select(x => x.name).ToList();
            }
            if (variablesMongo.VariableTypeName == VariableTypes.LKUP.ToString())
            {
                #region LKUP
                valueList = new List<string>();
                descriptionList = new List<string>();
                #endregion

                string suntype = string.Empty;
                if (variablesMongo.LookupEntitySubtypeName != null && variablesMongo.LookupEntitySubtypeName.Count() == 1)
                {
                    suntype = variablesMongo.LookupEntitySubtypeName.FirstOrDefault();
                }
                Guid lookupEntitySubtypeGuid = _dbContext.EntitySubTypes.Where(x => x.Name == suntype).Select(x => x.Guid).FirstOrDefault();

                #region MONGO entities
                try
                {
                    var mon = TestEnvironment_GetLookupVariablesPreview(variablesMongo, variablesMongo.LookupEntityTypeName, null, allEntities);
                    mon.ToList().ForEach(mngo =>
                    {
                        if (lookupEntitySubtypeGuid != Guid.Empty)
                        {
                            try
                            {
                                if (mngo.EntitySubtypeId == lookupEntitySubtypeGuid)
                                {
                                    valueList.Add(mngo.EntityName);
                                    descriptionList.Add(mngo.EntityName);
                                }
                            }
                            catch (Exception ef)
                            { }
                        }
                        else if (mngo.EntityName != null)
                        {
                            valueList.Add(mngo.EntityName);
                            descriptionList.Add(mngo.EntityName);
                        }
                    });


                }
                catch (Exception dd)
                { }
                #endregion
            }

            if (variablesMongo.VariableName == DefaultsVariables.LnkPro.ToString())
            {
                valueList = new List<string>();
                descriptionList = new List<string>();

                var projectCollections = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").FindAll().AsQueryable();

                var currentProj = projectCollections.FirstOrDefault(x => x.ProjectGuid == projectId);
                var currentProjRole = currentProj.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == loggedInUserId);
                string roleName = (currentProjRole != null ? currentProjRole.Role : string.Empty);

                string[] allowedRole = new string[] {
                    RoleTypes.System_Admin.ToString().Replace("_"," ")
                    , RoleTypes.Project_Admin.ToString().Replace("_"," ")
                };

                foreach (var project in projectCollections.GroupBy(x => x.ProjectGuid))
                {
                    var prj = project.OrderByDescending(x => x.ProjectInternalVersion).FirstOrDefault();

                    if (roleName != Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " "))
                    {
                        var stf = prj.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == loggedInUserId);
                        if (stf != null && allowedRole.Contains(stf.Role))
                        {
                            valueList.Add(prj.ProjectGuid.ToString());
                            descriptionList.Add(prj.ProjectName);
                        }
                    }
                    else
                    {
                        valueList.Add(prj.ProjectGuid.ToString());
                        descriptionList.Add(prj.ProjectName);
                    }
                }
            }
            if (variablesMongo.VariableName == DefaultsVariables.ProRole.ToString())
            {
                valueList = new List<string>();
                descriptionList = new List<string>();
                var roles = _dbContext.Roles.Where(x => x.DateDeactivated == null && x.Name != Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " ")).Select(x => new { id = x.Guid.ToString(), name = x.Name }).ToList();
                valueList = roles.Select(x => x.id).ToList();
                descriptionList = roles.Select(x => x.name).ToList();
            }
            IDictionary<List<string>, List<string>> resultValues = new Dictionary<List<string>, List<string>>();
            resultValues.Add(valueList, descriptionList);
            return resultValues;
        }

        public VariablesMongo ToSummaryPageFormVariableModel(VariablesMongo variable, int formId, int selectedValue)
        {
            return new VariablesMongo()
            {
                VariableGuid = variable.VariableGuid,
                VariableName = variable.VariableName,
                VariableId = variable.VariableId,
                HelpText = variable.HelpText,
                IsRequired = variable.IsRequired,
                MaxRange = variable.MaxRange,
                MinRange = variable.MinRange,
                Question = variable.Question,
                VariableRequiredMessage = variable.VariableRequiredMessage,
                ValueDescription = variable.ValueDescription,
                Values = variable.Values,
                VariableTypeName = variable.VariableTypeName,
                LookupEntityTypeName = variable.LookupEntityTypeName,
                LookupEntitySubtypeName = variable.LookupEntitySubtypeName,
            };
        }
        class FormVariableRoleMongoComparer : IEqualityComparer<FormVariableRoleMongo>
        {
            public bool Equals(FormVariableRoleMongo x, FormVariableRoleMongo y)
            {
                if (x.RoleName == y.RoleName && x.CanView == y.CanView)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public int GetHashCode(FormVariableRoleMongo obj)
            {
                return obj.RoleName.GetHashCode();
            }
        }

        public List<LinkedProjectGroupViewModel> GetLinkedProjectListWithGroup(VariablesMongo variablesMongo, Guid projectId, Guid loggedInUserId)
        {
            List<string> valueList = new List<string>();
            List<string> descriptionList = new List<string>();
            List<string> groupName = new List<string>();
            List<LinkedProjectGroupViewModel> returnModelList = new List<LinkedProjectGroupViewModel>();

            if (variablesMongo.VariableName == DefaultsVariables.LnkPro.ToString())
            {
                #region LnkProj
                Int32 projectStaffMemberRoleId = _dbContext.ProjectStaffMemberRoles.Where(x => x.FormDataEntry.Guid == projectId && x.UserLogin.Guid == loggedInUserId).Select(x => x.RoleId).FirstOrDefault();
                var roleDB = _dbContext.Roles.FirstOrDefault(x => x.Id == projectStaffMemberRoleId);
                string roleName = roleDB != null ? roleDB.Name : string.Empty;
                if (roleName == string.Empty)
                {
                    var userlogin = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == loggedInUserId);
                    if (userlogin != null)
                    {
                        var role = userlogin.UserRoles.FirstOrDefault();
                        roleName = role != null ? role.Role.Name : null;
                    }
                }
                valueList = new List<string>();
                descriptionList = new List<string>();
                groupName = new List<string>();

                int entityTypeId = _dbContext.EntityTypes.Where(x => x.Name == Core.Enum.EntityTypes.Project.ToString()).Select(x => x.Id).FirstOrDefault();

                List<FormDataEntry> allProjects = _dbContext.FormDataEntries.Where(x =>
                            x.EntityId == entityTypeId
                            && x.Activity.ActivityName == DefaultFormName.Project_Registration.ToString().Replace("_", " ")
                            ).ToList();

                string[] allowedRole = new string[] {
                    RoleTypes.System_Admin.ToString().Replace("_"," ")
                    , RoleTypes.Project_Admin.ToString().Replace("_"," ")
                };

                if (roleName != Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " "))
                {
                    var projectStaffList = _dbContext.ProjectStaffMemberRoles.Where(x => x.UserLogin.Guid == loggedInUserId);

                    List<Guid> removeList = new List<Guid>();
                    List<FormDataEntry> removeList1 = new List<FormDataEntry>();
                    allProjects.ForEach(frm =>
                    {
                        var staff = projectStaffList.Where(x => x.ProjectId == frm.Id).FirstOrDefault();

                        string projectRoleName = staff != null ? staff.Role.Name : string.Empty;

                        if (!allowedRole.Contains(projectRoleName))
                        {
                            removeList.Add(frm.Guid);
                        }
                    });
                    allProjects.RemoveAll(x => removeList.Contains(x.Guid));
                }
                allProjects.ForEach(pr =>
                {
                    valueList.Add(pr.Guid.ToString());
                    var pName = pr.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.Name.ToString());
                    string name = string.Empty;
                    if (pName != null && !string.IsNullOrEmpty(pName.SelectedValues))
                    {
                        name = pName.SelectedValues;
                    }
                    else
                    {
                        pName = pr.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.ProjectDisplayName.ToString());
                        name = pName != null && !string.IsNullOrEmpty(pName.SelectedValues) ? pName.SelectedValues : string.Empty;
                    }
                    descriptionList.Add(name);
                    var grpName = pr.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.ProSType.ToString());
                    int projGroup = grpName != null ? !string.IsNullOrEmpty(grpName.SelectedValues) ? Convert.ToInt32(grpName.SelectedValues) : 0 : 0;
                    groupName.Add(Enum.GetName(typeof(ProjectSubTypeEnum), projGroup));
                    var projectgroupname = Enum.GetName(typeof(ProjectSubTypeEnum), projGroup);
                    returnModelList.Add(new LinkedProjectGroupViewModel()
                    {
                        GroupName = !string.IsNullOrEmpty(projectgroupname) ? projectgroupname.Replace("_", " ") : "",
                        ProjectId = pr.Guid.ToString(),
                        ProjectName = name,
                    });
                });
                #endregion
            }
            return returnModelList.OrderBy(x => x.GroupName).ThenBy(y => y.ProjectName).ToList();// new Tuple<List<string>, List<string>, List<string>>(valueList, descriptionList, groupName);
        }
        public List<LinkedProjectGroupViewModel> TestEnvironment_GetLinkedProjectListWithGroup(VariablesMongo variablesMongo, Guid projectId, Guid loggedInUserId)
        {
            List<LinkedProjectGroupViewModel> returnModelList = new List<LinkedProjectGroupViewModel>();
            if (variablesMongo.VariableName == DefaultsVariables.LnkPro.ToString())
            {
                #region LnkProj
                var projectCollections = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").FindAll().AsQueryable();
                var currentProj = projectCollections.FirstOrDefault(x => x.ProjectGuid == projectId);
                var currentProjRole = currentProj.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == loggedInUserId);
                string roleName = (currentProjRole != null ? currentProjRole.Role : string.Empty);
                string[] allowedRole = new string[] {
                    RoleTypes.System_Admin.ToString().Replace("_"," ")
                    , RoleTypes.Project_Admin.ToString().Replace("_"," ")
                };
                var ids = projectCollections.Select(x => x.ProjectGuid).ToList();
                var projSubtype = _dbContext.FormDataEntries.Where(x => ids.Contains(x.Guid));
                foreach (var project in projectCollections.GroupBy(x => x.ProjectGuid))
                {
                    var prj = project.OrderByDescending(x => x.ProjectInternalVersion).FirstOrDefault();
                    var pr = projSubtype.FirstOrDefault(c => c.Guid == prj.ProjectGuid);
                    if (roleName != RoleTypes.System_Admin.ToString().Replace("_", " "))
                    {
                        var stf = prj.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == loggedInUserId);
                        if (stf != null && allowedRole.Contains(stf.Role))
                        {
                            var grpName = pr.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.ProSType.ToString());
                            int projGroup = grpName != null ? !string.IsNullOrEmpty(grpName.SelectedValues) ? Convert.ToInt32(grpName.SelectedValues) : 0 : 0;
                            projGroup = projGroup == 0 ? (int)ProjectSubTypeEnum.Other : projGroup;
                            returnModelList.Add(new LinkedProjectGroupViewModel()
                            {
                                GroupName = Enum.GetName(typeof(ProjectSubTypeEnum), projGroup).Replace("_", " "),
                                ProjectId = prj.ProjectGuid.ToString(),
                                ProjectName = prj.ProjectName,
                            });
                        }
                    }
                    else
                    {
                        var grpName = pr.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.ProSType.ToString());
                        int projGroup = grpName != null ? !string.IsNullOrEmpty(grpName.SelectedValues) ? Convert.ToInt32(grpName.SelectedValues) : 0 : 0;
                        projGroup = projGroup == 0 ? (int)ProjectSubTypeEnum.Other : projGroup;
                        returnModelList.Add(new LinkedProjectGroupViewModel()
                        {
                            GroupName = Enum.GetName(typeof(ProjectSubTypeEnum), projGroup).Replace("_", " "),
                            ProjectId = prj.ProjectGuid.ToString(),
                            ProjectName = prj.ProjectName,
                        });
                    }
                }
                #endregion
            }
            return returnModelList.OrderBy(x => x.GroupName).ThenBy(y => y.ProjectName).ToList();// new Tuple<List<string>, List<string>, List<string>>(valueList, descriptionList, groupName);
        }
        public IEnumerable<LookupVariablesPreviewViewModel> GetLookupVariablesPreview(VariablesMongo variablesMongo, string entityType, string entitySubType, IQueryable<FormDataEntryMongo> allEntities)
        {
            string entTypeName = entityType;
            if (entTypeName == "Person") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration); }
            else if (entTypeName == "Project") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration); }
            else if (entTypeName == "Participant") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration); }
            else if (entTypeName == "Place/Group") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration); }
            else { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration); }
            List<LookupVariablesPreviewViewModel> personEntitiesList = new List<LookupVariablesPreviewViewModel>();
            var entities = allEntities.Where(x => x.ActivityName == entTypeName).AsQueryable(); ;// _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().Where(x => x.ActivityName == entTypeName).AsQueryable();
            IQueryable<FormDataEntryMongo> person = null;
            IQueryable<FormDataEntryMongo> participant = null;
            IQueryable<FormDataEntryMongo> place__Group = null;
            IQueryable<FormDataEntryMongo> project = null;
            if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                person = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Person).OrderByDescending(x => x.CreatedDate).AsQueryable();
            if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                participant = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Participant).OrderByDescending(x => x.CreatedDate).AsQueryable();
            if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                place__Group = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Place__Group).OrderByDescending(x => x.CreatedDate).AsQueryable();
            if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                project = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Project).OrderByDescending(x => x.CreatedDate).AsQueryable();
            var entitySubtypes = _dbContext.EntitySubTypes.AsQueryable();
            var entityTypes = _dbContext.EntityTypes.AsQueryable();
            string Medical_Practitioner__Allied_Healt1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Medical_Practitioner__Allied_Healt);
            string Non_Medical__Practitioner1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Non_Medical__Practitioner);
            string Public_Overnight_Admissions1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Public_Overnight_Admissions);
            string Public_Day_Admissions_Only1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Public_Day_Admissions_Only);
            string Private_Overnight_Admissions1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Private_Overnight_Admissions);
            string Private_Day_Admissions_Only1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Private_Day_Admissions_Only);
            string Specialist_Clinic1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Specialist_Clinic);
            string General_Practice1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.General_Practice);
            string Allied_Health_Clinic1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Allied_Health_Clinic);
            string General_Laboratory1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.General_Laboratory);
            string Genetics_Laboratory1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Genetics_Laboratory);
            string Registry1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Registry);
            string Clinical_Trial1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Clinical_Trial);
            string Cohort_Study1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Cohort_Study);
            string Other1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Other);
            string State_Health_Network1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.State_Health_Network);
            string National_Health_Network1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.National_Health_Network);
            string Regulatory_Body_TGA1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Regulatory_Body_TGA);
            string Industry_Peak_Body1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Industry_Peak_Body);
            string Device_Manufacturer1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Device_Manufacturer);
            string Clinical_Craft_Group_Society1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Clinical_Craft_Group_Society);

            EntitySubType Medical_Practitioner__Allied_Healt = entitySubtypes.FirstOrDefault(x => x.Name == Medical_Practitioner__Allied_Healt1);
            EntitySubType Non_Medical__Practitioner = entitySubtypes.FirstOrDefault(x => x.Name == Non_Medical__Practitioner1);
            EntitySubType Public_Overnight_Admissions = entitySubtypes.FirstOrDefault(x => x.Name == Public_Overnight_Admissions1);
            EntitySubType Public_Day_Admissions_Only = entitySubtypes.FirstOrDefault(x => x.Name == Public_Day_Admissions_Only1);
            EntitySubType Private_Overnight_Admissions = entitySubtypes.FirstOrDefault(x => x.Name == Private_Overnight_Admissions1);
            EntitySubType Private_Day_Admissions_Only = entitySubtypes.FirstOrDefault(x => x.Name == Private_Day_Admissions_Only1);
            EntitySubType Specialist_Clinic = entitySubtypes.FirstOrDefault(x => x.Name == Specialist_Clinic1);
            EntitySubType General_Practice = entitySubtypes.FirstOrDefault(x => x.Name == General_Practice1);
            EntitySubType Allied_Health_Clinic = entitySubtypes.FirstOrDefault(x => x.Name == Allied_Health_Clinic1);
            EntitySubType General_Laboratory = entitySubtypes.FirstOrDefault(x => x.Name == General_Laboratory1);
            EntitySubType Genetics_Laboratory = entitySubtypes.FirstOrDefault(x => x.Name == Genetics_Laboratory1);
            EntitySubType Registry = entitySubtypes.FirstOrDefault(x => x.Name == Registry1);
            EntitySubType Clinical_Trial = entitySubtypes.FirstOrDefault(x => x.Name == Clinical_Trial1);
            EntitySubType Cohort_Study = entitySubtypes.FirstOrDefault(x => x.Name == Cohort_Study1);
            EntitySubType Other = entitySubtypes.FirstOrDefault(x => x.Name == Other1);
            EntitySubType State_Health_Network = entitySubtypes.FirstOrDefault(x => x.Name == State_Health_Network1);
            EntitySubType National_Health_Network = entitySubtypes.FirstOrDefault(x => x.Name == National_Health_Network1);
            EntitySubType Regulatory_Body_TGA = entitySubtypes.FirstOrDefault(x => x.Name == Regulatory_Body_TGA1);
            EntitySubType Industry_Peak_Body = entitySubtypes.FirstOrDefault(x => x.Name == Industry_Peak_Body1);
            EntitySubType Device_Manufacturer = entitySubtypes.FirstOrDefault(x => x.Name == Device_Manufacturer1);
            EntitySubType Clinical_Craft_Group_Society = entitySubtypes.FirstOrDefault(x => x.Name == Clinical_Craft_Group_Society1);
            int id = 0;

            List<long?> entitiesMongo = new List<long?>();
            if (participant != null && participant.Count() > 0)
            {
                participant.ToList().ForEach(parti =>
  {
      id++;
      FormDataEntryVariableMongo FirstName = parti.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.FirstName.ToString());
      FormDataEntryVariableMongo LastName = parti.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
      string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
      string ln = LastName != null ? LastName.SelectedValues : string.Empty;
      personEntitiesList.Add(new LookupVariablesPreviewViewModel
      {
          Id = id,
          EntityName = fn + " " + ln,
          EntitySubtypeId = Guid.Empty,
          EntityTypeId = parti != null ? parti.EntityTypeGuid : Guid.Empty,
      });

      entitiesMongo.Add(parti.EntityNumber);
  });
            }
            if (person != null && person.Count() > 0)
            {
                person.ToList().ForEach(per =>
                {
                    id++;
                    FormDataEntryVariableMongo FirstName = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.FirstName.ToString());
                    FormDataEntryVariableMongo LastName = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                    FormDataEntryVariableMongo PerSType = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PerSType.ToString());
                    string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
                    string ln = LastName != null ? LastName.SelectedValues : string.Empty;
                    string ps = PerSType != null ? PerSType.SelectedValues : string.Empty;
                    Guid entityTypeId = Guid.Empty;
                    var entityTypeId1 = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Person);
                    Guid entitySubtypeId = Guid.Empty;
                    switch (ps)
                    {
                        case "1":
                            entityTypeId = Medical_Practitioner__Allied_Healt != null ? Medical_Practitioner__Allied_Healt.EntityType.Guid : Guid.Empty;
                            entitySubtypeId = Medical_Practitioner__Allied_Healt != null ? Medical_Practitioner__Allied_Healt.Guid : Guid.Empty;
                            break;
                        case "2":
                            entityTypeId = Non_Medical__Practitioner != null ? Non_Medical__Practitioner.EntityType.Guid : Guid.Empty;
                            entitySubtypeId = Non_Medical__Practitioner != null ? Non_Medical__Practitioner.Guid : Guid.Empty;
                            break;
                        default:
                            break;
                    }
                    entityTypeId = entityTypeId1.Guid;
                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                    {
                        Id = id,
                        EntityName = fn + " " + ln,
                        EntitySubtypeId = entitySubtypeId,
                        EntityTypeId = entityTypeId,
                    });
                });
            }
            if (place__Group != null && place__Group.Count() > 0)
            {
                if (entityType == "Place/Group")
                {
                    EntityType placeentity = _dbContext.EntityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Place__Group);
                    place__Group.ToList().ForEach(plc =>
                    {
                        id++;
                        FormDataEntryVariableMongo FirstName = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                        FormDataEntryVariableMongo EntType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.EntType.ToString());

                        string plName = FirstName != null ? FirstName.SelectedValues : string.Empty;
                        string enttype = EntType != null ? EntType.SelectedValues : string.Empty;

                        Guid entityTypeId = (placeentity != null ? placeentity.Guid : Guid.Empty);
                        Guid entitySubtypeId = Guid.Empty;

                        personEntitiesList.Add(new LookupVariablesPreviewViewModel
                        {
                            Id = id,
                            EntityName = plName,
                            EntitySubtypeId = entitySubtypeId,
                            EntityTypeId = entityTypeId,
                        });

                        entitiesMongo.Add(plc.EntityNumber);
                    });
                }
                else
                {
                    EntityType placeentity = _dbContext.EntityTypes.FirstOrDefault(x => x.Name == entityType);
                    int etype = placeentity != null ? placeentity.Id : 0;
                    place__Group.ToList().ForEach(plc =>
                    {
                        id++;
                        FormDataEntryVariableMongo FirstName = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                        FormDataEntryVariableMongo EntType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.EntType.ToString());

                        string plName = FirstName != null ? FirstName.SelectedValues : string.Empty;
                        string enttype = EntType != null ? EntType.SelectedValues : string.Empty;

                        string enttypeFrm = GetDBEntTypeFromSelecedEntType(enttype);

                        if (plc.EntityNumber == 8477)
                        {

                        }
                        Guid entityTypeId = Guid.Empty;
                        Guid entitySubtypeId = Guid.Empty;

                        int entTypeDB = !string.IsNullOrEmpty(enttypeFrm) ? Convert.ToInt32(enttypeFrm) : 0;
                        int entTypeDBFRM = !string.IsNullOrEmpty(enttypeFrm) ? Convert.ToInt32(enttypeFrm) : 0;

                        if (entTypeDB == etype)
                        {
                            #region Place-Group
                            switch (entTypeDBFRM)
                            {
                                case (int)EntityTypesListInDB.Hospital:
                                    #region Hospital
                                    var hospital = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Hospital);
                                    entityTypeId = hospital != null ? hospital.Guid : Guid.Empty;

                                    FormDataEntryVariableMongo HospSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.HospSType.ToString());
                                    var hospSType = HospSType != null ? HospSType.SelectedValues : string.Empty;
                                    if (hospSType == "1")
                                    {
                                        entitySubtypeId = Public_Overnight_Admissions.Guid;
                                    }
                                    else if (hospSType == "2")
                                    {
                                        entitySubtypeId = Public_Day_Admissions_Only.Guid;
                                    }
                                    else if (hospSType == "3")
                                    {
                                        entitySubtypeId = Private_Overnight_Admissions.Guid;
                                    }
                                    else if (hospSType == "4")
                                    {
                                        entitySubtypeId = Private_Day_Admissions_Only.Guid;
                                    }
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Practice__Clinic:
                                    #region Practice/Clinic
                                    var practiceClinic = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Practice__Clinic);
                                    entityTypeId = practiceClinic != null ? practiceClinic.Guid : Guid.Empty;

                                    FormDataEntryVariableMongo pracSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PracSType.ToString());
                                    var PracSType = pracSType != null ? pracSType.SelectedValues : string.Empty;
                                    if (PracSType == "1")
                                    {
                                        entitySubtypeId = Specialist_Clinic.Guid;
                                    }
                                    else if (PracSType == "2")
                                    {
                                        entitySubtypeId = General_Practice.Guid;
                                    }
                                    else if (PracSType == "3")
                                    {
                                        entitySubtypeId = Allied_Health_Clinic.Guid;
                                    }
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Laboratory:
                                    #region Laboratory
                                    var laboratory = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Laboratory);
                                    entityTypeId = laboratory != null ? laboratory.Guid : Guid.Empty;
                                    FormDataEntryVariableMongo labSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.LabSType.ToString());
                                    var LabSType = labSType != null ? labSType.SelectedValues : string.Empty;
                                    if (LabSType == "1")
                                    {
                                        entitySubtypeId = General_Laboratory.Guid;
                                    }
                                    else if (LabSType == "2")
                                    {
                                        entitySubtypeId = Genetics_Laboratory.Guid;
                                    }
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Medical_Imaging:
                                    #region Medical imaging
                                    var medical_Imaging = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Medical_Imaging);
                                    entityTypeId = medical_Imaging != null ? medical_Imaging.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Research_facility__University:
                                    #region Research facility/University
                                    var research_facility__University = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Research_facility__University);
                                    entityTypeId = research_facility__University != null ? research_facility__University.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Healthcare_Group:
                                    #region Healthcare Group
                                    var healthcare_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Healthcare_Group);
                                    entityTypeId = healthcare_Group != null ? healthcare_Group.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Government_Organisation:
                                    #region Government Organisation
                                    var government_Organisation = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Government_Organisation);
                                    entityTypeId = government_Organisation != null ? government_Organisation.Guid : Guid.Empty;
                                    FormDataEntryVariableMongo govSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.GovSType.ToString());
                                    var GovSType = govSType != null ? govSType.SelectedValues : string.Empty;
                                    if (GovSType == "1")
                                    {
                                        entitySubtypeId = State_Health_Network.Guid;
                                    }
                                    else if (GovSType == "2")
                                    {
                                        entitySubtypeId = National_Health_Network.Guid;
                                    }
                                    else if (GovSType == "3")
                                    {
                                        entitySubtypeId = Regulatory_Body_TGA.Guid;
                                    }
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Industry_Group:
                                    #region Industry Group
                                    var industry_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Industry_Group);
                                    entityTypeId = industry_Group != null ? industry_Group.Guid : Guid.Empty;

                                    FormDataEntryVariableMongo indSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.IndSType.ToString());
                                    var IndSType = indSType != null ? indSType.SelectedValues : string.Empty;
                                    if (IndSType == "1")
                                    {
                                        entitySubtypeId = Industry_Peak_Body.Guid;
                                    }
                                    else if (IndSType == "2")
                                    {
                                        entitySubtypeId = Device_Manufacturer.Guid;
                                    }
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Consumer_Group:
                                    #region Consumer Group
                                    var consumer_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Consumer_Group);
                                    entityTypeId = consumer_Group != null ? consumer_Group.Guid : Guid.Empty;
                                    FormDataEntryVariableMongo conSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ConSType.ToString());
                                    var ConSType = conSType != null ? conSType.SelectedValues : string.Empty;
                                    if (ConSType == "1")
                                    {
                                        entitySubtypeId = Clinical_Craft_Group_Society.Guid;
                                    }
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Activity_Venue:
                                    #region Activity Venue
                                    var activity_Venue = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Activity_Venue);
                                    entityTypeId = activity_Venue != null ? activity_Venue.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Vehicle:
                                    #region Vehicle
                                    var vehicle = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Vehicle);
                                    entityTypeId = vehicle != null ? vehicle.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.MAC:
                                    #region MAC
                                    var mAC = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.MAC);
                                    entityTypeId = mAC != null ? mAC.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Ethics_Committee:
                                    #region Ethics Committee
                                    var ethics_Committee = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Ethics_Committee);
                                    entityTypeId = ethics_Committee != null ? ethics_Committee.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                default:
                                    break;
                            }
                            #endregion
                        }
                    });
                }
            }
            if (project != null && project.Count() > 0)
            {
                project.ToList().ForEach(proj =>
{
    id++;
    FormDataEntryVariableMongo Name = proj.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());

    string fn = Name != null ? Name.SelectedValues : string.Empty;

    Guid entityTypeId = entityTypes.Where(x => x.Id == (int)EntityTypesListInDB.Project).Select(x => x.Guid).FirstOrDefault();
    Guid entitySubtypeId = Guid.Empty;

    FormDataEntryVariableMongo proSType = proj.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ProSType.ToString());
    string pr = proSType != null ? proSType.SelectedValues : string.Empty;
    switch (pr)
    {
        case "1":
            entitySubtypeId = Registry != null ? Registry.Guid : Guid.Empty;
            break;
        case "2":
            entitySubtypeId = Clinical_Trial != null ? Clinical_Trial.Guid : Guid.Empty;
            break;

        case "3":
            entitySubtypeId = Cohort_Study != null ? Cohort_Study.Guid : Guid.Empty;
            break;
        case "4":
            entitySubtypeId = Other != null ? Other.Guid : Guid.Empty;
            break;
        default:
            break;
    }
    personEntitiesList.Add(new LookupVariablesPreviewViewModel
    {
        Id = id,
        EntityName = fn,
        EntitySubtypeId = entitySubtypeId,
        EntityTypeId = entityTypeId,
    });
    entitiesMongo.Add(proj.EntityNumber);
});
            }
            return personEntitiesList.OrderBy(x => x.EntityName);
        }
        public IEnumerable<LookupVariablesPreviewViewModel> GetLookupVariablesPreviewSQL(VariablesMongo variablesMongo, string entityType, string entitySubType, List<long?> mongoEntities)
        {
            #region  SQL
            string entTypeName = entityType;
            if (entTypeName == "Person") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration); }
            else if (entTypeName == "Project") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration); }
            else if (entTypeName == "Participant") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration); }
            else if (entTypeName == "Place/Group") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration); }
            else { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration); }

            EntityType entityTypeDB = _dbContext.EntityTypes.FirstOrDefault(x => x.Name == entityType);
            int entityTypeIdDB = entityTypeDB != null ? entityTypeDB.Id : 0;

            var entitySubtypes = _dbContext.EntitySubTypes.AsQueryable();
            var entityTypes = _dbContext.EntityTypes.AsQueryable();


            List<FormDataEntry> formDataEntryEntityTypesList = new List<FormDataEntry>();

            List<LookupVariablesPreviewViewModel> returnModel = new List<LookupVariablesPreviewViewModel>();
            if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                || entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
                || entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration)
                )
            {
                formDataEntryEntityTypesList = _dbContext.FormDataEntries.Where(t => t.EntityId == entityTypeIdDB && !mongoEntities.Contains(t.EntityNumber)).ToList();

                if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                {
                    formDataEntryEntityTypesList.ToList().ForEach(per =>
                    {

                        FormDataEntryVariable FirstName = per.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.FirstName.ToString());
                        FormDataEntryVariable LastName = per.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.Name.ToString());
                        FormDataEntryVariable PerSType = per.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.PerSType.ToString());

                        string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
                        string ln = LastName != null ? LastName.SelectedValues : string.Empty;
                        string ps = PerSType != null ? PerSType.SelectedValues : string.Empty;

                        Guid entityTypeId = Guid.Empty;

                        var entityTypeId1 = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Person);

                        Guid entitySubtypeId = Guid.Empty;

                        switch (ps)
                        {
                            case "1":
                                entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Medical_Practitioner__Allied_Healt).Select(x => x.Guid).FirstOrDefault();
                                break;
                            case "2":
                                entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Non_Medical__Practitioner).Select(x => x.Guid).FirstOrDefault();
                                break;
                            default:
                                break;
                        }

                        entityTypeId = entityTypeId1.Guid;


                        returnModel.Add(new LookupVariablesPreviewViewModel
                        {
                            EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                            EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                            EntityName = fn + " " + ln,
                            EntitySubtypeId = entitySubtypeId,
                            EntityTypeId = entityTypeId,
                        });

                    });
                }
                else if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                {
                    formDataEntryEntityTypesList.ToList().ForEach(per =>
                    {

                        FormDataEntryVariable FirstName = per.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.FirstName.ToString());
                        FormDataEntryVariable LastName = per.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.Name.ToString());

                        string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
                        string ln = LastName != null ? LastName.SelectedValues : string.Empty;

                        returnModel.Add(new LookupVariablesPreviewViewModel
                        {
                            EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                            EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                            EntityName = fn + " " + ln,
                            EntitySubtypeId = Guid.Empty,
                            EntityTypeId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                        });

                    });
                }
                else if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                {
                    formDataEntryEntityTypesList.ToList().ForEach(per =>
                    {
                        FormDataEntryVariable FirstName = per.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.FirstName.ToString());
                        FormDataEntryVariable proSType = per.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.ProSType.ToString());
                        string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;

                        Guid entityTypeId = entityTypes.Where(x => x.Id == (int)EntityTypesListInDB.Project).Select(x => x.Guid).FirstOrDefault();
                        Guid entitySubtypeId = Guid.Empty;

                        string pr = proSType != null ? proSType.SelectedValues : string.Empty;
                        switch (pr)
                        {
                            case "1":
                                entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Registry).Select(x => x.Guid).FirstOrDefault();
                                break;
                            case "2":
                                entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Clinical_Trial).Select(x => x.Guid).FirstOrDefault();
                                break;
                            case "3":
                                entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Cohort_Study).Select(x => x.Guid).FirstOrDefault();
                                break;
                            default:
                                break;
                        }
                        returnModel.Add(new LookupVariablesPreviewViewModel
                        {
                            EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                            EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                            EntityName = fn,
                            EntitySubtypeId = Guid.Empty,
                            EntityTypeId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                        });
                    });
                }
            }
            else if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
            {
                int[] notAllowedEntity = new int[]
                {
                    (int)Core.Enum.EntityTypesListInDB.Person
                    ,(int)Core.Enum.EntityTypesListInDB.Participant
                    ,(int)Core.Enum.EntityTypesListInDB.Project
                };

                formDataEntryEntityTypesList = _dbContext.FormDataEntries.Where(t => !notAllowedEntity.Contains(t.EntityId) && !mongoEntities.Contains(t.EntityNumber)).ToList();
                formDataEntryEntityTypesList.ForEach(plc =>
                {
                    var text = plc.FormDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.Name.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                    var enttype = plc.FormDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.EntType.ToString()).Select(x => x.SelectedValues).FirstOrDefault();

                    Guid entityTypeId = Guid.Empty;
                    Guid entitySubtypeId = Guid.Empty;

                    int entTypeDB = !string.IsNullOrEmpty(enttype) ? Convert.ToInt32(enttype) : 0;
                    if (entTypeDB == entityTypeIdDB)
                    {
                        #region Place-Group
                        switch (entityTypeIdDB)
                        {
                            case (int)EntityTypesListInDB.Hospital:
                                #region Hospital
                                var hospital = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Hospital);
                                entityTypeId = hospital != null ? hospital.Guid : Guid.Empty;
                                FormDataEntryVariable HospSType = plc.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.HospSType.ToString());
                                var hospSType = HospSType != null ? HospSType.SelectedValues : string.Empty;
                                if (hospSType == "1")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Public_Overnight_Admissions).Select(x => x.Guid).FirstOrDefault();
                                }
                                else if (hospSType == "2")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Public_Day_Admissions_Only).Select(x => x.Guid).FirstOrDefault();
                                }
                                else if (hospSType == "3")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Private_Overnight_Admissions).Select(x => x.Guid).FirstOrDefault();
                                }
                                else if (hospSType == "4")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Private_Day_Admissions_Only).Select(x => x.Guid).FirstOrDefault();
                                }
                                returnModel.Add(new LookupVariablesPreviewViewModel
                                {
                                    EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                                    EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                                    EntityName = text,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDB.Practice__Clinic:
                                #region Practice/Clinic
                                var practiceClinic = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Practice__Clinic);
                                entityTypeId = practiceClinic != null ? practiceClinic.Guid : Guid.Empty;

                                FormDataEntryVariable pracSType = plc.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.PracSType.ToString());
                                var PracSType = pracSType != null ? pracSType.SelectedValues : string.Empty;
                                if (PracSType == "1")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Specialist_Clinic).Select(x => x.Guid).FirstOrDefault();
                                }
                                else if (PracSType == "2")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.General_Practice).Select(x => x.Guid).FirstOrDefault();
                                }
                                else if (PracSType == "3")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Allied_Health_Clinic).Select(x => x.Guid).FirstOrDefault();
                                }

                                returnModel.Add(new LookupVariablesPreviewViewModel
                                {
                                    EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                                    EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                                    EntityName = text,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDB.Laboratory:
                                #region Laboratory
                                var laboratory = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Laboratory);
                                entityTypeId = laboratory != null ? laboratory.Guid : Guid.Empty;

                                FormDataEntryVariable labSType = plc.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.LabSType.ToString());
                                var LabSType = labSType != null ? labSType.SelectedValues : string.Empty;
                                if (LabSType == "1")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.General_Laboratory).Select(x => x.Guid).FirstOrDefault();
                                }
                                else if (LabSType == "2")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Genetics_Laboratory).Select(x => x.Guid).FirstOrDefault();
                                }

                                returnModel.Add(new LookupVariablesPreviewViewModel
                                {
                                    EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                                    EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                                    EntityName = text,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDB.Medical_Imaging:
                                #region Medical imaging
                                var medical_Imaging = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Medical_Imaging);
                                entityTypeId = medical_Imaging != null ? medical_Imaging.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;

                                returnModel.Add(new LookupVariablesPreviewViewModel
                                {
                                    EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                                    EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                                    EntityName = text,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDB.Research_facility__University:
                                #region Research facility/University
                                var research_facility__University = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Research_facility__University);
                                entityTypeId = research_facility__University != null ? research_facility__University.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;

                                returnModel.Add(new LookupVariablesPreviewViewModel
                                {
                                    EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                                    EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                                    EntityName = text,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDB.Healthcare_Group:
                                #region Healthcare Group
                                var healthcare_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Healthcare_Group);
                                entityTypeId = healthcare_Group != null ? healthcare_Group.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;
                                returnModel.Add(new LookupVariablesPreviewViewModel
                                {
                                    EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                                    EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                                    EntityName = text,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDB.Government_Organisation:
                                #region Government Organisation
                                var government_Organisation = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Government_Organisation);
                                entityTypeId = government_Organisation != null ? government_Organisation.Guid : Guid.Empty;
                                FormDataEntryVariable govSType = plc.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.GovSType.ToString());
                                var GovSType = govSType != null ? govSType.SelectedValues : string.Empty;
                                if (GovSType == "1")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.State_Health_Network).Select(x => x.Guid).FirstOrDefault();
                                }
                                else if (GovSType == "2")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.National_Health_Network).Select(x => x.Guid).FirstOrDefault();
                                }
                                else if (GovSType == "3")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Regulatory_Body_TGA).Select(x => x.Guid).FirstOrDefault();
                                }
                                returnModel.Add(new LookupVariablesPreviewViewModel
                                {
                                    EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                                    EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                                    EntityName = text,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDB.Industry_Group:
                                #region Industry Group
                                var industry_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Industry_Group);
                                entityTypeId = industry_Group != null ? industry_Group.Guid : Guid.Empty;

                                FormDataEntryVariable indSType = plc.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.IndSType.ToString());
                                var IndSType = indSType != null ? indSType.SelectedValues : string.Empty;
                                if (IndSType == "1")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Industry_Peak_Body).Select(x => x.Guid).FirstOrDefault();
                                }
                                else if (IndSType == "2")
                                {
                                    entitySubtypeId = entitySubtypes.Where(x => x.Id == (int)EntitySubTypesListInDB.Device_Manufacturer).Select(x => x.Guid).FirstOrDefault();
                                }
                                returnModel.Add(new LookupVariablesPreviewViewModel
                                {
                                    EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                                    EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                                    EntityName = text,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDB.Consumer_Group:
                                #region Consumer Group
                                var consumer_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Consumer_Group);
                                entityTypeId = consumer_Group != null ? consumer_Group.Guid : Guid.Empty;
                                returnModel.Add(new LookupVariablesPreviewViewModel
                                {
                                    EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                                    EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                                    EntityName = text,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDB.Activity_Venue:
                                #region Activity Venue
                                var activity_Venue = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Activity_Venue);
                                entityTypeId = activity_Venue != null ? activity_Venue.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;
                                returnModel.Add(new LookupVariablesPreviewViewModel
                                {
                                    EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                                    EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                                    EntityName = text,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDB.Vehicle:
                                #region Vehicle
                                var vehicle = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Vehicle);
                                entityTypeId = vehicle != null ? vehicle.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;
                                returnModel.Add(new LookupVariablesPreviewViewModel
                                {
                                    EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                                    EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                                    EntityName = text,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDB.MAC:
                                #region MAC
                                var mAC = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.MAC);
                                entityTypeId = mAC != null ? mAC.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;
                                returnModel.Add(new LookupVariablesPreviewViewModel
                                {
                                    EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                                    EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                                    EntityName = text,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDB.Ethics_Committee:
                                #region Ethics Committee
                                var ethics_Committee = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Ethics_Committee);
                                entityTypeId = ethics_Committee != null ? ethics_Committee.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;
                                returnModel.Add(new LookupVariablesPreviewViewModel
                                {
                                    EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                                    EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                                    EntityName = text,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            default:
                                break;
                        }
                        #endregion
                    }
                    else
                    {
                        returnModel.Add(new LookupVariablesPreviewViewModel
                        {
                            EntityGroupId = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty,
                            EntityGroupName = entityTypeDB != null ? entityTypeDB.Name : string.Empty,
                            EntityName = text,
                            EntitySubtypeId = entitySubtypeId,
                            EntityTypeId = entityTypeId,
                        });
                    }
                });
            }
            return returnModel;
            #endregion
        }

        public IEnumerable<LookupVariablesPreviewViewModel> TestEnvironment_GetLookupVariablesPreview1(string entityType, string entitySubType)
        {
            string entTypeName = entityType;
            if (entTypeName == "Person") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration); }
            else if (entTypeName == "Project") { EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration); }
            else if (entTypeName == "Participant") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration); }
            else if (entTypeName == "Place/Group") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration); }
            else { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration); }
            List<LookupVariablesPreviewViewModel> personEntitiesList = new List<LookupVariablesPreviewViewModel>();

            var entities = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().Where(x => x.ActivityName == entTypeName).AsQueryable();

            var person = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Person).OrderByDescending(x => x.CreatedDate).AsQueryable();
            var participant = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Participant).OrderByDescending(x => x.CreatedDate).AsQueryable();
            var place__Group = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Place__Group).OrderByDescending(x => x.CreatedDate).AsQueryable();
            var project = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Project).OrderByDescending(x => x.CreatedDate).AsQueryable();

            var entitySubtypes = _dbContext.EntitySubTypes.AsQueryable();
            var entityTypes = _dbContext.EntityTypes.AsQueryable();

            EntitySubType Medical_Practitioner__Allied_Healt = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Medical_Practitioner__Allied_Healt);
            EntitySubType Non_Medical__Practitioner = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Non_Medical__Practitioner);
            EntitySubType Public_Overnight_Admissions = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Public_Overnight_Admissions);
            EntitySubType Public_Day_Admissions_Only = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Public_Day_Admissions_Only);
            EntitySubType Private_Overnight_Admissions = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Private_Overnight_Admissions);
            EntitySubType Private_Day_Admissions_Only = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Private_Day_Admissions_Only);
            EntitySubType Specialist_Clinic = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Specialist_Clinic);
            EntitySubType General_Practice = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.General_Practice);
            EntitySubType Allied_Health_Clinic = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Allied_Health_Clinic);
            EntitySubType General_Laboratory = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.General_Laboratory);
            EntitySubType Genetics_Laboratory = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Genetics_Laboratory);
            EntitySubType Registry = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Registry);
            EntitySubType Clinical_Trial = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Clinical_Trial);
            EntitySubType Cohort_Study = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Cohort_Study);
            EntitySubType State_Health_Network = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.State_Health_Network);
            EntitySubType National_Health_Network = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.National_Health_Network);
            EntitySubType Regulatory_Body_TGA = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Regulatory_Body_TGA);
            EntitySubType Industry_Peak_Body = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Industry_Peak_Body);
            EntitySubType Device_Manufacturer = entitySubtypes.FirstOrDefault(x => x.Id == (int)EntitySubTypesListInDB.Device_Manufacturer);
            string clSubtype = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Clinical_Craft_Group_Society);
            EntitySubType Clinical_Craft_Group_Society = entitySubtypes.FirstOrDefault(x => x.Name == clSubtype);
            int id = 0;

            participant.ToList().ForEach(parti =>
            {
                id++;
                FormDataEntryVariableMongo FirstName = parti.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.FirstName.ToString());
                FormDataEntryVariableMongo LastName = parti.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
                string ln = LastName != null ? LastName.SelectedValues : string.Empty;

                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                {
                    Id = id,
                    EntityName = fn + " " + ln,
                    EntitySubtypeId = Guid.Empty,
                    EntityTypeId = parti != null ? parti.EntityTypeGuid : Guid.Empty,
                });
            });

            person.ToList().ForEach(per =>
            {
                id++;
                FormDataEntryVariableMongo FirstName = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.FirstName.ToString());
                FormDataEntryVariableMongo LastName = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                FormDataEntryVariableMongo PerSType = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PerSType.ToString());

                string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
                string ln = LastName != null ? LastName.SelectedValues : string.Empty;
                string ps = PerSType != null ? PerSType.SelectedValues : string.Empty;

                Guid entityTypeId = Guid.Empty;

                var entityTypeId1 = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Person);

                Guid entitySubtypeId = Guid.Empty;

                switch (ps)
                {
                    case "1":
                        entityTypeId = Medical_Practitioner__Allied_Healt != null ? Medical_Practitioner__Allied_Healt.EntityType.Guid : Guid.Empty;
                        entitySubtypeId = Medical_Practitioner__Allied_Healt != null ? Medical_Practitioner__Allied_Healt.Guid : Guid.Empty;
                        break;
                    case "2":
                        entityTypeId = Non_Medical__Practitioner != null ? Non_Medical__Practitioner.EntityType.Guid : Guid.Empty;
                        entitySubtypeId = Non_Medical__Practitioner != null ? Non_Medical__Practitioner.Guid : Guid.Empty;
                        break;
                    default:
                        break;
                }

                entityTypeId = entityTypeId1.Guid;

                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                {
                    Id = id,
                    EntityName = fn + " " + ln,
                    EntitySubtypeId = entitySubtypeId,
                    EntityTypeId = entityTypeId,
                });
            });

            if (entityType == "Place/Group")
            {
                EntityType placeentity = _dbContext.EntityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Place__Group);

                place__Group.ToList().ForEach(plc =>
                {
                    id++;
                    FormDataEntryVariableMongo FirstName = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                    FormDataEntryVariableMongo EntType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.EntType.ToString());

                    string plName = FirstName != null ? FirstName.SelectedValues : string.Empty;
                    string enttype = EntType != null ? EntType.SelectedValues : string.Empty;

                    Guid entityTypeId = (placeentity != null ? placeentity.Guid : Guid.Empty);
                    Guid entitySubtypeId = Guid.Empty;

                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                    {
                        Id = id,
                        EntityName = plName,
                        EntitySubtypeId = entitySubtypeId,
                        EntityTypeId = entityTypeId,
                    });
                });
            }
            else
            {
                EntityType placeentity = _dbContext.EntityTypes.FirstOrDefault(x => x.Name == entityType);

                int etype = placeentity != null ? placeentity.Id : 0;

                place__Group.ToList().ForEach(plc =>
                {
                    id++;
                    FormDataEntryVariableMongo FirstName = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                    FormDataEntryVariableMongo EntType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.EntType.ToString());

                    string plName = FirstName != null ? FirstName.SelectedValues : string.Empty;
                    string enttype = EntType != null ? EntType.SelectedValues : string.Empty;

                    string enttypeFrm = GetDBEntTypeFromSelecedEntType(enttype);

                    Guid entityTypeId = Guid.Empty;
                    Guid entitySubtypeId = Guid.Empty;

                    int entTypeDBFrm = !string.IsNullOrEmpty(enttypeFrm) ? Convert.ToInt32(enttypeFrm) : 0;
                    int entTypeDB = !string.IsNullOrEmpty(enttypeFrm) ? Convert.ToInt32(enttypeFrm) : 0;

                    if (entTypeDB == etype)
                    {
                        #region Place-Group
                        switch (etype)
                        {
                            case (int)EntityTypesListInDBSummary.Hospital:
                                #region Hospital
                                var hospital = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Hospital);
                                entityTypeId = hospital != null ? hospital.Guid : Guid.Empty;

                                FormDataEntryVariableMongo HospSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.HospSType.ToString());
                                var hospSType = HospSType != null ? HospSType.SelectedValues : string.Empty;
                                if (hospSType == "1")
                                {
                                    entitySubtypeId = Public_Overnight_Admissions.Guid;
                                }
                                else if (hospSType == "2")
                                {
                                    entitySubtypeId = Public_Day_Admissions_Only.Guid;
                                }
                                else if (hospSType == "3")
                                {
                                    entitySubtypeId = Private_Overnight_Admissions.Guid;
                                }
                                else if (hospSType == "4")
                                {
                                    entitySubtypeId = Private_Day_Admissions_Only.Guid;
                                }

                                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                {
                                    Id = id,
                                    EntityName = plName,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDBSummary.Practice__Clinic:
                                #region Practice/Clinic
                                var practiceClinic = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Practice__Clinic);
                                entityTypeId = practiceClinic != null ? practiceClinic.Guid : Guid.Empty;

                                FormDataEntryVariableMongo pracSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PracSType.ToString());
                                var PracSType = pracSType != null ? pracSType.SelectedValues : string.Empty;
                                if (PracSType == "1")
                                {
                                    entitySubtypeId = Specialist_Clinic.Guid;
                                }
                                else if (PracSType == "2")
                                {
                                    entitySubtypeId = General_Practice.Guid;
                                }
                                else if (PracSType == "3")
                                {
                                    entitySubtypeId = Allied_Health_Clinic.Guid;
                                }

                                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                {
                                    Id = id,
                                    EntityName = plName,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDBSummary.Laboratory:
                                #region Laboratory
                                var laboratory = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Laboratory);
                                entityTypeId = laboratory != null ? laboratory.Guid : Guid.Empty;
                                FormDataEntryVariableMongo labSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.LabSType.ToString());
                                var LabSType = labSType != null ? labSType.SelectedValues : string.Empty;
                                if (LabSType == "1")
                                {
                                    entitySubtypeId = General_Laboratory.Guid;
                                }
                                else if (LabSType == "2")
                                {
                                    entitySubtypeId = Genetics_Laboratory.Guid;
                                }
                                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                {
                                    Id = id,
                                    EntityName = plName,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDBSummary.Medical_Imaging:
                                #region Medical imaging
                                var medical_Imaging = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Medical_Imaging);
                                entityTypeId = medical_Imaging != null ? medical_Imaging.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;

                                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                {
                                    Id = id,
                                    EntityName = plName,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDBSummary.Research_facility__University:
                                #region Research facility/University
                                var research_facility__University = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Research_facility__University);
                                entityTypeId = research_facility__University != null ? research_facility__University.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;

                                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                {
                                    Id = id,
                                    EntityName = plName,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDBSummary.Healthcare_Group:
                                #region Healthcare Group
                                var healthcare_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Healthcare_Group);
                                entityTypeId = healthcare_Group != null ? healthcare_Group.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;

                                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                {
                                    Id = id,
                                    EntityName = plName,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDBSummary.Government_Organisation:
                                #region Government Organisation
                                var government_Organisation = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Government_Organisation);
                                entityTypeId = government_Organisation != null ? government_Organisation.Guid : Guid.Empty;

                                FormDataEntryVariableMongo govSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.GovSType.ToString());
                                var GovSType = govSType != null ? govSType.SelectedValues : string.Empty;
                                if (GovSType == "1")
                                {
                                    entitySubtypeId = State_Health_Network.Guid;
                                }
                                else if (GovSType == "2")
                                {
                                    entitySubtypeId = National_Health_Network.Guid;
                                }
                                else if (GovSType == "3")
                                {
                                    entitySubtypeId = Regulatory_Body_TGA.Guid;
                                }
                                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                {
                                    Id = id,
                                    EntityName = plName,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDBSummary.Industry_Group:
                                #region Industry Group
                                var industry_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Industry_Group);
                                entityTypeId = industry_Group != null ? industry_Group.Guid : Guid.Empty;

                                FormDataEntryVariableMongo indSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.IndSType.ToString());
                                var IndSType = indSType != null ? indSType.SelectedValues : string.Empty;
                                if (IndSType == "1")
                                {
                                    entitySubtypeId = Industry_Peak_Body.Guid;
                                }
                                else if (IndSType == "2")
                                {
                                    entitySubtypeId = Device_Manufacturer.Guid;
                                }

                                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                {
                                    Id = id,
                                    EntityName = plName,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDBSummary.Consumer_Group:
                                #region Consumer Group
                                var consumer_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Consumer_Group);
                                entityTypeId = consumer_Group != null ? consumer_Group.Guid : Guid.Empty;

                                FormDataEntryVariableMongo conSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ConSType.ToString());
                                var ConSType = conSType != null ? conSType.SelectedValues : string.Empty;
                                if (ConSType == "1")
                                {
                                    entitySubtypeId = Clinical_Craft_Group_Society.Guid;
                                }

                                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                {
                                    Id = id,
                                    EntityName = plName,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDBSummary.Activity_Venue:
                                #region Activity Venue
                                var activity_Venue = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Activity_Venue);
                                entityTypeId = activity_Venue != null ? activity_Venue.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;
                                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                {
                                    Id = id,
                                    EntityName = plName,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDBSummary.Vehicle:
                                #region Vehicle
                                var vehicle = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Vehicle);
                                entityTypeId = vehicle != null ? vehicle.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;

                                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                {
                                    Id = id,
                                    EntityName = plName,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDBSummary.MAC:
                                #region MAC
                                var mAC = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.MAC);
                                entityTypeId = mAC != null ? mAC.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;

                                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                {
                                    Id = id,
                                    EntityName = plName,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            case (int)EntityTypesListInDBSummary.Ethics_Committee:
                                #region Ethics Committee
                                var ethics_Committee = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Ethics_Committee);
                                entityTypeId = ethics_Committee != null ? ethics_Committee.Guid : Guid.Empty;
                                entitySubtypeId = Guid.Empty;

                                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                {
                                    Id = id,
                                    EntityName = plName,
                                    EntitySubtypeId = entitySubtypeId,
                                    EntityTypeId = entityTypeId,
                                });
                                #endregion
                                break;
                            default:
                                break;
                        }
                        #endregion
                    }
                });
            }

            project.ToList().ForEach(proj =>
            {
                id++;
                FormDataEntryVariableMongo Name = proj.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());

                string fn = Name != null ? Name.SelectedValues : string.Empty;

                Guid entityTypeId = entityTypes.Where(x => x.Id == (int)EntityTypesListInDB.Project).Select(x => x.Guid).FirstOrDefault();
                Guid entitySubtypeId = Guid.Empty;

                FormDataEntryVariableMongo proSType = proj.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ProSType.ToString());
                string pr = proSType != null ? proSType.SelectedValues : string.Empty;
                switch (pr)
                {
                    case "1":
                        entitySubtypeId = Registry != null ? Registry.Guid : Guid.Empty;
                        break;
                    case "2":
                        entitySubtypeId = Clinical_Trial != null ? Clinical_Trial.Guid : Guid.Empty;
                        break;

                    case "3":
                        entitySubtypeId = Cohort_Study != null ? Cohort_Study.Guid : Guid.Empty;
                        break;
                    default:
                        break;
                }
                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                {
                    Id = id,
                    EntityName = fn,
                    EntitySubtypeId = entitySubtypeId,
                    EntityTypeId = entityTypeId,
                });
            });
            return personEntitiesList;
        }

        public IEnumerable<LookupVariablesPreviewViewModel> TestEnvironment_GetLookupVariablesPreview(VariablesMongo variablesMongo, string entityType, string entitySubType, IQueryable<FormDataEntryMongo> allEntities)
        {
            string entTypeName = entityType;
            if (entTypeName == "Person") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration); }
            else if (entTypeName == "Project") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration); }
            else if (entTypeName == "Participant") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration); }
            else if (entTypeName == "Place/Group") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration); }
            else { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration); }

            List<LookupVariablesPreviewViewModel> personEntitiesList = new List<LookupVariablesPreviewViewModel>();

            var entities = allEntities.Where(x => x.ActivityName == entTypeName).AsQueryable(); ;// _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().Where(x => x.ActivityName == entTypeName).AsQueryable();

            IQueryable<FormDataEntryMongo> person = null;
            IQueryable<FormDataEntryMongo> participant = null;
            IQueryable<FormDataEntryMongo> place__Group = null;
            IQueryable<FormDataEntryMongo> project = null;

            if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                person = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Person).OrderByDescending(x => x.CreatedDate).AsQueryable();
            if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                participant = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Participant).OrderByDescending(x => x.CreatedDate).AsQueryable();
            if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                place__Group = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Place__Group).OrderByDescending(x => x.CreatedDate).AsQueryable();
            if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                project = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Project).OrderByDescending(x => x.CreatedDate).AsQueryable();

            var entitySubtypes = _dbContext.EntitySubTypes.AsQueryable();
            var entityTypes = _dbContext.EntityTypes.AsQueryable();

            string Medical_Practitioner__Allied_Healt1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Medical_Practitioner__Allied_Healt);
            string Non_Medical__Practitioner1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Non_Medical__Practitioner);
            string Public_Overnight_Admissions1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Public_Overnight_Admissions);
            string Public_Day_Admissions_Only1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Public_Day_Admissions_Only);
            string Private_Overnight_Admissions1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Private_Overnight_Admissions);
            string Private_Day_Admissions_Only1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Private_Day_Admissions_Only);
            string Specialist_Clinic1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Specialist_Clinic);
            string General_Practice1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.General_Practice);
            string Allied_Health_Clinic1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Allied_Health_Clinic);
            string General_Laboratory1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.General_Laboratory);
            string Genetics_Laboratory1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Genetics_Laboratory);
            string Registry1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Registry);
            string Clinical_Trial1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Clinical_Trial);
            string Cohort_Study1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Cohort_Study);
            string Other1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Other);
            string State_Health_Network1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.State_Health_Network);
            string National_Health_Network1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.National_Health_Network);
            string Regulatory_Body_TGA1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Regulatory_Body_TGA);
            string Industry_Peak_Body1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Industry_Peak_Body);
            string Device_Manufacturer1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Device_Manufacturer);
            string Clinical_Craft_Group_Society1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Clinical_Craft_Group_Society);

            EntitySubType Medical_Practitioner__Allied_Healt = entitySubtypes.FirstOrDefault(x => x.Name == Medical_Practitioner__Allied_Healt1);
            EntitySubType Non_Medical__Practitioner = entitySubtypes.FirstOrDefault(x => x.Name == Non_Medical__Practitioner1);
            EntitySubType Public_Overnight_Admissions = entitySubtypes.FirstOrDefault(x => x.Name == Public_Overnight_Admissions1);
            EntitySubType Public_Day_Admissions_Only = entitySubtypes.FirstOrDefault(x => x.Name == Public_Day_Admissions_Only1);
            EntitySubType Private_Overnight_Admissions = entitySubtypes.FirstOrDefault(x => x.Name == Private_Overnight_Admissions1);
            EntitySubType Private_Day_Admissions_Only = entitySubtypes.FirstOrDefault(x => x.Name == Private_Day_Admissions_Only1);
            EntitySubType Specialist_Clinic = entitySubtypes.FirstOrDefault(x => x.Name == Specialist_Clinic1);
            EntitySubType General_Practice = entitySubtypes.FirstOrDefault(x => x.Name == General_Practice1);
            EntitySubType Allied_Health_Clinic = entitySubtypes.FirstOrDefault(x => x.Name == Allied_Health_Clinic1);
            EntitySubType General_Laboratory = entitySubtypes.FirstOrDefault(x => x.Name == General_Laboratory1);
            EntitySubType Genetics_Laboratory = entitySubtypes.FirstOrDefault(x => x.Name == Genetics_Laboratory1);
            EntitySubType Registry = entitySubtypes.FirstOrDefault(x => x.Name == Registry1);
            EntitySubType Clinical_Trial = entitySubtypes.FirstOrDefault(x => x.Name == Clinical_Trial1);
            EntitySubType Cohort_Study = entitySubtypes.FirstOrDefault(x => x.Name == Cohort_Study1);
            EntitySubType Other = entitySubtypes.FirstOrDefault(x => x.Name == Other1);
            EntitySubType State_Health_Network = entitySubtypes.FirstOrDefault(x => x.Name == State_Health_Network1);
            EntitySubType National_Health_Network = entitySubtypes.FirstOrDefault(x => x.Name == National_Health_Network1);
            EntitySubType Regulatory_Body_TGA = entitySubtypes.FirstOrDefault(x => x.Name == Regulatory_Body_TGA1);
            EntitySubType Industry_Peak_Body = entitySubtypes.FirstOrDefault(x => x.Name == Industry_Peak_Body1);
            EntitySubType Device_Manufacturer = entitySubtypes.FirstOrDefault(x => x.Name == Device_Manufacturer1);
            EntitySubType Clinical_Craft_Group_Society = entitySubtypes.FirstOrDefault(x => x.Name == Clinical_Craft_Group_Society1);
            int id = 0;

            List<long?> entitiesMongo = new List<long?>();
            if (participant != null && participant.Count() > 0)
            {
                participant.ToList().ForEach(parti =>
                {

                    id++;
                    FormDataEntryVariableMongo FirstName = parti.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.FirstName.ToString());
                    FormDataEntryVariableMongo LastName = parti.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());

                    string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
                    string ln = LastName != null ? LastName.SelectedValues : string.Empty;
                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                    {
                        Id = id,
                        EntityName = fn + " " + ln,
                        EntitySubtypeId = Guid.Empty,
                        EntityTypeId = parti != null ? parti.EntityTypeGuid : Guid.Empty,
                    });

                    entitiesMongo.Add(parti.EntityNumber);
                });

            }

            if (person != null && person.Count() > 0)
            {
                person.ToList().ForEach(per =>
                {
                    id++;
                    FormDataEntryVariableMongo FirstName = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.FirstName.ToString());
                    FormDataEntryVariableMongo LastName = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                    FormDataEntryVariableMongo PerSType = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PerSType.ToString());

                    string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
                    string ln = LastName != null ? LastName.SelectedValues : string.Empty;
                    string ps = PerSType != null ? PerSType.SelectedValues : string.Empty;

                    Guid entityTypeId = Guid.Empty;

                    var entityTypeId1 = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Person);

                    Guid entitySubtypeId = Guid.Empty;

                    switch (ps)
                    {
                        case "1":
                            entityTypeId = Medical_Practitioner__Allied_Healt != null ? Medical_Practitioner__Allied_Healt.EntityType.Guid : Guid.Empty;
                            entitySubtypeId = Medical_Practitioner__Allied_Healt != null ? Medical_Practitioner__Allied_Healt.Guid : Guid.Empty;
                            break;
                        case "2":
                            entityTypeId = Non_Medical__Practitioner != null ? Non_Medical__Practitioner.EntityType.Guid : Guid.Empty;
                            entitySubtypeId = Non_Medical__Practitioner != null ? Non_Medical__Practitioner.Guid : Guid.Empty;
                            break;
                        default:
                            break;
                    }
                    entityTypeId = entityTypeId1.Guid;
                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                    {
                        Id = id,
                        EntityName = fn + " " + ln,
                        EntitySubtypeId = entitySubtypeId,
                        EntityTypeId = entityTypeId,
                    });
                });
            }
            if (place__Group != null && place__Group.Count() > 0)
            {
                if (entityType == "Place/Group")
                {
                    EntityType placeentity = _dbContext.EntityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Place__Group);

                    place__Group.ToList().ForEach(plc =>
                    {
                        id++;
                        FormDataEntryVariableMongo FirstName = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                        FormDataEntryVariableMongo EntType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.EntType.ToString());

                        string plName = FirstName != null ? FirstName.SelectedValues : string.Empty;
                        string enttype = EntType != null ? EntType.SelectedValues : string.Empty;

                        Guid entityTypeId = (placeentity != null ? placeentity.Guid : Guid.Empty);
                        Guid entitySubtypeId = Guid.Empty;

                        personEntitiesList.Add(new LookupVariablesPreviewViewModel
                        {
                            Id = id,
                            EntityName = plName,
                            EntitySubtypeId = entitySubtypeId,
                            EntityTypeId = entityTypeId,
                        });

                        entitiesMongo.Add(plc.EntityNumber);
                    });
                }
                else
                {
                    EntityType placeentity = _dbContext.EntityTypes.FirstOrDefault(x => x.Name == entityType);

                    int etype = placeentity != null ? placeentity.Id : 0;

                    place__Group.ToList().ForEach(plc =>
                    {
                        id++;
                        FormDataEntryVariableMongo FirstName = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                        FormDataEntryVariableMongo EntType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.EntType.ToString());

                        string plName = FirstName != null ? FirstName.SelectedValues : string.Empty;
                        string enttype = EntType != null ? EntType.SelectedValues : string.Empty;

                        string enttypeFrm = GetDBEntTypeFromSelecedEntType(enttype);

                        if (plc.EntityNumber == 8477)
                        {

                        }
                        Guid entityTypeId = Guid.Empty;
                        Guid entitySubtypeId = Guid.Empty;

                        int entTypeDB = !string.IsNullOrEmpty(enttypeFrm) ? Convert.ToInt32(enttypeFrm) : 0;
                        int entTypeDBFRM = !string.IsNullOrEmpty(enttypeFrm) ? Convert.ToInt32(enttypeFrm) : 0;

                        if (entTypeDB == etype)
                        {
                            #region Place-Group
                            switch (entTypeDBFRM)
                            {
                                case (int)EntityTypesListInDB.Hospital:
                                    #region Hospital
                                    var hospital = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Hospital);
                                    entityTypeId = hospital != null ? hospital.Guid : Guid.Empty;
                                    FormDataEntryVariableMongo HospSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.HospSType.ToString());
                                    var hospSType = HospSType != null ? HospSType.SelectedValues : string.Empty;
                                    if (hospSType == "1")
                                    {
                                        entitySubtypeId = Public_Overnight_Admissions.Guid;
                                    }
                                    else if (hospSType == "2")
                                    {
                                        entitySubtypeId = Public_Day_Admissions_Only.Guid;
                                    }
                                    else if (hospSType == "3")
                                    {
                                        entitySubtypeId = Private_Overnight_Admissions.Guid;
                                    }
                                    else if (hospSType == "4")
                                    {
                                        entitySubtypeId = Private_Day_Admissions_Only.Guid;
                                    }

                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Practice__Clinic:
                                    #region Practice/Clinic
                                    var practiceClinic = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Practice__Clinic);
                                    entityTypeId = practiceClinic != null ? practiceClinic.Guid : Guid.Empty;

                                    FormDataEntryVariableMongo pracSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PracSType.ToString());
                                    var PracSType = pracSType != null ? pracSType.SelectedValues : string.Empty;
                                    if (PracSType == "1")
                                    {
                                        entitySubtypeId = Specialist_Clinic.Guid;
                                    }
                                    else if (PracSType == "2")
                                    {
                                        entitySubtypeId = General_Practice.Guid;
                                    }
                                    else if (PracSType == "3")
                                    {
                                        entitySubtypeId = Allied_Health_Clinic.Guid;
                                    }
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Laboratory:
                                    #region Laboratory
                                    var laboratory = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Laboratory);
                                    entityTypeId = laboratory != null ? laboratory.Guid : Guid.Empty;

                                    FormDataEntryVariableMongo labSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.LabSType.ToString());
                                    var LabSType = labSType != null ? labSType.SelectedValues : string.Empty;
                                    if (LabSType == "1")
                                    {
                                        entitySubtypeId = General_Laboratory.Guid;
                                    }
                                    else if (LabSType == "2")
                                    {
                                        entitySubtypeId = Genetics_Laboratory.Guid;
                                    }
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Medical_Imaging:
                                    #region Medical imaging
                                    var medical_Imaging = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Medical_Imaging);
                                    entityTypeId = medical_Imaging != null ? medical_Imaging.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;

                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Research_facility__University:
                                    #region Research facility/University
                                    var research_facility__University = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Research_facility__University);
                                    entityTypeId = research_facility__University != null ? research_facility__University.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;

                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Healthcare_Group:
                                    #region Healthcare Group
                                    var healthcare_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Healthcare_Group);
                                    entityTypeId = healthcare_Group != null ? healthcare_Group.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;

                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Government_Organisation:
                                    #region Government Organisation
                                    var government_Organisation = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Government_Organisation);
                                    entityTypeId = government_Organisation != null ? government_Organisation.Guid : Guid.Empty;

                                    FormDataEntryVariableMongo govSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.GovSType.ToString());
                                    var GovSType = govSType != null ? govSType.SelectedValues : string.Empty;
                                    if (GovSType == "1")
                                    {
                                        entitySubtypeId = State_Health_Network.Guid;
                                    }
                                    else if (GovSType == "2")
                                    {
                                        entitySubtypeId = National_Health_Network.Guid;
                                    }
                                    else if (GovSType == "3")
                                    {
                                        entitySubtypeId = Regulatory_Body_TGA.Guid;
                                    }
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Industry_Group:
                                    #region Industry Group
                                    var industry_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Industry_Group);
                                    entityTypeId = industry_Group != null ? industry_Group.Guid : Guid.Empty;

                                    FormDataEntryVariableMongo indSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.IndSType.ToString());
                                    var IndSType = indSType != null ? indSType.SelectedValues : string.Empty;
                                    if (IndSType == "1")
                                    {
                                        entitySubtypeId = Industry_Peak_Body.Guid;
                                    }
                                    else if (IndSType == "2")
                                    {
                                        entitySubtypeId = Device_Manufacturer.Guid;
                                    }
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Consumer_Group:
                                    #region Consumer Group
                                    var consumer_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Consumer_Group);
                                    entityTypeId = consumer_Group != null ? consumer_Group.Guid : Guid.Empty;
                                    FormDataEntryVariableMongo conSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ConSType.ToString());
                                    var ConSType = conSType != null ? conSType.SelectedValues : string.Empty;
                                    if (ConSType == "1")
                                    {
                                        entitySubtypeId = Clinical_Craft_Group_Society.Guid;
                                    }
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Activity_Venue:
                                    #region Activity Venue
                                    var activity_Venue = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Activity_Venue);
                                    entityTypeId = activity_Venue != null ? activity_Venue.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;

                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Vehicle:
                                    #region Vehicle
                                    var vehicle = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Vehicle);
                                    entityTypeId = vehicle != null ? vehicle.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.MAC:
                                    #region MAC
                                    var mAC = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.MAC);
                                    entityTypeId = mAC != null ? mAC.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;
                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                case (int)EntityTypesListInDB.Ethics_Committee:
                                    #region Ethics Committee
                                    var ethics_Committee = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Ethics_Committee);
                                    entityTypeId = ethics_Committee != null ? ethics_Committee.Guid : Guid.Empty;
                                    entitySubtypeId = Guid.Empty;

                                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                                    {
                                        Id = id,
                                        EntityName = plName,
                                        EntitySubtypeId = entitySubtypeId,
                                        EntityTypeId = entityTypeId,
                                    });
                                    entitiesMongo.Add(plc.EntityNumber);
                                    #endregion
                                    break;
                                default:
                                    break;
                            }
                            #endregion
                        }
                    });
                }
            }
            if (project != null && project.Count() > 0)
            {
                project.ToList().ForEach(proj =>
                {
                    id++;
                    FormDataEntryVariableMongo Name = proj.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());

                    string fn = Name != null ? Name.SelectedValues : string.Empty;

                    Guid entityTypeId = entityTypes.Where(x => x.Id == (int)EntityTypesListInDB.Project).Select(x => x.Guid).FirstOrDefault();
                    Guid entitySubtypeId = Guid.Empty;

                    FormDataEntryVariableMongo proSType = proj.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ProSType.ToString());
                    string pr = proSType != null ? proSType.SelectedValues : string.Empty;
                    switch (pr)
                    {
                        case "1":
                            entitySubtypeId = Registry != null ? Registry.Guid : Guid.Empty;
                            break;
                        case "2":
                            entitySubtypeId = Clinical_Trial != null ? Clinical_Trial.Guid : Guid.Empty;
                            break;
                        case "3":
                            entitySubtypeId = Cohort_Study != null ? Cohort_Study.Guid : Guid.Empty;
                            break;
                        case "4":
                            entitySubtypeId = Other != null ? Other.Guid : Guid.Empty;
                            break;
                        default:
                            break;
                    }
                    personEntitiesList.Add(new LookupVariablesPreviewViewModel
                    {
                        Id = id,
                        EntityName = fn,
                        EntitySubtypeId = entitySubtypeId,
                        EntityTypeId = entityTypeId,
                    });
                    entitiesMongo.Add(proj.EntityNumber);
                });
            }
            return personEntitiesList.OrderBy(x => x.EntityName);
        }

        public SummaryPageLeftPanelViewModel UpdateLeftPanelSummaryPage(Guid projectId, Int64 entityId, bool isTestSite = false)
        {
            SummaryPageLeftPanelViewModel model = new SummaryPageLeftPanelViewModel();

            IMongoQuery conditionSummaryPageActivities = Query.And(Query<SummaryPageActivityViewModel>.EQ(q => q.PersonEntityId, entityId), Query<SummaryPageActivityViewModel>.EQ(q => q.ProjectGuid, projectId));
            IQueryable<MongoDB.Bson.ObjectId> summaryPageIds;

            if (isTestSite)
            {
                summaryPageIds = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").Find(conditionSummaryPageActivities).AsQueryable().Where(x => x.DateDeactivated == null).AsQueryable().Select(c => c.Id);
            }
            else
            {
                summaryPageIds = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").Find(conditionSummaryPageActivities).AsQueryable().Where(x => x.DateDeactivated == null).AsQueryable().Select(c => c.Id);
            }

            IMongoQuery conditionUserEntitiesCustomForms = Query.Or(Query<FormDataEntryMongo>.EQ(q => q.ParentEntityNumber, entityId), Query<FormDataEntryMongo>.EQ(q => q.EntityNumber, entityId));
            IQueryable<FormDataEntryMongo> userEntitiesCustomForms;
            if (isTestSite)
            {
                userEntitiesCustomForms = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionUserEntitiesCustomForms).AsQueryable().Where(x => x.ProjectGuid == projectId && x.DateDeactivated == null && summaryPageIds.Contains(new MongoDB.Bson.ObjectId(x.SummaryPageActivityObjId)));
            }
            else
            {
                userEntitiesCustomForms = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionUserEntitiesCustomForms).AsQueryable().Where(x => x.ProjectGuid == projectId && x.DateDeactivated == null && summaryPageIds.Contains(new MongoDB.Bson.ObjectId(x.SummaryPageActivityObjId)));
            }

            if (userEntitiesCustomForms != null && userEntitiesCustomForms.Count() > 0)
            {
                #region Default variables display in left panel
                try
                {
                    string[] defaultVariablesForLeftPanel = {
                        DefaultsVariables.Email.ToString()
                        , DefaultsVariables.Phone.ToString()
                        , DefaultsVariables.Fax.ToString()
                        , DefaultsVariables.StrtNum.ToString()
                        , DefaultsVariables.StrtNum2.ToString()
                        , DefaultsVariables.StrtNme.ToString()
                        , DefaultsVariables.StrtNme2.ToString()
                        , DefaultsVariables.Suburb.ToString()
                        , DefaultsVariables.State.ToString()
                        , DefaultsVariables.Postcode.ToString()
                    };
                    List<string> defaultEmailValues = new List<string>();
                    List<string> defaultPhoneValues = new List<string>();
                    List<string> defaultFaxValues = new List<string>();
                    List<string> defaultStrtNumValues = new List<string>();
                    List<string> defaultStrt2NumValues = new List<string>();
                    List<string> defaultStrtNmeValues = new List<string>();
                    List<string> defaultStrtNme2Values = new List<string>();
                    List<string> defaultSuburbValues = new List<string>();
                    List<string> defaultStateValues = new List<string>();
                    List<string> defaultPostcodeValues = new List<string>();

                    defaultEmailValues.Add(model.Email);
                    defaultPhoneValues.Add(model.Phone);
                    defaultSuburbValues.Add(model.Suburb);
                    defaultStateValues.Add(model.State);
                    
                    string[] defaultActivitiesName = new string[]
                    {
                        EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration)
                    };

                    FormDataEntryMongo defaultFrm = userEntitiesCustomForms.FirstOrDefault(c => defaultActivitiesName.Contains(c.ActivityName));
                    model.DefaultFormType = defaultFrm != null ? defaultFrm.FormTitle : string.Empty;

                    if (defaultFrm == null && model.DefaultFormType == string.Empty)
                    {
                        model.DefaultFormType = userEntitiesCustomForms.Select(c => c.EntityTypeName).FirstOrDefault();
                        if (model.DefaultFormType == EntityTypesListInDB.Person.ToString())
                        {
                            model.DefaultFormType = EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration);
                        }
                        else if (model.DefaultFormType == EntityTypesListInDB.Place__Group.ToString().Replace("__", "/"))
                        {
                            model.DefaultFormType = EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration);
                        }
                        else if (model.DefaultFormType == EntityTypesListInDB.Participant.ToString())
                        {
                            model.DefaultFormType = EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration);
                        }
                        else if (model.DefaultFormType == EntityTypesListInDB.Project.ToString())
                        {
                            model.DefaultFormType = EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration);
                        }
                    }

                    userEntitiesCustomForms.ToList().ForEach(customForm =>
                    {
                        bool frmDefaultVariables = customForm.formDataEntryVariableMongoList.Any(c => defaultVariablesForLeftPanel.Contains(c.VariableName));
                        if (frmDefaultVariables)
                        {
                            FormDataEntryVariableMongo emailObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.Email.ToString());
                            if (emailObj != null)
                            {
                                defaultEmailValues.Add(!string.IsNullOrEmpty(emailObj.SelectedValues) ? emailObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo phoneObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.Phone.ToString());
                            if (phoneObj != null)
                            {
                                defaultPhoneValues.Add(!string.IsNullOrEmpty(phoneObj.SelectedValues) ? phoneObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo faxObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.Fax.ToString());
                            if (faxObj != null)
                            {
                                defaultFaxValues.Add(!string.IsNullOrEmpty(faxObj.SelectedValues) ? faxObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo strtNumObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.StrtNum.ToString());
                            if (strtNumObj != null)
                            {
                                defaultStrtNumValues.Add(!string.IsNullOrEmpty(strtNumObj.SelectedValues) ? strtNumObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo strtNum2Obj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.StrtNum2.ToString());
                            if (strtNum2Obj != null)
                            {
                                defaultStrt2NumValues.Add(!string.IsNullOrEmpty(strtNum2Obj.SelectedValues) ? strtNum2Obj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo strtNmeObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.StrtNme.ToString());
                            if (strtNmeObj != null)
                            {
                                defaultStrtNmeValues.Add(!string.IsNullOrEmpty(strtNmeObj.SelectedValues) ? strtNmeObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo strtNme2Obj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.StrtNme2.ToString());
                            if (strtNme2Obj != null)
                            {
                                defaultStrtNme2Values.Add(!string.IsNullOrEmpty(strtNme2Obj.SelectedValues) ? strtNme2Obj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo suburbObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.Suburb.ToString());
                            if (suburbObj != null)
                            {
                                defaultSuburbValues.Add(!string.IsNullOrEmpty(suburbObj.SelectedValues) ? suburbObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo stateObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.State.ToString());
                            if (stateObj != null)
                            {
                                defaultStateValues.Add(!string.IsNullOrEmpty(stateObj.SelectedValues) ? stateObj.SelectedValues : string.Empty);
                            }

                            FormDataEntryVariableMongo postcodeObj = customForm.formDataEntryVariableMongoList.FirstOrDefault(c => c.VariableName == DefaultsVariables.Postcode.ToString());
                            if (postcodeObj != null)
                            {
                                defaultPostcodeValues.Add(!string.IsNullOrEmpty(postcodeObj.SelectedValues) ? postcodeObj.SelectedValues : string.Empty);
                            }
                        }
                    });

                    defaultEmailValues = defaultEmailValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultPhoneValues = defaultPhoneValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultSuburbValues = defaultSuburbValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultStateValues = defaultStateValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();

                    defaultFaxValues = defaultFaxValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultStrtNumValues = defaultStrtNumValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultStrt2NumValues = defaultStrt2NumValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultStrtNmeValues = defaultStrtNmeValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultStrtNmeValues = defaultStrtNme2Values.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();
                    defaultPostcodeValues = defaultPostcodeValues.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Select(cc => cc.Replace(":", "")).ToList();

                    model.Email = string.Join(":", defaultEmailValues);
                    model.Phone = string.Join(":", defaultPhoneValues);
                    model.State = string.Join(":", defaultStateValues);
                    model.Suburb = string.Join(":", defaultSuburbValues);
                    model.Postcode = string.Join(":", defaultPostcodeValues);
                    model.Fax = string.Join(":", defaultFaxValues);
                    model.StrtNum = string.Join(":", defaultStrtNumValues);
                    model.StrtNum2 = string.Join(":", defaultStrt2NumValues);
                    model.StrtNme = string.Join(":", defaultStrtNmeValues);
                    model.StrtNme2 = string.Join(":", defaultStrtNmeValues);

                    if (!string.IsNullOrEmpty(model.State))
                    {
                        var allStates = _dbContext.States;
                        List<string> newList = new List<string>();
                        defaultStateValues.ForEach(st =>
                        {
                            Guid stateGuid = new Guid(st);
                            string stateName = allStates.Where(c => c.Guid == stateGuid).Select(c => c.Name).FirstOrDefault();
                            if (!string.IsNullOrEmpty(stateName))
                            {
                                newList.Add(stateName);
                            }
                        });
                        model.State = string.Join(":", newList);
                    }
                }
                catch (Exception exDefaultVariablesList)
                { Console.WriteLine(exDefaultVariablesList.Message); }
                #endregion
                return model;
            }
            return null;
        }
        public string GetDBEntTypeFromSelecedEntType(string enttype)
        {
            switch (enttype)
            {
                case "7":
                    enttype = "8";
                    break;
                case "8":
                    enttype = "9";
                    break;
                case "9":
                    enttype = "10";
                    break;
                case "10":
                    enttype = "11";
                    break;
                case "11":
                    enttype = "12";
                    break;
                case "12":
                    enttype = "13";
                    break;
                case "13":
                    enttype = "14";
                    break;
                case "14":
                    enttype = "15";
                    break;
                case "15":
                    enttype = "";
                    break;
                default:
                    break;
            }
            return enttype;
        }
    }
}