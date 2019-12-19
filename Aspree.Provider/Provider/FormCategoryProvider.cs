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
    public class FormCategoryProvider : IFormCategoryProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        public FormCategoryProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
        }

        public FormCategoryViewModel Create(FormCategoryViewModel model)
        {
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);

            if (dbContext.FormCategories.Any(est => est.CategoryName.ToLower() == model.CategoryName.ToLower() && est.DateDeactivated == null))
            {
                throw new Core.AlreadyExistsException("Form category already exists.");
            }
            var tenant = dbContext.Tenants.FirstOrDefault(x => x.Guid == model.TenantId);

            var formCategory = new FormCategory
            {
                CategoryName = model.CategoryName,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
                Guid = Guid.NewGuid(),
                TenantId = tenant != null ? tenant.Id : (int?)null,
                IsDefaultFormCategory = (int)Core.Enum.DefaultFormType.Custom,
            };

            dbContext.FormCategories.Add(formCategory);

            SaveChanges();

            return ToModel(formCategory);
        }

        public FormCategoryViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var formCategory = dbContext.FormCategories.FirstOrDefault(fs => fs.Id == id);
            if (formCategory != null)
            {
                formCategory.DeactivatedBy = deactivatedBy.Id;
                formCategory.DateDeactivated = DateTime.UtcNow;
                return ToModel(formCategory);
            }

            return null;
        }
        public FormCategoryViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var formCategory = dbContext.FormCategories.FirstOrDefault(fs => fs.Guid == guid);
            if (formCategory != null)
            {
                formCategory.DeactivatedBy = deactivatedBy.Id;
                formCategory.DateDeactivated = DateTime.UtcNow;
                return ToModel(formCategory);
            }

            return null;
        }
        public IEnumerable<FormCategoryViewModel> GetAll()
        {
            return dbContext.FormCategories.Select(ToModel).ToList();
        }
        public IEnumerable<FormCategoryViewModel> GetAll(Guid tenantId)
        {
            return this.dbContext.FormCategories
                .Where(vc => vc.TenantId.HasValue && vc.Tenant.Guid == tenantId && vc.DateDeactivated == null)
                .Select(ToModel)
                .ToList();
        }

        public FormCategoryViewModel GetByGuid(Guid guid)
        {
            var formCategory = dbContext.FormCategories
                .FirstOrDefault(fs => fs.Guid == guid);
            if (formCategory != null)
                return ToModel(formCategory);
            return null;
        }

        public FormCategoryViewModel GetById(int id)
        {
            var formCategory = dbContext.FormCategories
                  .FirstOrDefault(fs => fs.Id == id);
            if (formCategory != null)
                return ToModel(formCategory);
            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public FormCategoryViewModel ToModel(FormCategory entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            return new FormCategoryViewModel
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
                Forms = entity.Forms.Where(c => c.DateDeactivated == null).Select(c => new Core.ViewModels.SubCategoryViewModel
                {
                    Guid = c.Guid,
                    Id = c.Id,
                    Name = c.FormTitle,
                    Status = c.FormStatu.Status,
                    IsPublished = c.IsPublished,
                    CreatedBy = c.CreatedBy,
                    IsDefaultVariable = c.IsDefaultForm,
                    ProjectId = c.FormDataEntry != null ? c.FormDataEntry.Guid : (Guid?)null,
                    EntityType = c.FormEntityTypes != null ? c.FormEntityTypes.Select(x => x.EntityType.Guid).ToList() : new List<Guid>(),
                }).OrderBy(x => x.Name).ToList(),
                IsDefaultFormCategory = entity.IsDefaultFormCategory,
            };
        }

        public FormCategoryViewModel Update(FormCategoryViewModel model)
        {
            if (dbContext.FormCategories.Any(est => est.CategoryName.ToLower() == model.CategoryName.ToLower()
                && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Form category already exists.");
            }
            var formCategory = dbContext.FormCategories
               .FirstOrDefault(fs => fs.Guid == model.Guid);
            var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);
            if (formCategory != null)
            {
                formCategory.CategoryName = model.CategoryName;
                formCategory.ModifiedBy = modifiedBy.Id;
                formCategory.ModifiedDate = DateTime.UtcNow;

                SaveChanges();

                return ToModel(formCategory);
            }
            return null;
        }
    }
}
