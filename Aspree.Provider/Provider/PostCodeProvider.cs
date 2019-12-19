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
    public class PostCodeProvider : IPostCodeProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        public PostCodeProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
        }

        public PostCodeViewModel Create(PostCodeViewModel model)
        {
            if (dbContext.PostCodes.Any(est => est.PostalCode.ToLower() == model.PostalCode.ToLower()))
            {
                throw new Core.AlreadyExistsException("Postcode is already exists.");
            }
            var state = this.dbContext.States.FirstOrDefault(x=>x.Guid==model.StateId);
            var suburb = this.dbContext.Suburbs.FirstOrDefault(x => x.Guid == model.SuburbId);
            var city = this.dbContext.Cities.FirstOrDefault(x => x.Guid == model.CityId);
            var postCode = new PostCode
            {
                PostalCode = model.PostalCode,
                StateId = state.Id,
                SuburbId = suburb.Id,
                CityId = city.Id,                
                Guid = Guid.NewGuid()
            };

            dbContext.PostCodes.Add(postCode);

            SaveChanges();

            return ToModel(postCode);
        }

        public PostCodeViewModel DeleteById(int id, Guid DeletedBy)
        {
            var postCode = dbContext.PostCodes
                .FirstOrDefault(fs => fs.Id == id);

            if (postCode != null)
            {
                dbContext.PostCodes.Remove(postCode);
                dbContext.SaveChanges();
                return ToModel(postCode);
            }

            return null;
        }

        public PostCodeViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var postCode = dbContext.PostCodes
                .FirstOrDefault(fs => fs.Guid == guid);

            if (postCode != null)
            {
                var removed = ToModel(postCode);
                dbContext.PostCodes.Remove(postCode);
                dbContext.SaveChanges();
                return removed;
            }

            return null;
        }

        public IEnumerable<PostCodeViewModel> GetAll()
        {
            return dbContext.PostCodes
               .Select(ToModel)
               .ToList();
        }

        public PostCodeViewModel GetByGuid(Guid guid)
        {
            var postCode = dbContext.PostCodes
               .FirstOrDefault(fs => fs.Guid == guid);

            if (postCode != null)
                return ToModel(postCode);

            return null;
        }

        public PostCodeViewModel GetById(int id)
        {
            var postCode = dbContext.PostCodes
                .FirstOrDefault(fs => fs.Id == id);

            if (postCode != null)
                return ToModel(postCode);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public PostCodeViewModel ToModel(PostCode entity)
        {
            return new PostCodeViewModel
            {
                PostalCode = entity.PostalCode,
                StateId = entity.State.Guid,
                CityId = entity.City.Guid,
                SuburbId=entity.Suburb.Guid,                
                Id = entity.Id,
                Guid = entity.Guid
            };
        }

        public PostCodeViewModel Update(PostCodeViewModel model)
        {
            if (dbContext.PostCodes.Any(est => est.PostalCode.ToLower() == model.PostalCode.ToLower()
               && est.Id != model.Id))
            {
                throw new Core.AlreadyExistsException("PostCode is already exists.");
            }

            var postCode = dbContext.PostCodes
               .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (postCode != null)
            {
                postCode.PostalCode = model.PostalCode;
                SaveChanges();

                return ToModel(postCode);
            }

            return null;
        }
    }
}

