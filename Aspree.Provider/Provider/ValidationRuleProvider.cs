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
    public class ValidationRuleProvider : IValidationRuleProvider
    {
        private readonly IRegExRuleProvider _RegExRuleProvider;
        private readonly AspreeEntities dbContext;
        public ValidationRuleProvider(AspreeEntities _dbContext,IRegExRuleProvider RegExRuleProvider)
        {
            this.dbContext = _dbContext;
            this._RegExRuleProvider = RegExRuleProvider;
        }

        public ValidationRuleViewModel Create(ValidationRuleViewModel model)
        {
            if (dbContext.ValidationRules.Any(est => est.RuleType.ToLower() == model.RuleType.ToLower()))
            {
                throw new Core.AlreadyExistsException("Rule type already exists.");
            }

            var regex = _RegExRuleProvider.GetByGuid(model.RegExId.Value);

            if(regex == null)
            {
                throw new Core.BadRequestException("Please provide a valid regular expression id.");
            }

            var validationRule = new ValidationRule
            { 
                ErrorMessage = model.ErrorMessage,
                MaxRange = model.MaxRange,
                MinRange = model.MinRange,
                RegExId = regex.Id,
                RuleType = model.RuleType,
                Guid = Guid.NewGuid()
            };

            dbContext.ValidationRules.Add(validationRule);

            SaveChanges();

            return ToModel(validationRule);
        }

        public ValidationRuleViewModel DeleteById(int id, Guid DeletedBy)
        {
            var validationRule = dbContext.ValidationRules.FirstOrDefault(fs => fs.Id == id);
            if (validationRule != null)
            {
                var delted = ToModel(validationRule);
                dbContext.ValidationRules.Remove(validationRule);
                return delted;
            }

            return null;
        }

        public ValidationRuleViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var validationRule = dbContext.ValidationRules.FirstOrDefault(fs => fs.Guid == guid);
            if (validationRule != null)
            {
                var delted = ToModel(validationRule);
                dbContext.ValidationRules.Remove(validationRule);
                return delted;
            }

            return null;
        }

        public IEnumerable<ValidationRuleViewModel> GetAll()
        {
            return dbContext.ValidationRules
                .Select(ToModel)
                .ToList();
        }

        public ValidationRuleViewModel GetByGuid(Guid guid)
        {
            var validationRule = dbContext.ValidationRules
                 .FirstOrDefault(fs => fs.Guid == guid);

            if (validationRule != null)
                return ToModel(validationRule);

            return null;
        }

        public ValidationRuleViewModel GetById(int id)
        {
            var validationRule = dbContext.ValidationRules
                 .FirstOrDefault(fs => fs.Id == id);

            if (validationRule != null)
                return ToModel(validationRule);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public ValidationRuleViewModel ToModel(ValidationRule entity)
        {
            var regex = _RegExRuleProvider.GetById(entity.RegExId.Value);
            return new ValidationRuleViewModel
            {
                RegEx = entity.RegExRule.RegEx,
                ErrorMessage = entity.ErrorMessage,
                MaxRange = entity.MaxRange,
                MinRange = entity.MinRange,
                RegExId = regex.Guid,
                RuleType = entity.RuleType,
                Guid = entity.Guid,
                Id = entity.Id
            };
        }

        public ValidationRuleViewModel Update(ValidationRuleViewModel model)
        {

            if (dbContext.ValidationRules.Any(est => est.RuleType.ToLower() == model.RuleType.ToLower()
                 && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Rule type already exists.");
            }

            var regex = _RegExRuleProvider.GetByGuid(model.RegExId.Value);

            if (regex == null)
            {
                throw new Core.BadRequestException("Please provide a valid regular expression id.");
            }

            var validationRule = dbContext.ValidationRules
                .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (validationRule != null)
            {
                validationRule.ErrorMessage = model.ErrorMessage;
                validationRule.MaxRange = model.MaxRange;
                validationRule.MinRange = model.MinRange;
                validationRule.RegExId = regex.Id;
                validationRule.RuleType = model.RuleType;

                SaveChanges();

                return ToModel(validationRule);
            }

            return null;
        }
    }
}
