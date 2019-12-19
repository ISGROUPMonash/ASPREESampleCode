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
    public class ActivityStatusProvider : IActivityStatusProvider
    {
        private readonly AspreeEntities dbContext;
        public ActivityStatusProvider(AspreeEntities _dbContext)
        {
            this.dbContext = _dbContext;
        }

        public ActivityStatusViewModel Create(ActivityStatusViewModel model)
        {
            if (dbContext.ActivityStatus.Any(est => est.Status.ToLower() == model.Status.ToLower()))
            {
                throw new Core.AlreadyExistsException("Activity status already exists.");
            }

            var activityStatus = new ActivityStatu
            {
                IsActive = true,
                Status = model.Status,
                Guid = Guid.NewGuid()
            };

            dbContext.ActivityStatus.Add(activityStatus);

            SaveChanges();

            return ToModel(activityStatus);
        }

        public ActivityStatusViewModel DeleteById(int id, Guid DeletedBy)
        {
            var activityStatus = dbContext.ActivityStatus
                .FirstOrDefault(fs => fs.Id == id);

            if (activityStatus != null)
            {
                activityStatus.IsActive = false;
                return ToModel(activityStatus);
            }

            return null;
        }

        public ActivityStatusViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var activityStatus = dbContext.ActivityStatus
                .FirstOrDefault(fs => fs.Guid == guid);

            if (activityStatus != null)
            {
                activityStatus.IsActive = false;
                return ToModel(activityStatus);
            }

            return null;
        }

        public IEnumerable<ActivityStatusViewModel> GetAll()
        {
            return dbContext.ActivityStatus
                .Where(a => a.IsActive)
                .Select(ToModel)
                .ToList();
        }

        public ActivityStatusViewModel GetByGuid(Guid guid)
        {
            var activityStatus = dbContext.ActivityStatus
                .FirstOrDefault(fs => fs.Guid == guid);

            if (activityStatus != null)
                return ToModel(activityStatus);

            return null;
        }

        public ActivityStatusViewModel GetById(int id)
        {
             var activityStatus = dbContext.ActivityStatus
                .FirstOrDefault(fs => fs.Id == id);

            if (activityStatus != null)
                return ToModel(activityStatus);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public ActivityStatusViewModel ToModel(ActivityStatu entity)
        {
            return new ActivityStatusViewModel
            {
                Guid = entity.Guid,
                IsActive = entity.IsActive,
                Id = entity.Id,
                Status = entity.Status
            };
        }

        public ActivityStatusViewModel Update(ActivityStatusViewModel model)
        {
            if (dbContext.ActivityStatus.Any(est => est.Status.ToLower() == model.Status.ToLower()
                && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Activity status already exists.");
            }

            var activityStatus = dbContext.ActivityStatus
               .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (activityStatus != null)
            {
                activityStatus.Status = model.Status;
                SaveChanges();

                return ToModel(activityStatus);
            }

            return null;
        }
    }
}
