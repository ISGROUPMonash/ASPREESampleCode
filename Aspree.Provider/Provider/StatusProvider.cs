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
    public class StatusProvider : IStatusProvider
    {
        private readonly AspreeEntities dbContext;
        public StatusProvider(AspreeEntities _dbContext)
        {
            this.dbContext = _dbContext;
        }

        public StatusViewModel Create(StatusViewModel model)
        {
            if (dbContext.Status.Any(est => est.Status1.ToLower() == model.Status.ToLower()))
            {
                throw new Core.AlreadyExistsException("Status already exists.");
            }

            var status = new Status
            {
                IsActive = true,
                Status1 = model.Status,
                Guid = Guid.NewGuid()
            };

            dbContext.Status.Add(status);

            SaveChanges();

            return ToModel(status);
        }

        public StatusViewModel DeleteById(int id, Guid DeletedBy)
        {
            var status = dbContext.Status.FirstOrDefault(fs => fs.Id == id);
            if (status != null)
            {
                status.IsActive = false;
                return ToModel(status);
            }

            return null;
        }

        public StatusViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var status = dbContext.Status.FirstOrDefault(fs => fs.Guid == guid);
            if (status != null)
            {
                status.IsActive = false;
                return ToModel(status);
            }

            return null;
        }

        public IEnumerable<StatusViewModel> GetAll()
        {
            return dbContext.Status.Select(ToModel).ToList();
        }

        public StatusViewModel GetByGuid(Guid guid)
        {
            var status = dbContext.Status
                .FirstOrDefault(fs => fs.Guid == guid);

            if (status != null)
                return ToModel(status);

            return null;
        }

        public StatusViewModel GetById(int id)
        {
            var status = dbContext.Status
               .FirstOrDefault(fs => fs.Id == id);

            if (status != null)
                return ToModel(status);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public StatusViewModel ToModel(Status entity)
        {
            return new StatusViewModel
            {
                Guid = entity.Guid,
                Id = entity.Id,
                IsActive = entity.IsActive,
                Status = entity.Status1
            };
        }

        public StatusViewModel Update(StatusViewModel model)
        {
            if (dbContext.Status.Any(est => est.Status1.ToLower() == model.Status.ToLower()
                && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Status already exists.");
            }

            var status = dbContext.Status
              .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (status != null)
            {
                status.Status1 = model.Status;

                SaveChanges();
                
                return ToModel(status);
            }

            return null;
        }
    }
}