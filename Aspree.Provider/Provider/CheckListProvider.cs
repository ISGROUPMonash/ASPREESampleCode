using Aspree.Provider.Interface;
using Aspree.Data;
using Aspree.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aspree.Provider.Provider
{
    public class CheckListProvider : ICheckListProvider
    {
        private readonly AspreeEntities dbContext;
        private readonly IUserLoginProvider _userLoginProvider;
    
        public CheckListProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
        }

        public IEnumerable<CheckListViewModel> GetAll()
        {
            return dbContext.CheckLists.Select(ToModel).ToList();
        }

        public CheckListViewModel GetById(int id)
        {
            var checkList = dbContext.CheckLists.FirstOrDefault(fs => fs.Id == id);
            if (checkList != null)
                return ToModel(checkList);
                return null;
        }

        public CheckListViewModel GetByGuid(Guid guid)
        {
            var checkList = dbContext.CheckLists
                .FirstOrDefault(fs => fs.Guid == guid);
            if (checkList != null)
                return ToModel(checkList);
            return null;
        }

        public CheckListViewModel Create(CheckListViewModel model)
        {
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            if (dbContext.CheckLists.Any(est => est.Title.ToLower() == model.Title.ToLower()))
            {
                throw new Core.AlreadyExistsException("Check list already exists.");
            }
            var checkList = new CheckList
            {
                Title = model.Title,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
                Guid = Guid.NewGuid()
            };
            dbContext.CheckLists.Add(checkList);
            SaveChanges();
            return ToModel(checkList);
        }

        public CheckListViewModel Update(CheckListViewModel model)
        {
            var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);
            if (dbContext.CheckLists.Any(est => est.Title.ToLower() == model.Title.ToLower()
                && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Check list already exists.");
            }
            var checkList = dbContext.CheckLists
               .FirstOrDefault(fs => fs.Guid == model.Guid);
            if (checkList != null)
            {
                checkList.Title = model.Title;
                checkList.ModifiedBy = modifiedBy.Id;
                checkList.ModifiedDate = DateTime.UtcNow;
                SaveChanges();
                return ToModel(checkList);
            }
            return null;
        }

        public CheckListViewModel ToModel(CheckList entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            return new CheckListViewModel
            {
                CreatedBy = createdBy.Guid,
                ModifiedDate = entity.ModifiedDate,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy == null ? (Guid?)null : deactivatedBy.Guid,
                Guid = entity.Guid,
                Id = entity.Id,
                ModifiedBy = modifiedBy == null ? (Guid?)null : modifiedBy.Guid,
                Title = entity.Title
            };
        }

        public CheckListViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var checkList = dbContext.CheckLists
                .FirstOrDefault(fs => fs.Id == id);
            if (checkList != null)
            {
                checkList.DateDeactivated = DateTime.UtcNow;
                checkList.DeactivatedBy = deactivatedBy.Id;
                return ToModel(checkList);
            }
            return null;
        }

        public CheckListViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var checkList = dbContext.CheckLists
                .FirstOrDefault(fs => fs.Guid == guid);
            if (checkList != null)
            {
                checkList.DateDeactivated = DateTime.UtcNow;
                checkList.DeactivatedBy = deactivatedBy.Id;
                return ToModel(checkList);
            }
            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }


    }
}
