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
    public class RegExRuleProvider : IRegExRuleProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        public RegExRuleProvider(AspreeEntities _dbContext,IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
        }

        public RegExRuleViewModel Create(RegExRuleViewModel model)
        {
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
         
            if (dbContext.RegExRules.Any(est => est.RegExName.ToLower() == model.RegExName.ToLower()))
            {
                throw new Core.AlreadyExistsException("RegEx rule already exists.");
            }

            var regExRule = new RegExRule
            {
                Description = model.Description,
                RegEx = model.RegEx,
                RegExName = model.RegExName,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
                Guid = Guid.NewGuid()
            };

            dbContext.RegExRules.Add(regExRule);

            SaveChanges();

            return ToModel(regExRule);
        }

        public RegExRuleViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var regExRule = dbContext.RegExRules
                .FirstOrDefault(fs => fs.Id == id);

            if (regExRule != null)
            {
                regExRule.DeactivatedBy = deactivatedBy.Id;
                regExRule.DateDeactivated = DateTime.UtcNow;
                return ToModel(regExRule);
            }

            return null;
        }

        public RegExRuleViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var regExRule = dbContext.RegExRules
                .FirstOrDefault(fs => fs.Guid == guid);

            if (regExRule != null)
            {
                regExRule.DeactivatedBy = deactivatedBy.Id;
                regExRule.DateDeactivated = DateTime.UtcNow;
                return ToModel(regExRule);
            }

            return null;
        }

        public IEnumerable<RegExRuleViewModel> GetAll()
        {
            return dbContext.RegExRules
                .Select(ToModel)
                .ToList();
        }

        public RegExRuleViewModel GetByGuid(Guid guid)
        {
            var regExRule = dbContext.RegExRules
                 .FirstOrDefault(fs => fs.Guid == guid);

            if (regExRule != null)
                return ToModel(regExRule);

            return null;
        }

        public RegExRuleViewModel GetById(int id)
        {
            var regExRule = dbContext.RegExRules
                 .FirstOrDefault(fs => fs.Id == id);

            if (regExRule != null)
                return ToModel(regExRule);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public RegExRuleViewModel ToModel(RegExRule entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;

            return new RegExRuleViewModel
            {
                Id = entity.Id,
                RegExName = entity.RegExName,
                RegEx = entity.RegEx,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy == null ? (Guid?)null : deactivatedBy.Guid,
                Guid = entity.Guid,
                ModifiedBy = modifiedBy == null ? (Guid?)null : modifiedBy.Guid,
                ModifiedDate = entity.ModifiedDate,
                CreatedBy = createdBy.Guid,
                Description = entity.Description,
                
            };
        }

        public RegExRuleViewModel Update(RegExRuleViewModel model)
        {
          
            if (dbContext.RegExRules.Any(est => est.RegExName.ToLower() == model.RegExName.ToLower()
                 && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("RegEx rule already exists.");
            }
            var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);
            var regExRule = dbContext.RegExRules
                 .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (regExRule != null)
            {
                regExRule.RegExName = model.RegExName;
                regExRule.RegEx = model.RegEx;
                regExRule.Description = model.Description;
                regExRule.ModifiedBy =modifiedBy.Id;
                regExRule.ModifiedDate = DateTime.UtcNow;

                SaveChanges();

                return ToModel(regExRule);
            }

            return null;
        }
    }
}