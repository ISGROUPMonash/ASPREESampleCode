using Aspree.Core.ViewModels;
using Aspree.Data;
using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Provider
{
    public class AuthenticationTypeProvider : IAuthenticationTypeProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;

        public AuthenticationTypeProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
        }

        public AuthenticationTypeViewModel Create(AuthenticationTypeViewModel model)
        {

            if (dbContext.LoginAuthTypeMasters.Any(est => est.ClientId.ToLower() == model.ClientId.ToLower() && est.Domain.ToLower() == model.Domain.ToLower()))
            {
                throw new Core.AlreadyExistsException("Client Id or Domain already exists.");
            }

            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);

            var loginAuthTypeMaster = new LoginAuthTypeMaster
            {
                UserName = model.UserName,
                AuthTypeName = model.AuthTypeName,
                AuthType = model.AuthType,
                ClientId = model.ClientId,
                ClientSecret = model.ClientSecret,
                Domain = model.Domain,
                Scope = model.Scope,
                State = "state-" + Guid.NewGuid(),
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
                Guid = Guid.NewGuid(),
                AuthenticationProviderClaim = model.AuthenticationProviderClaim,
                AuthorizeEndpoint = model.AuthorizeEndpoint,
                TokenEndpoint = model.TokenEndpoint,
                IntrospectEndpoint = model.IntrospectEndpoint,
                RevokeEndpoint = model.RevokeEndpoint,
                LogoutEndpoint = model.LogoutEndpoint,
                KeysEndpoint = model.KeysEndpoint,
                UserinfoEndpoint = model.UserinfoEndpoint,
                Status = (int)Core.Enum.Status.Active,
            };

            dbContext.LoginAuthTypeMasters.Add(loginAuthTypeMaster);

            SaveChanges();

            return ToModel(loginAuthTypeMaster);
        }

        public AuthenticationTypeViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var authTypeMasters = dbContext.LoginAuthTypeMasters.FirstOrDefault(fs => fs.Guid == guid);
            if (authTypeMasters != null)
            {
                authTypeMasters.DeactivatedBy = deactivatedBy.Id;
                authTypeMasters.DateDeactivated = DateTime.UtcNow;
                authTypeMasters.Status = (int)Core.Enum.Status.Deleted;
                return ToModel(authTypeMasters);
            }
            return null;
        }

        public AuthenticationTypeViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var authTypeMasters = dbContext.LoginAuthTypeMasters.FirstOrDefault(fs => fs.Id == id);
            if (authTypeMasters != null)
            {
                authTypeMasters.DeactivatedBy = deactivatedBy.Id;
                authTypeMasters.DateDeactivated = DateTime.UtcNow;
                authTypeMasters.Status = (int)Core.Enum.Status.Deleted;

                return ToModel(authTypeMasters);
            }
            return null;
        }

        public AuthenticationTypeViewModel InActiveByGuid(Guid guid, Guid DeletedBy)
        {
            var authTypeMasters = dbContext.LoginAuthTypeMasters.FirstOrDefault(fs => fs.Guid == guid);
            if (authTypeMasters != null)
            {
                if (authTypeMasters.Status == (int)Core.Enum.Status.InActive)
                {
                    authTypeMasters.Status = (int)Core.Enum.Status.Active;

                    SaveChanges();
                }
                else if (authTypeMasters.Status == (int)Core.Enum.Status.Active)
                {
                    authTypeMasters.Status = (int)Core.Enum.Status.InActive;

                    SaveChanges();
                }
                return ToModel(authTypeMasters);
            }
            return null;
        }


        public IEnumerable<AuthenticationTypeViewModel> GetAll()
        {
            return dbContext.LoginAuthTypeMasters
                .Where(ac => !ac.DateDeactivated.HasValue)
                .Select(ToModel).OrderByDescending(x => x.CreatedDate)
                .ToList();
        }

        public IEnumerable<AuthenticationTypeViewModel> GetAll(Guid tenantId)
        {
            throw new NotImplementedException();
        }

        public AuthenticationTypeViewModel GetByGuid(Guid guid)
        {
            var authTypeMasters = dbContext.LoginAuthTypeMasters
               .FirstOrDefault(fs => fs.Guid == guid);
            if (authTypeMasters != null)
                return ToModel(authTypeMasters);

            return null;
        }

        public AuthenticationTypeViewModel GetById(int id)
        {
            var authTypeMasters = dbContext.LoginAuthTypeMasters
               .FirstOrDefault(fs => fs.Id == id);
            if (authTypeMasters != null)
                return ToModel(authTypeMasters);
            return null;
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        public AuthenticationTypeViewModel ToModel(LoginAuthTypeMaster authType)
        {
            var createdBy = _userLoginProvider.GetById(authType.CreatedBy);
            var modifiedBy = authType.ModifiedBy.HasValue ? _userLoginProvider.GetById(authType.ModifiedBy.Value) : null;
            var deactivatedBy = authType.DeactivatedBy.HasValue ? _userLoginProvider.GetById(authType.DeactivatedBy.Value) : null;

            return new AuthenticationTypeViewModel
            {
                UserName = authType.UserName,
                AuthTypeName = authType.AuthTypeName,
                AuthType = authType.AuthType,
                ClientId = authType.ClientId,
                ClientSecret = authType.ClientSecret,
                Domain = authType.Domain,
                Scope = authType.Scope,
                State = authType.State,
                CreatedBy = createdBy.Guid,
                Id = authType.Id,
                CreatedDate = authType.CreatedDate,
                DateDeactivated = authType.DateDeactivated,
                DeactivatedBy = deactivatedBy == null ? (Guid?)null : deactivatedBy.Guid,
                Guid = authType.Guid,
                ModifiedBy = modifiedBy == null ? (Guid?)null : modifiedBy.Guid,
                ModifiedDate = authType.ModifiedDate,
                AuthenticationProviderClaim = authType.AuthenticationProviderClaim,
                AuthorizeEndpoint = authType.AuthorizeEndpoint,
                TokenEndpoint = authType.TokenEndpoint,
                IntrospectEndpoint = authType.IntrospectEndpoint,
                RevokeEndpoint = authType.RevokeEndpoint,
                LogoutEndpoint = authType.LogoutEndpoint,
                KeysEndpoint = authType.KeysEndpoint,
                UserinfoEndpoint = authType.UserinfoEndpoint,
                Status = authType.Status,
            };
        }

        public AuthenticationTypeViewModel Update(AuthenticationTypeViewModel model)
        {
            if (dbContext.LoginAuthTypeMasters.Any(est => est.ClientId.ToLower() == model.ClientId.ToLower()
                && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Client Id already exists.");
            }

            var authTypeMasters = dbContext.LoginAuthTypeMasters
               .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (authTypeMasters != null)
            {
                var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);
                authTypeMasters.UserName = model.UserName;
                authTypeMasters.AuthTypeName = model.AuthTypeName;
                authTypeMasters.AuthType = model.AuthType;
                authTypeMasters.ClientId = model.ClientId;
                authTypeMasters.ClientSecret = model.ClientSecret;
                authTypeMasters.Domain = model.Domain;
                authTypeMasters.Scope = model.Scope;
                authTypeMasters.AuthenticationProviderClaim = model.AuthenticationProviderClaim;
                authTypeMasters.AuthorizeEndpoint = model.AuthorizeEndpoint;
                authTypeMasters.TokenEndpoint = model.TokenEndpoint;
                authTypeMasters.IntrospectEndpoint = model.IntrospectEndpoint;
                authTypeMasters.RevokeEndpoint = model.RevokeEndpoint;
                authTypeMasters.LogoutEndpoint = model.LogoutEndpoint;
                authTypeMasters.KeysEndpoint = model.KeysEndpoint;
                authTypeMasters.UserinfoEndpoint = model.UserinfoEndpoint;
                authTypeMasters.ModifiedBy = modifiedBy.Id;
                authTypeMasters.ModifiedDate = DateTime.UtcNow;
                SaveChanges();
                return ToModel(authTypeMasters);
            }
            return null;
        }
        public AuthenticationTypeViewModel GetAuthenticationTypeByState(string state)
        {
            var authTypeMasters = dbContext.LoginAuthTypeMasters
               .FirstOrDefault(fs => fs.State == state);
            if (authTypeMasters != null)
                return ToModel(authTypeMasters);
            return null;
        }
    }
}