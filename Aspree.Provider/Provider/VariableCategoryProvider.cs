using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Aspree.Data;
using Aspree.Core.ViewModels;

namespace Aspree.Provider.Provider
{
    public class VariableCategoryProvider : IVariableCategoryProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        public VariableCategoryProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
        }

        public VariableCategoryViewModel Create(VariableCategoryViewModel model)
        {
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);

            if (this.dbContext.VariableCategories.Any(vc => vc.CategoryName.ToLower() == model.CategoryName.ToLower() && vc.Id != model.Id))
            {
                throw new Core.AlreadyExistsException("Variable Category is already exist.");
            }

            var tenantId = dbContext.Tenants.FirstOrDefault(x => x.Guid == model.TenantId);
            var newCategory = new VariableCategory()
            {
                CategoryName = model.CategoryName,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy.Id,
                TenantId = tenantId.Id,
                Guid = Guid.NewGuid(),
                IsDefaultVariableCategory = (int)Core.Enum.DefaultVariableType.Custom,
            };

            this.dbContext.VariableCategories.Add(newCategory);

            SaveChanges();

            return ToModel(newCategory);
        }

        public IEnumerable<VariableCategoryViewModel> GetAll()
        {
            return this.dbContext.VariableCategories.Select(ToModel).ToList();
        }

        public IEnumerable<VariableCategoryViewModel> GetAll(Guid tenantId)
        {
            return this.dbContext.VariableCategories
                .Where(vc => vc.TenantId.HasValue && vc.Tenant.Guid == tenantId && vc.DateDeactivated == null)
                .Select(ToModel)
                .ToList();
        }

        public VariableCategoryViewModel GetByGuid(Guid guid)
        {
            var category = this.dbContext.VariableCategories.FirstOrDefault(vc => vc.Guid == guid);
            if (category != null)
                return ToModel(category);
            else
                return null;
        }

        public VariableCategoryViewModel GetById(int Id)
        {
            var category = this.dbContext.VariableCategories.Find(Id);
            if (category != null)
                return ToModel(category);
            else
                return null;
        }

        public VariableCategoryViewModel ToModel(Aspree.Data.VariableCategory model)
        {
            var createdBy = _userLoginProvider.GetById(model.CreatedBy);
            var modifiedBy = model.ModifiedBy.HasValue ? _userLoginProvider.GetById(model.ModifiedBy.Value) : null;
            var deactivatedBy = model.DeactivatedBy.HasValue ? _userLoginProvider.GetById(model.DeactivatedBy.Value) : null;

            return new VariableCategoryViewModel()
            {
                Id = model.Id,
                CategoryName = model.CategoryName,
                CreatedBy = createdBy.Guid,
                CreatedDate = model.CreatedDate,
                ModifiedBy = modifiedBy == null ? (Guid?)null : modifiedBy.Guid,
                ModifiedDate = model.ModifiedDate,
                DeactivatedBy = deactivatedBy == null ? (Guid?)null : deactivatedBy.Guid,
                DeactivatedDate = model.DateDeactivated,
                Guid = model.Guid,
                Variables = model.Variables.Where(v => v.DateDeactivated == null).Select(c => new Core.ViewModels.SubCategoryViewModel
                {
                    Guid = c.Guid,
                    Id = c.Id,
                    Name = c.VariableName,
                    Status = string.Empty,
                    Type = c.VariableType.Type,
                    IsApproved = c.IsApproved,
                    IsDefaultVariable = c.IsDefaultVariable,
                }).OrderBy(x => x.IsDefaultVariable).ThenBy(x => x.Name).ToList(),
                IsDefaultVariableCategory = model.IsDefaultVariableCategory,
            };
        }

        public VariableCategoryViewModel Update(VariableCategoryViewModel model)
        {
            if (this.dbContext.VariableCategories.Any(vc => vc.CategoryName.ToLower() == model.CategoryName.ToLower() && vc.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Variable Category is already exist.");
            }

            var variableCategory = this.dbContext.VariableCategories
                .FirstOrDefault(vc => vc.Guid == model.Guid);

            var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);
            if (variableCategory != null)
            {
                variableCategory.CategoryName = model.CategoryName;
                variableCategory.ModifiedDate = DateTime.UtcNow;
                variableCategory.ModifiedBy = modifiedBy.Id;

                SaveChanges();

                return ToModel(variableCategory);
            }

            return null;
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        public VariableCategoryViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var variableCategory = this.dbContext.VariableCategories.Find(id);

            if (variableCategory != null)
            {
                variableCategory.DateDeactivated = DateTime.UtcNow;
                variableCategory.DeactivatedBy = deactivatedBy.Id;
                SaveChanges();
                return ToModel(variableCategory);
            }
            return null;
        }

        public VariableCategoryViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var variableCategory = this.dbContext.VariableCategories
                .FirstOrDefault(vc => vc.Guid == guid);

            if (variableCategory != null)
            {
                variableCategory.DateDeactivated = DateTime.UtcNow;
                variableCategory.DeactivatedBy = deactivatedBy.Id;
                SaveChanges();
                return ToModel(variableCategory);
            }
            return null;
        }
    }
}
