using Aspree.Core.ViewModels;
using Aspree.Data;
using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Provider
{
    public class EntityFormDataVariableProvider : IEntityFormDataVariableProvider
    {

        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        public EntityFormDataVariableProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
        }

        public EntityFormDataVariableViewModel Create(EntityFormDataVariableViewModel model)
        {
            if (dbContext.EntityFormDataVariables.Any(et => et.EntityName== model.EntityName))
            {
                throw new Core.AlreadyExistsException("Entity Name in Entity Form Data Variable already exists.");
            }
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            var entity = this.dbContext.Entities.FirstOrDefault(e => e.Guid == model.EntityGuid);
            var entityType = this.dbContext.EntityTypes.FirstOrDefault(et => et.Guid == model.EntityTypeGuid);
            var entitySubType = this.dbContext.EntitySubTypes.FirstOrDefault(et => et.Guid == model.EntitySubTypeGuid);
            var variable = this.dbContext.Variables.FirstOrDefault();
            var entityformdatavariable = new EntityFormDataVariable()
            {
                Guid = Guid.NewGuid(),
                EntityId = entity.Id,
                VariableId = variable.Id,
                SelectedValues = model.SelectedValues,
                EntityTypeId = entityType.Id,
                EntitySubTypeId = entitySubType != null ? entitySubType.Id : (int?)null,
                EntityName=model.EntityName,
                Json = model.Json,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
            };

            dbContext.EntityFormDataVariables.Add(entityformdatavariable);

            SaveChanges();

            return ToModel(entityformdatavariable);
        }

        public EntityFormDataVariableViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var entity = dbContext.EntityFormDataVariables.FirstOrDefault(fs => fs.Guid == guid);
            if (entity != null)
            {
                entity.DeactivatedBy = deactivatedBy.Id;
                entity.DateDeactivated = DateTime.UtcNow;
                SaveChanges();
                return ToModel(entity);
            }

            return null;
        }

        public EntityFormDataVariableViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var entity = dbContext.EntityFormDataVariables.FirstOrDefault(fs => fs.Id == id);
            if (entity != null)
            {
                entity.DeactivatedBy = deactivatedBy.Id;
                entity.DateDeactivated = DateTime.UtcNow;
                SaveChanges();
                return ToModel(entity);
            }

            return null;
        }
        public IEnumerable<EntityFormDataVariableViewModel> GetAll()
        {
            return dbContext.EntityFormDataVariables.Where(x => x.DeactivatedBy == null)
              .Select(ToModel)
              .ToList();
        }
        public EntityFormDataVariableViewModel GetByGuid(Guid guid)
        {
            var entity = dbContext.EntityFormDataVariables
                .FirstOrDefault(fs => fs.Guid == guid);

            if (entity != null)
                return ToModel(entity);

            return null;
        }
        public EntityFormDataVariableViewModel GetById(int id)
        {
            var entity = dbContext.EntityFormDataVariables
                 .FirstOrDefault(fs => fs.Id == id);

            if (entity != null)
                return ToModel(entity);

            return null;
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        public EntityFormDataVariableViewModel ToModel(EntityFormDataVariable entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            var entityData = this.dbContext.Entities.FirstOrDefault(x => x.Id == entity.EntityId);
            var entitytypeData = this.dbContext.EntityTypes.FirstOrDefault(x => x.Id == entity.EntityTypeId);
            var entitysubtypeData = this.dbContext.EntitySubTypes.FirstOrDefault(x => x.Id == entity.EntitySubTypeId);
            return new EntityFormDataVariableViewModel()
            {
                Id = entity.Id,
                CreatedBy = createdBy.Guid,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Id : (int?)null,
                DateDeactivated = entity.DateDeactivated,

                Guid = entity.Guid,
                EntityId = entity.EntityId,
                VariableId = entity.VariableId,
                SelectedValues = entity.SelectedValues,
                EntityTypeId = entity.EntityTypeId,
                EntitySubTypeId = entity.EntitySubTypeId,
                Json = entity.Json,

                VariableGuid = entity.Variable != null ? entity.Variable.Guid : (Guid?)null,

                EntityName = entity.EntityName,
                EntityTypeName = entitytypeData != null ? entitytypeData.Name : "",
                EntitySubTypeName = entitysubtypeData != null ? entitysubtypeData.Name : "",

                EntityGuid = entityData != null ? entityData.Guid : (Guid?)null,
                EntityTypeGuid = entitytypeData != null ? entitytypeData.Guid : (Guid?)null,
                EntitySubTypeGuid = entitysubtypeData != null ? entitysubtypeData.Guid : (Guid?)null,
            };
        }

        public EntityFormDataVariableViewModel Update(EntityFormDataVariableViewModel model)
        {
            if (dbContext.EntityFormDataVariables.Any(et => et.EntityName == model.EntityName
            && et.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Entity Form Data Variable already exists.");
            }

            var entity = dbContext.EntityFormDataVariables
              .FirstOrDefault(fs => fs.Guid == model.Guid);
            if (entity != null)
            {
                var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);
                var entityType = this.dbContext.EntityTypes.FirstOrDefault(et => et.Guid == model.EntityTypeGuid);
                var entitySubType = this.dbContext.EntitySubTypes.FirstOrDefault(et => et.Guid == model.EntitySubTypeGuid);


                entity.EntityTypeId = entityType.Id; ;
                entity.EntitySubTypeId = entitySubType != null ? entitySubType.Id : (int?)null;
                entity.Json = model.Json;
                entity.EntityName = model.EntityName;

                entity.ModifiedBy = modifiedBy.Id;
                entity.ModifiedDate = DateTime.UtcNow;

                return ToModel(entity);
            }
            return null;
        }
        public EntityVariableViewModel ToEntityVariableModel(EntityVariable entity)
        {
            var variable = dbContext.Variables.FirstOrDefault(x => x.Id == entity.VariableId);

            return new EntityVariableViewModel()
            {
                Guid = entity.Guid,
                EntityId = entity.EntityId,
                VariableId = entity.VariableId,
                Id = entity.Id,
                EntityVariableGuid = variable.Guid,
            };
        }
    }
}