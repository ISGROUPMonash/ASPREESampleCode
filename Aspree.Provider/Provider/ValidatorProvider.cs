using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.ViewModels;
using Aspree.Data;

namespace Aspree.Provider.Provider
{
    public class ValidatorProvider : IValidatorProvider
    {

        private readonly AspreeEntities _dbContext;
        public ValidatorProvider(AspreeEntities dbContext)
        {
            this._dbContext = dbContext;

        }

        public bool CheckExist(ValidatorViewModal validator)
        {
            bool exists = false;
            if (!validator.Guid.HasValue)
            {
                validator.Guid = Guid.Empty;
            }

            switch (validator.Property)
            {
                case "UserEmail":
                    exists = _dbContext.UserLogins.Any(ul => ul.Email.ToLower() == validator.Value.ToLower() && ul.Guid != validator.Guid && !ul.DateDeactivated.HasValue);
                    break;
                case "UserRole":
                    exists = _dbContext.Roles.Any(ul => ul.Name.ToLower() == validator.Value.ToLower() && ul.Guid != validator.Guid && !ul.DateDeactivated.HasValue);
                    break;
                default:
                    break;
            }
            
            return exists;
        }
    }
}
