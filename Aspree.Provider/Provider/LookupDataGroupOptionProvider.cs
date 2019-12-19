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
    public class LookupDataGroupOptionProvider : ILookupDataGroupOptionProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        private readonly ILookupDataOptionGroupProvider _LookupDataOptionGroupProvider;
        public LookupDataGroupOptionProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider,ILookupDataOptionGroupProvider LookupDataOptionGroupProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
            this._LookupDataOptionGroupProvider = LookupDataOptionGroupProvider;
        }

        public LookupDataGroupOptionViewModel Create(LookupDataGroupOptionViewModel model)
        {
            var optiongroup = _LookupDataOptionGroupProvider.GetByGuid(model.LookupDataOptionGroupId);
            var LookupDataGroupOptions = new LookupDataGroupOption
            {
                Text = model.Text,
                Value = model.Value,
                LookupDataOptionGroupId =optiongroup.Id,
                Guid = Guid.NewGuid()
            };

            dbContext.LookupDataGroupOptions.Add(LookupDataGroupOptions);

            SaveChanges();

            return ToModel(LookupDataGroupOptions);
        }

        public LookupDataGroupOptionViewModel DeleteById(int id, Guid DeletedBy)
        {
            var LookupDataGroupOptions = dbContext.LookupDataGroupOptions.FirstOrDefault(fs => fs.Id == id);
            if (LookupDataGroupOptions != null)
            {
                dbContext.LookupDataGroupOptions.Remove(LookupDataGroupOptions);
                return ToModel(LookupDataGroupOptions);
            }

            return null;
        }

        public LookupDataGroupOptionViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var LookupDataGroupOptions = dbContext.LookupDataGroupOptions
                .FirstOrDefault(fs => fs.Guid == guid);

            if (LookupDataGroupOptions != null)
            {
                dbContext.LookupDataGroupOptions.Remove(LookupDataGroupOptions);
                return ToModel(LookupDataGroupOptions);
            }

            return null;
        }

        public IEnumerable<LookupDataGroupOptionViewModel> GetAll()
        {
            return dbContext.LookupDataGroupOptions
                .Select(ToModel)
                .ToList();
        }

        public LookupDataGroupOptionViewModel GetByGuid(Guid guid)
        {
            var LookupDataGroupOptions = dbContext.LookupDataGroupOptions
                .FirstOrDefault(fs => fs.Guid == guid);

            if (LookupDataGroupOptions != null)
                return ToModel(LookupDataGroupOptions);

            return null;
        }

        public LookupDataGroupOptionViewModel GetById(int id)
        {
            var LookupDataGroupOptions = dbContext.LookupDataGroupOptions
                .FirstOrDefault(fs => fs.Id == id);

            if (LookupDataGroupOptions != null)
                return ToModel(LookupDataGroupOptions);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public LookupDataGroupOptionViewModel ToModel(LookupDataGroupOption entity)
        {
            var optiongroup = _LookupDataOptionGroupProvider.GetById(entity.LookupDataOptionGroupId);

            return new LookupDataGroupOptionViewModel
            {   
                Text = entity.Text,
                Value = entity.Value,
                LookupDataOptionGroupId = optiongroup.Guid,
                Guid = entity.Guid
            };
        }

        public LookupDataGroupOptionViewModel Update(LookupDataGroupOptionViewModel model)
        {
            var options = dbContext.LookupDataGroupOptions
                .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (options != null)
            {
                options.Text  = model.Text;
                options.Value = model.Value;
                SaveChanges();

                return ToModel(options);
            }

            return null;
        }
    }
}
