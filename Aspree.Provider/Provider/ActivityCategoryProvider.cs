using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Data;
using System.Linq.Expressions;
using Aspree.Core.ViewModels;

namespace Aspree.Provider.Provider
{
    public class ActivityCategoryProvider : IActivityCategoryProvider
    {
        private readonly AspreeEntities dbContext;
        private readonly IUserLoginProvider _userLoginProvider;
        public ActivityCategoryProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
        }
        public ActivityCategoryViewModel Create(ActivityCategoryViewModel model)
        {
            if (dbContext.ActivityCategories.Any(est => est.CategoryName.ToLower() == model.CategoryName.ToLower()))
            {
                throw new Core.AlreadyExistsException("Activity category already exists.");
            }
            var tenant = dbContext.Tenants.FirstOrDefault(x => x.Guid == model.TenantId);
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            var activityCategory = new ActivityCategory
            {
                CategoryName = model.CategoryName,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
                Guid = Guid.NewGuid(),
                TenantId = tenant != null ? tenant.Id : (int?)null,
            };
            dbContext.ActivityCategories.Add(activityCategory);
            SaveChanges();
            return ToModel(activityCategory);
        }

        public ActivityCategoryViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var activityCategory = dbContext.ActivityCategories.FirstOrDefault(fs => fs.Id == id);
            if (activityCategory != null)
            {
                activityCategory.DeactivatedBy = deactivatedBy.Id;
                activityCategory.DateDeactivated = DateTime.UtcNow;
                return ToModel(activityCategory);
            }
            return null;
        }

        public ActivityCategoryViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var activityCategory = dbContext.ActivityCategories.FirstOrDefault(fs => fs.Guid == guid);
            if (activityCategory != null)
            {
                activityCategory.DeactivatedBy = deactivatedBy.Id;
                activityCategory.DateDeactivated = DateTime.UtcNow;
                return ToModel(activityCategory);
            }

            return null;
        }

        public IEnumerable<ActivityCategoryViewModel> GetAll()
        {
            return dbContext.ActivityCategories
                .Where(ac => !ac.DateDeactivated.HasValue)
                .Select(ToModel)
                .ToList();
        }

        public IEnumerable<ActivityCategoryViewModel> GetAll(Guid tenantId)
        {
            List<ActivityCategoryViewModel> activityCategories = this.dbContext.ActivityCategories
                .Where(vc => vc.TenantId.HasValue && vc.Tenant.Guid == tenantId && vc.DateDeactivated == null)
                .Select(ToModel)
                .ToList();
            return activityCategories.Where(x => x.CategoryName == Core.Enum.ActivityCategories.Default.ToString())
                     .Concat(activityCategories.Where(x => x.CategoryName != Core.Enum.ActivityCategories.Default.ToString())
                     .OrderBy(x => x.CategoryName)).ToList();
        }

        public ActivityCategoryViewModel GetByGuid(Guid guid)
        {
            var activityCategory = dbContext.ActivityCategories
               .FirstOrDefault(fs => fs.Guid == guid);

            if (activityCategory != null)
                return ToModel(activityCategory);

            return null;
        }

        public ActivityCategoryViewModel GetById(int id)
        {
            var activityCategory = dbContext.ActivityCategories
               .FirstOrDefault(fs => fs.Id == id);
            if (activityCategory != null)
                return ToModel(activityCategory);
            return null;
        }
        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }
        public ActivityCategoryViewModel ToModel(ActivityCategory entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            return new ActivityCategoryViewModel
            {
                CategoryName = entity.CategoryName,
                CreatedBy = createdBy.Guid,
                Id = entity.Id,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy == null ? (Guid?)null : deactivatedBy.Guid,
                Guid = entity.Guid,
                ModifiedBy = modifiedBy == null ? (Guid?)null : modifiedBy.Guid,
                ModifiedDate = entity.ModifiedDate,
                Activities = entity.Activities.Where(c => c.DateDeactivated == null).Select(c => new Core.ViewModels.SubCategoryViewModel
                {
                    Guid = c.Guid,
                    Id = c.Id,
                    Name = c.ActivityName,
                    Status = c.ActivityStatu.Status,
                    ProjectId = c.FormDataEntry.Guid,
                    DateDeactivated = c.DateDeactivated,
                    RepeatationCount = c.RepeatationCount,
                    ScheduleType = c.ScheduleType,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    RepeatationOffset = c.RepeatationOffset,
                    IsDefaultVariable = c.IsDefaultActivity,
                    DeploymentStatus = c.ActivitySchedulings.FirstOrDefault(x => x.ActivityId == c.Id) != null ? c.ActivitySchedulings.FirstOrDefault(x => x.ActivityId == c.Id).Status : (int?)null,
                    IsAllVariableApprovedOfActivity = CheckAllVariableApproval(c),
                    EntityType = c.ActivityEntityTypes.Select(x => x.EntityType.Guid).ToList(),
                }).OrderBy(x => x.Name).ToList()
            };
        }
        public ActivityCategoryViewModel Update(ActivityCategoryViewModel model)
        {
            if (dbContext.ActivityCategories.Any(est => est.CategoryName.ToLower() == model.CategoryName.ToLower()
                && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Activity category already exists.");
            }
            var activityCategory = dbContext.ActivityCategories
               .FirstOrDefault(fs => fs.Guid == model.Guid);
            if (activityCategory != null)
            {
                var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);
                activityCategory.CategoryName = model.CategoryName;
                activityCategory.ModifiedBy = modifiedBy.Id;
                activityCategory.ModifiedDate = DateTime.UtcNow;
                SaveChanges();
                return ToModel(activityCategory);
            }
            return null;
        }

        public bool CheckAllVariableApproval(Activity activity)
        {
            bool IsAllVariableApprovedOfActivity = true;
            activity.ActivityForms.ToList().ForEach(x =>
            {
                x.Form.FormVariables.ToList().ForEach(variables =>
                {
                    if (!variables.Variable.IsApproved)
                    {
                        IsAllVariableApprovedOfActivity = false;
                    }
                });
            });
            return IsAllVariableApprovedOfActivity;
        }
    }
}