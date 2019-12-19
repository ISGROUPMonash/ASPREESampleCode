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
    public class PushEmailEventProvider : IPushEmailEventProvider
    {
        private readonly AspreeEntities dbContext;

        public PushEmailEventProvider(AspreeEntities _dbContext)
        {
            this.dbContext = _dbContext;
        }
        public PushEmailEventViewModel Create(PushEmailEventViewModel model)
        {
            throw new NotImplementedException();
        }

        public PushEmailEventViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            throw new NotImplementedException();
        }

        public PushEmailEventViewModel DeleteById(int id, Guid DeletedBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PushEmailEventViewModel> GetAll()
        {
            return this.dbContext.PushEmailEvents
              .Select(ToModel)
              .ToList();
        }

        public PushEmailEventViewModel GetByGuid(Guid guid)
        {
            var PushEmailEvent = dbContext.PushEmailEvents
               .FirstOrDefault(fs => fs.Guid == guid);

            if (PushEmailEvent != null)
                return ToModel(PushEmailEvent);

            return null;
        }

        public PushEmailEventViewModel GetById(int id)
        {
            var PushEmailEvent = dbContext.PushEmailEvents
                .FirstOrDefault(fs => fs.Id == id);

            if (PushEmailEvent != null)
                return ToModel(PushEmailEvent);

            return null;
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public PushEmailEventViewModel ToModel(PushEmailEvent entity)
        {
            return new PushEmailEventViewModel
            {
                Id = entity.Id,
                DisplayName = entity.DisplayName,
                EventName = entity.EventName,
                Guid = entity.Guid,
                IsEmailEvent = entity.IsEmailEvent
            };
        }

        public PushEmailEventViewModel Update(PushEmailEventViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
