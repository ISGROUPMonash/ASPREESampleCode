using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.ViewModels;
using Aspree.Data;
using Aspree.Core.ViewModels.MongoViewModels;
using Aspree.Data.MongoDB;
using MongoDB.Driver.Builders;

namespace Aspree.Provider.Provider
{
    public class SchedulingProvider : ISchedulingProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        private readonly IActivityProvider _activityProvider;
        private readonly IRoleProvider _RoleProvider;
        private readonly MongoDBContext _mongoDBContext;

        public SchedulingProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider, IActivityProvider activityProvider, IRoleProvider roleProvider, MongoDBContext mongoDBContext)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
            this._activityProvider = activityProvider;
            this._RoleProvider = roleProvider;
            this._mongoDBContext = mongoDBContext;
        }
        public SchedulingViewModel Create(SchedulingViewModel model)
        {
            var otherActivity = dbContext.Activities.FirstOrDefault(p => p.Guid == model.OtherActivity);
            var specifiedActivity = dbContext.Activities.FirstOrDefault(p => p.Guid == model.SpecifiedActivity);

            FormDataEntry project = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == model.ProjectId);

            Activity activity = dbContext.Activities.FirstOrDefault(x => x.Guid == model.ActivityId);

            if (activity != null && activity.IsDefaultActivity == (int)Core.Enum.DefaultActivityType.Default)
            {
                throw new Core.BadRequestException("Default activities can not schedule.");
            }

            var activityScheduling = new ActivityScheduling()
            {
                Guid = Guid.NewGuid(),
                ActivityId = activity.Id,
                ScheduledToBeCompleted = model.ScheduledToBeCompleted,
                ActivityAvailableForCreation = model.ActivityAvailableForCreation,
                OtherActivity = otherActivity != null ? otherActivity.Id : (int?)null,
                OffsetCount = model.OffsetCount,
                OffsetType = model.OffsetType,
                SpecifiedActivity = specifiedActivity != null ? specifiedActivity.Id : (int?)null,
                CreationWindowOpens = model.CreationWindowOpens,
                CreationWindowClose = model.CreationWindowClose,
                IsScheduled = true,
                ScheduleDate = DateTime.UtcNow,
                Status = (int)Core.Enum.ActivityDeploymentStatus.Scheduled,
                ProjectId = project.Id,
                CanCreatedMultipleTime = model.CanCreatedMultipleTime,
            };
            dbContext.ActivitySchedulings.Add(activityScheduling);
            SaveChanges();

            var allRoles = dbContext.Roles.ToList();
            #region role to create activity regardless scheduled
            foreach (var schedulingRole in model.RoleToCreateActivityRegardlessScheduled)
            {
                var role = allRoles.FirstOrDefault(x => x.Guid == schedulingRole);
                this.dbContext.RoleToCreateActivityRegardlessScheduleds.Add(new RoleToCreateActivityRegardlessScheduled()
                {
                    Guid = Guid.NewGuid(),
                    ActivitySchedulingId = activityScheduling.Id,
                    RoleId = role != null ? role.Id : 0,
                });
            }
            #endregion
            #region role to create activity scheduling
            foreach (var schedulingRole in model.RolesToCreateActivity)
            {
                var role = allRoles.FirstOrDefault(x => x.Guid == schedulingRole);
                this.dbContext.RolesToCreateActivitySchedulings.Add(new RolesToCreateActivityScheduling()
                {
                    Guid = Guid.NewGuid(),
                    ActivitySchedulingId = activityScheduling.Id,
                    RoleId = role != null ? role.Id : 0,
                });
            }
            #endregion
            SaveChanges();
            return GetByGuid(activity.Guid);
        }
        public SchedulingViewModel Update(SchedulingViewModel model)
        {
            var activityScheduling = dbContext.ActivitySchedulings
              .FirstOrDefault(fs => fs.Guid == model.Guid);
            if (activityScheduling != null)
            {
                if (activityScheduling.Activity != null && activityScheduling.Activity.IsDefaultActivity == (int)Core.Enum.DefaultActivityType.Default)
                {
                    throw new Core.BadRequestException("Default activities can not schedule.");
                }

                var otherActivity = dbContext.Activities.FirstOrDefault(p => p.Guid == model.OtherActivity);
                var specifiedActivity = dbContext.Activities.FirstOrDefault(p => p.Guid == model.SpecifiedActivity);

                activityScheduling.ScheduledToBeCompleted = model.ScheduledToBeCompleted;
                activityScheduling.ActivityAvailableForCreation = model.ActivityAvailableForCreation;
                activityScheduling.OtherActivity = otherActivity != null ? otherActivity.Id : (int?)null;

                activityScheduling.OffsetCount = model.OffsetCount;
                activityScheduling.OffsetType = model.OffsetType;
                activityScheduling.SpecifiedActivity = specifiedActivity != null ? specifiedActivity.Id : (int?)null;
                activityScheduling.CreationWindowOpens = model.CreationWindowOpens;
                activityScheduling.CreationWindowClose = model.CreationWindowClose;
                activityScheduling.IsScheduled = true;
                activityScheduling.ScheduleDate = DateTime.UtcNow;
                activityScheduling.CanCreatedMultipleTime = model.CanCreatedMultipleTime;

                SaveChanges();

                var allRoles = dbContext.Roles.ToList();

                IEnumerable<RoleToCreateActivityRegardlessScheduled> roleToCreateActivityRegardlessScheduleds = dbContext.RoleToCreateActivityRegardlessScheduleds.Where(x => x.ActivitySchedulingId == activityScheduling.Id).ToList();
                dbContext.RoleToCreateActivityRegardlessScheduleds.RemoveRange(roleToCreateActivityRegardlessScheduleds);

                IEnumerable<RolesToCreateActivityScheduling> rolesToCreateActivityScheduling = dbContext.RolesToCreateActivitySchedulings.Where(x => x.ActivitySchedulingId == activityScheduling.Id).ToList();
                dbContext.RolesToCreateActivitySchedulings.RemoveRange(rolesToCreateActivityScheduling);

                #region role to create activity regardless scheduled
                foreach (var schedulingRole in model.RoleToCreateActivityRegardlessScheduled)
                {
                    var role = allRoles.FirstOrDefault(x => x.Guid == schedulingRole);
                    this.dbContext.RoleToCreateActivityRegardlessScheduleds.Add(new RoleToCreateActivityRegardlessScheduled()
                    {
                        Guid = Guid.NewGuid(),
                        ActivitySchedulingId = activityScheduling.Id,
                        RoleId = role != null ? role.Id : 0,
                    });
                }
                #endregion
                #region role to create activity scheduling
                foreach (var schedulingRole in model.RolesToCreateActivity)
                {
                    var role = allRoles.FirstOrDefault(x => x.Guid == schedulingRole);
                    this.dbContext.RolesToCreateActivitySchedulings.Add(new RolesToCreateActivityScheduling()
                    {
                        Guid = Guid.NewGuid(),
                        ActivitySchedulingId = activityScheduling.Id,
                        RoleId = role != null ? role.Id : 0,
                    });
                }
                #endregion
                SaveChanges();
                return GetByGuid(model.Guid);
            }
            return null;
        }
        public SchedulingViewModel GetByGuid(Guid guid)
        {
            var scheduling = dbContext.ActivitySchedulings
             .FirstOrDefault(fs => fs.Guid == guid);

            if (scheduling != null)
                return ToModel(scheduling);

            return null;
        }
        public SchedulingViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            throw new NotImplementedException();
        }
        public SchedulingViewModel DeleteById(int id, Guid DeletedBy)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<SchedulingViewModel> GetAll()
        {
            return dbContext.ActivitySchedulings
            .Select(ToModel)
            .ToList();
        }
        public IEnumerable<SchedulingViewModel> GetAll(Guid tenantId)
        {
            throw new NotImplementedException();
        }
        public SchedulingViewModel GetById(int id)
        {
            var form = dbContext.ActivitySchedulings
            .FirstOrDefault(fs => fs.Id == id);
            if (form != null)
                return ToModel(form);

            return null;
        }
        public SchedulingViewModel ToModel(ActivityScheduling scheduling)
        {
            var rolesToCreateActivity = dbContext.Roles.FirstOrDefault(x => x.Id == scheduling.RolesToCreateActivity);
            var roleToCreateActivityRegardlessScheduled = dbContext.Roles.FirstOrDefault(x => x.Id == scheduling.RoleToCreateActivityRegardlessScheduled);
            var otherActivity = dbContext.Activities.FirstOrDefault(p => p.Id == scheduling.OtherActivity);
            var specifiedActivity = dbContext.Activities.FirstOrDefault(p => p.Id == scheduling.SpecifiedActivity);
            var activity = dbContext.Activities.FirstOrDefault(x => x.Id == scheduling.ActivityId);
            var project = dbContext.FormDataEntries.FirstOrDefault(x => x.Id == scheduling.ProjectId);

            List<Guid> otherActivities = new List<Guid>();
            try
            {
                var activityUsedin = dbContext.ActivitySchedulings.Where(x => x.OtherActivity == scheduling.ActivityId && x.IsScheduled == true).AsQueryable();
                if (activityUsedin != null)
                {
                    activityUsedin.ToList().ForEach(act =>
                    {
                        otherActivities.Add(act.Activity.Guid);
                    });
                }
            }
            catch (Exception ex) { }
            List<Guid> usedAsSpecifiedActivitiesList = new List<Guid>();
            try
            {
                var activitySpecifiedActivities = dbContext.ActivitySchedulings.Where(x => x.SpecifiedActivity == scheduling.ActivityId && x.IsScheduled == true).AsQueryable();
                if (activitySpecifiedActivities != null)
                {
                    activitySpecifiedActivities.ToList().ForEach(act =>
                    {
                        usedAsSpecifiedActivitiesList.Add(act.Activity.Guid);
                    });
                }
            }
            catch (Exception ex) { }
            return new SchedulingViewModel()
            {
                Id = scheduling.Id,
                ActivityId = scheduling.Activity.Guid,
                ActivityName = scheduling.Activity.ActivityName,
                ScheduledToBeCompleted = scheduling.ScheduledToBeCompleted,
                ActivityAvailableForCreation = scheduling.ActivityAvailableForCreation,
                OtherActivity = otherActivity != null ? otherActivity.Guid : (Guid?)null,
                OffsetCount = scheduling.OffsetCount,
                OffsetType = scheduling.OffsetType,
                SpecifiedActivity = specifiedActivity != null ? specifiedActivity.Guid : (Guid?)null,
                CreationWindowOpens = scheduling.CreationWindowOpens,
                CreationWindowClose = scheduling.CreationWindowClose,
                IsScheduled = scheduling.IsScheduled,
                ScheduleDate = scheduling.ScheduleDate,
                Guid = scheduling.Guid,

                RolesToCreateActivity = dbContext.RolesToCreateActivitySchedulings.Where(x => x.ActivitySchedulingId == scheduling.Id).Select(s => s.Role.Guid).ToList(),
                RolesToCreateActivity_Name = dbContext.RolesToCreateActivitySchedulings.Where(x => x.ActivitySchedulingId == scheduling.Id).Select(s => s.Role.Name).ToList(),

                RoleToCreateActivityRegardlessScheduled = dbContext.RoleToCreateActivityRegardlessScheduleds.Where(x => x.ActivitySchedulingId == scheduling.Id).Select(s => s.Role.Guid).ToList(),
                RoleToCreateActivityRegardlessScheduled_Name = dbContext.RoleToCreateActivityRegardlessScheduleds.Where(x => x.ActivitySchedulingId == scheduling.Id).Select(s => s.Role.Name).ToList(),

                EntityTypes = activity != null ? activity.ActivityEntityTypes.Select(c => c.EntityType.Guid).ToList() : new List<Guid>(),
                Status = scheduling.Status,

                ProjectId = project.Guid,
                IsDefaultActivity = activity.IsDefaultActivity,
                CanCreatedMultipleTime = scheduling.CanCreatedMultipleTime,

                UsedAsOtherActivitiesList = otherActivities,
                UsedAsSpecifiedActivitiesList = usedAsSpecifiedActivitiesList,

                ActivityStatusId = scheduling.Activity.ActivityStatusId,
            };
        }
        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        public SchedulingViewModel GetByActivityGuid(Guid guid)
        {
            var scheduling = dbContext.ActivitySchedulings
             .FirstOrDefault(fs => fs.Activity.Guid == guid);

            if (scheduling != null)
                return ToModel(scheduling);
            return null;
        }
        public IEnumerable<SchedulingViewModel> GetAllScheduledActivityByProjectId(Guid projectId)
        {
            return dbContext.ActivitySchedulings.Where(x => x.IsScheduled == true && x.Activity.FormDataEntry.Guid == projectId)
             .Select(ToModel)
             .OrderByDescending(x => x.ScheduleDate)
             .ToList();
        }

        public bool PushScheduledActivities(List<Guid> activitiesList, int statusType, Guid loggedInUserGuid)
        {
            try
            {
                var activityResult = dbContext.ActivitySchedulings.Where(x => activitiesList.Contains(x.Guid)).ToList();
                var projectId = activityResult.FirstOrDefault().FormDataEntry.Guid;

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
                            //deployedFormsId = u.Activity.ActivityForms.Select(x=>x.FormId).ToList();
                            deployedFormsId.AddRange(u.Activity.ActivityForms.Select(x => x.FormId).ToList());
                            deployedActivitiesId.Add(u.ActivityId);
                        }
                        else
                        {
                            u.Status = (int)Core.Enum.ActivityDeploymentStatus.Scheduled;
                        }
                    }
                });
                if (deployedFormsId.Count() > 0)
                {
                    var deployedFormsResult = dbContext.Forms.Where(x => deployedFormsId.Contains(x.Id)).ToList();
                    deployedFormsResult.ToList().ForEach(f =>
                    {
                        f.IsPublished = true;
                        f.FormState = (int)Core.Enum.FormStateTypes.Published;
                        f.FormStatusId = (int)Core.Enum.FormStatusTypes.Published;
                    });
                }
                if (deployedActivitiesId.Count() > 0)
                {
                    var deployedActivitiesResult = dbContext.Activities.Where(x => deployedActivitiesId.Contains(x.Id)).ToList();
                    deployedActivitiesResult.ForEach(a =>
                    {
                        a.ActivityStatusId = (int)Core.Enum.ActivityStatusTypes.Active;
                    });
                }
                SaveChanges();
                var projectJson = getFullProjectJsonData(projectId, statusType, loggedInUserGuid);

                var prid = activityResult.FirstOrDefault().FormDataEntry.Id;
                var chkProjectBuilderJSON = dbContext.ProjectBuilderJSONs.FirstOrDefault(x => x.ProjectId == prid);
                if (chkProjectBuilderJSON == null)
                {
                    this.dbContext.ProjectBuilderJSONs.Add(new Data.ProjectBuilderJSON
                    {
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = 1,
                        Guid = Guid.NewGuid(),
                        ProjectBuilderJSONData = projectJson,
                        ProjectId = prid,
                        ProjectType = statusType,
                        Version = .1,
                    });
                }
                else
                {
                    var oldJson = chkProjectBuilderJSON.ProjectBuilderJSONData;
                    DeployProjectJsonViewModel deserialized = new System.Web.Script.Serialization.JavaScriptSerializer().
                        Deserialize<Core.ViewModels.DeployProjectJsonViewModel>(chkProjectBuilderJSON.ProjectBuilderJSONData);
                    if (deserialized.AddSummaryPageActivityViewModelList == null)
                    {
                        deserialized.AddSummaryPageActivityViewModelList = new List<AddSummaryPageActivityViewModel>();
                    }
                    var oldAc = deserialized.AddSummaryPageActivityViewModelList;
                    DeployProjectJsonViewModel newDeserialized = new System.Web.Script.Serialization.JavaScriptSerializer().
                        Deserialize<Core.ViewModels.DeployProjectJsonViewModel>(projectJson);
                    if (newDeserialized.AddSummaryPageActivityViewModelList == null)
                    {
                        newDeserialized.AddSummaryPageActivityViewModelList = new List<AddSummaryPageActivityViewModel>();
                    }
                    newDeserialized.AddSummaryPageActivityViewModelList.AddRange(oldAc);
                    var newJson = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(newDeserialized);
                    chkProjectBuilderJSON.ModifiedDate = DateTime.UtcNow;
                    chkProjectBuilderJSON.ModifiedBy = 1;
                    chkProjectBuilderJSON.ProjectBuilderJSONData = newJson;
                    chkProjectBuilderJSON.ProjectType = statusType;
                    chkProjectBuilderJSON.Version = .1;
                }
                SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public string getFullProjectJsonData(Guid projectGuid, int projectType, Guid loggedInUserGuid)
        {
            var project = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == projectGuid);
            var formdataentry = dbContext.FormDataEntryVariables.FirstOrDefault(x => x.FormDataEntryId == project.Id && x.Variable.VariableName == "Name");
            DeployProjectJsonViewModel model = new DeployProjectJsonViewModel();
            model.ProjectGuid = project.Guid;
            model.ProjectId = project.Id;
            model.ProjectName = formdataentry.SelectedValues;
            model.ProjectVersion = 1;
            if (projectType == (int)Core.Enum.ActivityDeploymentStatus.Pushed)
            {
                model.State = "Testing";
            }
            else if (projectType == (int)Core.Enum.ActivityDeploymentStatus.Deployed)
            {
                model.State = "Live";
            }
            var allActivities = dbContext.ActivitySchedulings.Where(x => x.ProjectId == project.Id && x.Status == projectType).Select(ToJsonSchedulingViewModel).ToList();
            model.JsonSchedulingViewModelList = allActivities;
            var allSystemAdmin = dbContext.UserRoles.Where(x => x.Role.Name == "System Admin").Select(x => x.UserLogin).ToList();
            model.projectStaffMemberList = new List<JsonProjectStaffMembers>();
            foreach (var user in allSystemAdmin)
            {
                if (user != null)
                {
                    JsonProjectStaffMembers staff = new JsonProjectStaffMembers();
                    staff.ProjectGuid = project.Guid;
                    staff.ProjectId = project.Id;
                    staff.ProjectName = formdataentry.SelectedValues;
                    staff.ProjectUserGuid = user.Guid;
                    staff.ProjectUserId = user.Id;
                    staff.ProjectUserName = user.FirstName + " " + user.LastName;
                    staff.ProjectUserRoleGuid = user.UserRoles.FirstOrDefault().Role.Guid;
                    staff.ProjectUserRoleId = user.UserRoles.FirstOrDefault().RoleId;
                    staff.ProjectUserRoleName = user.UserRoles.FirstOrDefault().Role.Name;
                    model.projectStaffMemberList.Add(staff);
                }
            }
            var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(model);
            return json;
        }

        public JsonSchedulingViewModel ToJsonSchedulingViewModel(ActivityScheduling scheduling)
        {
            var rolesToCreateActivity = dbContext.RolesToCreateActivitySchedulings.Where(x => x.ActivitySchedulingId == scheduling.RolesToCreateActivity).Select(x => x.Role.Guid).ToList();
            var roleToCreateActivityRegardlessScheduled = dbContext.RoleToCreateActivityRegardlessScheduleds.Where(x => x.ActivitySchedulingId == scheduling.RoleToCreateActivityRegardlessScheduled).Select(x => x.Role.Guid).ToList();
            var otherActivity = dbContext.Activities.FirstOrDefault(p => p.Id == scheduling.OtherActivity);
            var specifiedActivity = dbContext.Activities.FirstOrDefault(p => p.Id == scheduling.SpecifiedActivity);
            var project = dbContext.FormDataEntries.FirstOrDefault(p => p.Id == scheduling.ProjectId);

            return new JsonSchedulingViewModel()
            {
                ProjectId = project.Guid,
                schedulingId = scheduling.Id,
                SchedulingGuid = scheduling.Guid,

                ActivityId = scheduling.Activity.Id,
                ActivityName = scheduling.Activity.ActivityName,
                ScheduledToBeCompleted = scheduling.ScheduledToBeCompleted,
                ActivityAvailableForCreation = scheduling.ActivityAvailableForCreation,
                RolesToCreateActivity = rolesToCreateActivity,
                RoleToCreateActivityRegardlessScheduled = roleToCreateActivityRegardlessScheduled,
                OtherActivity = otherActivity != null ? otherActivity.Guid : (Guid?)null,
                OffsetCount = scheduling.OffsetCount,
                OffsetType = scheduling.OffsetType,
                SpecifiedActivity = specifiedActivity != null ? specifiedActivity.Guid : (Guid?)null,
                CreationWindowOpens = scheduling.CreationWindowOpens,
                CreationWindowClose = scheduling.CreationWindowClose,
                IsScheduled = scheduling.IsScheduled,
                ScheduleDate = scheduling.ScheduleDate != null ? scheduling.ScheduleDate.Value.ToString("dd-MMM-yyyy") : string.Empty,
                Status = scheduling.Status,
                JsonActivityViewModelData = dbContext.Activities.Where(x => x.Id == scheduling.ActivityId).Select(ToJsonActivityViewModel).FirstOrDefault()
            };
        }

        public JsonActivityViewModel ToJsonActivityViewModel(Activity entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;

            var activity = entity.DependentActivityId.HasValue ? dbContext.Activities.FirstOrDefault(p => p.Id == entity.DependentActivityId) : null;

            var entityTypeName = string.Empty;
            var entityTypeId = 0;
            var entityTypeGuid = Guid.Empty;
            var activityEntityType = entity.ActivityEntityTypes.FirstOrDefault();
            if (activityEntityType != null)
            {
                entityTypeName = activityEntityType.EntityType.Name;
                entityTypeId = activityEntityType.EntityTypeId;
                entityTypeGuid = activityEntityType.EntityType.Guid;
            }


            List<FormViewModel> jsonFormViewModelList = new List<FormViewModel>();
            FormViewModel jsonFormViewModel = new FormViewModel();
            foreach (var form in entity.ActivityForms)
            {
                jsonFormViewModel = new FormViewModel();
                jsonFormViewModelList.Add(ToActivityFormModel(form.Form, 0));
            }

            return new JsonActivityViewModel()
            {
                ActivityId = entity.Id,
                ActivityGuid = entity.Guid,
                ActivityName = entity.ActivityName,

                ActivityCategoryId = entity.ActivityCategory.Id,
                ActivityCategoryGuid = entity.ActivityCategory.Guid,
                ActivityCategoryName = entity.ActivityCategory.CategoryName,

                ScheduleType = entity.ScheduleType,
                ActivityStatusId = entity.ActivityStatu.Guid,

                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                CreatedDateString = entity.CreatedDate != null ? entity.CreatedDate.ToString("dd-MMM-yyyy") : string.Empty,
                ProjectId = entity.FormDataEntry.Guid,
                TenantId = entity.Tenant.Guid,

                EntityTypeId = entityTypeId,
                EntityTypeGuid = entityTypeGuid,
                EntityTypeName = entityTypeName,

                PreviewSchedulingDone = entity.PreviewSchedulingDone ?? false,
                IsDefaultActivity = entity.IsDefaultActivity,
                IsActivityRequireAnEntity = entity.IsActivityRequireAnEntity,

                JsonFormViewModelList = jsonFormViewModelList,
            };
        }
        public bool InactivateActivity(List<Guid> activityIdList, int statusType, Guid LoggedInUserId, Guid projectId)
        {

            List<Int32> deployedActivitiesId = new List<int>();
            List<Guid> deployedActivitiesGuId = new List<Guid>();

            var allActivitySchedulings = dbContext.ActivitySchedulings.Where(x => activityIdList.Contains(x.Activity.Guid)).ToList();
            allActivitySchedulings.ToList().ForEach(u =>
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
                    }
                    else
                    {
                        u.Status = (int)Core.Enum.ActivityDeploymentStatus.Scheduled;
                        deployedActivitiesId.Add(u.ActivityId);
                        deployedActivitiesGuId.Add(u.Activity.Guid);
                    }

                }
            });
            if (deployedActivitiesId.Count() > 0)
            {
                var deployedActivitiesResult = dbContext.Activities.Where(x => deployedActivitiesId.Contains(x.Id)).ToList();
                deployedActivitiesResult.ForEach(a =>
                {
                    a.ActivityStatusId = (int)Core.Enum.ActivityStatusTypes.Draft;
                });
            }
            var condition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectId);
            var project = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
            if (project != null)
            {
                var oldActivities = project.ProjectActivitiesList;
                deployedActivitiesGuId.ForEach(c =>
                {
                    var d = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == c);
                    if (d != null)
                    {
                        oldActivities.FirstOrDefault(x => x.ActivityGuid == c).DateDeactivated = DateTime.UtcNow;
                        oldActivities.FirstOrDefault(x => x.ActivityGuid == c).DeactivatedBy = LoggedInUserId;
                    }
                });
                project.ProjectActivitiesList = oldActivities;
                var userObjectid = Query<ProjectDeployViewModel>.EQ(p => p.Id, project.Id);
                // Document Collections
                var collection = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects");
                // Document Upsdate which need Id and Data to Update  
                var result = collection.Update(userObjectid, MongoDB.Driver.Builders.Update.Replace(project), MongoDB.Driver.UpdateFlags.None);
            }
            SaveChanges();
            return true;
        }


        public FormViewModel ToActivityFormModel(Form entity, int entId)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;

            var project = entity.ProjectId.HasValue ? dbContext.Projects.FirstOrDefault(p => p.Id == entity.ProjectId) : null;

            return new FormViewModel()
            {
                Id = entity.Id,
                Guid = entity.Guid,
                ApprovedBy = entity.ApprovedBy,
                ApprovedDate = entity.ApprovedDate,
                Version = entity.Version,
                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                FormCategoryId = entity.FormCategory.Guid,
                FormState = entity.FormState,
                FormStatusId = entity.FormStatu.Guid,
                FormTitle = entity.FormTitle,
                IsPublished = entity.IsPublished,
                IsTemplate = entity.IsTemplate,
                PreviousVersion = entity.PreviousVersion,
                ProjectId = project != null ? project.Guid : (Guid?)null,
                TenantId = entity.Tenant.Guid,
                Variables = entity.FormVariables.Where(x => x.DateDeactivated == null).Select(c => new FormVariableViewModel()
                {
                    VariableId = c.Variable.Guid,
                    VariableName = c.Variable.VariableName,
                    FormVariableRoles = c.FormVariableRoles.Select(fv => fv.Role.Guid).ToList(),
                    ValidationRuleType = c.ValidationRuleType,
                    HelpText = c.HelpText,
                    IsRequired = c.IsRequired,
                    MaxRange = c.MaxRange,
                    MinRange = c.MinRange,
                    RegEx = c.RegEx,

                    DependentVariableId = c.DependentVariableId != null ? dbContext.Variables.Where(x => x.Id == c.DependentVariableId).Select(s => s.Guid).FirstOrDefault() : (Guid?)null,//.DependentVariableId,
                                                                                                                                                                                           //                    DependentVariableId = dbContext.Variables.Where(x => x.Id == c.DependentVariableId).Select(s => s.Guid).FirstOrDefault(),//.DependentVariableId,
                    ResponseOption = c.ResponseOption,
                    ValidationMessage = c.ValidationMessage,
                    VariableType = c.Variable.VariableType.Type,
                    variableViewModel = ToActivityFormVariableModel(c.Variable, entity.Id, entId),

                    IsSearchVisible = c.IsSearchVisible,
                    SearchPageOrder = c.SearchPageOrder,

                    formVariableRoleViewModel = c.FormVariableRoles.Select(f => new FormVariableRoleViewModel()
                    {
                        FormVariableId = f.FormVariableId,
                        RoleGuidId = f.Role.Guid,
                        Guid = f.Guid,
                        CanCreate = f.CanCreate,
                        CanView = f.CanView,
                        CanEdit = f.CanEdit,
                        CanDelete = f.CanDelete,
                    }).ToList(),
                    QuestionText = c.QuestionText,
                    IsDefaultVariableType = c.Variable.IsDefaultVariable,

                }).ToList(),
                EntityTypes = entity.FormEntityTypes.Select(c => c.EntityType.Guid).ToList(),
                IsDefaultForm = entity.IsDefaultForm,
            };
        }
        public VariableViewModel ToActivityFormVariableModel(Variable model, int formId, int selectedValue)
        {
            var ValidationRuleIds = this.dbContext.VariableValidationRules.Where(v => v.VariableId == model.Id).Select(x => x.ValidationId).ToList();
            var variableValidationRuleGuidIds = this.dbContext.ValidationRules
                .Where(r => ValidationRuleIds.Contains(r.Id)).Select(x => x.Guid)
                .ToList();

            var listOfRule = this.dbContext.VariableValidationRules.Where(x => x.VariableId == model.Id).Select(ToVariableValidationRuleViewModel).ToList();

            var FormDataEntryVariables = this.dbContext.FormDataEntryVariables.Where(x => x.Variable.VariableName == "EntID" && x.SelectedValues == selectedValue.ToString());
            int formdataentryid = 0;
            try
            {
                formdataentryid = FormDataEntryVariables.FirstOrDefault(x => x.SelectedValues == selectedValue.ToString() && x.Variable.VariableName == "EntID").FormDataEntryId;
            }
            catch (Exception e)
            {}

            if (model.VariableName == "AuthenticationMethod")
            {
                var authTypeId = dbContext.LoginAuthTypeMasters.Where(x => x.DateDeactivated == null).Select(x => new { id = x.Guid.ToString(), name = x.AuthTypeName }).ToList();

                model.Values = string.Join("|", authTypeId.Select(x => x.id).ToList());
                model.VariableValueDescription = string.Join("|", authTypeId.Select(x => x.name).ToList());
            }
            var formDataEntryVariables = this.dbContext.FormDataEntryVariables.FirstOrDefault(x => x.FormDataEntryId == formdataentryid && x.VariableId == model.Id);
            var formDataEntry = this.dbContext.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == "EntID" && x.SelectedValues == selectedValue.ToString());
            var variableEntityType = this.dbContext.VariableEntityTypes.FirstOrDefault(x => x.VariableId == model.Id);

            Guid? usernameGuid = null;
            if (model.VariableName == "Username")
            {
                var email = formDataEntryVariables != null ? formDataEntryVariables.SelectedValues : null;
                try
                {
                    var username = dbContext.UserLogins.FirstOrDefault(x => x.UserName == email && x.Id == formDataEntry.FormDataEntry.ThisUserId);
                    if (username != null)
                    {
                        usernameGuid = username.Guid;
                    }
                }
                catch (Exception ex) { }
            }

            if (model.VariableName == "Email")
            {
                var email = formDataEntryVariables != null ? formDataEntryVariables.SelectedValues : null;
                try
                {
                    var username = dbContext.UserLogins.FirstOrDefault(x => x.Email == email && x.Id == formDataEntry.FormDataEntry.ThisUserId);
                    if (username != null)
                    {
                        usernameGuid = username.Guid;
                    }
                }
                catch (Exception ex) { }
            }
            if (model.VariableType.Type == "LKUP")
            {
                var entType = model.VariableEntityTypes.FirstOrDefault();
                int entTypeid = entType != null ? entType.EntityTypeId : 0;
                var entTypes = dbContext.EntityTypes.FirstOrDefault(x => x.Id == entTypeid);
                int? entsubtype = null;
                string entsubtypeID = string.Empty;
                if (model.VariableEntityTypes.Count == 1)
                {
                    entsubtype = entType != null ? entType.EntitySubTypeId : (int?)null;
                }
                string entTypeName = entTypes != null ? entTypes.Name : string.Empty;
                if (entTypeName == "Person") { entTypeName = "Person Registration"; }
                else if (entTypeName == "Project") { entTypeName = "Project Registration"; }
                else if (entTypeName == "Participant") { entTypeName = "Participant Registration"; }
                else if (entTypeName == "Place/Group") { entTypeName = "Place/Group Registration"; }

                List<string> entityValues = new List<string>();
                List<string> entityText = new List<string>();
                var getAllformWithentTypeid = dbContext.FormEntityTypes.Where(x => x.EntityTypeId == entTypeid).Select(x => x.FormId).ToList();
                var formDataEntryEntityTypesList = dbContext.FormDataEntries.Where(t => getAllformWithentTypeid.Contains((int)t.FormId)).ToList();

                if (formDataEntryEntityTypesList.Count() > 0)
                {
                    foreach (var proj in formDataEntryEntityTypesList)
                    {
                        var text = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                        var valuse = proj.Guid;

                        if (entsubtype != null)
                        {
                            text = null;
                            if (entTypeName == "Person Registration")
                            {
                                var persuntype = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "PerSType").Select(x => x.SelectedValues).FirstOrDefault();
                                var entSubTyp = entsubtype != null ? entsubtype.ToString() : string.Empty;
                                if (!string.IsNullOrEmpty(entSubTyp))
                                {
                                    if (persuntype == entSubTyp)
                                    {
                                        var text1 = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "FirstName").Select(x => x.SelectedValues).FirstOrDefault();
                                        var text2 = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                                        text = text1 + " " + text2;
                                    }
                                }
                            }
                            else if (entTypeName == "Project Registration")
                            {
                                var projectsubtype = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "ProSType").Select(x => x.SelectedValues).FirstOrDefault();
                                var entSubTyp = entsubtype != null ? entsubtype.ToString() : string.Empty;
                                if (!string.IsNullOrEmpty(entSubTyp))
                                {
                                    if (projectsubtype == entSubTyp)
                                    {
                                        text = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                                    }
                                }
                            }
                        }
                        if (text != null)
                        {
                            entityValues.Add(valuse.ToString());
                            entityText.Add(text);
                        }
                    }
                    model.Values = entityValues != null ? string.Join("|", entityValues) : string.Empty;
                    model.VariableValueDescription = entityText != null ? string.Join("|", entityText) : string.Empty;
                }
            }

            return new VariableViewModel()
            {
                Guid = model.Guid,
                VariableName = model.VariableName,
                Id = model.Id,
                CanCollectMultiple = model.CanCollectMultiple,
                HelpText = model.HelpText,
                IsApproved = model.IsApproved,
                IsRequired = model.IsRequired,
                IsSoftRange = model.IsSoftRange,
                MaxRange = model.MaxRange,
                MinRange = model.MinRange,
                Question = model.Question,
                RegEx = model.RegEx,
                RequiredMessage = model.RequiredMessage,
                ValidationMessage = model.ValidationMessage,

                ValueDescription = model.ValueDescription,
                Values = model.Values.Split('|').ToList(),

                VariableLabel = model.VariableLabel,
                ModifiedDate = model.ModifiedDate,
                DateDeactivated = model.DateDeactivated,
                VariableRoles = model.VariableRoles.Select(r => r.Role.Guid).ToList(),
                VariableTypeName = model.VariableType.Type,
                Comment = model.Comment,

                ValidationRuleIds = variableValidationRuleGuidIds,
                variableValidationRuleViewModel = listOfRule,
                VariableValueDescription = model.VariableValueDescription != null ? model.VariableValueDescription.Split('|').ToList() : null,
                VariableSelectedValues = formDataEntryVariables != null ? formDataEntryVariables.SelectedValues : "",
                FormDataEntryGuid = formDataEntry != null ? formDataEntry.FormDataEntry.Guid : (Guid?)null,

                LookupEntityType = variableEntityType != null ? variableEntityType.EntityType != null ? variableEntityType.EntityType.Guid : (Guid?)null : (Guid?)null,
                LookupEntitySubtype = variableEntityType != null ? variableEntityType.EntitySubType != null ? variableEntityType.EntitySubType.Guid : (Guid?)null : (Guid?)null,

                LookupEntityTypeName = variableEntityType != null ? variableEntityType.EntityType != null ? variableEntityType.EntityType.Name : null : null,
                LookupEntitySubtypeName = variableEntityType != null ? variableEntityType.EntitySubType != null ? variableEntityType.EntitySubType.Name : null : null,
                CanFutureDate = model.CanFutureDate,
                UserNameVariableGuid = usernameGuid,
            };
        }
        public VariableValidationRuleViewModel ToVariableValidationRuleViewModel(VariableValidationRule data)
        {
            return new VariableValidationRuleViewModel()
            {
                Id = data.Id,
                Guid = data.Guid,
                LimitType = data.LimitType,
                Max = data.Max,
                Min = data.Min,
                RegEx = data.RegEx,
                ValidationId = data.ValidationId,
                ValidationMessage = data.ValidationMessage,
                VariableId = data.VariableId,
                ValidationName = data.ValidationRule != null ? data.ValidationRule.RuleType : "Custom",
            };
        }

    }
}