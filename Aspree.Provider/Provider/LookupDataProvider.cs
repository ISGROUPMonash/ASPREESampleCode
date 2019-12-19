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
    public class LookupDataProvider : ILookupDataProvider
    {
        private readonly IVariableTypeProvider _VariableTypeProvider;
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;

        public LookupDataProvider(AspreeEntities _dbContext,IUserLoginProvider userLoginProvider,IVariableTypeProvider VariableTypeProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
            this._VariableTypeProvider = VariableTypeProvider;
        }

        public LookupDataViewModel Create(LookupDataViewModel model)
        {
            if (dbContext.LookupDatas.Any(est => est.Title.ToLower() == model.Title.ToLower()))
            {
                throw new Core.AlreadyExistsException("Lookup title already exists.");
            }
            var createdby = _userLoginProvider.GetByGuid(model.CreatedBy);
          
            var variableType = _VariableTypeProvider.GetByGuid(model.VariableTypeId);

            if (variableType == null) {
                throw new Core.BadRequestException("Please provide a valid variable type");
            }
            var LookupDatas = new LookupData
            {
                Title = model.Title,
                CreatedBy= createdby.Id,
                CreatedDate=model.CreatedDate,
                Status = model.Status,
                VariableTypeId = variableType.Id,
               
                LookupDataType = model.LookupDataType,
                Guid = Guid.NewGuid()
            };

            dbContext.LookupDatas.Add(LookupDatas);

            SaveChanges();

            return ToModel(LookupDatas);
        }

        public LookupDataViewModel DeleteById(int id, Guid DeletedBy)
        {
            var LookupDatas = dbContext.LookupDatas.FirstOrDefault(fs => fs.Id == id);
            if (LookupDatas != null)
            {
                dbContext.LookupDatas.Remove(LookupDatas);
                return ToModel(LookupDatas);
            }

            return null;
        }

        public LookupDataViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var LookupDatas = dbContext.LookupDatas.FirstOrDefault(fs => fs.Guid == guid);
            if (LookupDatas != null)
            {
                LookupDatas.DeactivatedBy = deactivatedBy.Id;
                LookupDatas.DateDeactivated = DateTime.UtcNow;
                dbContext.LookupDatas.Remove(LookupDatas);
                return ToModel(LookupDatas);
            }

            return null;
        }

        public IEnumerable<LookupDataViewModel> GetAll()
        {
            return dbContext.LookupDatas
                .Select(ToModel)
                .ToList();
        }

        public LookupDataViewModel GetByGuid(Guid guid)
        {
            var LookupDatas = dbContext.LookupDatas
                .FirstOrDefault(fs => fs.Guid == guid);

            if (LookupDatas != null)
                return ToModel(LookupDatas);

            return null;
        }

        public LookupDataViewModel GetById(int id)
        {
            var LookupDatas = dbContext.LookupDatas
                .FirstOrDefault(fs => fs.Id == id);

            if (LookupDatas != null)
                return ToModel(LookupDatas);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public LookupDataViewModel ToModel(LookupData entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            var variableType = _VariableTypeProvider.GetById(entity.VariableTypeId);
            return new LookupDataViewModel
            {
                Title = entity.Title,
                Status = entity.Status,
                CreatedBy = createdBy.Guid,
                CreatedDate =entity.CreatedDate,
                ModifiedBy = modifiedBy == null ? (Guid?)null : modifiedBy.Guid,
                ModifiedDate = entity.ModifiedDate,
                DeactivatedBy = deactivatedBy == null ? (Guid?)null : deactivatedBy.Guid,
                DateDeactivated = entity.DateDeactivated,
                VariableTypeId = variableType.Guid,
                LookupDataType = entity.LookupDataType,
                Guid = entity.Guid ?? Guid.Empty,
                Id = entity.Id
            };
        }

        public LookupDataViewModel Update(LookupDataViewModel model)
        {
            if (dbContext.LookupDatas.Any(est => est.Title.ToLower() == model.Title.ToLower()
                 && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Lookup data already exists.");
            }

            var LookupDatas = dbContext.LookupDatas
                .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (LookupDatas != null)
            {
                var variableType = _VariableTypeProvider.GetByGuid(model.VariableTypeId);

                if (variableType == null)
                {
                    throw new Core.BadRequestException("Please provide a valid variable type");
                }

                var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);
                LookupDatas.Status = model.Status;
                LookupDatas.Title = model.Title;
                LookupDatas.ModifiedBy = modifiedBy.Id;
                LookupDatas.ModifiedDate = DateTime.UtcNow;
                LookupDatas.VariableTypeId = variableType.Id;
                SaveChanges();

                return ToModel(LookupDatas);
            }

            return null;
        }
    }
}
