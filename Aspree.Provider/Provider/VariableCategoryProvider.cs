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
        private readonly AspreeEntities dbContext;
        public VariableCategoryProvider(AspreeEntities _dbContext)
        {
            this.dbContext = _dbContext;
        }
        /// <summary>
        /// Method to get all variable categories on the basis of respective tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of VariableCategoryViewModel model</returns>
        public IEnumerable<VariableCategoryViewModel> GetAll(Guid tenantId)
        {
            return this.dbContext.VariableCategories
                .Where(vc => vc.TenantId.HasValue && vc.Tenant.Guid == tenantId && vc.DateDeactivated == null)
                .Select(ToModel)
                .ToList();
        }
        /// <summary>
        /// Map VariableCategory to VariableCategoryViewModel model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public VariableCategoryViewModel ToModel(VariableCategory model)
        {
            return new VariableCategoryViewModel()
            {
                Id = model.Id,
                CategoryName = model.CategoryName,
                CreatedDate = model.CreatedDate,
                ModifiedDate = model.ModifiedDate,
                DeactivatedDate = model.DateDeactivated,
                Guid = model.Guid,
                Variables = model.Variables.Where(v => v.DateDeactivated == null).Select(c => new SubCategoryViewModel
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
        
    }
}
