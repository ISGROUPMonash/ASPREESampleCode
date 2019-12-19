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
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities _dbContext;
        public RoleProvider(AspreeEntities dbContext, IUserLoginProvider userLoginProvider)
        {
            this._dbContext = dbContext;
            this._userLoginProvider = userLoginProvider;
        }

        public RoleModel Create(RoleModel model)
        {
            if (_dbContext.Roles.Any(est => est.Name.ToLower() == model.Name.ToLower() && !est.DateDeactivated.HasValue))
            {
                throw new Core.AlreadyExistsException("Role already exists.", "Name");
            }

            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            var tenant = this._dbContext.Tenants.FirstOrDefault(t => t.Guid == model.TenantId);

            var newRole = new Role
            {
                Id = model.Id,
                Name = model.Name,
                IsSystemRole = model.IsSystemRole,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
                Guid = Guid.NewGuid(),
                Status = (int)Core.Enum.Status.Active,
                TenantId = tenant != null ? tenant.Id : 0
            };
            _dbContext.Roles.Add(newRole);
            SaveChanges();
            return ToModel(newRole);
        }

        public RoleModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var role = _dbContext.Roles.FirstOrDefault(fs => fs.Guid == guid);
            if (role != null)
            {
                if (role.DateDeactivated.HasValue)
                {
                    role.DateDeactivated = null;
                    role.DeactivatedBy = null;
                }
                else
                {
                    role.DateDeactivated = DateTime.UtcNow;
                    role.DeactivatedBy = deactivatedBy.Id;
                }
                return ToModel(role);
            }
            return null;
        }

        public RoleModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var role = _dbContext.Roles.FirstOrDefault(fs => fs.Id == id);
            if (role != null)
            {
                if (role.DateDeactivated.HasValue)
                {
                    role.DateDeactivated = null;
                    role.DeactivatedBy = null;
                }
                else
                {
                    role.DateDeactivated = DateTime.UtcNow;
                    role.DeactivatedBy = deactivatedBy.Id;
                }
                return ToModel(role);
            }
            return null;
        }

        public IEnumerable<RoleModel> GetAll()
        {
            return _dbContext.Roles
                .Where(r => r.Name != Core.Enum.RoleTypes.Definition_Admin.ToString().Replace("_", " "))
                .OrderByDescending(d => d.Id)
                .Select(ToModel)
                .ToList();
        }

        public IEnumerable<RoleModel> GetAll(Guid tenantId)
        {
            return _dbContext.Roles
                .Where(r => r.Name != Core.Enum.RoleTypes.Definition_Admin.ToString().Replace("_", " ")
                         && (!r.TenantId.HasValue || (r.TenantId.HasValue && r.Tenant.Guid == tenantId)))
                .OrderByDescending(d => d.Id)
                .Select(ToModel)
                .ToList();
        }

        public RoleModel GetByGuid(Guid guid)
        {
            var role = _dbContext.Roles
               .FirstOrDefault(fs => fs.Guid == guid);

            if (role != null)
                return ToModel(role);

            return null;
        }

        public RoleModel GetById(int id)
        {
            var role = _dbContext.Roles
                .FirstOrDefault(fs => fs.Id == id);

            if (role != null)
                return ToModel(role);

            return null;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public RoleModel ToModel(Role entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;

            return new RoleModel
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy == null ? (Guid?)null : deactivatedBy.Guid,
                Guid = entity.Guid,
                ModifiedBy = modifiedBy == null ? (Guid?)null : modifiedBy.Guid,
                ModifiedDate = entity.ModifiedDate,
                IsSystemRole = entity.IsSystemRole,
                TenantId = entity.TenantId.HasValue ? entity.Tenant.Guid : (Guid?)(null),
                Privileges = new List<Guid>(),
            };
        }

        public RoleModel Update(RoleModel model)
        {
            if (_dbContext.Roles.Any(est => est.Name.ToLower() == model.Name.ToLower()
               && est.Guid != model.Guid && !est.DateDeactivated.HasValue))
            {
                throw new Core.AlreadyExistsException("Role already exists.", "Name");
            }

            var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);

            var editRole = _dbContext.Roles
               .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (editRole != null)
            {
                editRole.Name = model.Name;
                editRole.ModifiedBy = modifiedBy.Id;
                editRole.ModifiedDate = DateTime.UtcNow;
                _dbContext.RolePrivileges.RemoveRange(editRole.RolePrivileges.ToList());
                SaveChanges();

                return ToModel(editRole);
            }

            return null;
        }

        public bool ChangeStatus(Guid roleGuid, int newStatus)
        {
            var role = this._dbContext.Roles.FirstOrDefault(ul => ul.Guid == roleGuid);
            if (role == null)
            {
                throw new Core.NotFoundException("Role was not found");
            }

            role.Status = newStatus;

            SaveChanges();

            return true;
        }
    }
}
