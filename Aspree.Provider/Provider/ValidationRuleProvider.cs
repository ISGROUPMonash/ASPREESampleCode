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
        private readonly AspreeEntities dbContext;
        public ValidationRuleProvider(AspreeEntities _dbContext)
        {
            this.dbContext = _dbContext;
        }
        /// <summary>
        /// Method to get all Validation rules
        /// </summary>
        /// <returns>List of ValidationRuleViewModel model</returns>
        public IEnumerable<ValidationRuleViewModel> GetAll()
        {
            return dbContext.ValidationRules
                .Select(ToModel)
                .ToList();
        }
        /// <summary>
        /// Map ValidationRule entity to ValidationRuleViewModel view model
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ValidationRuleViewModel ToModel(ValidationRule entity)
        {
            return new ValidationRuleViewModel
            {
                ErrorMessage = entity.ErrorMessage,
                MaxRange = entity.MaxRange,
                MinRange = entity.MinRange,
                RegExId = entity.RegExId,
                RuleType = entity.RuleType,
                Guid = entity.Guid,
                Id = entity.Id
            };
        }
        
    }
}
