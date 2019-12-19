using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.ViewModels;
using Aspree.Data;
using Aspree.Core.Enum;
using Aspree.Data.MongoDB;
using Aspree.Core.ViewModels.MongoViewModels;

namespace Aspree.Provider.Provider
{
    public class ActivityProvider : IActivityProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        private readonly IActivityCategoryProvider _activityCategoryProvider;
        private readonly IRoleProvider _RoleProvider;
        private readonly IFormCategoryProvider _FormCategoryProvider;
        private readonly IEntityTypeProvider _EntityTypeProvider;
        private readonly MongoDBContext _mongoDBContext;
        static List<string> resultid = new List<string>();

        public ActivityProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider, IActivityCategoryProvider activityCategoryProvider, IRoleProvider RoleProvider, IFormCategoryProvider FormCategoryProvider, IEntityTypeProvider EntityTypeProvider, MongoDBContext mongoDBContext)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
            this._activityCategoryProvider = activityCategoryProvider;
            this._RoleProvider = RoleProvider;
            this._FormCategoryProvider = FormCategoryProvider;
            this._EntityTypeProvider = EntityTypeProvider;
            this._mongoDBContext = mongoDBContext;
            resultid = new List<string>();
        }

        public ActivityViewModel Create(ActivityViewModel model)
        {
            if (dbContext.Activities.Any(et => et.FormDataEntry.Guid == model.ProjectId && et.ActivityName.ToLower() == model.ActivityName.ToLower() && et.DateDeactivated == null))
            {
                throw new Core.AlreadyExistsException("Activity already exists.");
            }
            if (model.Forms == null || model.Forms.Count == 0)
            {
                throw new Core.BadRequestException("Please select atleast 1 form.");
            }
            if (model.Forms.Any(c => c.Roles == null || c.Roles.Count == 0))
            {
                //throw new Core.BadRequestException("Please select form roles for activity.");
            }
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            var activityCategory = this.dbContext.ActivityCategories.FirstOrDefault(et => et.Guid == model.ActivityCategoryId);
            var tenant = this.dbContext.Tenants.FirstOrDefault(et => et.Guid == model.TenantId);
            var acitvityStatus = this.dbContext.ActivityStatus.FirstOrDefault(et => et.Status == "Draft");
            var dependentActivity = model.DependentActivityId.HasValue ? dbContext.Activities.FirstOrDefault(p => p.Guid == model.DependentActivityId) : null;
            var project = dbContext.FormDataEntries.FirstOrDefault(p => p.Guid == model.ProjectId);
            var activity = new Activity()
            {
                Guid = Guid.NewGuid(),
                ActivityName = model.ActivityName,
                ActivityCategoryId = activityCategory != null ? activityCategory.Id : (int?)null,
                ActivityStatusId = acitvityStatus.Id,
                DependentActivityId = dependentActivity != null ? dependentActivity.Id : (int?)null,
                EndDate = model.EndDate,
                RepeatationCount = model.RepeatationCount,
                RepeatationOffset = model.RepeatationOffset,
                RepeatationType = model.RepeatationType,
                ScheduleType = model.ScheduleType,
                StartDate = model.StartDate,
                ProjectId = project.Id,
                TenantId = tenant.Id,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
                IsDefaultActivity = (int)Core.Enum.DefaultActivityType.Custom,
                IsActivityRequireAnEntity = model.IsActivityRequireAnEntity,
            };
            dbContext.Activities.Add(activity);
            SaveChanges();

            var varIds = model.Forms.Select(v => v.Id).ToList();
            var roleIds = model.Forms.SelectMany(v => v.Roles).ToList();
            var forms = dbContext.Forms.Where(c => varIds.Contains(c.Guid)).ToList();
            var roles = dbContext.Roles.Where(c => roleIds.Contains(c.Guid)).ToList();
            var entityTypes = dbContext.EntityTypes.Where(c => model.EntityTypes.Contains(c.Guid)).ToList();

            foreach (var entityType in entityTypes)
            {
                this.dbContext.ActivityEntityTypes.Add(new ActivityEntityType()
                {
                    Guid = Guid.NewGuid(),
                    ActivityId = activity.Id,
                    EntityTypeId = entityType.Id
                });
            }
            SaveChanges();
            foreach (var form in model.Forms)
            {
                var formDb = forms.FirstOrDefault(v => v.Guid == form.Id);
                var activityForm = new ActivityForm()
                {
                    Guid = Guid.NewGuid(),
                    FormId = formDb.Id,
                    ActivityId = activity.Id
                };
                dbContext.ActivityForms.Add(activityForm);
                SaveChanges();
            }
            return GetByGuid(activity.Guid);
        }

        public ActivityViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var activity = dbContext.Activities.FirstOrDefault(fs => fs.Guid == guid);
            if (activity != null)
            {
                activity.DeactivatedBy = deactivatedBy.Id;
                activity.DateDeactivated = DateTime.UtcNow;
                resultid = new List<string>();
                resultid.Add(activity.Id.ToString());
                dbContext.Activities.Where(x => x.DependentActivityId == activity.Id).ToList().ForEach(x =>
                getDependentActivityId(x)
                );
                dbContext.Activities.Where(x => resultid.Contains(x.Id.ToString())).ToList().ForEach(y =>
                    {
                        y.DeactivatedBy = deactivatedBy.Id;
                        y.DateDeactivated = DateTime.UtcNow;
                    }
                );
                SaveChanges();
                return ToModel(activity);
            }
            return null;
        }

        public ActivityViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var activity = dbContext.Activities.FirstOrDefault(fs => fs.Id == id);
            if (activity != null)
            {
                activity.DeactivatedBy = deactivatedBy.Id;
                activity.DateDeactivated = DateTime.UtcNow;
                SaveChanges();
                return ToModel(activity);
            }
            return null;
        }

        public IEnumerable<ActivityViewModel> GetAll()
        {
            return dbContext.Activities
            .Select(ToModel)
            .ToList();
        }

        public IEnumerable<ActivityViewModel> GetAll(Guid tenantId)
        {
            return dbContext.Activities
            .Where(v => v.TenantId.HasValue && v.Tenant.Guid == tenantId)
            .Select(ToModel)
            .ToList();
        }

        public ActivityViewModel GetByGuid(Guid guid)
        {
            var form = dbContext.Activities
             .FirstOrDefault(fs => fs.Guid == guid);
            if (form != null)
                return ToModel(form);
            return null;
        }

        public ActivityViewModel GetById(int id)
        {
            var form = dbContext.Activities
            .FirstOrDefault(fs => fs.Id == id);
            if (form != null)
                return ToModel(form);
            return null;
        }

        public ActivityViewModel GetActivityByGuid(Guid guid, Guid logginuserId, Guid projectid)
        {
            var Activity = dbContext.Activities
              .FirstOrDefault(fs => fs.Guid == guid);
            if (Activity != null)
                return ToNewModel(Activity, logginuserId, projectid);
            return null;
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        public ActivityViewModel ToModel(Activity entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            var activity = entity.DependentActivityId.HasValue ? dbContext.Activities.FirstOrDefault(p => p.Id == entity.DependentActivityId) : null;
            return new ActivityViewModel()
            {
                Id = entity.Id,
                Guid = entity.Guid,
                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                ActivityCategoryId = entity.ActivityCategory.Guid,
                ActivityName = entity.ActivityName,
                ActivityStatusId = entity.ActivityStatu.Guid,
                ActivityStatusName = entity.ActivityStatu.Status,
                EndDate = entity.EndDate,
                DependentActivityId = activity != null ? activity.Guid : (Guid?)null,
                RepeatationCount = entity.RepeatationCount,
                RepeatationOffset = entity.RepeatationOffset,
                RepeatationType = entity.RepeatationType,
                ScheduleType = entity.ScheduleType,
                StartDate = entity.StartDate,
                TenantId = entity.Tenant.Guid,
                Forms = entity.ActivityForms.Select(c => new FormActivityViewModel()
                {
                    FormTitle = c.Form.FormTitle,
                    Id = c.Form.Guid,
                    Status = c.Form.FormStatu.Status,
                }).ToList(),
                EntityTypes = entity.ActivityEntityTypes.Select(c => c.EntityType.Guid).ToList(),
                ProjectId = entity.FormDataEntry.Guid,
                PreviewSchedulingDone = entity.PreviewSchedulingDone ?? false,
                IsDefaultActivity = entity.IsDefaultActivity,
                IsActivityRequireAnEntity = entity.IsActivityRequireAnEntity,
            };
        }

        public ActivityViewModel ToNewModel(Activity entity, Guid? loggedUserGuid = null, Guid? Projectid = null)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            var userRole = "";
            if (loggedUserGuid.HasValue && Projectid.HasValue)
            {
                userRole = _userLoginProvider.GetUserRoleByProjectId(loggedUserGuid.Value, Projectid.Value);
            }
            var activity = entity.DependentActivityId.HasValue ? dbContext.Activities.FirstOrDefault(p => p.Id == entity.DependentActivityId) : null;

            int count = 0;
            try
            {
                count = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().Where(x => x.ActivityGuid == entity.Guid).AsQueryable().Count();
                count = count != 0 ? count : entity.FormDataEntries.Count;
            }
            catch (Exception ee)
            { }

            return new ActivityViewModel()
            {
                Id = entity.Id,
                Guid = entity.Guid,
                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                ActivityCategoryId = entity.ActivityCategory.Guid,
                ActivityName = entity.ActivityName,
                ActivityStatusId = entity.ActivityStatu.Guid,
                ActivityStatusName = entity.ActivityStatu.Status,
                UserTypeRole = userRole,
                IsFormContaindData = count > 0,
                EndDate = entity.EndDate,
                DependentActivityId = activity != null ? activity.Guid : (Guid?)null,
                RepeatationCount = entity.RepeatationCount,
                RepeatationOffset = entity.RepeatationOffset,
                RepeatationType = entity.RepeatationType,
                ScheduleType = entity.ScheduleType,
                StartDate = entity.StartDate,
                TenantId = entity.Tenant.Guid,
                Forms = entity.ActivityForms.Where(x => x.Form.DateDeactivated == null).Select(c => new FormActivityViewModel()
                {
                    FormTitle = c.Form.FormTitle,
                    Id = c.Form.Guid,
                    Status = c.Form.FormStatu.Status,
                }).ToList(),
                EntityTypes = entity.ActivityEntityTypes.Select(c => c.EntityType.Guid).ToList(),
                ProjectId = entity.FormDataEntry.Guid,
                PreviewSchedulingDone = entity.PreviewSchedulingDone ?? false,
                IsDefaultActivity = entity.IsDefaultActivity,
                IsActivityRequireAnEntity = entity.IsActivityRequireAnEntity,
                ActivityStatusIdInt = entity.ActivityStatusId,
            };
        }

        public ActivityViewModel Update(ActivityViewModel model)
        {
            if (dbContext.Activities.Any(et => et.FormDataEntry.Guid == model.ProjectId && et.ActivityName.ToLower() == model.ActivityName.ToLower()
            && et.Guid != model.Guid && et.DateDeactivated == null))
            {
                throw new Core.AlreadyExistsException("Activity already exists.");
            }

            if (dbContext.Activities.Any(et => et.ActivityStatusId == (int)ActivityStatusTypes.Active && et.Guid == model.Guid && et.DateDeactivated == null))
            {
                throw new Core.AlreadyExistsException("Current activity is already Active. So can not edit current activity. Please Inactive the current activity first to edit.");
            }

            if (model.Forms == null || model.Forms.Count == 0)
            {
                throw new Core.BadRequestException("Please select atleast 1 form.");
            }

            if (model.Forms.Any(c => c.Roles == null || c.Roles.Count == 0))
            {
                //throw new Core.BadRequestException("Please select form roles for activity.");
            }

            var activity = dbContext.Activities
              .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (activity != null && activity.IsDefaultActivity == (int)DefaultActivityType.Default)
            {
                throw new Core.BadRequestException("Default activity can not edit.");
            }

            var activityCategory = this.dbContext.ActivityCategories.FirstOrDefault(et => et.Guid == model.ActivityCategoryId);
            var dependentActivity = model.DependentActivityId.HasValue ? dbContext.Activities.FirstOrDefault(p => p.Guid == model.DependentActivityId) : null;
            if (activity != null)
            {
                dbContext.Activities.Where(c => c.DependentActivityId == activity.Id).ToList().ForEach(x =>
                {
                    if ((dependentActivity != null ? dependentActivity.Id : (int?)null) == x.Id)
                    {
                        throw new Core.BadRequestException("Please select valid Dependent Activity.");
                    }
                });

                var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);
                activity.ModifiedDate = DateTime.UtcNow;
                activity.ModifiedBy = modifiedBy.Id;
                activity.ActivityName = model.ActivityName;
                activity.ActivityCategoryId = activityCategory != null ? activityCategory.Id : (int?)null;
                activity.DependentActivityId = dependentActivity != null ? dependentActivity.Id : (int?)null;
                activity.EndDate = model.EndDate;
                activity.RepeatationCount = model.RepeatationCount;
                activity.RepeatationOffset = model.RepeatationOffset;
                activity.RepeatationType = model.RepeatationType;
                activity.ScheduleType = model.ScheduleType;
                activity.StartDate = model.StartDate;
                activity.IsActivityRequireAnEntity = model.IsActivityRequireAnEntity;

                Guid previousEntityType = activity.ActivityEntityTypes.Select(x => x.EntityType.Guid).FirstOrDefault();
                Guid currentEntityType = model.EntityTypes.FirstOrDefault();

                if (previousEntityType != currentEntityType)
                {
                    var scheduling = dbContext.ActivitySchedulings.FirstOrDefault(x => x.ActivityId == activity.Id);
                    if (scheduling != null)
                    {
                        scheduling.IsScheduled = false;
                        SaveChanges();
                    }

                    var schedulingOtherActivity = dbContext.ActivitySchedulings.Where(x => x.OtherActivity == activity.Id).AsQueryable();
                    if (schedulingOtherActivity != null)
                    {
                        schedulingOtherActivity.ToList().ForEach(a =>
                        {
                            a.IsScheduled = false;
                        });
                        SaveChanges();
                    }


                    var specifiedActivityScheduling = dbContext.ActivitySchedulings.Where(x => x.SpecifiedActivity == activity.Id).AsQueryable();
                    if (specifiedActivityScheduling != null)
                    {
                        specifiedActivityScheduling.ToList().ForEach(a =>
                        {
                            a.IsScheduled = false;
                        });
                        SaveChanges();
                    }
                }
                dbContext.ActivityForms.RemoveRange(activity.ActivityForms.Select(c => c).ToList());
                dbContext.ActivityEntityTypes.RemoveRange(activity.ActivityEntityTypes.Select(c => c).ToList());

                SaveChanges();

                var varIds = model.Forms.Select(v => v.Id).ToList();
                var roleIds = model.Forms.SelectMany(v => v.Roles).ToList();
                var forms = dbContext.Forms.Where(c => varIds.Contains(c.Guid)).ToList();
                var roles = dbContext.Roles.Where(c => roleIds.Contains(c.Guid)).ToList();
                var entityTypes = dbContext.EntityTypes.Where(c => model.EntityTypes.Contains(c.Guid)).ToList();

                foreach (var entityType in entityTypes)
                {
                    this.dbContext.ActivityEntityTypes.Add(new ActivityEntityType()
                    {
                        Guid = Guid.NewGuid(),
                        ActivityId = activity.Id,
                        EntityTypeId = entityType.Id
                    });
                }

                SaveChanges();
                foreach (var form in model.Forms)
                {
                    var formDb = forms.FirstOrDefault(v => v.Guid == form.Id);
                    var activityForm = new ActivityForm()
                    {
                        Guid = Guid.NewGuid(),
                        FormId = formDb.Id,
                        ActivityId = activity.Id,
                    };

                    dbContext.ActivityForms.Add(activityForm);
                    SaveChanges();
                }

                return GetByGuid(model.Guid);
            }

            return null;
        }

        public ProjectBuilderActivityViewModel GetProjectBuilderActivities(Guid tenantId, Guid LoggedInUserId, Guid projectId)
        {
            #region check login
            string userRole = _userLoginProvider.GetUserRoleByProjectId(LoggedInUserId, projectId);
            string[] allowedRole = new string[] {
                Core.Enum.RoleTypes.System_Admin.ToString().Replace("_"," ")
                , Core.Enum.RoleTypes.Project_Admin.ToString().Replace("_"," ")
            };
            if (!allowedRole.Contains(userRole))
            {
                throw new Core.UnauthorizedException("Authorization has been denied for this request.");
            }
            #endregion

            ProjectBuilderActivityViewModel projectBuilderActivityViewModel = new ProjectBuilderActivityViewModel();

            projectBuilderActivityViewModel.ActivityCategory = _activityCategoryProvider.GetAll(tenantId);
            projectBuilderActivityViewModel.Roles = _RoleProvider.GetAll(tenantId);
            projectBuilderActivityViewModel.FormCategory = _FormCategoryProvider.GetAll();
            projectBuilderActivityViewModel.EntityTypes = _EntityTypeProvider.GetAll();

            return projectBuilderActivityViewModel;
        }

        public List<string> getDependentActivityId(Activity activity)
        {
            if (!resultid.Contains(activity.Id.ToString()))
            {
                resultid.Add(activity.Id.ToString());
            }
            dbContext.Activities.Where(x => x.DependentActivityId == activity.Id).ToList().ForEach(x =>
                getDependentActivityId(x)
           );
            return resultid;
        }

        public ActivityViewModel UpdateActivityScheduling(ScheduleActivityViewModel model)
        {
            var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);

            List<Activity> activityList = new List<Activity>();
            Activity activity = new Activity();

            foreach (var item in model.ScheduleActivityList)
            {
                model.Guid = item.Guid;

                activity = dbContext.Activities.FirstOrDefault(x => x.Guid == item.Guid);

                activity.ScheduleType = item.ScheduleType;
                activity.StartDate = item.StartDate;
                activity.EndDate = item.EndDate;
                activity.RepeatationOffset = item.RepeatationOffset;
                activity.RepeatationCount = item.RepeatationCount;
                activity.ModifiedDate = DateTime.UtcNow;
                activity.ModifiedBy = modifiedBy.Id;

                if (item.ScheduleType == 3)
                {
                    activity.StartDate = activity.FormDataEntry.CreatedDate;
                    var sDate = activity.StartDate ?? DateTime.UtcNow;
                    int days = item.RepeatationOffset ?? 0;
                    activity.EndDate = sDate.AddDays(days);
                }
                activityList.Add(activity);
            }
            SaveChanges();
            return GetByGuid(model.Guid);
        }

        public ActivityViewModel RemoveScheduledActivity(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var activity = dbContext.Activities.FirstOrDefault(fs => fs.Guid == guid);
            if (activity != null)
            {
                activity.ScheduleType = 0;
                activity.StartDate = null;
                activity.EndDate = null;
                activity.PreviewSchedulingDone = null;
                SaveChanges();
                return ToModel(activity);
            }
            return null;
        }

        public ActivityViewModel SavePreviewScheduledActivity(string guid, Guid SavedBy)
        {
            List<string> ls = guid.Split(',').ToList();
            dbContext.Activities.Where(x => ls.Contains(x.Guid.ToString())).ToList().ForEach(y =>
            {
                y.PreviewSchedulingDone = true;
            });
            return ToModel(dbContext.Activities.FirstOrDefault());
        }

        public ActivityViewModel ScheduleActivity_New(NewScheduleActivityViewModel model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SchedulingViewModel> GetAllActivityScheduling(Guid activityId)
        {
            return dbContext.ActivitySchedulings
            .Where(v => v.Activity.Guid == activityId)
            .Select(ToActivitySchedulingModel)
            .ToList();
        }
        public SchedulingViewModel ToActivitySchedulingModel(ActivityScheduling scheduling)
        {
            var rolesToCreateActivity = dbContext.RolesToCreateActivitySchedulings.Where(x => x.ActivitySchedulingId == scheduling.RolesToCreateActivity).Select(x => x.Role.Guid).ToList();
            var roleToCreateActivityRegardlessScheduled = dbContext.RoleToCreateActivityRegardlessScheduleds.Where(x => x.ActivitySchedulingId == scheduling.RoleToCreateActivityRegardlessScheduled).Select(x => x.Role.Guid).ToList();
            var otherActivity = dbContext.Activities.FirstOrDefault(p => p.Id == scheduling.OtherActivity);
            var specifiedActivity = dbContext.Activities.FirstOrDefault(p => p.Id == scheduling.SpecifiedActivity);

            return new SchedulingViewModel()
            {
                Id = scheduling.Id,
                ActivityId = scheduling.Activity.Guid,
                ScheduledToBeCompleted = scheduling.ScheduledToBeCompleted,
                ActivityAvailableForCreation = scheduling.ActivityAvailableForCreation,
                RolesToCreateActivity = rolesToCreateActivity,
                RoleToCreateActivityRegardlessScheduled = roleToCreateActivityRegardlessScheduled,
                OtherActivity = otherActivity != null ? otherActivity.Guid : (Guid?)null,
                SpecifiedActivity = specifiedActivity != null ? specifiedActivity.Guid : (Guid?)null,
                CreationWindowOpens = scheduling.CreationWindowOpens,
                CreationWindowClose = scheduling.CreationWindowClose,
                IsScheduled = scheduling.IsScheduled,
                ScheduleDate = scheduling.ScheduleDate,
                Guid = scheduling.Guid,
            };
        }

        public AddSummaryPageActivityViewModel AddSummaryPageActivity(AddSummaryPageActivityViewModel model)
        {
            var personEntity = dbContext.FormDataEntryVariables.FirstOrDefault(x => x.SelectedValues == model.PersonEntityId && x.Variable.VariableName == "EntID");
            var entId = personEntity != null ? personEntity.FormDataEntryId : 0;
            var project = dbContext.FormDataEntries.FirstOrDefault(p => p.Guid == model.ProjectId);
            var scheduling = dbContext.ActivitySchedulings.FirstOrDefault(x => x.Activity.Guid == model.ActivityId);
            bool canCollectMultipe = scheduling != null ? scheduling.CanCreatedMultipleTime : false;
            if (!canCollectMultipe)
            {
                if (dbContext.AddActivities.Any(et => et.Activity.Guid == model.ActivityId && et.ProjectId == project.Id && et.PersonEntityId == entId && et.DateDeactivated == null))
                {
                    throw new Core.AlreadyExistsException("Activity already added.");
                }
            }
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            var activityCompletedBy = _userLoginProvider.GetByGuid(model.ActivityCompletedByUser);

            var activity = dbContext.Activities.FirstOrDefault(p => p.Guid == model.ActivityId);

            var addSummaryPageActivity = this.dbContext.AddActivities.Add(new AddActivity()
            {
                Guid = Guid.NewGuid(),
                ProjectId = project.Id,

                CreatedBy = createdBy != null ? createdBy.Id : 1,
                CreatedDate = DateTime.UtcNow,
                ActivityId = activity != null ? activity.Id : 1,
                ActivityCompletedByUser = activityCompletedBy != null ? activityCompletedBy.Id : 1,
                ActivityDate = model.ActivityDate,
                IsActivityAdded = model.IsActivityAdded,
                PersonEntityId = personEntity != null ? personEntity.FormDataEntryId : 1,
            });

            SaveChanges();
            model.Id = addSummaryPageActivity.Id;
            model.Guid = addSummaryPageActivity.Guid;
            return ToSummaryPageActivityModel(addSummaryPageActivity);
        }
        public AddSummaryPageActivityViewModel TestEnvironment_AddSummaryPageActivity(AddSummaryPageActivityViewModel model)
        {
            var personEntity = dbContext.FormDataEntryVariables.FirstOrDefault(x => x.SelectedValues == model.PersonEntityId && x.Variable.VariableName == "EntID");
            var entId = personEntity != null ? personEntity.FormDataEntryId : 0;
            var project = dbContext.FormDataEntries.FirstOrDefault(p => p.Guid == model.ProjectId);
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            var activityCompletedBy = _userLoginProvider.GetByGuid(model.ActivityCompletedByUser);
            var activity = dbContext.Activities.FirstOrDefault(p => p.Guid == model.ActivityId);
            ProjectBuilderJSON projectBuilderJSONs = dbContext.ProjectBuilderJSONs.FirstOrDefault(x => x.ProjectId == project.Id);
            AddSummaryPageActivityViewModel addsummarypageactivityModel = new AddSummaryPageActivityViewModel();
            if (projectBuilderJSONs != null)
            {
                DeployProjectJsonViewModel deserialized = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.DeployProjectJsonViewModel>(projectBuilderJSONs.ProjectBuilderJSONData);

                addsummarypageactivityModel.ProjectId = model.ProjectId;
                addsummarypageactivityModel.CreatedBy = model.CreatedBy;
                addsummarypageactivityModel.CreatedDate = DateTime.UtcNow;
                addsummarypageactivityModel.ActivityId = activity.Guid;
                addsummarypageactivityModel.ActivityName = activity.ActivityName;
                addsummarypageactivityModel.ActivityCompletedByUser = activityCompletedBy != null ? activityCompletedBy.Guid : Guid.Empty;
                addsummarypageactivityModel.ActivityCompletedByUserName = activityCompletedBy != null ? activityCompletedBy.FirstName + " " + activityCompletedBy.LastName : string.Empty;
                addsummarypageactivityModel.ActivityDate = model.ActivityDate;
                addsummarypageactivityModel.IsActivityAdded = true;
                addsummarypageactivityModel.PersonEntityId = model.PersonEntityId;
                addsummarypageactivityModel.Forms = activity.ActivityForms.Select(c => new FormActivityViewModel()
                {
                    FormTitle = c.Form.FormTitle,
                    Id = c.Form.Guid,
                    Status = c.Form.FormStatu.Status,
                }).ToList();

                if (deserialized.AddSummaryPageActivityViewModelList != null)
                    deserialized.AddSummaryPageActivityViewModelList.Add(addsummarypageactivityModel);
                else
                {
                    deserialized.AddSummaryPageActivityViewModelList = new List<AddSummaryPageActivityViewModel>();
                    deserialized.AddSummaryPageActivityViewModelList.Add(addsummarypageactivityModel);
                }


                string newjsonData = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(deserialized);
                projectBuilderJSONs.ProjectBuilderJSONData = newjsonData;
                SaveChanges();
            }
            return addsummarypageactivityModel;
        }

        public AddSummaryPageActivityViewModel EditSummaryPageActivity(AddSummaryPageActivityViewModel model)
        {
            var modifiedBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            var activityCompletedBy = _userLoginProvider.GetByGuid(model.ActivityCompletedByUser);
            AddActivity editExistingActivity = this.dbContext.AddActivities.FirstOrDefault(x => x.Id == model.Id);
            if (editExistingActivity != null)
            {
                editExistingActivity.ModifiedBy = modifiedBy.Id;
                editExistingActivity.ModifiedDate = DateTime.UtcNow;
                editExistingActivity.ActivityCompletedByUser = activityCompletedBy != null ? activityCompletedBy.Id : 2;
                editExistingActivity.ActivityDate = model.ActivityDate;
                SaveChanges();
            }
            return ToSummaryPageActivityModel(editExistingActivity);
        }


        public AddSummaryPageActivityViewModel DeleteSummaryPageActivity(int id, Guid loggedInUser)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(loggedInUser);
            AddActivity editExistingActivity = this.dbContext.AddActivities.FirstOrDefault(x => x.Id == id);
            if (editExistingActivity != null)
            {
                if (editExistingActivity.Activity.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                    || editExistingActivity.Activity.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
                    || editExistingActivity.Activity.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)
                    || editExistingActivity.Activity.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                {
                    throw new Core.NotFoundException("Can not delete default registration activities.");
                }
                editExistingActivity.DeactivatedBy = deactivatedBy.Id;
                editExistingActivity.DateDeactivated = DateTime.UtcNow;
                SaveChanges();
            }
            return ToSummaryPageActivityModel(editExistingActivity);
        }

        public IEnumerable<AddSummaryPageActivityViewModel> GetAllSummaryPageActivity(string entId, Guid projectId, Guid loggedInUserId)
        {
            var ent = dbContext.FormDataEntryVariables.FirstOrDefault(x => x.SelectedValues == entId && x.Variable.VariableName == "EntID");

            var isAdded = dbContext.AddActivities.FirstOrDefault(x => x.PersonEntityId == ent.FormDataEntryId && x.DateDeactivated == null);
            if (isAdded == null)
            {
                #region linkage from summary page
                var project = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == projectId);

                string title = string.Empty;
                try
                {
                    title = ent.FormDataEntry.Form.FormTitle;
                }
                catch (Exception exc)
                { }

                var activity = dbContext.Activities.FirstOrDefault(x => x.ProjectId == project.Id && x.ActivityName == title);

                var activityCompletedBy = dbContext.UserLogins.FirstOrDefault(x => x.Email.ToLower() == "systemadmin@aspree.com");
                var addSummaryPageActivity = this.dbContext.AddActivities.Add(new AddActivity()
                {
                    Guid = Guid.NewGuid(),
                    ProjectId = project != null ? project.Id : 1,
                    CreatedBy = 2,
                    CreatedDate = DateTime.UtcNow,
                    ActivityId = activity != null ? activity.Id : 1,
                    ActivityCompletedByUser = activityCompletedBy != null ? activityCompletedBy.Id : 1,
                    ActivityDate = ent.CreatedDate,
                    IsActivityAdded = true,
                    PersonEntityId = ent.FormDataEntryId,
                });
                SaveChanges();
                #endregion
            }
            var addedActivities = dbContext.AddActivities
            .Where(v => v.PersonEntityId == ent.FormDataEntryId && v.DateDeactivated == null)
            .Select(ToSummaryPageActivityModel)
            .ToList();

            #region apply project admin restriction
            try
            {
                    string sysAppr = dbContext.FormDataEntryVariables.FirstOrDefault(x => x.FormDataEntryId == ent.FormDataEntryId && x.Variable.VariableName == DefaultsVariables.SysAppr.ToString()) != null
                        ? dbContext.FormDataEntryVariables.FirstOrDefault(x => x.FormDataEntryId == ent.FormDataEntryId && x.Variable.VariableName == DefaultsVariables.SysAppr.ToString()).SelectedValues
                        : null;
                    if (string.IsNullOrEmpty(sysAppr) || sysAppr == "0")
                    {
                        string[] summaryPageDefaultActivities = new string[] {
                            EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
                            ,EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                            ,EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)
                            ,EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration)
                        };
                        addedActivities.RemoveAll(activity => !summaryPageDefaultActivities.Contains(activity.ActivityName));
                    }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            #endregion
            return addedActivities;
        }
        public IEnumerable<AddSummaryPageActivityViewModel> TestEnvironment_GetAllSummaryPageActivity(string entId, Guid projectId)
        {
            ProjectBuilderJSON projectBuilderJSONs = dbContext.ProjectBuilderJSONs.FirstOrDefault(x => x.FormDataEntry.Guid == projectId);
            if (projectBuilderJSONs != null)
            {
                DeployProjectJsonViewModel deserialized = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.DeployProjectJsonViewModel>(projectBuilderJSONs.ProjectBuilderJSONData);
                if (deserialized.AddSummaryPageActivityViewModelList == null) deserialized.AddSummaryPageActivityViewModelList = new List<AddSummaryPageActivityViewModel>();
                List<AddSummaryPageActivityViewModel> summaryPageActivities = deserialized.AddSummaryPageActivityViewModelList.Where(x => x.PersonEntityId == entId).ToList();
                if (summaryPageActivities.Count() == 0)
                {
                    int intEntid = Convert.ToInt32(entId);
                    var actid = dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.EntId == intEntid);
                    var deserializedEntity = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.JsonFormDataEntries>(actid.ProjectBuilderJSONValues);
                    var frms = new List<FormActivityViewModel>();
                    frms.Add(new FormActivityViewModel()
                    {
                        FormTitle = deserializedEntity.FormTitle,
                        Id = deserializedEntity.FormGuid,
                        Status = Enum.GetName(typeof(Core.Enum.FormStatusTypes), deserializedEntity.Status),
                    });

                    var activityCompletedBy = dbContext.UserLogins.FirstOrDefault(x => x.Email.ToLower() == "testsystemadmin@aspree.com");

                    deserialized.AddSummaryPageActivityViewModelList.Add(new AddSummaryPageActivityViewModel()
                    {
                        ActivityCompletedByUser = activityCompletedBy != null ? activityCompletedBy.Guid : Guid.Empty,
                        ActivityCompletedByUserName = activityCompletedBy != null ? activityCompletedBy.FirstName + " " + activityCompletedBy.LastName : string.Empty,
                        ActivityDate = DateTime.UtcNow,
                        ActivityId = deserializedEntity.ActivityGuid,
                        ActivityName = deserializedEntity.ActivityName,
                        CreatedBy = deserializedEntity.CreatedByGuid,
                        CreatedDate = deserializedEntity.CreatedDate,
                        Forms = frms,
                        IsActivityAdded = true,
                        PersonEntityId = entId,
                        ProjectId = projectId,
                    });
                    var jsonnew = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(deserialized);
                    projectBuilderJSONs.ProjectBuilderJSONData = jsonnew;
                    SaveChanges();
                    summaryPageActivities = deserialized.AddSummaryPageActivityViewModelList.Where(x => x.PersonEntityId == entId).ToList();
                }
                return summaryPageActivities;
            }
            return new List<AddSummaryPageActivityViewModel>();
        }

        public AddSummaryPageActivityViewModel ToSummaryPageActivityModel(AddActivity entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            var personEntity = dbContext.FormDataEntryVariables.FirstOrDefault(x => x.FormDataEntryId == entity.PersonEntityId && x.Variable.VariableName == "EntID");

            var activityCompletedByUser = dbContext.UserLogins.FirstOrDefault(x => x.Id == entity.ActivityCompletedByUser);

            string linkedProjectName = string.Empty;
            Guid linkedProjectGuid = Guid.Empty;
         
            List<FormActivityViewModel> forms = new List<FormActivityViewModel>();
            FormActivityViewModel frm = new FormActivityViewModel();
            foreach (var form in entity.Activity.ActivityForms)
            {
                string status = string.Empty;
                try
                {
                    int? parentEntId = personEntity != null ? !string.IsNullOrEmpty(personEntity.SelectedValues) ? Convert.ToInt32(personEntity.SelectedValues) : (int?)null : (int?)null;
                    var st = dbContext.FormDataEntries.FirstOrDefault(x => x.SubjectId == entity.Id && x.Form.FormTitle == form.Form.FormTitle);
                    if (st == null)
                    {
                        st = personEntity.FormDataEntry;
                        st.Status = (int)Core.Enum.FormStatusTypes.Not_entered;
                    }
                    status = Enum.GetName(typeof(Core.Enum.FormStatusTypes), st.Status);
                    try
                    {
                        if (entity.Activity.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Linkage))
                        {
                            var lnkPro = st.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.LnkPro.ToString());
                            Guid lnkProGuid = !string.IsNullOrEmpty(lnkPro.SelectedValues) ? new Guid(lnkPro.SelectedValues) : Guid.Empty;
                            FormDataEntry projectName = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == lnkProGuid);
                            linkedProjectGuid = lnkProGuid;
                            linkedProjectName = projectName.FormDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.Name.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                        }
                    }
                    catch (Exception excc)
                    { }
                }
                catch (Exception ex)
                { }

                frm = new FormActivityViewModel();
                frm.FormTitle = form.Form.FormTitle;
                frm.Id = form.Form.Guid;
                frm.Status = status;
                forms.Add(frm);
            }

            return new AddSummaryPageActivityViewModel()
            {
                Id = entity.Id,
                Guid = entity.Guid,
                ProjectId = entity.FormDataEntry.Guid,
                ActivityId = entity.Activity.Guid,
                ActivityName = entity.Activity.ActivityName,
                ActivityCompletedByUser = activityCompletedByUser.Guid,
                ActivityCompletedByUserName = activityCompletedByUser.FirstName + " " + activityCompletedByUser.LastName,
                ActivityDate = entity.ActivityDate,
                IsActivityAdded = entity.IsActivityAdded,
                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                PersonEntityId = personEntity != null ? personEntity.SelectedValues : string.Empty,
                Forms = forms.ToList(),
                LinkedProjectName = linkedProjectName,
                LinkedProjectGuid = linkedProjectGuid,
            };
        }

        public AddSummaryPageActivityViewModel TestEnvironment_DeleteSummaryPageActivity(int id, Guid loggedinuser)
        {
            throw new NotImplementedException();
        }
    }
}