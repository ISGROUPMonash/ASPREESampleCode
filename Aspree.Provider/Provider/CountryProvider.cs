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
    public class CountryProvider : ICountryProvider
    {
        private readonly AspreeEntities dbContext;
        private readonly IUserLoginProvider _userLoginProvider;
        public CountryProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
        }
       
        public CountryViewModel Create(CountryViewModel model)
        {
            if (dbContext.Countries.Any(est => est.Name.ToLower() == model.Name.ToLower()))
            {
                throw new Core.AlreadyExistsException("Country name is already exists.");
            }

            var country = new Country
            {
                Name = model.Name,
                Abbr = model.Abbr,
                Guid = Guid.NewGuid()
            };

            dbContext.Countries.Add(country);

            SaveChanges();

            return ToModel(country);
        }

        public CountryViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var country = dbContext.Countries.FirstOrDefault(fs => fs.Id == id);
            if (country != null)
            {
                dbContext.Countries.Remove(country);
                dbContext.SaveChanges();

                return ToModel(country);
            }

            return null;
        }

        public CountryViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var country = dbContext.Countries.FirstOrDefault(fs => fs.Guid == guid);
            if (country != null)
            {
                dbContext.Countries.Remove(country);
                dbContext.SaveChanges();

                return ToModel(country);
            }

            return null;
        }

        public IEnumerable<CountryViewModel> GetAll()
        {            
            return dbContext.Countries
               .Select(ToModel)
               .ToList();
        }

        public CountryViewModel GetByGuid(Guid guid)
        {
            var country = dbContext.Countries
               .FirstOrDefault(fs => fs.Guid == guid);

            if (country != null)
                return ToModel(country);

            return null;
        }

        public CountryViewModel GetById(int id)
        {
            var country = dbContext.Countries
                .FirstOrDefault(fs => fs.Id == id);

            if (country != null)
                return ToModel(country);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public CountryViewModel ToModel(Country entity)
        {
            return new CountryViewModel
            {
                Name = entity.Name,
                Abbr = entity.Abbr,
                Id = entity.Id,
                Guid = entity.Guid
            };
        }

        public CountryViewModel Update(CountryViewModel model)
        {
            if (dbContext.Countries.Any(est => est.Name.ToLower() == model.Name.ToLower()
                && est.Id != model.Id))
            {
                throw new Core.AlreadyExistsException("Country name is already exists.");
            }

            var country = dbContext.Countries
               .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (country != null)
            {
                country.Name = model.Name;
                SaveChanges();

                return ToModel(country);
            }

            return null;
        }


    }
}
