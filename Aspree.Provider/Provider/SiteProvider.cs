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
    public class SiteProvider : ISiteProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        private readonly ICountryProvider _countryProvider;
        private readonly ICityProvider _cityProvider;
        private readonly IStateProvider _stateProvider;
        public SiteProvider(AspreeEntities _dbContext
            , IUserLoginProvider userLoginProvider
            , ICountryProvider countryProvider
            , ICityProvider cityProvider
            , IStateProvider stateProvider
            )
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
            this._countryProvider = countryProvider;
            this._cityProvider = cityProvider;
            this._stateProvider = stateProvider;
        }

        public SiteViewModel Create(SiteViewModel model)
        {
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
           
            if (dbContext.Sites.Any(est => est.Name.ToLower() == model.Name.ToLower()))
            {
                throw new Core.AlreadyExistsException("Site already exists.");
            }

            int? country = null;
            int? state = null;
            int? city = null;

            if (model.CountyId.HasValue)
            {
                try
                {
                    country = _countryProvider.GetByGuid(model.CountyId.Value).Id;
                }
                catch
                {
                    throw new Core.BadRequestException("Please provide valid county.");
                }
        }

            if (model.StateId.HasValue)
            {
                try
                {
                    state = _stateProvider.GetByGuid(model.StateId.Value).Id;
                }
                catch
                {
                    throw new Core.BadRequestException("Please provide valid state.");
                }
            }

            if (model.CityId.HasValue)
            {
                try
                {
                    city = _cityProvider.GetByGuid(model.CityId.Value).Id;
                }
                catch
                {
                    throw new Core.BadRequestException("Please provide valid city.");
                }
            }

            var site = new Site
            {  
                Name = model.Name,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                CityId = city,
                StateId = state,
                CountyId = country,
                GPSLocations = model.GPSLocations,
                PostCode = model.PostCode,
                Suburb = model.Suburb,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
                Guid = Guid.NewGuid(),
                
            };

            dbContext.Sites.Add(site);

            SaveChanges();

            return ToModel(site);
        }

        public SiteViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var site = dbContext.Sites.FirstOrDefault(fs => fs.Id == id);
            if (site != null)
            {
                site.DeactivatedBy = deactivatedBy.Id;
                site.DateDeactivated = DateTime.UtcNow;
                return ToModel(site);
            }

            return null;
        }

        public SiteViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var site = dbContext.Sites.FirstOrDefault(fs => fs.Guid == guid);
            if (site != null)
            {
                site.DeactivatedBy = deactivatedBy.Id;
                site.DateDeactivated = DateTime.UtcNow;
                return ToModel(site);
            }

            return null;
        }

        public IEnumerable<SiteViewModel> GetAll()
        {
            return dbContext.Sites
                .Select(ToModel)
                .ToList();
        }

        public SiteViewModel GetByGuid(Guid guid)
        {
            var site = dbContext.Sites
                .FirstOrDefault(fs => fs.Guid == guid);

            if (site != null)
                return ToModel(site);

            return null;
        }

        public SiteViewModel GetById(int id)
        {
            var site = dbContext.Sites
                .FirstOrDefault(fs => fs.Id == id);

            if (site != null)
                return ToModel(site);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public SiteViewModel ToModel(Site entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;

            Guid? country = null;
            Guid? state = null;
            Guid? city = null;

            if (entity.CountyId.HasValue  && entity.CountyId.Value>0)
            {
                country = _countryProvider.GetById(entity.CountyId.Value).Guid;
            }

            if (entity.StateId.HasValue && entity.StateId.Value > 0)
            {
                state = _stateProvider.GetById(entity.StateId.Value).Guid;
            }

            if (entity.CityId.HasValue && entity.CityId.Value > 0)
            {
                city = _cityProvider.GetById(entity.CityId.Value).Guid;
            }



            return new SiteViewModel
            {
                AddressLine1 = entity.AddressLine1,
                Id = entity.Id,
                Suburb = entity.Suburb,
                AddressLine2 = entity.AddressLine2,
                CityId = city,
                CountyId = country,
                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy == null ? (Guid?)null : deactivatedBy.Guid,
                Guid = entity.Guid,
                ModifiedBy = modifiedBy == null ? (Guid?)null : modifiedBy.Guid,
                ModifiedDate = entity.ModifiedDate,
                GPSLocations = entity.GPSLocations,
                Name = entity.Name,
                PostCode = entity.PostCode,
                StateId = state
                
            };
        }

        public SiteViewModel Update(SiteViewModel model)
        {
            var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);
            if (dbContext.Sites.Any(est => est.Name.ToLower() == model.Name.ToLower()
                 && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Site name already exists.");
            }

            var site = dbContext.Sites
               .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (site != null)
            {
                int? country = null;
                int? state = null;
                int? city = null;

                if (model.CountyId.HasValue)
                {
                    try
                    {
                        country = _countryProvider.GetByGuid(model.CountyId.Value).Id;
                    }
                    catch
                    {
                        throw new Core.BadRequestException("Please provide valid county.");
                    }
                }

                if (model.StateId.HasValue)
                {
                    try { 
                        state = _stateProvider.GetByGuid(model.StateId.Value).Id;
                    }
                    catch
                    {
                        throw new Core.BadRequestException("Please provide valid state.");
                    }
                }

                if (model.CityId.HasValue)
                {
                    try
                    {
                        city = _cityProvider.GetByGuid(model.CityId.Value).Id;
                    }
                    catch
                    {
                        throw new Core.BadRequestException("Please provide valid city.");
                    }
                }

                site.AddressLine1 = model.AddressLine1;
                site.Suburb = model.Suburb;
                site.AddressLine2 = model.AddressLine2;
                site.CityId = city;
                site.CountyId = country;
                site.Guid = model.Guid;
                site.ModifiedBy = modifiedBy.Id;
                site.ModifiedDate = model.ModifiedDate;
                site.GPSLocations = model.GPSLocations;
                site.Name = model.Name;
                site.PostCode = model.PostCode;
                site.StateId = state;

                SaveChanges();

                return ToModel(site);
            }

            return null;
        }
    }
}
