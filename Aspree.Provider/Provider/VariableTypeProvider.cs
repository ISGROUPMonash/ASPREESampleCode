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

        public VariableTypeViewModel Create(VariableTypeViewModel model)
        {
            if (dbContext.VariableTypes.Any(est => est.Type.ToLower() == model.Type.ToLower()))
            {
                throw new Core.AlreadyExistsException("Variable type already exists.");
            }

            var variableType = new VariableType
           {  
                Type = model.Type,
                Status = model.Status,
                Guid = Guid.NewGuid()
                
            };

            dbContext.VariableTypes.Add(variableType);

            SaveChanges();

            return ToModel(variableType);
        }

        public VariableTypeViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var variableType = dbContext.VariableTypes.FirstOrDefault(fs => fs.Guid == guid);
            if (variableType != null)
            {
                dbContext.VariableTypes.Remove(variableType);
                return ToModel(variableType);
            }

            return null;
        }

        public VariableTypeViewModel DeleteById(int id, Guid DeletedBy)
        {
            var variableType = dbContext.VariableTypes.FirstOrDefault(fs => fs.Id == id);
            if (variableType != null)
            {
                dbContext.VariableTypes.Remove(variableType);
                return ToModel(variableType);
            }

            return null;
        }

        public IEnumerable<VariableTypeViewModel> GetAll()
        {
            return dbContext.VariableTypes
                .Select(ToModel)
                .ToList();
        }

        public VariableTypeViewModel GetByGuid(Guid guid)
        {
            var variableType = dbContext.VariableTypes
               .FirstOrDefault(fs => fs.Guid == guid);

            if (variableType != null)
                return ToModel(variableType);

            return null;
        }

        public VariableTypeViewModel GetById(int id)
        {
            var variableType = dbContext.VariableTypes
              .FirstOrDefault(fs => fs.Id == id);

            if (variableType != null)
                return ToModel(variableType);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

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

        public VariableTypeViewModel Update(VariableTypeViewModel model)
        {
            if (dbContext.VariableTypes.Any(est => est.Type.ToLower() == model.Type.ToLower()
                 && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Variable type already exists.");
            }

            var variableType = dbContext.VariableTypes
               .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (variableType != null)
            {
                variableType.Status = model.Status;
                variableType.Type = model.Type;

                SaveChanges();

                return ToModel(variableType);
            }

            return null;
        }
    }
}
