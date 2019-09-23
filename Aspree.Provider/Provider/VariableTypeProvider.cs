using Aspree.Core.Enum;
using Aspree.Core.ViewModels;
using Aspree.Data;
using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Provider
{
    public class VariableTypeProvider : IVariableTypeProvider
    {
        private readonly AspreeEntities dbContext;
        public VariableTypeProvider(AspreeEntities _dbContext)
        {
            this.dbContext = _dbContext;
        }
        /// <summary>
        /// Method to get all variable types
        /// </summary>
        /// <returns>List of VariableTypeViewModel model</returns>
        public IEnumerable<VariableTypeViewModel> GetAll()
        {
            return dbContext.VariableTypes
                .Select(ToModel)
                .ToList();
        }
        /// <summary>
        /// Method to map VariableType entity to VariableTypeViewModel model.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public VariableTypeViewModel ToModel(VariableType entity)
        {
            return new VariableTypeViewModel()
            {
                Guid = entity.Guid,
                Id = entity.Id,
                Status = entity.Status,
                Type = entity.Type
            };
        }
    }
}
