using Aspree.Core.Enum;
using Aspree.Core.ViewModels;
using Aspree.Data;
using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Provider
{
    public class RoleProvider : IRoleProvider 
    {
        private readonly AspreeEntities _dbContext;
        public RoleProvider(AspreeEntities dbContext)
        {
            this._dbContext = dbContext;
        }
        /// <summary>
        /// Method to get all roles on the basis of respective tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of RoleModel model</returns>
        public IEnumerable<RoleModel> GetAll(Guid tenantId)
        {
            return _dbContext.Roles
                .Where(r => r.Name != Core.Enum.RoleTypes.Definition_Admin.ToString().Replace("_", " ")
                         && (!r.TenantId.HasValue || (r.TenantId.HasValue && r.Tenant.Guid == tenantId)))
                .OrderByDescending(d => d.Id)
                .Select(ToModel)
                .ToList();
        }
        /// <summary>
        /// Map Role entity to Role Model
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public RoleModel ToModel(Role entity)
        {
            return new RoleModel
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                Guid = entity.Guid,
                ModifiedDate = entity.ModifiedDate,
                IsSystemRole = entity.IsSystemRole,
                TenantId = entity.TenantId.HasValue ? entity.Tenant.Guid : (Guid?)(null)
            };
        }
    }
}
