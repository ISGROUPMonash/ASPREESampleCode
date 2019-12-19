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
    public class EntitySubTypeProvider : IEntitySubTypeProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        private readonly IEntityTypeProvider _entityTypeProvider;
        public EntitySubTypeProvider(AspreeEntities _dbContext, IEntityTypeProvider entityTypeProvider, IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._entityTypeProvider = entityTypeProvider;
            this._userLoginProvider = userLoginProvider;
        }

        public EntitySubTypeViewModel Create(EntitySubTypeViewModel model)
        {
            if (dbContext.EntitySubTypes.Any(est => est.Name.ToLower() == model.Name.ToLower()))
            {
                throw new Core.AlreadyExistsException("Entity sub type already exists.");
            }

            var entityType = _entityTypeProvider.GetByGuid(model.EntityTypeId);

            if (entityType == null)
            {
                throw new Core.BadRequestException("Please provide valid entity type.");
            }

            var entitySubType = new EntitySubType
            {
                EntityTypeId = entityType.Id,
                Name = model.Name,
                Guid = Guid.NewGuid()
            };

            dbContext.EntitySubTypes.Add(entitySubType);

            SaveChanges();

            return ToModel(entitySubType);
        }

        public EntitySubTypeViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var entitySubType = dbContext.EntitySubTypes
                .FirstOrDefault(fs => fs.Id == deactivatedBy.Id);

            if (entitySubType != null)
            {
                dbContext.EntitySubTypes.Remove(entitySubType);
                return ToModel(entitySubType);
            }

            return null;
        }

        public EntitySubTypeViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var entitySubType = dbContext.EntitySubTypes
                .FirstOrDefault(fs => fs.Guid == guid);

            if (entitySubType != null)
            {
                dbContext.EntitySubTypes.Remove(entitySubType);
                return ToModel(entitySubType);
            }

            return null;
        }

        public IEnumerable<EntitySubTypeViewModel> GetAll()
        {
            return dbContext.EntitySubTypes
                .Select(ToModel)
                .ToList();
        }

        public EntitySubTypeViewModel GetByGuid(Guid guid)
        {
            var entitySubType = dbContext.EntitySubTypes
                .FirstOrDefault(fs => fs.Guid == guid);

            if (entitySubType != null)
                return ToModel(entitySubType);

            return null;
        }

        public EntitySubTypeViewModel GetById(int id)
        {
            var entitySubType = dbContext.EntitySubTypes
                .FirstOrDefault(fs => fs.Id == id);

            if (entitySubType != null)
                return ToModel(entitySubType);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public EntitySubTypeViewModel ToModel(EntitySubType entity)
        {
            var entityType = _entityTypeProvider.GetById(entity.EntityTypeId);

            return new EntitySubTypeViewModel
            {
                EntityTypeId = entityType.Guid,
                Id = entity.Id,
                Guid = entity.Guid,
                Name = entity.Name,
                EntityTypeName=entityType.Name,
            };
        }

        public EntitySubTypeViewModel Update(EntitySubTypeViewModel model)
        {
            if (dbContext.EntitySubTypes.Any(est => est.Name.ToLower() == model.Name.ToLower()
                && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Entity sub type already exists.");
            }

            var entitySubType = dbContext.EntitySubTypes
                .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (entitySubType != null)
            {
                var entityType = _entityTypeProvider.GetByGuid(model.EntityTypeId);

                if (entityType == null)
                {
                    throw new Core.BadRequestException("Please provide valid entity type.");
                }

                entitySubType.Name = model.Name;

                SaveChanges();

                return ToModel(entitySubType);
            }

            return null;
        }
    }
}
