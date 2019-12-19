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
    public class StateProvider : IStateProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        private readonly ICountryProvider _CountryProvider;
        public StateProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider,ICountryProvider CountryProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
            this._CountryProvider = CountryProvider;
        }

        public Country CheckCountryById(Guid guid)
        {
            return dbContext.Countries.FirstOrDefault(x => x.Guid == guid);
        }

        public StateViewModel Create(StateViewModel model)
        {
            if (dbContext.States.Any(est => est.Name.ToLower() == model.Name.ToLower()))
            {
                throw new Core.AlreadyExistsException("State is already exists.");
            }

            var country = this.CheckCountryById(model.CountryId);

            var state = new State
            {
                Name = model.Name,
                CountryId = country.Id,
                Guid = Guid.NewGuid()
            };

            dbContext.States.Add(state);

            SaveChanges();

            return ToModel(state);
        }

        public StateViewModel DeleteById(int id, Guid DeletedBy)
        {
            var States = dbContext.States
                .FirstOrDefault(fs => fs.Id == id);

            if (States != null)
            {
                dbContext.States.Remove(States);
                dbContext.SaveChanges();
                return ToModel(States);
            }

            return null;
        }

        public StateViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var States = dbContext.States
                .FirstOrDefault(fs => fs.Guid == guid);

            if (States != null)
            {
                dbContext.States.Remove(States);
                dbContext.SaveChanges();
                return ToModel(States);
            }

            return null;
        }

        public IEnumerable<StateViewModel> GetAll()
        {
            return dbContext.States
               .Select(ToModel)
               .ToList();
        }

        public StateViewModel GetByGuid(Guid guid)
        {
            var States = dbContext.States
                .FirstOrDefault(fs => fs.Guid == guid);

            if (States != null)
                return ToModel(States);

            return null;
        }

        public StateViewModel GetById(int id)
        {
            var States = dbContext.States
                .FirstOrDefault(fs => fs.Id == id);

            if (States != null)
                return ToModel(States);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public StateViewModel ToModel(State entity)
        {
            var country = _CountryProvider.GetById(entity.CountryId);
            return new StateViewModel
            {
                Name = entity.Name,
                CountryId = country.Guid,
                Id = entity.Id,
                Guid = entity.Guid
            };
        }

        public StateViewModel Update(StateViewModel model)
        {
            if (dbContext.States.Any(est => est.Name.ToLower() == model.Name.ToLower()
                && est.Id != model.Id))
            {
                throw new Core.AlreadyExistsException("State name is already exists.");
            }

            var States = dbContext.States
                .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (States != null)
            {
                States.Name = model.Name;

                SaveChanges();

                return ToModel(States);
            }

            return null;
        }
    }
}
