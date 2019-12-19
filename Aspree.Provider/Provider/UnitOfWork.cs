using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Provider
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Aspree.Data.AspreeEntities _dbContext;
        
        public UnitOfWork(Aspree.Data.AspreeEntities dbContext)
        {
            _dbContext = dbContext;
            this.StatusProvider = new StatusProvider(_dbContext);
            this.RoleProvider = new RoleProvider(_dbContext, UserLoginProvider);
            this.VariableTypeProvider = new VariableTypeProvider(_dbContext);
            this.VariableCategoryProvider = new VariableCategoryProvider(_dbContext, UserLoginProvider);
            this.ValidationRuleProvider = new ValidationRuleProvider(_dbContext,RegExRuleProvider);
            this.RegExRuleProvider = new RegExRuleProvider(_dbContext, UserLoginProvider);
            this.FormCategoryProvider = new FormCategoryProvider(_dbContext, UserLoginProvider);
            this.FormStatusProvider = new FormStatusProvider(_dbContext, UserLoginProvider);
            this.UserLoginProvider = new UserLoginProvider(_dbContext, this.TenantProvider, _mongoDBContext);
            this.ActivityCategoryProvider = new ActivityCategoryProvider(_dbContext, UserLoginProvider);
            this.ActivityStatusProvider = new ActivityStatusProvider(_dbContext);
            this.EntityTypeProvider = new EntityTypeProvider (_dbContext, UserLoginProvider);
            this.EntitySubTypeProvider = new EntitySubTypeProvider(_dbContext, this.EntityTypeProvider, UserLoginProvider);
            this.CountryProvider = new CountryProvider(_dbContext, UserLoginProvider);
            this.StateProvider = new StateProvider(_dbContext, UserLoginProvider,CountryProvider);
            this.CityProvider = new CityProvider(_dbContext, UserLoginProvider);
            this.LookupDataProvider = new LookupDataProvider(_dbContext, UserLoginProvider, VariableTypeProvider);
            this.LookupDataGroupOptionProvider = new LookupDataGroupOptionProvider(_dbContext, UserLoginProvider, LookupDataOptionGroupProvider);
            this.LookupDataOptionGroupProvider = new LookupDataOptionGroupProvider(_dbContext, UserLoginProvider, LookupDataProvider);
            this.CheckListProvider = new CheckListProvider(_dbContext, UserLoginProvider);
            this.SiteProvider = new SiteProvider(_dbContext, UserLoginProvider,this.CountryProvider,this.CityProvider,this.StateProvider);
            this.ProjectStatusProvider = new ProjectStatusProvider(_dbContext, UserLoginProvider);
            this.SecurityQuestionProvider = new SecurityQuestionProvider(_dbContext);
            this.PrivilegeProvider = new PrivilegeProvider(_dbContext);
            this.TenantProvider = new TenantProvider(_dbContext);
            this.PushEmailEventProvider = new PushEmailEventProvider(_dbContext);
            this.EmailTemplateProvider = new EmailTemplateProvider(_dbContext, UserLoginProvider, PushEmailEventProvider);
        }

        public ITenantProvider TenantProvider { get; private set; }
        public IPrivilegeProvider PrivilegeProvider { get; private set; }
        public IStatusProvider StatusProvider { get; private set; }
        public IRoleProvider RoleProvider { get; private set; }
        public IVariableTypeProvider VariableTypeProvider { get; private set; }
        public IVariableCategoryProvider VariableCategoryProvider { get; private set; }
        public IValidationRuleProvider ValidationRuleProvider { get; private set; }
        public IRegExRuleProvider RegExRuleProvider { get; private set; }
        public IFormCategoryProvider FormCategoryProvider { get; private set; }
        public IFormStatusProvider FormStatusProvider { get; private set; }
        public IActivityCategoryProvider ActivityCategoryProvider { get; private set; }
        public IActivityStatusProvider ActivityStatusProvider { get; private set; }
        public IEntityTypeProvider EntityTypeProvider { get; private set; }
        public IEntitySubTypeProvider EntitySubTypeProvider { get; private set; }
        public ICountryProvider CountryProvider { get; private set; }
        public IStateProvider StateProvider { get; private set; }
        public ICityProvider CityProvider { get; private set; }
        public ILookupDataProvider LookupDataProvider { get; private set; }
        public ILookupDataGroupOptionProvider LookupDataGroupOptionProvider { get; private set; }
        public ILookupDataOptionGroupProvider LookupDataOptionGroupProvider { get; private set; }
        public ICheckListProvider CheckListProvider { get; private set; }
        public ISiteProvider SiteProvider { get; private set; }
        public IProjectStatusProvider ProjectStatusProvider { get; private set; }
        public ISecurityQuestionProvider SecurityQuestionProvider { get; private set; }
        public IUserLoginProvider UserLoginProvider { get; private set; }
        public IEmailTemplateProvider EmailTemplateProvider { get; private set; }
        public IPushEmailEventProvider PushEmailEventProvider { get; private set; }
        public ISecurity Security { get; set; }

        public Aspree.Data.MongoDB.MongoDBContext _mongoDBContext;

        public int Commit()
        {
            try
            {
                return _dbContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var error = ex.EntityValidationErrors.First().ValidationErrors.First();
                throw new ApplicationException("Invalid Entity Error occured while saving to the database.", ex);
            }
            catch (UpdateException ex)
            {
                throw new ApplicationException("Error occured while saving to the database.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occured while saving to the database.", ex);
            }
        }

        public Task<int> CommitAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
