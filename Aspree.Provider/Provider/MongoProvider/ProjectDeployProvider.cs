using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Provider.Interface;
using Aspree.Data;
using Aspree.Data.MongoDB;
using Aspree.Provider.Interface.MongoProvider;
using Aspree.Core.ViewModels;
using Aspree.Core.ViewModels.MongoViewModels;
using MongoDB.Driver.Builders;
using Aspree.Core.Enum;

namespace Aspree.Provider.Provider.MongoProvider
{
    public class ProjectDeployProvider : IProjectDeployProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        private readonly IActivityProvider _activityProvider;
        private readonly IRoleProvider _RoleProvider;
        private readonly MongoDBContext _mongoDBContext;
        private readonly IFormDataEntryProvider _formDataEntryProvider;
        private readonly TestMongoDBContext _testMongoDBContext;
        public ProjectDeployProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider, IActivityProvider activityProvider, IRoleProvider roleProvider, IFormDataEntryProvider formDataEntryProvider, MongoDBContext mongoDBContext, TestMongoDBContext testMongoDBContext)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
            this._activityProvider = activityProvider;
            this._RoleProvider = roleProvider;
            this._formDataEntryProvider = formDataEntryProvider;
            this._mongoDBContext = mongoDBContext;
            this._testMongoDBContext = testMongoDBContext;
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }
        public ProjectDeployViewModel Create(Guid projectId, List<Guid> activitiesList, int statusType)
        {
            Core.ViewModels.NewCategory.WriteLog("push project in mongo start ");
            {
                var activityResult = dbContext.ActivitySchedulings.Where(x => activitiesList.Contains(x.Guid)).ToList();
                List<Int32> deployedFormsId = new List<int>();
                List<Int32> deployedActivitiesId = new List<int>();
                activityResult.ToList().ForEach(u =>
                {
                    if (u.Activity.IsDefaultActivity != (int)Core.Enum.DefaultActivityType.Default)
                    {
                        if (statusType == (int)Core.Enum.ActivityDeploymentStatus.Pushed)
                        {
                            u.Status = (int)Core.Enum.ActivityDeploymentStatus.Pushed;
                        }
                        else if (statusType == (int)Core.Enum.ActivityDeploymentStatus.Deployed)
                        {
                            u.Status = (int)Core.Enum.ActivityDeploymentStatus.Deployed;
                            deployedFormsId.AddRange(u.Activity.ActivityForms.Select(x => x.FormId).ToList());
                            deployedActivitiesId.Add(u.ActivityId);
                        }
                        else
                        {
                            u.Status = (int)Core.Enum.ActivityDeploymentStatus.Scheduled;
                        }
                    }
                });

                if (deployedActivitiesId.Count() > 0)
                {
                    var deployedActivitiesResult = dbContext.Activities.Where(x => deployedActivitiesId.Contains(x.Id)).ToList();
                    deployedActivitiesResult.ForEach(a =>
                    {
                        a.ActivityStatusId = (int)Core.Enum.ActivityStatusTypes.Active;
                    });
                }
                SaveChanges();
                ProjectDeployViewModel model = new ProjectDeployViewModel();
                var document = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects");
                var projectDetailscount = document.FindAs<ProjectDeployViewModel>(Query.EQ("ProjectGuid", projectId)).Count();

                model.ProjectVersion = "0";
                model.ProjectInternalVersion = 1;
                if (projectDetailscount > 0)
                {
                    model.ProjectVersion = Convert.ToString(projectDetailscount);
                    model.ProjectInternalVersion = (Int32)(++projectDetailscount);
                }

                var project = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == projectId);
                var projCreatedBy = dbContext.UserLogins.FirstOrDefault(x => x.Id == project.CreatedBy);
                var formdataentry = dbContext.FormDataEntryVariables.FirstOrDefault(x => x.FormDataEntryId == project.Id && x.Variable.VariableName == "Name");

                model.ProjectGuid = project.Guid;
                model.ProjectId = project.Id;
                model.ProjectName = formdataentry.SelectedValues;
                model.ProjectState = "Live";
                model.ProjectActivitiesList = dbContext.ActivitySchedulings
                    .Where(x =>
                        x.ProjectId == project.Id
                        && x.Status == (int)Core.Enum.ActivityDeploymentStatus.Deployed

                        && (x.Activity.ActivityForms.Where(y => y.Form.DateDeactivated == null).Count() != 0)
                        )
                    .Select(ToActivitiesMongo).ToList();

                model.ProjectStaffListMongo = dbContext.ProjectStaffMemberRoles.Where(x => x.ProjectId == project.Id && x.UserLogin.UserTypeId != (int)Core.Enum.UsersLoginType.Test).Select(p => new ProjectStaffMongo()
                {
                    StaffGuid = p.UserLogin.Guid,
                    StaffName = p.UserLogin.FirstName + " " + p.UserLogin.LastName,
                    Role = p.Role.Name,
                }).ToList();
                model.ProjectCreatedDate = project.CreatedDate;
                model.ProjectDeployDate = DateTime.UtcNow;
                model.ProjectCreatedBy = projCreatedBy != null ? projCreatedBy.FirstName + " " + projCreatedBy.LastName : string.Empty;
                var recStartDate = project.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.RecruitStart.ToString());
                var recEndDate = project.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.RecruitEnd.ToString());

                try { model.RecruitmentStartDate = recStartDate != null ? Convert.ToDateTime(recStartDate.SelectedValues) : (DateTime?)null; } catch (Exception ex) { }
                try { model.RecruitmentEndDate = recEndDate != null ? Convert.ToDateTime(recEndDate.SelectedValues) : (DateTime?)null; } catch (Exception ex) { }

                var result = document.Insert(model);
                deployEntitiesInMongo(model.ProjectId, model.ProjectName, model.ProjectGuid, model.ProjectInternalVersion);

                var UpdateProjectStatus = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == projectId);
                UpdateProjectStatus.ProjectDeployStatus = (int)Core.Enum.ProjectStatusTypes.Published;
                UpdateProjectStatus.ProjectDeployedId = Convert.ToString(model.Id);
                UpdateProjectStatus.ProjectDeployedVersion = model.ProjectVersion;

                SaveChanges();
                Core.ViewModels.NewCategory.WriteLog("push project in mongo end");
                return model;
            }
        }
        public ActivitiesMongo ToActivitiesMongo(ActivityScheduling activityScheduling)
        {
            string activityAvailableForCreation = Enum.GetName(typeof(Core.Enum.SchedulingActivityAvailableForCreation), activityScheduling.ActivityAvailableForCreation);
            string ActivityEntityTypeName = activityScheduling.Activity.ActivityEntityTypes.FirstOrDefault().EntityType.Name;
            string OffsetTypeName = activityScheduling.OffsetType != null ? Enum.GetName(typeof(Core.Enum.SchedulingOffsetType), activityScheduling.OffsetType) : string.Empty;
            var otherActivity = dbContext.Activities.FirstOrDefault(x => x.Id == activityScheduling.OtherActivity);
            List<string> rolesToCreateActivity = activityScheduling.RolesToCreateActivitySchedulings.Select(x => x.Role.Name).ToList();
            List<string> roleToCreateActivityRegardlessScheduled = activityScheduling.RoleToCreateActivityRegardlessScheduleds.Select(x => x.Role.Name).ToList();
            string ScheduledToBeCompleted = activityScheduling.ScheduledToBeCompleted != 0 ? Enum.GetName(typeof(Core.Enum.ScheduledToBeCompleted), activityScheduling.ScheduledToBeCompleted) : string.Empty;
            Activity specifiedActivity = dbContext.Activities.FirstOrDefault(x => x.Id == activityScheduling.SpecifiedActivity);
            return new ActivitiesMongo()
            {
                ActivityId = activityScheduling.Activity.Id,
                ActivityGuid = activityScheduling.Activity.Guid,
                ActivityName = activityScheduling.Activity.ActivityName,
                ActivityCategoryName = activityScheduling.Activity.ActivityCategory.CategoryName,
                ActivityAvailableForCreation = activityAvailableForCreation,
                ActivityEntityTypeName = ActivityEntityTypeName,
                CreationWindowClose = activityScheduling.CreationWindowClose,
                CreationWindowOpens = activityScheduling.CreationWindowOpens,
                OffsetCount = activityScheduling.OffsetCount,
                OffsetTypeName = OffsetTypeName,
                OtherActivityName = otherActivity != null ? otherActivity.ActivityName : string.Empty,
                ScheduleDate = activityScheduling.ScheduleDate,
                ScheduledToBeCompleted = ScheduledToBeCompleted,
                SpecifiedActivityName = specifiedActivity != null ? specifiedActivity.ActivityName : string.Empty,
                RolesToCreateActivity = rolesToCreateActivity,
                RoleToCreateActivityRegardlessScheduled = roleToCreateActivityRegardlessScheduled,
                FormsListMongo = activityScheduling.Activity.ActivityForms.Where(x => x.Form.DateDeactivated == null).Select(ToFormsMongo).ToList(),
                IsDefaultActivity = activityScheduling.Activity.IsDefaultActivity,
                CanCreatedMultipleTime = activityScheduling.CanCreatedMultipleTime,
            };
        }
        public FormsMongo ToFormsMongo(ActivityForm activityForm)
        {
            Form form = dbContext.Forms.FirstOrDefault(x => x.Id == activityForm.FormId);
            return new FormsMongo()
            {
                FormCategoryName = form.FormCategory.CategoryName,
                FormEntityTypes = form.FormEntityTypes.Select(en => en.EntityType.Name).ToList(),
                FormId = form.Id,
                FormGuid = form.Guid,
                FormTitle = form.FormTitle,
                FormVersion = Convert.ToString(form.Version),
                VariablesListMongo = form.FormVariables.Where(x => x.DateDeactivated == null).Select(ToVariablesMongo).ToList(),
                IsDefaultForm = form.IsDefaultForm,
            };
        }
        public VariablesMongo ToVariablesMongo(FormVariable formVariable)
        {
            var dependentVariable = dbContext.Variables.FirstOrDefault(x => x.Id == formVariable.DependentVariableId);
            List<string> LookupEntitySubtypeName = null;
            string LookupEntityTypeName = null;
            try
            {
                if (formVariable.Variable.VariableType.Type == VariableTypes.LKUP.ToString())
                {
                    var lkupEType = dbContext.Variables.FirstOrDefault(x => x.Id == formVariable.VariableId);
                    var lkupETypeName = lkupEType.VariableEntityTypes.Count() == 1 ? lkupEType.VariableEntityTypes.First() : null;
                    LookupEntityTypeName = lkupEType.VariableEntityTypes.FirstOrDefault() != null ? lkupEType.VariableEntityTypes.FirstOrDefault().EntityType.Name : null;
                    string subtypeList = lkupETypeName != null ? (lkupETypeName.EntitySubTypeId != null ? lkupETypeName.EntitySubType.Name : null) : null;
                    if (subtypeList != null)
                    {
                        LookupEntitySubtypeName = new List<string>() { subtypeList };
                    }
                }
            }
            catch (Exception ex)
            { }
            return new VariablesMongo()
            {
                VariableId = formVariable.VariableId,
                VariableGuid = formVariable.Variable.Guid,
                VariableName = formVariable.Variable.VariableName,
                VariableTypeName = formVariable.Variable.VariableType.Type,
                Question = !string.IsNullOrEmpty(formVariable.QuestionText) ? formVariable.QuestionText : formVariable.Variable.Question,
                HelpText = !string.IsNullOrEmpty(formVariable.HelpText) ? formVariable.HelpText : formVariable.Variable.HelpText,
                VariableRequiredMessage = formVariable.Variable.RequiredMessage,
                Values = formVariable.Variable.Values.Split('|').ToList(),
                ValueDescription = formVariable.Variable.VariableValueDescription.Split('|').ToList(),
                MinRange = formVariable.Variable.MinRange,
                MaxRange = formVariable.Variable.MaxRange,
                DependentVariableId = formVariable.DependentVariableId,
                DependentVariableName = dependentVariable != null ? dependentVariable.VariableName : string.Empty,
                ResponseOption = formVariable.ResponseOption,
                IsDefaultVariable = formVariable.Variable.IsDefaultVariable,
                IsRequired = formVariable.IsRequired,
                VariableRoleListMongo = formVariable.FormVariableRoles.Select(ToFormVariableRoleMongo).ToList(),
                VariableValidationRuleListMongo = formVariable.Variable.VariableValidationRules.Select(ToVariableValidationRuleMongo).ToList(),
                IsSearchVisible = formVariable.IsSearchVisible,
                SearchPageOrder = formVariable.SearchPageOrder,
                CanFutureDate = formVariable.Variable.CanFutureDate,
                VariableOrderNo = formVariable.VariableOrderNo,
                FormPageVariableId = formVariable.Id,
                LookupEntityTypeName = LookupEntityTypeName,
                LookupEntitySubtypeName = LookupEntitySubtypeName,
                IsBlank = formVariable.IsBlank,
            };
        }
        public FormVariableRoleMongo ToFormVariableRoleMongo(FormVariableRole formVariableRole)
        {
            return new FormVariableRoleMongo()
            {
                CanCreate = formVariableRole.CanCreate,
                CanView = formVariableRole.CanView,
                CanEdit = formVariableRole.CanEdit,
                CanDelete = formVariableRole.CanDelete,
                RoleName = formVariableRole.Role.Name,
            };
        }
        public VariableValidationRuleMongo ToVariableValidationRuleMongo(VariableValidationRule variableValidationRule)
        {
            return new VariableValidationRuleMongo()
            {
                LimitType = variableValidationRule.LimitType,
                Max = variableValidationRule.Max,
                Min = variableValidationRule.Min,
                RegEx = variableValidationRule.RegEx,
                ValidationMessage = variableValidationRule.ValidationMessage,
                ValidationName = variableValidationRule.ValidationRule != null ? variableValidationRule.ValidationRule.RuleType : "Custom",
            };
        }


        public ProjectDeployViewModel GetProjectByGuid(Guid projectId)
        {
            Core.ViewModels.NewCategory.WriteLog("mongo project get start");
            ProjectDeployViewModel model = new ProjectDeployViewModel();
            var projectDetailscount = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(Query.EQ("ProjectGuid", projectId)).AsQueryable().Count();
            if (projectDetailscount > 0)
            {
                var userObjectid = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectId);
                var project = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(userObjectid).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                try
                {
                    FormDataEntry formDataEntry = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == projectId);
                    if (formDataEntry != null)
                    {
                        #region Does this project have ethics approval
                        try
                        {
                            FormDataEntryVariable does_This_ProjectHaveEthicsApproval = formDataEntry.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.Ethics.ToString());
                            if (does_This_ProjectHaveEthicsApproval != null)
                            {
                                int? ethicsApproval = !string.IsNullOrEmpty(does_This_ProjectHaveEthicsApproval.SelectedValues) ? Convert.ToInt32(does_This_ProjectHaveEthicsApproval.SelectedValues) : (int?)null;
                                project.ProjectEthicsApproval = ethicsApproval != null ? (ethicsApproval == 1 ? true : false) : (bool?)null;
                            }
                        }
                        catch (Exception ethicsApproval) { } 
                        #endregion

                        FormDataEntryVariable recruitStart = formDataEntry.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.RecruitStart.ToString());
                        FormDataEntryVariable recruitEnd = formDataEntry.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.RecruitEnd.ToString());

                        if (recruitStart != null)
                        {
                            DateTime? StartDate = null;
                            try
                            {
                                if (!string.IsNullOrEmpty(recruitStart.SelectedValues))
                                {
                                    List<int> dateValidationTypes = new List<int> { 6, 9, 10, 11, 12, 13 };
                                    VariableValidationRule variableValidationRules = recruitStart.Variable.VariableValidationRules.Where(c => dateValidationTypes.Contains(c.ValidationId ?? 0)).FirstOrDefault();
                                    project.RecruitmentStartDate = ConvertStringToDate(variableValidationRules, recruitStart.SelectedValues);
                                }
                            }
                            catch (Exception stDate) { project.RecruitmentStartDate = null; }

                        }
                        if (recruitEnd != null)
                        {
                            DateTime? EndDate = null;
                            try
                            {
                                if (!string.IsNullOrEmpty(recruitEnd.SelectedValues))
                                {
                                    List<int> dateValidationTypes = new List<int> { 6, 9, 10, 11, 12, 13 };
                                    VariableValidationRule variableValidationRules = recruitEnd.Variable.VariableValidationRules.Where(c => dateValidationTypes.Contains(c.ValidationId ?? 0)).FirstOrDefault();
                                    project.RecruitmentEndDate = ConvertStringToDate(variableValidationRules, recruitEnd.SelectedValues);
                                }
                            }
                            catch (Exception stEnd) { project.RecruitmentEndDate = null; }
                        }
                    }
                }
                catch (Exception exc) { }
                Core.ViewModels.NewCategory.WriteLog("mongo project get end");
                return project;
            }
            return null;
        }
        public ProjectDeployViewModel TestEnvironment_GetProjectByGuid(Guid projectId)
        {
            ProjectDeployViewModel model = new ProjectDeployViewModel();
            var projectDetailscount = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(Query.EQ("ProjectGuid", projectId)).AsQueryable().Count();
            if (projectDetailscount > 0)
            {
                var userObjectid = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectId);
                var project = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(userObjectid).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                #region Project recruitment logic
                try
                {
                    FormDataEntry formDataEntry = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == projectId);
                    if (formDataEntry != null)
                    {
                        #region Does this project have ethics approval
                        try
                        {
                            FormDataEntryVariable does_This_ProjectHaveEthicsApproval = formDataEntry.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.Ethics.ToString());
                            if (does_This_ProjectHaveEthicsApproval != null)
                            {
                                int? ethicsApproval = !string.IsNullOrEmpty(does_This_ProjectHaveEthicsApproval.SelectedValues) ? Convert.ToInt32(does_This_ProjectHaveEthicsApproval.SelectedValues) : (int?)null;
                                project.ProjectEthicsApproval = ethicsApproval != null ? (ethicsApproval == 1 ? true : false) : (bool?)null;
                            }
                        }
                        catch (Exception ethicsApproval) { }
                        #endregion
                        FormDataEntryVariable recruitStart = formDataEntry.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.RecruitStart.ToString());
                        FormDataEntryVariable recruitEnd = formDataEntry.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.RecruitEnd.ToString());
                        if (recruitStart != null)
                        {
                            DateTime? StartDate = null;
                            try
                            {
                                StartDate = !string.IsNullOrEmpty(recruitStart.SelectedValues) ? Convert.ToDateTime(recruitStart.SelectedValues) : (DateTime?)null;
                                project.RecruitmentStartDate = StartDate;
                            }
                            catch (Exception stDate) { project.RecruitmentStartDate = null; }

                        }
                        if (recruitEnd != null)
                        {
                            DateTime? EndDate = null;
                            try
                            {
                                EndDate = !string.IsNullOrEmpty(recruitEnd.SelectedValues) ? Convert.ToDateTime(recruitEnd.SelectedValues) : (DateTime?)null;
                                project.RecruitmentEndDate = EndDate;
                            }
                            catch (Exception stEnd) { project.RecruitmentEndDate = null; }
                        }
                    }
                }
                catch (Exception exc) { }
                #endregion
                return project;
            }
            return null;
        }

        public IEnumerable<FormDataEntryProjectsViewModel> GetAllDeployedProject(Guid loggedInUserId)
        {
            UserLogin loggedInUser = dbContext.UserLogins.FirstOrDefault(x => x.Guid == loggedInUserId);
            List<String> loggedInUserRoles = new List<String>();
            if (loggedInUser != null)
                loggedInUserRoles = loggedInUser.UserRoles.Select(x => x.Role.Name).ToList();

            IEnumerable<FormDataEntryProjectsViewModel> projects = _formDataEntryProvider.GetAllDataEntryProjectList();
            return projects;
        }

        public IEnumerable<FormDataEntryProjectsViewModel> TestEnvironment_GetAllDeployedProject(Guid loggedInUserId)
        {
            UserLogin loggedInUser = dbContext.UserLogins.FirstOrDefault(x => x.Guid == loggedInUserId);
            List<String> loggedInUserRoles = new List<String>();
            if (loggedInUser != null)
                loggedInUserRoles = loggedInUser.UserRoles.Select(x => x.Role.Name).ToList();

            List<string> dbRoles = dbContext.Roles.Where(x => x.DateDeactivated == null).Select(rl => rl.Name).ToList();
            var projectCollections = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").FindAll();
            List<FormDataEntryProjectsViewModel> projList = new List<FormDataEntryProjectsViewModel>();
            FormDataEntryProjectsViewModel projectModel = new FormDataEntryProjectsViewModel();
            foreach (var project in projectCollections.GroupBy(x => x.ProjectGuid))
            {
                projectModel = new FormDataEntryProjectsViewModel();
                var prj = project.OrderByDescending(x => x.ProjectInternalVersion).FirstOrDefault();
                projectModel.Id = prj.ProjectId;
                projectModel.Guid = prj.ProjectGuid;
                projectModel.ProjectName = prj.ProjectName;

                #region project appearance
                try
                {
                    var projSQL = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == prj.ProjectGuid);
                    string entid = projSQL.FormDataEntryVariables.Where(x => x.Variable.VariableName == "EntID").Select(x => x.SelectedValues).FirstOrDefault();
                    int ent = !string.IsNullOrEmpty(entid) ? Convert.ToInt32(entid) : 0;
                    var userent = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().Where(x => x.ProjectGuid == prj.ProjectGuid && x.EntityNumber == ent).FirstOrDefault();
                    string logo = userent.formDataEntryVariableMongoList.Where(x => x.VariableName == "ProjectLogo").Select(x => x.SelectedValues).FirstOrDefault();
                    string color = userent.formDataEntryVariableMongoList.Where(x => x.VariableName == "ProjectColor").Select(x => x.SelectedValues).FirstOrDefault();
                    string displayname = userent.formDataEntryVariableMongoList.Where(x => x.VariableName == "ProjectDisplayName").Select(x => x.SelectedValues).FirstOrDefault();
                    string textcolor = userent.formDataEntryVariableMongoList.Where(x => x.VariableName == "ProjectDisplayNameTextColour").Select(x => x.SelectedValues).FirstOrDefault();
                    projectModel.ProjectLogo = !string.IsNullOrEmpty(logo) ? logo : string.Empty;
                    projectModel.ProjectColor = !string.IsNullOrEmpty(color) ? color : string.Empty;
                    projectModel.ProjectDisplayName = !string.IsNullOrEmpty(displayname) ? displayname : string.Empty;
                    projectModel.ProjectDisplayNameTextColour = !string.IsNullOrEmpty(textcolor) ? textcolor : string.Empty;
                }
                catch (Exception exc) { }
                #endregion
                NewProjectStaffMemberRoleViewModel newProjectStaffMemberRoleViewModel = new NewProjectStaffMemberRoleViewModel();
                projectModel.ProjectStaffMembersRoles = new List<NewProjectStaffMemberRoleViewModel>();
                prj.ProjectStaffListMongo.Where(rl => dbRoles.Contains(rl.Role)).ToList().ForEach(c =>
                {
                    if (c.IsActiveProjectUser == true)
                    {
                        var userProjectJoinDate = Convert.ToDateTime(c.ProjectJoinedDate);
                        if (DateTime.Now.Date >= userProjectJoinDate.Date)
                        {
                            newProjectStaffMemberRoleViewModel = new NewProjectStaffMemberRoleViewModel();
                            newProjectStaffMemberRoleViewModel.ProjectUserRoleName = c.Role;
                            newProjectStaffMemberRoleViewModel.UserGuid = c.StaffGuid;
                            newProjectStaffMemberRoleViewModel.ProjectUserName = c.StaffName;
                            projectModel.ProjectStaffMembersRoles.Add(newProjectStaffMemberRoleViewModel);
                        }
                    }
                    else
                    {
                        var userProjectJoinDate = Convert.ToDateTime(c.ProjectJoinedDate);
                        var userProjectLeftDate = Convert.ToDateTime(c.ProjectLeftDate);

                        if (DateTime.Now.Date >= userProjectJoinDate.Date && DateTime.Now.Date < userProjectLeftDate.Date)
                        {
                            newProjectStaffMemberRoleViewModel = new NewProjectStaffMemberRoleViewModel();
                            newProjectStaffMemberRoleViewModel.ProjectUserRoleName = c.Role;
                            newProjectStaffMemberRoleViewModel.UserGuid = c.StaffGuid;
                            newProjectStaffMemberRoleViewModel.ProjectUserName = c.StaffName;

                            projectModel.ProjectStaffMembersRoles.Add(newProjectStaffMemberRoleViewModel);
                        }
                    }
                });
                projList.Add(projectModel);
            }
            return projList.OrderBy(x => x.Id);
        }

        public bool deployEntitiesInMongo(Int32 ProjectId, string ProjectName, Guid ProjectGuid, int ProjectVersion)
        {
            IQueryable<FormDataEntry> getAllEntitiesOfThisProject = dbContext.FormDataEntries;
            foreach (var entity in getAllEntitiesOfThisProject)
            {
                if (entity != null)
                {
                    Int64 id = entity.EntityNumber != null ? Convert.ToInt64(entity.EntityNumber) : 0;
                    if (id == 0)
                    {
                        try { id = Convert.ToInt64(entity.FormDataEntryVariables.Where(x => x.Variable.VariableName == "EntID").Select(x => x.SelectedValues).FirstOrDefault()); } catch (Exception e) { }
                    }
                    var checkEntityExestence = Query<FormDataEntryMongo>.EQ(q => q.EntityNumber, id);
                    var checkEntityExestenceCountInMongo = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(checkEntityExestence).Count();
                    if (checkEntityExestenceCountInMongo == 0)
                    {
                        if(ProjectGuid== entity.Guid)
                        {
                            continue;
                        }
                        FormDataEntryMongo entityModelMongo = new FormDataEntryMongo();
                        int summaryPageActiveId = entity.SubjectId != null ? (int)entity.SubjectId : 0;
                        if (summaryPageActiveId != 0)
                            entityModelMongo.SummaryPageActivityObjId = deploySummaryPageActive(summaryPageActiveId, ProjectGuid, id, ProjectVersion);

                        EntityType entityTypeDB = dbContext.EntityTypes.FirstOrDefault(x => x.Id == entity.EntityId);
                        UserLogin createdBy = dbContext.UserLogins.FirstOrDefault(x => x.Id == entity.CreatedBy);
                        UserLogin thisUserIdDetails = dbContext.UserLogins.FirstOrDefault(x => x.Id == entity.ThisUserId);

                        entityModelMongo.ProjectVersion = ProjectVersion;
                        entityModelMongo.ProjectId = ProjectId;
                        entityModelMongo.ProjectGuid = ProjectGuid;
                        entityModelMongo.ProjectName = ProjectName;

                        entityModelMongo.FormId = entity.FormId != null ? (int)entity.FormId : 1;
                        entityModelMongo.FormGuid = entity.FormId != null ? entity.Form.Guid : Guid.Empty;
                        entityModelMongo.FormTitle = entity.FormId != null ? entity.Form.FormTitle : null;

                        entityModelMongo.ActivityId = entity.Activity.Id;
                        entityModelMongo.ActivityGuid = entity.Activity.Guid;
                        entityModelMongo.ActivityName = entity.Activity.ActivityName;

                        entityModelMongo.EntityTypeName = entityTypeDB != null ? entityTypeDB.Name : null;
                        entityModelMongo.EntityTypeId = entityTypeDB != null ? entityTypeDB.Id : entity.EntityId;
                        entityModelMongo.EntityTypeGuid = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty;

                        entityModelMongo.Status = entity.Status;
                        entityModelMongo.CreatedById = createdBy != null ? createdBy.Id : 2;
                        entityModelMongo.CreatedByGuid = createdBy != null ? createdBy.Guid : Guid.Empty;
                        entityModelMongo.CreatedByName = createdBy != null ? createdBy.FirstName + " " + createdBy.LastName : null;
                        entityModelMongo.CreatedDate = entity.CreatedDate;
                        entityModelMongo.Guid = Guid.Empty;
                        entityModelMongo.ThisUserId = entity.ThisUserId;
                        entityModelMongo.ThisUserName = thisUserIdDetails != null ? thisUserIdDetails.FirstName + " " + thisUserIdDetails.LastName : null;
                        entityModelMongo.ThisUserGuid = thisUserIdDetails != null ? thisUserIdDetails.Guid : (Guid?)null;
                        entityModelMongo.EntityNumber = id;
                        entityModelMongo.ParentEntityNumber = entity.ParentEntityNumber;

                        entityModelMongo.formDataEntryVariableMongoList = new List<FormDataEntryVariableMongo>();
                        FormDataEntryVariableMongo formDataEntrtyVariable = new FormDataEntryVariableMongo();

                        var formVariables = entity.FormDataEntryVariables;
                        foreach (var variable in formVariables)
                        {
                            formDataEntrtyVariable = new FormDataEntryVariableMongo();
                            formDataEntrtyVariable.VariableId = variable.VariableId;
                            formDataEntrtyVariable.VariableGuid = variable.Variable.Guid;
                            formDataEntrtyVariable.VariableName = variable.Variable.VariableName;
                            formDataEntrtyVariable.SelectedValues = variable.SelectedValues;
                            formDataEntrtyVariable.CreatedBy = variable.CreatedBy;
                            formDataEntrtyVariable.CreatedDate = variable.CreatedDate;
                            formDataEntrtyVariable.ParentId = id;

                            entityModelMongo.formDataEntryVariableMongoList.Add(formDataEntrtyVariable);
                        }

                        var document = _mongoDBContext._database.GetCollection<MongoDB.Bson.BsonDocument>("UserEntities");
                        var result = document.Insert(entityModelMongo);
                    }
                }
            }
            return true;
        }

        public string deploySummaryPageActive(int summaryPageActiveId, Guid projectId, Int64 entityNumber, int projectVersion)
        {
            AddActivity addActivity = dbContext.AddActivities.FirstOrDefault(x => x.Id == summaryPageActiveId);
            if (addActivity != null)
            {
                var project = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == projectId);
                var activity = dbContext.Activities.FirstOrDefault(x => x.Id == addActivity.ActivityId);

                string[] defaultActivitiesName = new string[]
{
                        EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration)
                        , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Linkage)
};

                if (!defaultActivitiesName.Contains(activity.ActivityName))
                {
                    return string.Empty;
                }
                var parentEntity = dbContext.FormDataEntries.FirstOrDefault(x => x.EntityNumber == entityNumber);
                Int64? parentEntityNumber = parentEntity != null ? (parentEntity.ParentEntityNumber != null ? (Int64)parentEntity.ParentEntityNumber : (Int64)parentEntity.EntityNumber) : entityNumber;

                #region linkage from summary page                
                var activityCompletedBy = dbContext.UserLogins.FirstOrDefault(x => x.Id == addActivity.ActivityCompletedByUser);
                SummaryPageActivityViewModel summaryPageActivityViewModel = new SummaryPageActivityViewModel();
                summaryPageActivityViewModel.ActivityId = addActivity.Activity.Id;
                summaryPageActivityViewModel.ActivityGuid = addActivity.Activity.Guid;
                summaryPageActivityViewModel.ActivityName = addActivity.Activity.ActivityName;
                summaryPageActivityViewModel.ActivityCompletedById = activityCompletedBy.Id;
                summaryPageActivityViewModel.ActivityCompletedByGuid = activityCompletedBy.Guid;
                summaryPageActivityViewModel.ActivityCompletedByName = activityCompletedBy.FirstName + " " + activityCompletedBy.LastName;
                summaryPageActivityViewModel.ActivityDate = addActivity.ActivityDate;

                summaryPageActivityViewModel.ProjectGuid = project != null ? project.Guid : Guid.Empty;
                summaryPageActivityViewModel.ProjectName = project.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == "Name") != null ? project.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == "Name").SelectedValues : string.Empty;

                summaryPageActivityViewModel.PersonEntityId = parentEntityNumber != null ? (Int64)parentEntityNumber : entityNumber;
                summaryPageActivityViewModel.CreatedByName = addActivity.UserLogin.FirstName + " " + addActivity.UserLogin.LastName;
                summaryPageActivityViewModel.CreatedDate = DateTime.UtcNow;

                summaryPageActivityViewModel.SummaryPageActivityFormsList = new List<SummaryPageActivityForms>();
                SummaryPageActivityForms from = new SummaryPageActivityForms();
                foreach (var item in activity.ActivityForms)
                {
                    var status = dbContext.FormDataEntries.FirstOrDefault(x => x.EntityNumber == entityNumber);
                    int statusid = status != null ? status.Status : (int)Core.Enum.FormStatusTypes.Draft;

                    summaryPageActivityViewModel.SummaryPageActivityFormsList.Add(new SummaryPageActivityForms()
                    {
                        FormGuid = item.Form.Guid,
                        FormId = item.FormId,
                        FormTitle = item.Form.FormTitle,
                        FormStatusId = statusid,
                        FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), statusid),
                    });
                }

                summaryPageActivityViewModel.ProjectVersion = projectVersion;
                var summaryPageActivity = _mongoDBContext._database.GetCollection<MongoDB.Bson.BsonDocument>("SummaryPageActivity");
                var summaryPageActivityResult = summaryPageActivity.Insert(summaryPageActivityViewModel);

                return summaryPageActivityViewModel.Id.ToString();
                #endregion
            }
            return string.Empty;
        }

        public ProjectDeployViewModel CreateTestProject(Guid projectId, List<Guid> activitiesList, int statusType)
        {
            Core.ViewModels.NewCategory.WriteLog("push project in mongo start ");
            try
            {
                var defaultActivities = dbContext.Activities.Where(x => x.FormDataEntry.Guid == projectId && x.IsDefaultActivity == (int)Core.Enum.DefaultActivityType.Default).Select(x => x.Id).ToList();
                var activityResult = dbContext.ActivitySchedulings.Where(x => defaultActivities.Contains(x.ActivityId)).Select(x => x.Guid).ToList();
                activitiesList.AddRange(activityResult);

                List<Int32> deployedFormsId = new List<int>();
                List<Int32> deployedActivitiesId = new List<int>();
                ProjectDeployViewModel model = new ProjectDeployViewModel();
                var document = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects");
                var projectDetailscount = document.FindAs<ProjectDeployViewModel>(Query.EQ("ProjectGuid", projectId)).Count();

                model.ProjectVersion = "0";
                model.ProjectInternalVersion = 1;
                if (projectDetailscount > 0)
                {
                    model.ProjectVersion = Convert.ToString(projectDetailscount);
                    model.ProjectInternalVersion = (Int32)(++projectDetailscount);
                }

                var project = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == projectId);
                var projCreatedBy = dbContext.UserLogins.FirstOrDefault(x => x.Id == project.CreatedBy);
                var formdataentry = dbContext.FormDataEntryVariables.FirstOrDefault(x => x.FormDataEntryId == project.Id && x.Variable.VariableName == "Name");

                model.ProjectGuid = project.Guid;
                model.ProjectId = project.Id;
                model.ProjectName = formdataentry.SelectedValues;
                model.ProjectState = "Test";
                model.ProjectActivitiesList = dbContext.ActivitySchedulings
                    .Where(x => x.ProjectId == project.Id && activitiesList.Contains(x.Guid))
                    .Select(ToActivitiesMongo).ToList();


                model.ProjectStaffListMongo = dbContext.ProjectStaffMemberRoles.Where(x => x.ProjectId == project.Id && x.UserLogin.UserTypeId == (int)Core.Enum.UsersLoginType.Test).Select(p => new ProjectStaffMongo()
                {
                    StaffGuid = p.UserLogin.Guid,
                    StaffName = p.UserLogin.FirstName + " " + p.UserLogin.LastName,
                    Role = p.Role.Name,

                    IsActiveProjectUser = p.IsActiveProjectUser,
                    ProjectJoinedDate = p.ProjectJoinedDate,
                    ProjectLeftDate = p.ProjectLeftDate,
                }).ToList();

                model.ProjectCreatedDate = project.CreatedDate;
                model.ProjectDeployDate = DateTime.UtcNow;
                model.ProjectCreatedBy = projCreatedBy != null ? projCreatedBy.FirstName + " " + projCreatedBy.LastName : string.Empty;

                var recStartDate = project.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.RecruitStart.ToString());
                var recEndDate = project.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.RecruitEnd.ToString());

                try { model.RecruitmentStartDate = recStartDate != null ? Convert.ToDateTime(recStartDate.SelectedValues) : (DateTime?)null; } catch (Exception ex) { }
                try { model.RecruitmentEndDate = recEndDate != null ? Convert.ToDateTime(recEndDate.SelectedValues) : (DateTime?)null; } catch (Exception ex) { }

                var ethicsApproval = project.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.Ethics.ToString());
                                
                try { model.ProjectEthicsApproval = ethicsApproval != null ? Convert.ToBoolean(Convert.ToInt16(ethicsApproval.SelectedValues)) : (bool?)null; } catch (Exception ex) { }


                var result = document.Insert(model);

                TestDeployEntitiesInMongo(model.ProjectId, model.ProjectName, model.ProjectGuid, model.ProjectInternalVersion);

                Core.ViewModels.NewCategory.WriteLog("push project in mongo end");
                return model;

            }
            catch (Exception ddf)
            {
                Core.ViewModels.NewCategory.WriteLog("error::::" + ddf.Message);
                Core.ViewModels.NewCategory.WriteLog("error::::" + ddf.StackTrace);
                throw;
            }
        }
        public bool TestDeployEntitiesInMongo(Int32 ProjectId, string ProjectName, Guid ProjectGuid, int ProjectVersion)
        {
            var getAllEntitiesOfThisProject = dbContext.FormDataEntries.Where(x => x.Guid == ProjectGuid);

            foreach (var entity in getAllEntitiesOfThisProject)
            {
                if (entity != null)
                {
                    Int64 id = entity.EntityNumber != null ? Convert.ToInt64(entity.EntityNumber) : 0;
                    if (id == 0)
                    {
                        try { id = Convert.ToInt64(entity.FormDataEntryVariables.Where(x => x.Variable.VariableName == "EntID").Select(x => x.SelectedValues).FirstOrDefault()); } catch (Exception e) { }
                    }

                    var checkEntityExestence = Query<FormDataEntryMongo>.EQ(q => q.EntityNumber, id);
                    var checkEntityExestenceCountInMongo = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(checkEntityExestence).Count();
                    if (checkEntityExestenceCountInMongo == 0)
                    {
                        EntityType entityTypeDB = dbContext.EntityTypes.FirstOrDefault(x => x.Id == entity.EntityId);
                        UserLogin createdBy = dbContext.UserLogins.FirstOrDefault(x => x.Id == entity.CreatedBy);
                        UserLogin thisUserIdDetails = dbContext.UserLogins.FirstOrDefault(x => x.Id == entity.ThisUserId);

                        FormDataEntryMongo entityModelMongo = new FormDataEntryMongo();
                        entityModelMongo.ProjectVersion = ProjectVersion;
                        entityModelMongo.ProjectId = ProjectId;
                        entityModelMongo.ProjectGuid = ProjectGuid;
                        entityModelMongo.ProjectName = ProjectName;

                        entityModelMongo.FormId = entity.FormId != null ? (int)entity.FormId : 1;
                        entityModelMongo.FormGuid = entity.FormId != null ? entity.Form.Guid : Guid.Empty;
                        entityModelMongo.FormTitle = entity.FormId != null ? entity.Form.FormTitle : null;

                        entityModelMongo.ActivityId = entity.Activity.Id;
                        entityModelMongo.ActivityGuid = entity.Activity.Guid;
                        entityModelMongo.ActivityName = entity.Activity.ActivityName;

                        entityModelMongo.EntityTypeName = entityTypeDB != null ? entityTypeDB.Name : null;
                        entityModelMongo.EntityTypeId = entityTypeDB != null ? entityTypeDB.Id : entity.EntityId;
                        entityModelMongo.EntityTypeGuid = entityTypeDB != null ? entityTypeDB.Guid : Guid.Empty;

                        entityModelMongo.Status = entity.Status;
                        entityModelMongo.CreatedById = createdBy != null ? createdBy.Id : 2;
                        entityModelMongo.CreatedByGuid = createdBy != null ? createdBy.Guid : Guid.Empty;
                        entityModelMongo.CreatedByName = createdBy != null ? createdBy.FirstName + " " + createdBy.LastName : null;
                        entityModelMongo.CreatedDate = entity.CreatedDate;
                        entityModelMongo.Guid = Guid.Empty;
                        entityModelMongo.ThisUserId = entity.ThisUserId;
                        entityModelMongo.ThisUserName = thisUserIdDetails != null ? thisUserIdDetails.FirstName + " " + thisUserIdDetails.LastName : null;
                        entityModelMongo.ThisUserGuid = thisUserIdDetails != null ? thisUserIdDetails.Guid : (Guid?)null;
                        entityModelMongo.EntityNumber = id;
                        entityModelMongo.ParentEntityNumber = entity.ParentEntityNumber;

                        entityModelMongo.formDataEntryVariableMongoList = new List<FormDataEntryVariableMongo>();
                        FormDataEntryVariableMongo formDataEntrtyVariable = new FormDataEntryVariableMongo();

                        var formVariables = entity.FormDataEntryVariables;
                        foreach (var variable in formVariables)
                        {
                            formDataEntrtyVariable = new FormDataEntryVariableMongo();
                            formDataEntrtyVariable.VariableId = variable.VariableId;
                            formDataEntrtyVariable.VariableGuid = variable.Variable.Guid;
                            formDataEntrtyVariable.VariableName = variable.Variable.VariableName;
                            formDataEntrtyVariable.SelectedValues = variable.SelectedValues;
                            formDataEntrtyVariable.CreatedBy = variable.CreatedBy;
                            formDataEntrtyVariable.CreatedDate = variable.CreatedDate;
                            formDataEntrtyVariable.ParentId = id;

                            entityModelMongo.formDataEntryVariableMongoList.Add(formDataEntrtyVariable);
                        }


                        var addSummaryPageActivity = dbContext.AddActivities.Where(x => x.ProjectId == ProjectId && x.FormDataEntry1.EntityNumber == id);
                        foreach (var addActivity in addSummaryPageActivity)
                        {
                            #region linkage from summary page                
                            var activityAddedBy = dbContext.UserLogins.FirstOrDefault(x => x.Id == addActivity.CreatedBy);
                            SummaryPageActivityViewModel summaryPageActivityViewModel = new SummaryPageActivityViewModel();
                            summaryPageActivityViewModel.ActivityId = addActivity.ActivityId;
                            summaryPageActivityViewModel.ActivityGuid = addActivity.Activity.Guid;
                            summaryPageActivityViewModel.ActivityName = addActivity.Activity.ActivityName;
                            summaryPageActivityViewModel.ActivityCompletedById = addActivity.ActivityCompletedByUser;
                            summaryPageActivityViewModel.ActivityCompletedByGuid = addActivity.UserLogin.Guid;
                            summaryPageActivityViewModel.ActivityCompletedByName = addActivity.UserLogin.FirstName + " " + addActivity.UserLogin.LastName;
                            summaryPageActivityViewModel.ActivityDate = addActivity.ActivityDate;

                            summaryPageActivityViewModel.ProjectGuid = ProjectGuid;
                            summaryPageActivityViewModel.ProjectName = ProjectName;

                            summaryPageActivityViewModel.PersonEntityId = id;
                            summaryPageActivityViewModel.CreatedByName = activityAddedBy != null ? activityAddedBy.FirstName + " " + activityAddedBy.LastName : null;
                            summaryPageActivityViewModel.CreatedDate = addActivity.CreatedDate;

                            summaryPageActivityViewModel.ProjectVersion = ProjectVersion;
                            summaryPageActivityViewModel.SummaryPageActivityFormsList = new List<SummaryPageActivityForms>();
                            foreach (var activityForm in addActivity.Activity.ActivityForms)
                            {
                                summaryPageActivityViewModel.SummaryPageActivityFormsList.Add(new SummaryPageActivityForms()
                                {
                                    FormId = activityForm.FormId,
                                    FormGuid = activityForm.Form.Guid,
                                    FormTitle = activityForm.Form.FormTitle,
                                    FormStatusId = (int)Core.Enum.FormStatusTypes.Draft,
                                    FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), 3),
                                });
                            }

                            var summaryPageActivity = _testMongoDBContext._database.GetCollection<MongoDB.Bson.BsonDocument>("SummaryPageActivity");
                            var summaryPageActivityResult = summaryPageActivity.Insert(summaryPageActivityViewModel);
                            #endregion
                        }

                        var document = _testMongoDBContext._database.GetCollection<MongoDB.Bson.BsonDocument>("UserEntities");
                        var result = document.Insert(entityModelMongo);
                    }
                }
            }
            return true;
        }


        public ProjectStaffMemberRoleViewModel SQL_CheckEntityLinkedProject(Guid projectId, int entityId)
        {
            var currentEntity = dbContext.FormDataEntries.FirstOrDefault(x => x.EntityNumber == entityId);
            int? thisUserId = 0;
            if (currentEntity != null)
            {
                thisUserId = currentEntity.ThisUserId;
            }

            ProjectStaffMemberRole projectStaffMemberRole = dbContext.ProjectStaffMemberRoles.FirstOrDefault(x => x.UserId == thisUserId && x.FormDataEntry.Guid == projectId);
            if (projectStaffMemberRole != null)
            {
                return new ProjectStaffMemberRoleViewModel()
                {
                    ProjectId = projectStaffMemberRole.ProjectId,
                    RoleId = projectStaffMemberRole.RoleId,
                    Guid = projectStaffMemberRole.Guid,
                    Id = projectStaffMemberRole.Id,
                    CreatedBy = projectStaffMemberRole.CreatedBy,
                    UserId = projectStaffMemberRole.UserId,
                };
            }
            return null;
        }
        public ProjectStaffMemberRoleViewModel CheckEntityLinkedProject(Guid projectId, int entityId)
        {
            var conditionEntity = Query<FormDataEntryMongo>.EQ(q => q.EntityNumber, entityId);
            FormDataEntryMongo currentEntity = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionEntity).AsQueryable().FirstOrDefault();
            int? thisUserId = 0;
            if (currentEntity != null)
            {
                thisUserId = currentEntity.ThisUserId;
            }

            ProjectStaffMemberRole projectStaffMemberRole = dbContext.ProjectStaffMemberRoles.FirstOrDefault(x => x.UserId == thisUserId && x.FormDataEntry.Guid == projectId);
            if (projectStaffMemberRole != null)
            {
                return new ProjectStaffMemberRoleViewModel()
                {
                    ProjectId = projectStaffMemberRole.ProjectId,
                    RoleId = projectStaffMemberRole.RoleId,
                    Guid = projectStaffMemberRole.Guid,
                    Id = projectStaffMemberRole.Id,
                    CreatedBy = projectStaffMemberRole.CreatedBy,
                    UserId = projectStaffMemberRole.UserId,
                };
            }
            return null;
        }
        public ProjectStaffMemberRoleViewModel TestEnvironment_CheckEntityLinkedProject(Guid projectId, int entityId)
        {
            var conditionEntity = Query<FormDataEntryMongo>.EQ(q => q.EntityNumber, entityId);
            FormDataEntryMongo currentEntity = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionEntity).FirstOrDefault();
            int? thisUserId = 0;
            if (currentEntity != null)
            {
                thisUserId = currentEntity.ThisUserId;
            }

            ProjectStaffMemberRole projectStaffMemberRole = dbContext.ProjectStaffMemberRoles.FirstOrDefault(x => x.UserId == thisUserId && x.FormDataEntry.Guid == projectId);
            if (projectStaffMemberRole != null)
            {
                return new ProjectStaffMemberRoleViewModel()
                {
                    ProjectId = projectStaffMemberRole.ProjectId,
                    RoleId = projectStaffMemberRole.RoleId,
                    Guid = projectStaffMemberRole.Guid,
                    Id = projectStaffMemberRole.Id,
                    CreatedBy = projectStaffMemberRole.CreatedBy,
                    UserId = projectStaffMemberRole.UserId,
                };
            }
            return null;
        }



        public DateTime? ConvertStringToDate(VariableValidationRule variableValidationRule, string inputVal)
        {

            try
            {
                if (variableValidationRule.ValidationRule.RuleType.Contains("Date_MMMYYYY"))
                {
                    string[] dateSplit = inputVal.Split('-');
                    return Convert.ToDateTime(dateSplit[1] + "-" + dateSplit[0] + "-" + DateTime.Today.Day);
                }
                else if (variableValidationRule.ValidationRule.RuleType.Contains("Date_DDMMMYYYY"))
                {
                    string[] dateSplit = inputVal.Split('-');
                    return Convert.ToDateTime(dateSplit[2] + "-" + dateSplit[1] + "-" + dateSplit[0]);
                }
                else if (variableValidationRule.ValidationRule.RuleType.Contains("Date_YYYY"))
                {
                    return Convert.ToDateTime(inputVal + "-" + DateTime.UtcNow.Month + "-" + DateTime.Today.Day);
                }
                else if (variableValidationRule.ValidationRule.RuleType.Contains("Date_MMYYYY"))
                {
                    string[] dateSplit = inputVal.Split('-');
                    return Convert.ToDateTime(dateSplit[0] + "-" + dateSplit[1] + "-" + DateTime.Today.Day);
                }
                else if (variableValidationRule.ValidationRule.RuleType.Contains("Date_MMDDYYYY"))
                {
                    string[] dateSplit = inputVal.Split('-');
                    return Convert.ToDateTime(dateSplit[2] + "-" + dateSplit[0] + "-" + dateSplit[1]);
                }
                else if (variableValidationRule.ValidationRule.RuleType.Contains("Date"))
                {
                    string[] dateSplit = inputVal.Split('-');
                    return Convert.ToDateTime(dateSplit[2] + "-" + dateSplit[1] + "-" + dateSplit[0]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }
    }
}