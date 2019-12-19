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
    public class CityProvider : ICityProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        public CityProvider(AspreeEntities _dbContext,IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
        }

        public State CheckStateById(Guid guid)
        {
            return dbContext.States
                .FirstOrDefault(x => x.Guid == guid);
           
        }

        public CityViewModel Create(CityViewModel model)
        {
            if (dbContext.Cities.Any(est => est.Name.ToLower() == model.Name.ToLower()))
            {
                throw new Core.AlreadyExistsException("City name is already exists.");
            }

            var state = this.CheckStateById(model.StatedId);

            var city = new City
            {
                Name = model.Name,
                StatedId = state.Id,
                Abbr = model.Abbr,
                Guid = Guid.NewGuid()
            };

            dbContext.Cities.Add(city);

            SaveChanges();

            return ToModel(city);
        }

        public CityViewModel DeleteById(int id, Guid DeletedBy)
        {
            var city = dbContext.Cities
                .FirstOrDefault(fs => fs.Id == id);

            if (city != null)
            {
                dbContext.Cities.Remove(city);
                dbContext.SaveChanges();
                return ToModel(city);
            }

            return null;
        }

        public CityViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var city = dbContext.Cities
                .FirstOrDefault(fs => fs.Guid == guid);

            if (city != null)
            {
                var removed = ToModel(city);
                dbContext.Cities.Remove(city);
                dbContext.SaveChanges();
                return removed;
            }

            return null;
        }

        public IEnumerable<CityViewModel> GetAll()
        {
            return dbContext.Cities
               .Select(ToModel)
               .ToList();
        }

        public CityViewModel GetByGuid(Guid guid)
        {
            var Cities = dbContext.Cities
               .FirstOrDefault(fs => fs.Guid == guid);

            if (Cities != null)
                return ToModel(Cities);

            return null;
        }

        public CityViewModel GetById(int id)
        {
            var Cities = dbContext.Cities
                .FirstOrDefault(fs => fs.Id == id);

            if (Cities != null)
                return ToModel(Cities);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public CityViewModel ToModel(City entity)
        {
            return new CityViewModel
            {
                Name = entity.Name,
                StatedId = entity.State.Guid,
                Abbr = entity.Abbr,
                Id = entity.Id,
                Guid = entity.Guid
            };
        }

        public CityViewModel Update(CityViewModel model)
        {
            if (dbContext.Cities.Any(est => est.Name.ToLower() == model.Name.ToLower()
               && est.Id != model.Id))
            {
                throw new Core.AlreadyExistsException("City name is already exists.");
            }

            var city = dbContext.Cities
               .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (city != null)
            {
                city.Name = model.Name;
                city.Abbr = model.Abbr;
                SaveChanges();

                return ToModel(city);
            }

            return null;
        }
    }
}
