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
    public class EntityTypeProvider : IEntityTypeProvider
    {
        
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        public EntityTypeProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
        }

        public EntityTypeViewModel Create(EntityTypeViewModel model)
        {
            if (dbContext.EntityTypes.Any(et => et.Name.ToLower() == model.Name.ToLower()))
            {
                throw new Core.AlreadyExistsException("Entity type already exists.");
            }

            var tenant = dbContext.Tenants.FirstOrDefault(x => x.Guid == model.TenantId);

            var entityType = new EntityType()
            {
                Guid = Guid.NewGuid(),
                Name = model.Name,
                TenantId = tenant != null ? tenant.Id : (int?)null,

            };

            dbContext.EntityTypes.Add(entityType);

            SaveChanges();

            return ToModel(entityType);
        }

        public EntityTypeViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var entityType = dbContext.EntityTypes.FirstOrDefault(fs => fs.Id == id);
            if (entityType != null)
            {
                dbContext.EntityTypes.Remove(entityType);
                return ToModel(entityType);
            }

            return null;
        }

        public EntityTypeViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var entityType = dbContext.EntityTypes.FirstOrDefault(fs => fs.Guid == guid);
            if (entityType != null)
            {
                dbContext.EntityTypes.Remove(entityType);
                return ToModel(entityType);
            }

            return null;
        }

        public IEnumerable<EntityTypeViewModel> GetAll()
        {
            return dbContext.EntityTypes
                .Select(ToModel)
                .ToList();
        }

        public IEnumerable<EntityTypeViewModel> GetAll(Guid tenantId)
        {
            return dbContext.EntityTypes
                .Select(ToModel)
                .ToList();
        }

        public EntityTypeViewModel GetByGuid(Guid guid)
        {
            var entityType = dbContext.EntityTypes
                .FirstOrDefault(fs => fs.Guid == guid);

            if (entityType != null)
                return ToModel(entityType);

            return null;
        }

        public EntityTypeViewModel GetById(int id)
        {
            var entityType = dbContext.EntityTypes
                .FirstOrDefault(fs => fs.Id == id);

            if (entityType != null)
                return ToModel(entityType);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public EntityTypeViewModel ToModel(EntityType entity)
        {
            return new EntityTypeViewModel()
            {
                Guid = entity.Guid,
                Name = entity.Name,
                Id = entity.Id,
            };
        }

        public EntityTypeViewModel Update(EntityTypeViewModel model)
        {
            if (dbContext.EntityTypes.Any(et => et.Name.ToLower() == model.Name.ToLower()
            && et.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Entity type already exists.");
            }

            var entityType = dbContext.EntityTypes
              .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (entityType != null)
            {
                entityType.Name = model.Name;

                return ToModel(entityType);
            }

            return null;
        }
    }
}
