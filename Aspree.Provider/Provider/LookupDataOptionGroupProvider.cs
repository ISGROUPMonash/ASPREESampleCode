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
    public class LookupDataOptionGroupProvider : ILookupDataOptionGroupProvider
    {
        private readonly ILookupDataProvider  _LookupDataProvider ;
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        public LookupDataOptionGroupProvider(AspreeEntities _dbContext,IUserLoginProvider userLoginProvider,ILookupDataProvider LookupDataProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
            this._LookupDataProvider = LookupDataProvider;
        }

        public LookupDataOptionGroupViewModel Create(LookupDataOptionGroupViewModel model)
        {
            var optiongroup = _LookupDataProvider.GetByGuid(model.LookupDataId);
            var LookupDataOptionGroups = new LookupDataOptionGroup
            {
                Label = model.Label,
                LookupDataId = optiongroup.Id,
                Value = model.Value,
                Guid = Guid.NewGuid(),
            };

            dbContext.LookupDataOptionGroups.Add(LookupDataOptionGroups);
            SaveChanges();
            return ToModel(LookupDataOptionGroups);
        }

        public LookupDataOptionGroupViewModel DeleteById(int id, Guid DeletedBy)
        {
            var LookupDataOptionGroups = dbContext.LookupDataOptionGroups.FirstOrDefault(fs => fs.Id == id);
            if (LookupDataOptionGroups != null)
            {
                dbContext.LookupDataOptionGroups.Remove(LookupDataOptionGroups);
                return ToModel(LookupDataOptionGroups);
            }
            return null;
        }

        public LookupDataOptionGroupViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var LookupDataOptionGroups = dbContext.LookupDataOptionGroups
                .FirstOrDefault(fs => fs.Guid == guid);

            if (LookupDataOptionGroups != null)
            {
                dbContext.LookupDataOptionGroups.Remove(LookupDataOptionGroups);
                return ToModel(LookupDataOptionGroups);
            }

            return null;
        }

        public IEnumerable<LookupDataOptionGroupViewModel> GetAll()
        {
            return dbContext.LookupDataOptionGroups
                .Select(ToModel)
                .ToList();
        }

        public LookupDataOptionGroupViewModel GetByGuid(Guid guid)
        {
            var LookupDataOptionGroups = dbContext.LookupDataOptionGroups
                .FirstOrDefault(fs => fs.Guid == guid);

            if (LookupDataOptionGroups != null)
                return ToModel(LookupDataOptionGroups);
            return null;
        }

        public LookupDataOptionGroupViewModel GetById(int id)
        {
            var LookupDataOptionGroups = dbContext.LookupDataOptionGroups
                .FirstOrDefault(fs => fs.Id == id);

            if (LookupDataOptionGroups != null)
                return ToModel(LookupDataOptionGroups);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public LookupDataOptionGroupViewModel ToModel(LookupDataOptionGroup entity)
        {
            var optiongroup = _LookupDataProvider.GetById(entity.LookupDataId);
            return new LookupDataOptionGroupViewModel
            {    Id=entity.Id,
                 Label = entity.Label,
                 Value = entity.Value,
                 LookupDataId = optiongroup.Guid,
                 Guid = entity.Guid
            };
        }

        public LookupDataOptionGroupViewModel Update(LookupDataOptionGroupViewModel model)
        {
            var options = dbContext.LookupDataOptionGroups
                .FirstOrDefault(fs => fs.Guid == model.Guid);
            if (options != null)
            {
                options.Label = model.Label;
                options.Value = model.Value;
                SaveChanges();
                return ToModel(options);
            }
            return null;
        }
    }
}