using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.ViewModels;
using Aspree.Data;

namespace Aspree.Provider.Provider
{
    public class TenantProvider : ITenantProvider
    {
        private readonly AspreeEntities dbContext;
        
        public TenantProvider(AspreeEntities _dbContext)
        {
            this.dbContext = _dbContext;
        }

        public TenantViewModel Create(TenantViewModel model)
        {
            throw new NotImplementedException();
        }

        public TenantViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            throw new NotImplementedException();
        }

        public TenantViewModel DeleteById(int id, Guid DeletedBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TenantViewModel> GetAll()
        {
            return this.dbContext.Tenants
               .Select(ToModel)
               .ToList();
        }

        public TenantViewModel GetByGuid(Guid guid)
        {
            var userLogin = dbContext.Tenants.FirstOrDefault(ul => ul.Guid == guid);

            if (userLogin != null)
                return ToModel(userLogin);

            return null;
        }

        public TenantViewModel GetById(int id)
        {
            var userLogin = dbContext.Tenants.FirstOrDefault(ul => ul.Id == id);

            if (userLogin != null)
                return ToModel(userLogin);

            return null;
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        public TenantViewModel ToModel(Tenant entity)
        {
            return new TenantViewModel {
                CompanyName = entity.CompanyName,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = entity.DeactivatedBy,
                Email = entity.Email,
                FirstName = entity.Email,
                Guid = entity.Guid,
                Id = entity.Id,
                LastName = entity.LastName,
                ModifiedBy = entity.ModifiedBy,
                ModifiedDate = entity.ModifiedDate,
                Status = entity.Status
            };
        }

        public TenantViewModel Update(TenantViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
