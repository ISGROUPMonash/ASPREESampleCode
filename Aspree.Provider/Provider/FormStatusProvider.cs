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
    public class FormStatusProvider : IFormStatusProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        public FormStatusProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
        }

        public FormStatusViewModel Create(FormStatusViewModel model)
        {
            if (dbContext.FormStatus.Any(est => est.Status.ToLower() == model.Status.ToLower()))
            {
                throw new Core.AlreadyExistsException("Form status already exists.");
            }


            var formStatus = new FormStatu
            {
                IsActive = true,
                Status = model.Status,
                Guid = Guid.NewGuid()
            };

            dbContext.FormStatus.Add(formStatus);

            SaveChanges();

            return ToModel(formStatus);

        }

        public FormStatusViewModel DeleteById(int id, Guid DeletedBy)
        {
            var formStatus = dbContext.FormStatus.FirstOrDefault(fs => fs.Id == id);
            if (formStatus != null)
            {
                formStatus.IsActive = false;
                return ToModel(formStatus);
            }

            return null;
        }

        public FormStatusViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var formStatus = dbContext.FormStatus.FirstOrDefault(fs => fs.Guid == guid);
            if (formStatus != null)
            {
                formStatus.IsActive = false;
                return ToModel(formStatus);
            }

            return null;
        }

        public IEnumerable<FormStatusViewModel> GetAll()
        {
            return dbContext.FormStatus
                .Select(ToModel)
                .ToList();
        }

        public FormStatusViewModel GetByGuid(Guid guid)
        {
            var formStatus = dbContext.FormStatus
                .FirstOrDefault(fs => fs.Guid == guid);

            if (formStatus != null)
                return ToModel(formStatus);

            return null;
        }

        public FormStatusViewModel GetById(int id)
        {
            var formStatus = dbContext.FormStatus
                .FirstOrDefault(fs => fs.Id == id);

            if (formStatus != null)
                return ToModel(formStatus);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public FormStatusViewModel ToModel(FormStatu entity)
        {
            return new FormStatusViewModel
            {
                Guid = entity.Guid,
                IsActive = entity.IsActive,
                Id = entity.Id,
                Status = entity.Status
            };
        }

        public FormStatusViewModel Update(FormStatusViewModel model)
        {
            if (dbContext.FormStatus.Any(est => est.Status.ToLower() == model.Status.ToLower()
                && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Form status already exists.");
            }


            var formStatus = dbContext.FormStatus
                .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (formStatus != null)
            {
                formStatus.Status = model.Status;

                SaveChanges();

                return ToModel(formStatus);
            }

            return null;
        }
    }
}
